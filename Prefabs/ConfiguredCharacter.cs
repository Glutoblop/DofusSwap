using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DofusSwap.Draggable;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredCharacter : UserControl
    {
        
        public event Action<ConfiguredCharacter> OnSelected;
        public event Action<ConfiguredCharacter> OnModified;
        public event Action<ConfiguredCharacter> OnDeleted;

        public string DisplayName => CharacterLabel.Text;

        public Keys Key => Enum.TryParse(CharacterHotkeyButton.Text, true, out Keys key) ? key : Keys.None;

        private bool _WaitingForKeyPress = false;
        private Keys _Keyhit = Keys.None;

        public RichTextBox NameLabel => CharacterLabel;
        
        public ConfiguredCharacter()
        {
            this.Draggable(true);
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

        private void CharacterLabel_MouseClick(object sender, MouseEventArgs e)
        {
            OnSelected?.Invoke(this);
        }

        private bool _MouseSelected = false;

        private void DragSelect_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is ConfiguredCharacter)
            {

            }
        }

        private void ConfiguredCharacter_MouseUp(object sender, MouseEventArgs e)
        {
            _MouseSelected = false;
        }

        private void ConfiguredCharacter_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseSelected = true;
        }

        private void ConfiguredCharacter_MouseMove(object sender, MouseEventArgs e)
        {
            if (_MouseSelected)
            {
                if (sender is ConfiguredCharacter cc)
                {
                    cc.Location = new Point(0, cc.Location.Y);
                }

            }

        }
    }
}