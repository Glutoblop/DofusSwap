using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredCharacter : UserControl
    {
        public event Action<ConfiguredCharacter> OnModified;
        public event Action<ConfiguredCharacter> OnDeleted;

        public string DisplayName => CharacterLabel.Text;

        public Keys Key => Enum.TryParse(CharacterHotkeyButton.Text, true, out Keys key) ? key : Keys.None;

        private bool _WaitingForKeyPress = false;
        private Keys _Keyhit = Keys.None;

        public ConfiguredCharacter()
        {
            InitializeComponent();
            SetDisplayName("");
            SetHotkey(Keys.None);
        }

        public void SetDisplayName(string displayName)
        {
            if (displayName == "") displayName = "[ NOT ASSIGNED ]";
            CharacterLabel.Text = displayName;
        }

        public void SetHotkey(Keys key)
        {
            CharacterHotkeyButton.Text = key == Keys.None ? "[ NOT ASSIGNED ] " : key.ToString();
        }

        private void CharacterLabel_Leave(object sender, EventArgs e)
        {
            OnModified?.Invoke(this);
        }

        private void RemoveCharacterButton_Click(object sender, EventArgs e)
        {
            OnDeleted?.Invoke(this);
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