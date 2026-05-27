using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredHotkey : UserControl
    {
        public Action<ConfiguredHotkey> OnModified { get; set; }
        public Action<ConfiguredHotkey> OnDeleted { get; set; }

        private Keys _key = Keys.None;
        public Keys Key => _key;

        public bool RequireShift => ShiftOn.Checked;
        public void SetRequireShift(bool require) => ShiftOn.Checked = require;
        public bool RequireControl => ControlOn.Checked;
        public void SetRequireControl(bool require) => ControlOn.Checked = require;

        private bool _WaitingForKeyPress;
        private TaskCompletionSource<Keys> _keyAssignment;

        public ConfiguredHotkey()
        {
            InitializeComponent();
            SetHotkey(Keys.None);
            ShiftOn.CheckedChanged += (s, e) => OnModified?.Invoke(this);
            ControlOn.CheckedChanged += (s, e) => OnModified?.Invoke(this);
        }

        private void RemoveConfig_Click(object sender, EventArgs e)
        {
            OnDeleted?.Invoke(this);
        }

        public void SetHotkey(Keys key)
        {
            _key = key;
            CharacterHotkeyButton.Text = key == Keys.None ? "[ NOT ASSIGNED ]" : key.ToString();
        }

        private async void CharacterHotkeyButton_Click(object sender, EventArgs e)
        {
            if (_WaitingForKeyPress) return;
            _WaitingForKeyPress = true;

            var cachedKey = _key;
            CharacterHotkeyButton.Text = "Press Key..";

            _keyAssignment = new TaskCompletionSource<Keys>();
            var timeout = Task.Delay(10000);
            var completed = await Task.WhenAny(_keyAssignment.Task, timeout);

            if (completed == _keyAssignment.Task)
            {
                SetHotkey(_keyAssignment.Task.Result);
                OnModified?.Invoke(this);
            }
            else
            {
                SetHotkey(cachedKey);
            }

            _keyAssignment = null;
            _WaitingForKeyPress = false;
        }

        public bool OnKeyPressed(Keys key)
        {
            if (!_WaitingForKeyPress || _keyAssignment == null) return false;
            _keyAssignment.TrySetResult(key);
            return true;
        }
    }
}
