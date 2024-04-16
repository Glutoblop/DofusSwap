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

        private Dictionary<Keys, bool> _KeyDown = new Dictionary<Keys, bool>();

        public DofusForm()
        {
            _Initialising = true;

            _TrayManager = new TrayManager();
            _TrayManager.OnVisbilityToggled += TrayManagerOnOnVisibilityToggled;

            _KeyboardManager = new KeyboardManager();
            _KeyboardManager.OnKeyPressed += OnKeyboardHookPress;
            _KeyboardManager.OnKeyReleased += OnKeyboardHookReleased;

            _DofusClientManager = new DofusClientManager();

            _TrayManager.Init();
            _DofusClientManager.Init();

            KeyPreview = true;

            Shown += OnShown;
            Closed += OnClosed;

            InitializeComponent();

            _DofusClientManager.RefreshConfig();

            foreach (var client in _DofusClientManager.Clients)
            {
                AddCharacter(client.name, client.KeyBind, client.shift, client.control);
            }

            CheckUseOfF4();

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

        private void CheckUseOfF4()
        {
            foreach (var client in _DofusClientManager.Clients)
            {
                if (client.KeyBind != Keys.F4) continue;
                _KeyboardManager.ConsumeAlt = true;
                return;
            }
            _KeyboardManager.ConsumeAlt = false;
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
                CheckUseOfF4();
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
                CheckUseOfF4();
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
            if (_KeyDown[key]) return false;
            _KeyDown[key] = true;

            if (Visible)
            {
                foreach (var hotkey in _ActiveHotkeys)
                {
                    if (hotkey.OnKeyPressed(key))
                    {
                        break;
                    }
                }
            }
            else
            {
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
            return false;
#endif
        }

        private bool OnKeyboardHookReleased(Keys key)
        {
            if (!_KeyDown[key]) return false;

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
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            _TrayManager.VisibilityChanged(Visible = false);
            _DofusClientManager.RefreshProcessList();
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

    }
}
