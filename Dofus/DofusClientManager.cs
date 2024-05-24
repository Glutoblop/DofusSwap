﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    internal class DofusClientManager
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        //SendInput tutorial
        //https://www.codeproject.com/Articles/5264831/How-to-Send-Inputs-using-Csharp

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008
        }

        [Flags]
        public enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public HardwareInput hi;
        }

        public struct Input
        {
            public int type;
            public InputUnion u;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;

        public List<DofusClientData> Clients;
        private List<Process> _DofusProcesses;

        public static string CONFIG_FILE_PATH = "";

        public Action<bool> OnSimulatingAltIsPressed { get; set; }

        public void Init()
        {
            CONFIG_FILE_PATH = Path.Combine(Environment.CurrentDirectory, "dofusclients.json");

            if (!File.Exists(CONFIG_FILE_PATH))
            {
                using (File.CreateText(CONFIG_FILE_PATH))
                {
                }
            }

            RefreshConfig();
            UpdateConfig(Clients);
            RefreshProcessList();
        }

        public void UpdateConfig(List<DofusClientData> clients = null)
        {
            if (clients == null) clients = Clients;

            var clientsJson = JsonConvert.SerializeObject(clients, Formatting.Indented);

            File.WriteAllText(CONFIG_FILE_PATH, clientsJson);
        }

        public void RefreshConfig()
        {
            var clientConfig = File.ReadAllText(CONFIG_FILE_PATH);
            Clients = JsonConvert.DeserializeObject<List<DofusClientData>>(clientConfig) ?? new List<DofusClientData>();

            foreach (var dofusClient in Clients)
                dofusClient.KeyBind = Enum.TryParse(dofusClient.key, true, out Keys key) ? key : Keys.None;
        }

        private DofusClientData GetClient(Keys key, out Process clientProcess)
        {
            clientProcess = null;

            foreach (var process in _DofusProcesses)
            foreach (var dofusClient in Clients)
                if (dofusClient.KeyBind == key && process.MainWindowTitle.StartsWith(dofusClient.name))
                {
                    clientProcess = process;
                    return dofusClient;
                }

            return null;
        }

        public bool HandleKeyDown(Keys keyPressed)
        {
            //Find the process that matches the key press
            var clientData = GetClient(keyPressed, out var clientProcess);
            if (clientData == null)
            {
                RefreshProcessList();
                clientData = GetClient(keyPressed, out clientProcess);
            }

            if (clientData == null) return false;

            //OLD - Simualte alt key down
            //keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);
            //keybd_event(0x12,0xb8,0 , 0);

            Input[] altDown = {
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = 0xb8,
                            dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };
            OnSimulatingAltIsPressed?.Invoke(true);
            SendInput((uint)altDown.Length, altDown, Marshal.SizeOf(typeof(Input)));

            SetForegroundWindow(clientProcess.MainWindowHandle);
            SwitchToThisWindow(clientProcess.MainWindowHandle, true);
            BringWindowToTop(clientProcess.MainWindowHandle);

            // OLD - Simulate a key release
            //keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);
            //keybd_event(0x12,0xb8,0x0002,0);

            Input[] altUp = {
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = 0xb8,
                            dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };
            SendInput((uint)altUp.Length, altUp, Marshal.SizeOf(typeof(Input)));
            OnSimulatingAltIsPressed?.Invoke(false);

            //https://www.codeproject.com/Articles/7305/Keyboard-Events-Simulation-using-keybd-event-funct

            return true;

        }

        public void RefreshProcessList()
        {
            if (Clients == null) return;

            _DofusProcesses = new List<Process>();

            var processes = Process.GetProcesses().Where(s => s.ProcessName.ToLowerInvariant().Contains("dofus"))
                .ToArray();
            foreach (var process in processes)
            foreach (var client in Clients)
                if (process.MainWindowTitle.StartsWith(client.name))
                {
                    _DofusProcesses.Add(process);
                    break;
                }
        }
    }
}