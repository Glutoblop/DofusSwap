using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredHotkey : UserControl
    {
        public Action<ConfiguredHotkey> OnModified { get; set; }
        public Action<ConfiguredHotkey> OnDeleted { get; set; }

        public Keys Key => Enum.TryParse(CharacterHotkeyButton.Text, true, out Keys key) ? key : Keys.None;

        public bool RequireShift => ShiftOn.Checked;
        public void SetRequireShift(bool require) => ShiftOn.Checked = require;
        public bool RequireControl => ControlOn.Checked;
        public void SetRequireControl(bool require) => ControlOn.Checked = require;

        private bool _WaitingForKeyPress = false;
        private Keys _Keyhit = Keys.None;

        public ConfiguredHotkey()
        {
            InitializeComponent();
            SetHotkey(Keys.None);
        }

        private void RemoveConfig_Click(object sender, EventArgs e)
        {
            OnDeleted?.Invoke(this);
        }

        public void SetHotkey(Keys key)
        {
            CharacterHotkeyButton.Text = key == Keys.None ? "[ NOT ASSIGNED ] " : key.ToString();
        }

        private async void CharacterHotkeyButton_Click(object sender, EventArgs e)
        {
            if (_WaitingForKeyPress) return;
            _WaitingForKeyPress = true;

            var cachedKey = Key;
            _Keyhit = Keys.None;

            CharacterHotkeyButton.Text = "Press Key..";

            var keyPressTask = Task.Factory.StartNew(() =>
            {
                var endTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                while (true)
                {
                    if (_Keyhit != Keys.None)
                    {
                        return;
                    }

                    //If 5 seconds have passed, timeout
                    if (endTime < DateTime.UtcNow) return;
                }
            });

            await keyPressTask;

            if (_Keyhit != Keys.None)
            {
                SetHotkey(_Keyhit);
                _Keyhit = Keys.None;
                OnModified?.Invoke(this);
            }
            else
            {
                SetHotkey(cachedKey);
            }

            _WaitingForKeyPress = false;
        }

        public bool OnKeyPressed(Keys key)
        {
            if (!_WaitingForKeyPress) return false;
            _Keyhit = key;
            return true;
        }
    }
}
