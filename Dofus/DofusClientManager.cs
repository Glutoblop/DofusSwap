using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    class DofusClientManager
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        public List<DofusClientData> Clients;
        private Process[] _DofusProcesses;

        private static string jsonFileName = "dofusclients.json";

        public void Init()
        {
            RefreshConfig();
            RefreshProcessList();
        }

        public void UpdateConfig(List<DofusClientData> clients)
        {
            var clientsJson = JsonConvert.SerializeObject(clients);
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
                SetForegroundWindow(clientProcess.MainWindowHandle);
                SwitchToThisWindow(clientProcess.MainWindowHandle, true);
                BringWindowToTop(clientProcess.MainWindowHandle);

                return true;
            }

            return false;
        }

        private void RefreshProcessList()
        {
            _DofusProcesses = Process.GetProcesses().Where(s =>
            {
                string processName = s.ProcessName;
                return !processName.Equals(Application.ProductName) && processName.Contains("Dofus");

            }).ToArray();
        }
    }
}
