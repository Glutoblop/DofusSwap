using DofusSwap.Dofus;
using DofusSwap.KeyboardHook;
using DofusSwap.Prefabs;
using DofusSwap.Tray;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DofusSwap
{
    public partial class DofusForm : Form
    {
        private TrayManager _TrayManager;
        private KeyboardManager _KeyboardManager;
        private DofusClientManager _DofusClientManager;

        private bool _Initialising = false;
        private List<ConfiguredCharacterName> _ActiveCharacters = new List<ConfiguredCharacterName>();
        private List<ConfiguredHotkey> _ActiveHotkeys = new List<ConfiguredHotkey>();

        private int _FocusedIndex = 0;

        private bool _AutoDetect = true;
        private const string AutodetectPath = "autodetect.txt";
        
        private Dictionary<Keys, bool> _KeyDown = new Dictionary<Keys, bool>();

        public DofusForm()
        {
            _Initialising = true;

            if(!File.Exists(AutodetectPath)) File.WriteAllText(AutodetectPath, "true");
            _AutoDetect = bool.Parse(File.ReadAllText(AutodetectPath));

            _TrayManager = new TrayManager();
            _TrayManager.OnVisbilityToggled += TrayManagerOnOnVisibilityToggled;

            _KeyboardManager = new KeyboardManager();
            _KeyboardManager.OnKeyPressed += OnKeyboardHookPress;
            _KeyboardManager.OnKeyReleased += OnKeyboardHookReleased;

            _DofusClientManager = new DofusClientManager();
            _DofusClientManager.OnSimulatingAltIsPressed += (simAltPressed) =>
            {
                _KeyboardManager.ConsumeAlt = simAltPressed;
            };

            _DofusClientManager.OnNewDofusClientDetected += (dofusCharacterName) =>
            {
                AddCharacter(dofusCharacterName,Keys.None, false, false);
            };

            _DofusClientManager.OnClientFocused += (clientFocused) =>
            {
                //ignored
            };

            _DofusClientManager.OnNextHotkeySet += (nextHotkey) =>
            {
                NextCharacterHotkey.Text = nextHotkey == Keys.None ? "Next Char Hotkey" : $"[ {nextHotkey:G} ]";
            };

            InitializeComponent();

            _TrayManager.Init();
            _DofusClientManager.Init();

            KeyPreview = true;

            Shown += OnShown;
            Closed += OnClosed;

            UpdateAutodetect();

            _DofusClientManager.RefreshConfig();

            foreach (var client in _DofusClientManager.Clients)
            {
                AddCharacter(client.name, client.KeyBind, client.shift, client.control);
            }

            foreach (Keys key in Enum.GetValues(typeof(Keys)).OfType<Keys>())
            {
                if (_KeyDown.ContainsKey(key)) continue;
                _KeyDown.Add(key,false);
            }

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Text = $"Dofus Swap - {version}";

            _Initialising = false;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void AddCharacter(string displayName, Keys key, bool shift, bool control)
        {
            // --- ADD CHARACTER
            var configuredCharacter = new ConfiguredCharacterName();
            configuredCharacter.SetDisplayName(displayName);
            configuredCharacter.Location = new Point(0, _ActiveCharacters.Count * configuredCharacter.Size.Height);
            configuredCharacter.UpdateIndex();

            configuredCharacter.OnSelected += character =>
            {
                _FocusedIndex = _ActiveCharacters.IndexOf(character);
            };

            configuredCharacter.OnModified += character =>
            {
                UpdateConfigs();
            };

            configuredCharacter.OnMovedIndex += (character, oldindex, newindex) =>
            {
                if (newindex >= _ActiveCharacters.Count) return;

                //Move the character in the new index, into the old index.
                var replaced = _ActiveCharacters[newindex];
                _ActiveCharacters[newindex] = character;
                _ActiveCharacters[oldindex] = replaced;

                for (var i = 0; i < _ActiveCharacters.Count; i++)
                {
                    var activeChar = _ActiveCharacters[i];
                    if (activeChar == character) continue;
                    activeChar.Location = new Point(0, i * activeChar.Size.Height);
                }
            };

            configuredCharacter.OnDropped += (character) =>
            {
                for (var i = 0; i < _ActiveCharacters.Count; i++)
                {
                    var activeChar = _ActiveCharacters[i];
                    activeChar.Location = new Point(0, i * activeChar.Size.Height);
                    activeChar.UpdateIndex();
                }
                UpdateConfigs();
            };

            _ActiveCharacters.Add(configuredCharacter);
            ActiveCharacters.Controls.Add(configuredCharacter);

            // --- ADD HOTKEY

            var hotkey = new ConfiguredHotkey();
            hotkey.SetRequireShift(shift);
            hotkey.SetRequireControl(control);
            hotkey.SetHotkey(key);
            hotkey.Location = new Point(0, _ActiveHotkeys.Count * configuredCharacter.Size.Height);

            hotkey.OnModified += modifiedHotkey =>
            {
                //Ignored for now
            };

            hotkey.OnDeleted += deletedHotkey =>
            {
                var index = _ActiveHotkeys.IndexOf(deletedHotkey);
                _ActiveHotkeys.RemoveAt(index);
                ActiveHotkeys.Controls.Remove(deletedHotkey);

                var character = _ActiveCharacters[index];
                _ActiveCharacters.RemoveAt(index);
                ActiveCharacters.Controls.Remove(character);

                for (var i = 0; i < _ActiveCharacters.Count; i++)
                {
                    var activeChar = _ActiveCharacters[i];
                    activeChar.Location = new Point(0, i * activeChar.Size.Height);
                    activeChar.UpdateIndex();

                    _ActiveHotkeys[i].Location = new Point(0, i * configuredCharacter.Size.Height);
                }

                UpdateConfigs();
            };
            _ActiveHotkeys.Add(hotkey);
            ActiveHotkeys.Controls.Add(hotkey);

            AddCharacterButton.Enabled = _ActiveCharacters.Count < 8;

            UpdateConfigs();
        }

        private void UpdateConfigs()
        {
            if (_Initialising) return;

            List<DofusClientData> clients = new List<DofusClientData>();
            for (var index = 0; index < _ActiveCharacters.Count; index++)
            {
                var activeCharacter = _ActiveCharacters[index];
                var hotkey = _ActiveHotkeys[index];

                clients.Add(new DofusClientData
                {
                    KeyBind = hotkey.Key,
                    key = hotkey.Key.ToString(),
                    shift = hotkey.RequireShift,
                    control = hotkey.RequireControl,
                    name = activeCharacter.DisplayName,
                });
            }

            _DofusClientManager.UpdateConfig(clients);
            _DofusClientManager.RefreshConfig();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _TrayManager?.Stop();
        }

        private void OnShown(object sender, EventArgs e)
        {
            //Visible = false;
        }

        private void TrayManagerOnOnVisibilityToggled(bool vis)
        {
            Visible = vis;

            if (!vis) return;
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private bool OnKeyboardHookPress(Keys key)
        {
            _KeyDown[key] = true;

            if (Visible)
            {
                if (_DofusClientManager.CheckNextHotkeyAssignment(key))
                {
                    return false;
                }

                foreach (var hotkey in _ActiveHotkeys)
                {
                    if (hotkey.OnKeyPressed(key))
                    {
                        break;
                    }
                }

                //Don't need to do anything special here, want the keyboard to work as normal while the application is visible.
                return false;
            }
            else
            {
                if (_DofusClientManager.CheckNextHotkeyTrigger(key))
                {
                    return false;
                }

                var shift = _KeyDown[Keys.Shift] || _KeyDown[Keys.ShiftKey] || _KeyDown[Keys.LShiftKey] ||
                            _KeyDown[Keys.RShiftKey];
                var control = _KeyDown[Keys.Control] || _KeyDown[Keys.ControlKey] || _KeyDown[Keys.LControlKey] || _KeyDown[Keys.RControlKey];

                foreach (var hotkey in _ActiveHotkeys)
                {
                    if (hotkey.Key != key) continue;

                    if (hotkey.RequireShift && !shift)
                    {
#if DEBUG
                        Console.WriteLine($"Key pressed: {key} but required Shift and not pressed");
#endif
                        return false;
                    }

                    if (hotkey.RequireControl && !control)
                    {
#if DEBUG
                        Console.WriteLine($"Key pressed: {key} but required Control and not pressed");
#endif
                        return false;
                    }
                }

                
                return _DofusClientManager.HandleKeyDown(key);

            }

#if DEBUG
            Console.WriteLine($"Key pressed: {key}");
#endif
            return false;
        }

        private bool OnKeyboardHookReleased(Keys key)
        {
#if DEBUG
            Console.WriteLine($"Key released: {key}");
#endif
            _KeyDown[key] = false;

            return false;
        }
        
        #region Overrides of Form

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _TrayManager.VisibilityChanged(Visible = true);
            _DofusClientManager.SetVisible(true);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            _TrayManager.VisibilityChanged(Visible = false);
            _DofusClientManager.SetVisible(false);
        }

        #endregion

        private void AddCharacterButton_Click(object sender, EventArgs e)
        {
            if (_ActiveCharacters.Count == 8) return;
            AddCharacter("", Keys.None, false, false);
        }

        private void DofusForm_Load(object sender, EventArgs e)
        {
            _KeyboardManager.SetHook();
        }

        private void DofusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _KeyboardManager.UnHook();
        }

        private void ConfigToolStrip_OnClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem ts && ts.Name == "ConfigToolMenuStripItem")
            {
                var dir = new FileInfo(DofusClientManager.CONFIG_FILE_PATH);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = dir.DirectoryName,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            UpdateConfigs();
        }

        private void DofusForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                UpdateConfigs();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                _FocusedIndex += 1;
                _FocusedIndex %= _ActiveCharacters.Count;
                _ActiveCharacters[_FocusedIndex].NameLabel.Select();
                _ActiveCharacters[_FocusedIndex].NameLabel.SelectAll();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void autoDetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _AutoDetect = !_AutoDetect;
            File.WriteAllText(AutodetectPath, _AutoDetect ? "true" : "false");

            UpdateAutodetect();
        }

        private void UpdateAutodetect()
        {
            AutoDetectMenuItem.Text = _AutoDetect ? "Auto Detecting" : "Manual Adding";
            _DofusClientManager.SetAutoDetecting(_AutoDetect);
        }

        private void NextCharacterHotkey_Click(object sender, EventArgs e)
        {
            NextCharacterHotkey.Text = $"Press Key..";
            _DofusClientManager.StartAssignNextHotKey();
        }
    }
}
