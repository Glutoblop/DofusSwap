using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DofusSwap.UI
{
    public class HotkeyAssignOverlay : Panel
    {
        private readonly Panel _dialog;
        private readonly Label _titleLabel;
        private readonly Label _contextLabel;
        private readonly Label _keyLabel;
        private readonly Panel _keyPanel;
        private readonly CheckBox _shiftCheckBox;
        private readonly CheckBox _controlCheckBox;
        private readonly CheckBox _altCheckBox;
        private readonly Button _confirmButton;
        private readonly Button _cancelButton;
        private readonly Button _clearButton;

        private Keys _capturedKey = Keys.None;
        private Action<Keys, bool, bool, bool> _onConfirm;

        public HotkeyAssignOverlay()
        {
            Visible = false;
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(20, 20, 34);

            _dialog = new Panel
            {
                Size = new Size(340, 260),
                BackColor = AppTheme.Surface,
            };
            _dialog.Paint += (s, e) =>
            {
                using (var pen = new Pen(AppTheme.Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, _dialog.Width - 1, _dialog.Height - 1);
            };

            _titleLabel = new Label
            {
                Text = "Assign Hotkey",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(24, 18),
                BackColor = Color.Transparent,
            };

            _contextLabel = new Label
            {
                Font = new Font("Segoe UI", 9f),
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(24, 44),
                BackColor = Color.Transparent,
            };

            _keyPanel = new Panel
            {
                Location = new Point(24, 74),
                Size = new Size(292, 50),
                BackColor = AppTheme.InputBg,
            };
            _keyPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(AppTheme.Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, _keyPanel.Width - 1, _keyPanel.Height - 1);
            };

            _keyLabel = new Label
            {
                Text = "Press any key...",
                Font = new Font("Segoe UI", 11f),
                ForeColor = AppTheme.Text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            };
            _keyPanel.Controls.Add(_keyLabel);

            _shiftCheckBox = CreateCheckBox("Shift", 24, 140);
            _controlCheckBox = CreateCheckBox("Control", 110, 140);
            _altCheckBox = CreateCheckBox("Alt", 210, 140);

            _confirmButton = CreateButton("Confirm", 24, 185, false);
            _confirmButton.Enabled = false;
            _confirmButton.Click += (s, e) =>
            {
                var callback = _onConfirm;
                Hide();
                callback?.Invoke(_capturedKey, _shiftCheckBox.Checked, _controlCheckBox.Checked, _altCheckBox.Checked);
            };

            _clearButton = CreateButton("Clear", 130, 185, true);
            _clearButton.Click += (s, e) =>
            {
                var callback = _onConfirm;
                Hide();
                callback?.Invoke(Keys.None, false, false, false);
            };

            _cancelButton = CreateButton("Cancel", 236, 185, false);
            _cancelButton.Click += (s, e) => Hide();

            _dialog.Controls.Add(_titleLabel);
            _dialog.Controls.Add(_contextLabel);
            _dialog.Controls.Add(_keyPanel);
            _dialog.Controls.Add(_shiftCheckBox);
            _dialog.Controls.Add(_controlCheckBox);
            _dialog.Controls.Add(_altCheckBox);
            _dialog.Controls.Add(_confirmButton);
            _dialog.Controls.Add(_clearButton);
            _dialog.Controls.Add(_cancelButton);

            Controls.Add(_dialog);
            Resize += (s, e) => CenterDialog();
        }

        public void ShowForAssignment(string context, Keys currentKey, bool shift, bool control, bool alt, Action<Keys, bool, bool, bool> onConfirm)
        {
            _onConfirm = onConfirm;
            _contextLabel.Text = context;
            _capturedKey = currentKey;
            _shiftCheckBox.Checked = shift;
            _controlCheckBox.Checked = control;
            _altCheckBox.Checked = alt;
            UpdateKeyDisplay();
            _confirmButton.Enabled = _capturedKey != Keys.None;
            CenterDialog();
            Visible = true;
            BringToFront();
        }

        public new void Hide()
        {
            Visible = false;
            _onConfirm = null;
        }

        public bool HandleKeyPress(Keys key, bool shift, bool control, bool alt)
        {
            if (!Visible) return false;
            if (IsModifierKey(key)) return true;

            _capturedKey = key;
            _shiftCheckBox.Checked = shift;
            _controlCheckBox.Checked = control;
            _altCheckBox.Checked = alt;
            UpdateKeyDisplay();
            _confirmButton.Enabled = true;
            return true;
        }

        public static string FormatKeyCombo(Keys key, bool shift, bool control, bool alt)
        {
            if (key == Keys.None) return "NOT SET";
            var parts = new List<string>();
            if (control) parts.Add("Ctrl");
            if (shift) parts.Add("Shift");
            if (alt) parts.Add("Alt");
            parts.Add(key.ToString("G"));
            return string.Join("+", parts);
        }

        private void UpdateKeyDisplay()
        {
            _keyLabel.Text = _capturedKey == Keys.None
                ? "Press any key..."
                : FormatKeyCombo(_capturedKey, _shiftCheckBox.Checked, _controlCheckBox.Checked, _altCheckBox.Checked);
        }

        private void CenterDialog()
        {
            _dialog.Location = new Point(
                (Width - _dialog.Width) / 2,
                (Height - _dialog.Height) / 2);
        }

        private CheckBox CreateCheckBox(string text, int x, int y)
        {
            var chk = new CheckBox
            {
                Text = text,
                AutoSize = true,
                Location = new Point(x, y),
                ForeColor = AppTheme.TextMuted,
                Font = new Font("Segoe UI", 9f),
                BackColor = Color.Transparent,
            };
            chk.CheckedChanged += (s, e) => UpdateKeyDisplay();
            return chk;
        }

        private static Button CreateButton(string text, int x, int y, bool danger)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(80, 32),
            };
            AppTheme.StyleButton(btn, danger);
            return btn;
        }

        private static bool IsModifierKey(Keys key)
        {
            return key == Keys.ShiftKey || key == Keys.LShiftKey || key == Keys.RShiftKey
                || key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey
                || key == Keys.Menu || key == Keys.LMenu || key == Keys.RMenu;
        }
    }
}
