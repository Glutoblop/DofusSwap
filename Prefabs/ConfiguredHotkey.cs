using System;
using System.Windows.Forms;
using DofusSwap.UI;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredHotkey : UserControl
    {
        public Action<ConfiguredHotkey> OnModified { get; set; }
        public Action<ConfiguredHotkey> OnDeleted { get; set; }
        public Action<ConfiguredHotkey> OnAssignRequested { get; set; }

        private Keys _key = Keys.None;
        public Keys Key => _key;

        private bool _requireShift;
        private bool _requireControl;
        private bool _requireAlt;

        public bool RequireShift => _requireShift;
        public bool RequireControl => _requireControl;
        public bool RequireAlt => _requireAlt;

        public void SetRequireShift(bool require) => _requireShift = require;
        public void SetRequireControl(bool require) => _requireControl = require;
        public void SetRequireAlt(bool require) => _requireAlt = require;

        public ConfiguredHotkey()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private void RemoveConfig_Click(object sender, EventArgs e)
        {
            OnDeleted?.Invoke(this);
        }

        public void SetHotkey(Keys key)
        {
            _key = key;
            UpdateDisplay();
        }

        public void SetAll(Keys key, bool shift, bool control, bool alt)
        {
            _key = key;
            _requireShift = shift;
            _requireControl = control;
            _requireAlt = alt;
            UpdateDisplay();
            OnModified?.Invoke(this);
        }

        private void UpdateDisplay()
        {
            CharacterHotkeyButton.Text = _key == Keys.None
                ? "[ NOT ASSIGNED ]"
                : $"[ {HotkeyAssignOverlay.FormatKeyCombo(_key, _requireShift, _requireControl, _requireAlt)} ]";
        }

        private void CharacterHotkeyButton_Click(object sender, EventArgs e)
        {
            OnAssignRequested?.Invoke(this);
        }
    }
}
