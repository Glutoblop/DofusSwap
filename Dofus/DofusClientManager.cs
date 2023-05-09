﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    class DofusClientManager
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;

        public List<DofusClientData> Clients;
        private List<Process> _DofusProcesses;

        private static string jsonFileName = "dofusclients.json";

        public void Init()
        {
            RefreshConfig();
            RefreshProcessList();
        }

        public void UpdateConfig(List<DofusClientData> clients = null)
        {
            if (clients == null) clients = Clients;

            var clientsJson = JsonConvert.SerializeObject(clients, Formatting.Indented);

            string dir = "dofusclients.json";
#if !DEBUG
            jsonFileName = Path.Combine(Environment.CurrentDirectory, "dofusclients.json");
#endif

            File.WriteAllText(jsonFileName, clientsJson);
        }

        public void RefreshConfig()
        {
            if (!File.Exists(jsonFileName))
            {
                File.CreateText(jsonFileName);
            }

            string clientConfig = File.ReadAllText(jsonFileName);
            Clients = JsonConvert.DeserializeObject<List<DofusClientData>>(clientConfig);

            foreach (var dofusClient in Clients)
            {
                dofusClient.KeyBind = Enum.TryParse(dofusClient.key, true, out Keys key) ? key : Keys.None;
            }
        }

        private DofusClientData GetClient(Keys key, out Process clientProcess)
        {
            clientProcess = null;

            foreach (Process process in _DofusProcesses)
            {
                foreach (DofusClientData dofusClient in Clients)
                {
                    if (dofusClient.KeyBind == key && process.MainWindowTitle.StartsWith(dofusClient.name))
                    {
                        clientProcess = process;
                        return dofusClient;
                    }
                }
            }

            return null;
        }

        public bool HandleKeyDown(Keys keyPressed)
        {
            //Find the process that matches the key press
            DofusClientData clientData = GetClient(keyPressed, out Process clientProcess);
            if (clientData == null)
            {
                RefreshProcessList();
                clientData = GetClient(keyPressed, out clientProcess);
            }

            if (clientData != null)
            {
                //Simualte alt key down
                keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

                SetForegroundWindow(clientProcess.MainWindowHandle);
                SwitchToThisWindow(clientProcess.MainWindowHandle, true);
                BringWindowToTop(clientProcess.MainWindowHandle);

                // Simulate a key release
                keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

                return true;
            }

            return false;
        }

        private void RefreshProcessList()
        {
            _DofusProcesses = new List<Process>();

            var processes = Process.GetProcesses().Where(s => s.ProcessName.ToLowerInvariant().Contains("dofus")).ToArray();
            foreach (var process in processes)
            {
                foreach (var client in Clients)
                {
                    if (process.MainWindowTitle.StartsWith(client.name))
                    {
                        _DofusProcesses.Add(process);
                        break;
                    }
                }
            }
        }
    }
}
