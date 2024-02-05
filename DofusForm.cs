using DofusSwap.Dofus;
using DofusSwap.Prefabs;
using DofusSwap.Tray;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DofusSwap.KeyboardHook;
using System.IO;

namespace DofusSwap
{
    public partial class DofusForm : Form
    {
        #region GLOBAL HOOK
        

        #endregion

        private TrayManager _TrayManager;
        private KeyboardManager _KeyboardManager;
        private DofusClientManager _DofusClientManager;

        private bool _Initialising = false;
        private List<ConfiguredCharacter> _ActiveCharacters = new List<ConfiguredCharacter>();

        private int _FocusedIndex = 0;

        public DofusForm()
        {
            _Initialising = true;

            _TrayManager = new TrayManager();
            _TrayManager.OnVisbilityToggled += TrayManagerOnOnVisbilityToggled;

            _KeyboardManager = new KeyboardManager();
            _KeyboardManager.OnKeyPressed += OnKeyboardHookPress;

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
                AddCharacter(client.name, client.KeyBind);
            }

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Text = $"Dofus Swap - {version}";

            _Initialising = false;
        }

        private void AddCharacter(string displayName, Keys key)
        {
            var configuredCharacter = new ConfiguredCharacter();
            configuredCharacter.SetDisplayName(displayName);
            configuredCharacter.SetHotkey(key);
            ActiveCharacters.Controls.Add(configuredCharacter);

            configuredCharacter.OnModified += character => { UpdateConfigs(); };

            configuredCharacter.OnDeleted += deletedCharacter =>
            {
                _ActiveCharacters.Remove(deletedCharacter);
                ActiveCharacters.Controls.Remove(deletedCharacter);
            };

            _ActiveCharacters.Add(configuredCharacter);
            
            AddCharacterButton.Enabled = _ActiveCharacters.Count < 8;

            UpdateConfigs();
        }

        private void UpdateConfigs()
        {
            if (_Initialising) return;

            List<DofusClientData> clients = new List<DofusClientData>();
            foreach (var activeCharacter in _ActiveCharacters)
            {
                clients.Add(new DofusClientData
                {
                    KeyBind = activeCharacter.Key,
                    key = activeCharacter.Key.ToString(),
                    name = activeCharacter.DisplayName
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
            Visible = false;
        }

        private void TrayManagerOnOnVisbilityToggled(bool vis)
        {
            Visible = vis;
        }

        private void OnKeyboardHookPress(Keys key)
        {
            if (Visible)
            {
                foreach (var configuredCharacter in _ActiveCharacters)
                {
                    if (configuredCharacter.OnKeyPressed(key))
                    {
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Key pressed: {key}");
                _DofusClientManager.HandleKeyDown(key);

            }
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
        }

        #endregion

        private void AddCharacterButton_Click(object sender, EventArgs e)
        {
            if (_ActiveCharacters.Count == 8) return;
            AddCharacter("", Keys.None);
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

#if !DEBUG
                
#endif
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
