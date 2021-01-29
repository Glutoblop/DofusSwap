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

        public List<DofusClient> Clients;
        private Process[] _DofusProcesses;

        public void Init()
        {
            RefreshConfig();

            foreach (DofusClient dofusClient in Clients)
            {
                dofusClient.KeyBind = (Keys)Enum.Parse(typeof(Keys), dofusClient.key);
            }

            RefreshProcessList();
        }

        public void RefreshConfig()
        {
            string clientConfig = File.ReadAllText("dofusclients.json");
            Clients = JsonConvert.DeserializeObject<List<DofusClient>>(clientConfig);
        }

        private DofusClient GetClient(Keys key, out Process clientProcess)
        {
            clientProcess = null;

            foreach (Process process in _DofusProcesses)
            {
                foreach (DofusClient dofusClient in Clients)
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

        public void OnKeyDown(IntPtr mHWnd, Keys keyPressed)
        {
            //Find the process that matches the key press
            DofusClient client = GetClient(keyPressed, out Process clientProcess);
            if (client == null)
            {
                RefreshProcessList();
                client = GetClient(keyPressed, out clientProcess);
            }

            if (client != null)
            {
                SetForegroundWindow(clientProcess.MainWindowHandle);
                SwitchToThisWindow(clientProcess.MainWindowHandle, true);
                BringWindowToTop(clientProcess.MainWindowHandle);
            }
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
