
namespace DofusSwap.Prefabs
{
    partial class ConfiguredCharacterName
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfiguredCharacterName));
            this.CharacterLabel = new System.Windows.Forms.RichTextBox();
            this.DragSelect = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DragSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // CharacterLabel
            // 
            this.CharacterLabel.AcceptsTab = true;
            this.CharacterLabel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.CharacterLabel.Location = new System.Drawing.Point(40, 10);
            this.CharacterLabel.Multiline = false;
            this.CharacterLabel.Name = "CharacterLabel";
            this.CharacterLabel.Size = new System.Drawing.Size(228, 25);
            this.CharacterLabel.TabIndex = 3;
            this.CharacterLabel.Text = "[ NOT ASSIGNED ]";
            this.CharacterLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CharacterLabel_MouseClick);
            this.CharacterLabel.Leave += new System.EventHandler(this.CharacterLabel_Leave);
            // 
            // DragSelect
            // 
            this.DragSelect.BackColor = System.Drawing.Color.Transparent;
            this.DragSelect.Enabled = false;
            this.DragSelect.Image = ((System.Drawing.Image)(resources.GetObject("DragSelect.Image")));
            this.DragSelect.Location = new System.Drawing.Point(0, 10);
            this.DragSelect.Name = "DragSelect";
            this.DragSelect.Size = new System.Drawing.Size(30, 21);
            this.DragSelect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DragSelect.TabIndex = 5;
            this.DragSelect.TabStop = false;
            // 
            // ConfiguredCharacterName
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DragSelect);
            this.Controls.Add(this.CharacterLabel);
            this.Name = "ConfiguredCharacterName";
            this.Size = new System.Drawing.Size(276, 45);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ConfiguredCharacter_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ConfiguredCharacter_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ConfiguredCharacter_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.DragSelect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox CharacterLabel;
        private System.Windows.Forms.PictureBox DragSelect;
    }
}
