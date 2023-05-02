using DofusSwap.Dofus;
using DofusSwap.Prefabs;
using DofusSwap.Tray;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DofusSwap
{
    public partial class DofusForm : Form
    {
        private TrayManager _TrayManager;
        private DofusClientManager _DofusClientManager;
        private Hotkey[] _FunctionHotKeys;

        private List<ConfiguredCharacter> _ActiveCharacters = new List<ConfiguredCharacter>();

        public DofusForm()
        {
            _TrayManager = new TrayManager();
            _TrayManager.OnVisbilityToggled += TrayManagerOnOnVisbilityToggled;

            _DofusClientManager = new DofusClientManager();

            _TrayManager.Init();
            _DofusClientManager.Init();

            _FunctionHotKeys = new Hotkey[_DofusClientManager.Clients.Count];
            for (int i = 0; i < _FunctionHotKeys.Length; i++)
            {
                _FunctionHotKeys[i] = new Hotkey(Hotkey.Constants.NOMOD, _DofusClientManager.Clients[i].KeyBind, this);
                _FunctionHotKeys[i].Register();
            }

            KeyPreview = true;

            Shown += OnShown;
            Closed += OnClosed;

            InitializeComponent();

            _DofusClientManager.RefreshConfig();

            for (int i = 0; i < _DofusClientManager.Clients.Count; i++)
            {
                AddCharacter(_DofusClientManager.Clients[i].name, _DofusClientManager.Clients[i].KeyBind);
            }
        }

        private void AddCharacter(string displayName, Keys key)
        {
            if (_ActiveCharacters.Count == 8) return;

            var configuredCharacter = new ConfiguredCharacter();
            configuredCharacter.SetDisplayName(displayName);
            configuredCharacter.SetHotkey(key);
            ActiveCharacters.Controls.Add(configuredCharacter);

            configuredCharacter.OnModified += character =>
            {
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
            };

            configuredCharacter.OnDeleted += deletedCharacter =>
            {
                _ActiveCharacters.Remove(deletedCharacter);
                ActiveCharacters.Controls.Remove(deletedCharacter);
            };

            _ActiveCharacters.Add(configuredCharacter);

            AddCharacterButton.Enabled = _ActiveCharacters.Count < 8;
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

        private Keys GetKey(IntPtr LParam)
        {
            return (Keys)((LParam.ToInt32()) >> 16); // not all of the parenthesis are needed, I just found it easier to see what's happening
        }


        #region Overrides of Form

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            foreach (var configuredCharacter in _ActiveCharacters)
            {
                configuredCharacter.KeyPressed(e.KeyCode);
            }
        }


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Hotkey.Constants.WM_HOTKEY_MSG_ID)
            {
                Keys keyPressed = GetKey(m.LParam);
                Console.WriteLine($"[Hot Key Detected] {keyPressed}");
                _DofusClientManager.OnKeyDown(keyPressed);
            }

            base.WndProc(ref m);
        }

        #endregion
        
        private void AddCharacterButton_Click(object sender, EventArgs e)
        {
            AddCharacter("", Keys.None);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            //Unregistering hotkeys doesnt seem to work, so instead just restarting the application will release the hotkeys of old
            Application.Restart();
        }
    }
}
