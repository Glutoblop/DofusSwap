using DofusSwap.Dofus;
using DofusSwap.KeyboardHook;
using DofusSwap.Prefabs;
using DofusSwap.Tray;
using DofusSwap.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DofusSwap
{
    public partial class DofusForm : Form
    {
        private TrayManager _TrayManager;
        private KeyboardManager _KeyboardManager;
        private DofusClientManager _DofusClientManager;
        private HotkeyAssignOverlay _hotkeyOverlay;

        private bool _Initialising;
        private List<ConfiguredCharacterName> _ActiveCharacters = new List<ConfiguredCharacterName>();
        private List<ConfiguredHotkey> _ActiveHotkeys = new List<ConfiguredHotkey>();

        private int _FocusedIndex;

        private bool _AutoDetect = true;
        private const string AutodetectPath = "autodetect.txt";

        private readonly HashSet<Keys> _keysDown = new HashSet<Keys>();

        public DofusForm()
        {
            _Initialising = true;

            DoubleBuffered = true;

            if (!File.Exists(AutodetectPath)) File.WriteAllText(AutodetectPath, "true");
            _AutoDetect = bool.Parse(File.ReadAllText(AutodetectPath));

            _TrayManager = new TrayManager();
            _TrayManager.OnVisbilityToggled += TrayManagerOnOnVisibilityToggled;

            _KeyboardManager = new KeyboardManager();
            _KeyboardManager.OnKeyPressed += OnKeyboardHookPress;
            _KeyboardManager.OnKeyReleased += OnKeyboardHookReleased;

            _DofusClientManager = new DofusClientManager();
            _DofusClientManager.OnSimulatingAltIsPressed += simAltPressed =>
            {
                _KeyboardManager.ConsumeAlt = simAltPressed;
            };

            _DofusClientManager.OnNewDofusClientDetected += dofusCharacterName =>
            {
                AddCharacter(dofusCharacterName, Keys.None, false, false, false);
            };

            _DofusClientManager.OnNextHotkeySet += (key, shift, control, alt) =>
            {
                NextCharacterHotkey.Text = key == Keys.None
                    ? "Next Char Hotkey"
                    : HotkeyAssignOverlay.FormatKeyCombo(key, shift, control, alt);
            };

            _DofusClientManager.OnPrevHotkeySet += (key, shift, control, alt) =>
            {
                PrevCharacterHotkey.Text = key == Keys.None
                    ? "Prev Char Hotkey"
                    : HotkeyAssignOverlay.FormatKeyCombo(key, shift, control, alt);
            };

            InitializeComponent();

            _hotkeyOverlay = new HotkeyAssignOverlay();
            Controls.Add(_hotkeyOverlay);
            _hotkeyOverlay.BringToFront();

            AppTheme.Apply(this);
            AppTheme.EnableDoubleBuffering(ActiveCharacters);
            AppTheme.EnableDoubleBuffering(ActiveHotkeys);

            _TrayManager.Init();
            _DofusClientManager.Init();

            KeyPreview = true;
            Closed += OnClosed;

            UpdateAutodetect();

            _DofusClientManager.RefreshConfig();

            foreach (var client in _DofusClientManager.Clients)
            {
                AddCharacter(client.name, client.KeyBind, client.shift, client.control, client.alt);
            }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Text = $"Dofus Swap - {fvi.FileVersion}";

            _Initialising = false;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void AddCharacter(string displayName, Keys key, bool shift, bool control, bool alt)
        {
            var configuredCharacter = new ConfiguredCharacterName();
            configuredCharacter.SetDisplayName(displayName);
            configuredCharacter.Location = new Point(0, _ActiveCharacters.Count * configuredCharacter.Size.Height);
            configuredCharacter.UpdateIndex();
            AppTheme.ApplyToControl(configuredCharacter);

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

                var replaced = _ActiveCharacters[newindex];
                _ActiveCharacters[newindex] = character;
                _ActiveCharacters[oldindex] = replaced;

                for (var i = 0; i < _ActiveCharacters.Count; i++)
                {
                    var activeChar = _ActiveCharacters[i];
                    if (activeChar == character) continue;
                    Animator.SlideTo(activeChar, new Point(0, i * activeChar.Size.Height), 150);
                }
            };

            configuredCharacter.OnDropped += character =>
            {
                for (var i = 0; i < _ActiveCharacters.Count; i++)
                {
                    Animator.SlideTo(_ActiveCharacters[i], new Point(0, i * _ActiveCharacters[i].Size.Height), 150);
                    _ActiveCharacters[i].UpdateIndex();
                }
                UpdateConfigs();
            };

            _ActiveCharacters.Add(configuredCharacter);
            ActiveCharacters.Controls.Add(configuredCharacter);

            var hotkey = new ConfiguredHotkey();
            hotkey.SetRequireShift(shift);
            hotkey.SetRequireControl(control);
            hotkey.SetRequireAlt(alt);
            hotkey.SetHotkey(key);
            hotkey.Location = new Point(0, _ActiveHotkeys.Count * configuredCharacter.Size.Height);
            AppTheme.ApplyToControl(hotkey);

            hotkey.OnAssignRequested += requestedHotkey =>
            {
                var charIndex = _ActiveHotkeys.IndexOf(requestedHotkey);
                var charName = charIndex >= 0 && charIndex < _ActiveCharacters.Count
                    ? _ActiveCharacters[charIndex].DisplayName
                    : "";
                var context = string.IsNullOrEmpty(charName)
                    ? "for Character"
                    : $"for Character: {charName}";

                _hotkeyOverlay.ShowForAssignment(context,
                    requestedHotkey.Key, requestedHotkey.RequireShift, requestedHotkey.RequireControl, requestedHotkey.RequireAlt,
                    (k, s, c, a) =>
                    {
                        requestedHotkey.SetAll(k, s, c, a);
                        UpdateConfigs();
                    });
            };

            hotkey.OnModified += modifiedHotkey =>
            {
                UpdateConfigs();
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
                    Animator.SlideTo(_ActiveCharacters[i], new Point(0, i * _ActiveCharacters[i].Size.Height), 150);
                    _ActiveCharacters[i].UpdateIndex();
                    Animator.SlideTo(_ActiveHotkeys[i], new Point(0, i * _ActiveHotkeys[i].Size.Height), 150);
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

            var clients = new List<DofusClientData>();
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
                    alt = hotkey.RequireAlt,
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

        private void TrayManagerOnOnVisibilityToggled(bool vis)
        {
            if (vis)
            {
                Opacity = 0;
                Visible = true;
                WindowState = FormWindowState.Minimized;
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
                Animator.FadeTo(this, 1.0, 180);
            }
            else
            {
                Visible = false;
            }
        }

        private bool OnKeyboardHookPress(Keys key)
        {
            _keysDown.Add(key);

            var shift = _keysDown.Contains(Keys.ShiftKey)
                        || _keysDown.Contains(Keys.LShiftKey)
                        || _keysDown.Contains(Keys.RShiftKey);
            var control = _keysDown.Contains(Keys.ControlKey)
                          || _keysDown.Contains(Keys.LControlKey)
                          || _keysDown.Contains(Keys.RControlKey);
            var alt = _keysDown.Contains(Keys.Menu)
                      || _keysDown.Contains(Keys.LMenu)
                      || _keysDown.Contains(Keys.RMenu);

            if (Visible)
            {
                if (_hotkeyOverlay.HandleKeyPress(key, shift, control, alt))
                    return true;

                return false;
            }

            if (_DofusClientManager.CheckNextHotkeyTrigger(key, shift, control, alt))
                return true;

            if (_DofusClientManager.CheckPrevHotkeyTrigger(key, shift, control, alt))
                return true;

            bool isHotkey = false;

            foreach (var hotkey in _ActiveHotkeys)
            {
                if (hotkey.Key != key) continue;
                isHotkey = true;

                if (hotkey.RequireShift && !shift)
                    return false;

                if (hotkey.RequireControl && !control)
                    return false;

                if (hotkey.RequireAlt && !alt)
                    return false;
            }

            if (isHotkey)
                return _DofusClientManager.HandleKeyDown(key);

            return false;
        }

        private bool OnKeyboardHookReleased(Keys key)
        {
            _keysDown.Remove(key);
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
            AddCharacter("", Keys.None, false, false, false);
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
            var dir = new FileInfo(DofusClientManager.CONFIG_FILE_PATH);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = dir.DirectoryName,
                UseShellExecute = true,
                Verb = "open"
            });
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
                if (_hotkeyOverlay.Visible)
                    _hotkeyOverlay.Hide();
                else
                    WindowState = FormWindowState.Minimized;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_hotkeyOverlay.Visible)
                return true;

            if (keyData == Keys.Tab && _ActiveCharacters.Count > 0)
            {
                _FocusedIndex = (_FocusedIndex + 1) % _ActiveCharacters.Count;
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
            _hotkeyOverlay.ShowForAssignment("for Next Character Cycle",
                Keys.None, false, false, false,
                (key, shift, control, alt) => _DofusClientManager.SetNextHotkey(key, shift, control, alt));
        }

        private void PrevCharacterHotkey_Click(object sender, EventArgs e)
        {
            _hotkeyOverlay.ShowForAssignment("for Previous Character Cycle",
                Keys.None, false, false, false,
                (key, shift, control, alt) => _DofusClientManager.SetPrevHotkey(key, shift, control, alt));
        }
    }
}
