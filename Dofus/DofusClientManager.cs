using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public List<DofusClientData> Clients { get; private set; }

        private Timer _RefreshTimer;
        private bool _AutoDetect = true;
        private bool _Visible = true;

        public static string CONFIG_FILE_PATH = "";

        private bool _AwaitingNextHotkey = false;
        private Keys _NextHotKey = Keys.None;
        private Timer _NextHotkeyTimer;
        private const string NextHotkeyPath = "nexthotkey.txt";
        private int _NextCharIndex = 0;

        private List<Process> _DofusProcesses = new List<Process>();

        public Action<bool> OnSimulatingAltIsPressed { get; set; }

        public Action<string> OnNewDofusClientDetected { get; set; }
        public Action<DofusClientData> OnClientFocused { get; set; }
        public Action<Keys> OnNextHotkeySet { get; set; }

        public bool IsInit = false;

        public void Init()
        {
            CONFIG_FILE_PATH = Path.Combine(Environment.CurrentDirectory, "dofusclients.json");

            if (!File.Exists(CONFIG_FILE_PATH))
            {
                using (File.CreateText(CONFIG_FILE_PATH))
                {
                }
            }

            if (!File.Exists(NextHotkeyPath)) File.WriteAllText(NextHotkeyPath, Keys.None.ToString());
            _NextHotKey = (Keys)Enum.Parse(typeof(Keys), File.ReadAllText(NextHotkeyPath));

            IsInit = true;

            OnNextHotkeySet?.Invoke(_NextHotKey);

            RefreshConfig();
            UpdateConfig(Clients);

            _RefreshTimer = new Timer();
            _RefreshTimer.Tick += RefreshTimerOnTick;
            _RefreshTimer.Interval = (int)TimeSpan.FromSeconds(0.5).TotalMilliseconds;
            _RefreshTimer.Start();
        }

        public void SetAutoDetecting(bool autoDetect)
        {
            _AutoDetect = autoDetect;
            UpdateTimerState();
        }

        public void SetVisible(bool visible)
        {
            _Visible = visible;
            UpdateTimerState();
            UpdateConfig(Clients);

            if (!_Visible) return;

            RefreshDofusProcesses();
        }

        private void RefreshDofusProcesses()
        {
            _DofusProcesses = new List<Process>();
            List<Process> processes =
                Process.GetProcesses().Where(s => s.ProcessName.ToLowerInvariant().Contains("dofus")).ToList();
            foreach (var process in processes)
            {
                if ("DofusSwap" == process.ProcessName) continue;

                var windowTitleName = process.MainWindowTitle.ToLowerInvariant();

                //If it only says "dofus [version number]" then its a client in character select, wait.
                //if (!windowTitleName.Contains("- dofus "))
                //{
                //    continue;
                //}

                _DofusProcesses.Add(process);
            }
        }

        private void UpdateTimerState()
        {
            if (_Visible && _AutoDetect)
            {
                _RefreshTimer.Start();
                RefreshTimerOnTick(null, EventArgs.Empty);
            }
            else
            {
                _RefreshTimer.Stop();
            }

        }

        private void RefreshTimerOnTick(object sender, EventArgs e)
        {
            _RefreshTimer.Stop();

            RefreshDofusProcesses();

            foreach (var process in _DofusProcesses)
            {
                var windowTitleName = process.MainWindowTitle.ToLowerInvariant();

                bool newClientFound = true;

                foreach (var client in Clients)
                {
                    if (!windowTitleName.StartsWith(client.name.ToLowerInvariant())) continue;
                    newClientFound = false;
                    break;
                }

                if (!newClientFound) continue;

                string dofusCharacterName = process.MainWindowTitle.Split(" ".ToCharArray()).First();
                OnNewDofusClientDetected?.Invoke(dofusCharacterName);
            }

            _RefreshTimer.Start();
        }

        public void UpdateConfig(List<DofusClientData> clients = null)
        {
            if(!IsInit) return;

            if (clients == null) clients = Clients;
            Clients = clients;

            var clientsJson = JsonConvert.SerializeObject(clients, Formatting.Indented);

            File.WriteAllText(CONFIG_FILE_PATH, clientsJson);
        }

        public void RefreshConfig()
        {
            var clientConfig = File.ReadAllText(CONFIG_FILE_PATH);
            Clients = JsonConvert.DeserializeObject<List<DofusClientData>>(clientConfig) ?? new List<DofusClientData>();

            foreach (var dofusClient in Clients)
            {
                dofusClient.KeyBind = Enum.TryParse(dofusClient.key, true, out Keys key) ? key : Keys.None;
            }
        }

        private DofusClientData GetClient(Keys key, out Process clientProcess)
        {
            clientProcess = null;

            foreach (var process in _DofusProcesses)
            {
                foreach (var dofusClient in Clients)
                {
                    if (dofusClient.KeyBind != key || !process.MainWindowTitle.Contains(dofusClient.name)) continue;

                    clientProcess = process;
                    return dofusClient;
                }
            }

            return null;
        }

        public bool HandleKeyDown(Keys keyPressed)
        {
            //Find the process that matches the key press
            var clientData = GetClient(keyPressed, out var clientProcess);
            if (clientData == null)
            {
                RefreshDofusProcesses();
                clientData = GetClient(keyPressed, out clientProcess);
                if (clientData == null) return false;
            }

            bool success = FocusProcessWindow(clientProcess);

            _NextCharIndex = Clients.IndexOf(clientData);

            OnClientFocused?.Invoke(clientData);

            //https://www.codeproject.com/Articles/7305/Keyboard-Events-Simulation-using-keybd-event-funct

            return success;

        }

        private bool FocusProcessWindow(Process clientProcess)
        {
            //OLD - Simulate alt key down
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

            bool success = true;
            success &= SetForegroundWindow(clientProcess.MainWindowHandle);
            SwitchToThisWindow(clientProcess.MainWindowHandle, true);
            success &= BringWindowToTop(clientProcess.MainWindowHandle);

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

            return success;
        }

        public bool CheckNextHotkeyAssignment(Keys key)
        {
            if (!_AwaitingNextHotkey)
            {
                return false;
            }

            _NextHotKey = key;

            OnNextHotkeySet?.Invoke(_NextHotKey);

            _NextHotkeyTimer.Stop();
            _NextHotkeyTimer.Dispose();
            _NextHotkeyTimer = null;

            File.WriteAllText(NextHotkeyPath, _NextHotKey.ToString());

            _AwaitingNextHotkey = false;

            return true;
        }

        public bool CheckNextHotkeyTrigger(Keys key)
        {
            if (_NextHotKey != key) return false;

            _NextCharIndex++;
            if (_NextCharIndex >= Clients.Count) _NextCharIndex = 0;

            var client = Clients[_NextCharIndex];

            IEnumerable<Process> processes = Process.GetProcesses().Where(s => s.ProcessName.ToLowerInvariant().Contains("dofus"));
            foreach (var process in processes)
            {
                if (!process.MainWindowTitle.StartsWith(client.name)) continue;

                return FocusProcessWindow(process);
            }

            return false;
        }

        public void StartAssignNextHotKey()
        {
            if (_AwaitingNextHotkey)
            {
                _AwaitingNextHotkey = false;
                _NextHotkeyTimer?.Stop();
                _NextHotkeyTimer?.Dispose();
                _NextHotkeyTimer = null;
                OnNextHotkeySet?.Invoke(_NextHotKey = Keys.None);
                return;
            }

            _NextHotkeyTimer = new Timer();
            _NextHotkeyTimer.Tick += (o, args) =>
            {
                OnNextHotkeySet?.Invoke(_NextHotKey);

                _NextHotkeyTimer.Stop();
                _NextHotkeyTimer = null;
                _AwaitingNextHotkey = false;
            };
            _NextHotkeyTimer.Interval = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            _NextHotkeyTimer.Start();

            _AwaitingNextHotkey = true;
        }
    }
}