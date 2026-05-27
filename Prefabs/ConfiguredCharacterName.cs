using System;
using System.Drawing;
using System.Windows.Forms;
using DofusSwap.Draggable;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredCharacterName : UserControl
    {
        public Action<ConfiguredCharacterName> OnSelected { get; set; }
        public Action<ConfiguredCharacterName, int, int> OnMovedIndex { get; set; }
        public Action<ConfiguredCharacterName> OnDropped { get; set; }
        public Action<ConfiguredCharacterName> OnModified { get; set; }

        private string _displayName = "";
        public string DisplayName => _displayName;

        public RichTextBox NameLabel => CharacterLabel;

        public int RowIndex;

        public ConfiguredCharacterName()
        {
            this.Draggable(true);
            InitializeComponent();
            SetDisplayName("");
        }

        public void SetDisplayName(string displayName)
        {
            _displayName = displayName ?? "";
            CharacterLabel.Text = string.IsNullOrEmpty(_displayName) ? "[ NOT ASSIGNED ]" : _displayName;
        }

        public void UpdateIndex()
        {
            RowIndex = Math.Max(0, (Location.Y + Size.Height / 2) / Size.Height);
        }

        private void CharacterLabel_Leave(object sender, EventArgs e)
        {
            var text = CharacterLabel.Text.Trim();
            _displayName = text == "[ NOT ASSIGNED ]" ? "" : text;
            OnModified?.Invoke(this);
        }

        private void CharacterLabel_MouseClick(object sender, MouseEventArgs e)
        {
            OnSelected?.Invoke(this);
        }

        private bool _MouseSelected;

        private void ConfiguredCharacter_MouseUp(object sender, MouseEventArgs e)
        {
            _MouseSelected = false;
            OnDropped?.Invoke(this);
        }

        private void ConfiguredCharacter_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseSelected = true;
        }

        private void ConfiguredCharacter_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MouseSelected) return;

            if (sender is ConfiguredCharacterName cc)
            {
                cc.Location = new Point(0, cc.Location.Y);
                int hoveredIndex = Math.Max(0, (Location.Y + Size.Height / 2) / Size.Height);

                if (hoveredIndex != RowIndex)
                {
                    var oldIndex = RowIndex;
                    RowIndex = hoveredIndex;
                    OnMovedIndex?.Invoke(this, oldIndex, hoveredIndex);
                }
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
