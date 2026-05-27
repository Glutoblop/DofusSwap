using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    internal class DofusClientManager
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [Flags]
        private enum KeyEventF : uint
        {
            KeyDown = 0x0000,
            KeyUp = 0x0002,
            Scancode = 0x0008
        }

        private enum InputType
        {
            Keyboard = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int dx, dy;
            public uint mouseData, dwFlags, time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public uint uMsg;
            public ushort wParamL, wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public HardwareInput hi;
        }

        private struct Input
        {
            public int type;
            public InputUnion u;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        private const ushort ALT_SCAN_CODE = 0xb8;

        public List<DofusClientData> Clients { get; private set; }

        private Timer _RefreshTimer;
        private bool _AutoDetect = true;
        private bool _Visible = true;

        public static string CONFIG_FILE_PATH { get; private set; } = "";

        private Keys _NextHotKey = Keys.None;
        private bool _NextHotKeyShift;
        private bool _NextHotKeyControl;
        private bool _NextHotKeyAlt;
        private const string NextHotkeyPath = "nexthotkey.txt";
        private int _NextCharIndex;

        private Keys _PrevHotKey = Keys.None;
        private bool _PrevHotKeyShift;
        private bool _PrevHotKeyControl;
        private bool _PrevHotKeyAlt;
        private const string PrevHotkeyPath = "prevhotkey.txt";

        private List<Process> _DofusProcesses = new List<Process>();
        private readonly Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

        public Action<bool> OnSimulatingAltIsPressed { get; set; }
        public Action<string> OnNewDofusClientDetected { get; set; }
        public Action<Keys, bool, bool, bool> OnNextHotkeySet { get; set; }
        public Action<Keys, bool, bool, bool> OnPrevHotkeySet { get; set; }

        private bool _IsInit;

        public void Init()
        {
            CONFIG_FILE_PATH = Path.Combine(Environment.CurrentDirectory, "dofusclients.json");

            if (!File.Exists(CONFIG_FILE_PATH))
            {
                using (File.CreateText(CONFIG_FILE_PATH)) { }
            }

            LoadCycleHotkey(NextHotkeyPath, out _NextHotKey, out _NextHotKeyShift, out _NextHotKeyControl, out _NextHotKeyAlt);
            LoadCycleHotkey(PrevHotkeyPath, out _PrevHotKey, out _PrevHotKeyShift, out _PrevHotKeyControl, out _PrevHotKeyAlt);

            _IsInit = true;

            OnNextHotkeySet?.Invoke(_NextHotKey, _NextHotKeyShift, _NextHotKeyControl, _NextHotKeyAlt);
            OnPrevHotkeySet?.Invoke(_PrevHotKey, _PrevHotKeyShift, _PrevHotKeyControl, _PrevHotKeyAlt);

            RefreshConfig();
            UpdateConfig(Clients);

            _RefreshTimer = new Timer();
            _RefreshTimer.Tick += RefreshTimerOnTick;
            _RefreshTimer.Interval = 500;
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

            if (_Visible)
                RefreshDofusProcesses();
        }

        private bool WindowTitleMatchesClient(string windowTitle, string clientName)
        {
            if (string.IsNullOrEmpty(clientName)) return false;

            if (!_regexCache.TryGetValue(clientName, out var regex))
            {
                var escaped = Regex.Escape(clientName);
                regex = new Regex($@"\b{escaped}\b", RegexOptions.Compiled);
                _regexCache[clientName] = regex;
            }

            return regex.IsMatch(windowTitle);
        }

        private void RefreshDofusProcesses()
        {
            foreach (var p in _DofusProcesses)
                p.Dispose();
            _DofusProcesses.Clear();

            var allProcesses = Process.GetProcesses();
            foreach (var process in allProcesses)
            {
                if (process.ProcessName.IndexOf("dofus", StringComparison.OrdinalIgnoreCase) < 0
                    || process.ProcessName.Equals("DofusSwap", StringComparison.OrdinalIgnoreCase))
                {
                    process.Dispose();
                    continue;
                }

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
                bool alreadyTracked = false;
                foreach (var dofusClient in Clients)
                {
                    if (WindowTitleMatchesClient(process.MainWindowTitle, dofusClient.name))
                    {
                        alreadyTracked = true;
                        break;
                    }
                }

                if (alreadyTracked) continue;

                string dofusCharacterName = process.MainWindowTitle.Split(' ').FirstOrDefault();
                if (string.IsNullOrEmpty(dofusCharacterName)) continue;
                OnNewDofusClientDetected?.Invoke(dofusCharacterName);
            }

            _RefreshTimer.Start();
        }

        public void UpdateConfig(List<DofusClientData> clients = null)
        {
            if (!_IsInit) return;

            if (clients != null) Clients = clients;

            _regexCache.Clear();

            var clientsJson = JsonConvert.SerializeObject(Clients, Formatting.Indented);
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
                    if (dofusClient.KeyBind != key) continue;
                    if (!WindowTitleMatchesClient(process.MainWindowTitle, dofusClient.name)) continue;

                    clientProcess = process;
                    return dofusClient;
                }
            }

            return null;
        }

        public bool HandleKeyDown(Keys keyPressed)
        {
            var clientData = GetClient(keyPressed, out var clientProcess);
            if (clientData == null)
            {
                RefreshDofusProcesses();
                clientData = GetClient(keyPressed, out clientProcess);
                if (clientData == null) return false;
            }

            bool success = FocusProcessWindow(clientProcess);
            _NextCharIndex = Clients.IndexOf(clientData);

            return success;
        }

        private bool FocusProcessWindow(Process clientProcess)
        {
            var altDown = new[]
            {
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = ALT_SCAN_CODE,
                            dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };

            OnSimulatingAltIsPressed?.Invoke(true);
            SendInput((uint)altDown.Length, altDown, Marshal.SizeOf(typeof(Input)));

            bool success = SetForegroundWindow(clientProcess.MainWindowHandle);
            SwitchToThisWindow(clientProcess.MainWindowHandle, true);
            success &= BringWindowToTop(clientProcess.MainWindowHandle);

            var altUp = new[]
            {
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = ALT_SCAN_CODE,
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

        private static void LoadCycleHotkey(string path, out Keys key, out bool shift, out bool control, out bool alt)
        {
            if (!File.Exists(path)) File.WriteAllText(path, Keys.None.ToString());
            var parts = File.ReadAllText(path).Split(',');
            key = (Keys)Enum.Parse(typeof(Keys), parts[0]);
            shift = parts.Length > 1 && bool.Parse(parts[1]);
            control = parts.Length > 2 && bool.Parse(parts[2]);
            alt = parts.Length > 3 && bool.Parse(parts[3]);
        }

        private static void SaveCycleHotkey(string path, Keys key, bool shift, bool control, bool alt)
        {
            File.WriteAllText(path, $"{key},{shift},{control},{alt}");
        }

        public void SetNextHotkey(Keys key, bool shift, bool control, bool alt)
        {
            _NextHotKey = key;
            _NextHotKeyShift = shift;
            _NextHotKeyControl = control;
            _NextHotKeyAlt = alt;
            SaveCycleHotkey(NextHotkeyPath, key, shift, control, alt);
            OnNextHotkeySet?.Invoke(key, shift, control, alt);
        }

        public void SetPrevHotkey(Keys key, bool shift, bool control, bool alt)
        {
            _PrevHotKey = key;
            _PrevHotKeyShift = shift;
            _PrevHotKeyControl = control;
            _PrevHotKeyAlt = alt;
            SaveCycleHotkey(PrevHotkeyPath, key, shift, control, alt);
            OnPrevHotkeySet?.Invoke(key, shift, control, alt);
        }

        public bool CheckNextHotkeyTrigger(Keys key, bool shift, bool control, bool alt)
        {
            if (_NextHotKey != key) return false;
            if (_NextHotKeyShift && !shift) return false;
            if (_NextHotKeyControl && !control) return false;
            if (_NextHotKeyAlt && !alt) return false;
            if (Clients == null || Clients.Count == 0) return false;

            RefreshDofusProcesses();

            var startingIndex = _NextCharIndex;
            do
            {
                _NextCharIndex = (_NextCharIndex + 1) % Clients.Count;
                var dofusClient = Clients[_NextCharIndex];

                foreach (var process in _DofusProcesses)
                {
                    if (WindowTitleMatchesClient(process.MainWindowTitle, dofusClient.name))
                        return FocusProcessWindow(process);
                }
            }
            while (startingIndex != _NextCharIndex);

            return false;
        }

        public bool CheckPrevHotkeyTrigger(Keys key, bool shift, bool control, bool alt)
        {
            if (_PrevHotKey != key) return false;
            if (_PrevHotKeyShift && !shift) return false;
            if (_PrevHotKeyControl && !control) return false;
            if (_PrevHotKeyAlt && !alt) return false;
            if (Clients == null || Clients.Count == 0) return false;

            RefreshDofusProcesses();

            var startingIndex = _NextCharIndex;
            do
            {
                _NextCharIndex = (_NextCharIndex - 1 + Clients.Count) % Clients.Count;
                var dofusClient = Clients[_NextCharIndex];

                foreach (var process in _DofusProcesses)
                {
                    if (WindowTitleMatchesClient(process.MainWindowTitle, dofusClient.name))
                        return FocusProcessWindow(process);
                }
            }
            while (startingIndex != _NextCharIndex);

            return false;
        }
    }
}
