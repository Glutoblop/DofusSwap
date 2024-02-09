using System;
using System.Drawing;
using System.Windows.Forms;
using DofusSwap.Draggable;

namespace DofusSwap.Prefabs
{
    public partial class ConfiguredCharacterName : UserControl
    {
        
        public Action<ConfiguredCharacterName> OnSelected { get; set; }
        public Action<ConfiguredCharacterName> OnModified { get; set; }

        public string DisplayName => CharacterLabel.Text;

        public RichTextBox NameLabel => CharacterLabel;
        
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

        private void CharacterLabel_Leave(object sender, EventArgs e)
        {
            OnModified?.Invoke(this);
        }

        private void CharacterLabel_MouseClick(object sender, MouseEventArgs e)
        {
            OnSelected?.Invoke(this);
        }

        private bool _MouseSelected = false;

        private void DragSelect_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is ConfiguredCharacterName)
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
                if (sender is ConfiguredCharacterName cc)
                {
                    cc.Location = new Point(0, cc.Location.Y);
                }

            }

        }
    }
}