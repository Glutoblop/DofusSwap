using Newtonsoft.Json;
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

        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;

        public List<DofusClientData> Clients;
        private List<Process> _DofusProcesses;

        public static string CONFIG_FILE_PATH = "";

        public void Init()
        {
            CONFIG_FILE_PATH = "dofusclients.json";
#if !DEBUG
            CONFIG_FILE_PATH = Path.Combine(Environment.CurrentDirectory, "dofusclients.json");
#endif

            if (!File.Exists(CONFIG_FILE_PATH)) File.CreateText(CONFIG_FILE_PATH);

            RefreshConfig();
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
            Clients = JsonConvert.DeserializeObject<List<DofusClientData>>(clientConfig);

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

            if (clientData != null)
            {
                //Simualte alt key down
                //keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);
                keybd_event(0x12,0xb8,0 , 0);

                SetForegroundWindow(clientProcess.MainWindowHandle);
                SwitchToThisWindow(clientProcess.MainWindowHandle, true);
                BringWindowToTop(clientProcess.MainWindowHandle);

                // Simulate a key release
                //keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);
                keybd_event(0x12,0xb8,0x0002,0);

                //https://www.codeproject.com/Articles/7305/Keyboard-Events-Simulation-using-keybd-event-funct

                return true;
            }

            return false;
        }

        private void RefreshProcessList()
        {
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