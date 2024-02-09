using System;
using System.Drawing;
using System.Windows.Forms;
using DofusSwap.Draggable;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredCharacterName : UserControl
    {
        
        public Action<ConfiguredCharacterName> OnSelected { get; set; }
        /// <summary>This name has moved from index, to new index</summary>
        public Action<ConfiguredCharacterName, int, int> OnMovedIndex { get; set; }
        public Action<ConfiguredCharacterName> OnDropped { get; set; }
        public Action<ConfiguredCharacterName> OnModified { get; set; }

        public string DisplayName => CharacterLabel.Text;

        public RichTextBox NameLabel => CharacterLabel;
        
        public int RowIndex = 0;

        public ConfiguredCharacterName()
        {
            this.Draggable(true);
            InitializeComponent();
            SetDisplayName("");
        }

        public void SetDisplayName(string displayName)
        {
            if (displayName == "") displayName = "[ NOT ASSIGNED ]";
            CharacterLabel.Text = displayName;
        }

        public void UpdateIndex()
        {
            RowIndex = Math.Max(0,(Location.Y + Size.Height/2) / Size.Height);
        }

        private void CharacterLabel_Leave(object sender, EventArgs e)
        {
            OnModified?.Invoke(this);
        }

        private void CharacterLabel_MouseClick(object sender, MouseEventArgs e)
        {
            OnSelected?.Invoke(this);
        }

        private bool _MouseSelected = false;
        
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
            if (_MouseSelected)
            {
                if (sender is ConfiguredCharacterName cc)
                {
                    cc.Location = new Point(0, cc.Location.Y);

                    int hovered_index = Math.Max(0,(Location.Y + Size.Height/2) / Size.Height);

                    if (hovered_index != RowIndex)
                    {
                        var old_index = RowIndex;
                        RowIndex = hovered_index;
                        OnMovedIndex?.Invoke(this, old_index, hovered_index);
                    }
                }

            }

        }

        public override string ToString()
        {
            return $"{DisplayName}";
        }
    }
}