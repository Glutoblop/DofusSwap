
namespace DofusSwap.Prefabs
{
    partial class ConfiguredCharacter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfiguredCharacter));
            this.CharacterHotkeyButton = new System.Windows.Forms.Button();
            this.CharacterLabel = new System.Windows.Forms.RichTextBox();
            this.RemoveCharacterButton = new System.Windows.Forms.Button();
            this.DragSelect = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DragSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // CharacterHotkeyButton
            // 
            this.CharacterHotkeyButton.Location = new System.Drawing.Point(293, 7);
            this.CharacterHotkeyButton.Name = "CharacterHotkeyButton";
            this.CharacterHotkeyButton.Size = new System.Drawing.Size(175, 29);
            this.CharacterHotkeyButton.TabIndex = 2;
            this.CharacterHotkeyButton.Text = "[HOT_KEY]";
            this.CharacterHotkeyButton.UseVisualStyleBackColor = true;
            this.CharacterHotkeyButton.Click += new System.EventHandler(this.CharacterHotkeyButton_Click);
            // 
            // CharacterLabel
            // 
            this.CharacterLabel.AcceptsTab = true;
            this.CharacterLabel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.CharacterLabel.Location = new System.Drawing.Point(40, 10);
            this.CharacterLabel.Multiline = false;
            this.CharacterLabel.Name = "CharacterLabel";
            this.CharacterLabel.Size = new System.Drawing.Size(239, 25);
            this.CharacterLabel.TabIndex = 3;
            this.CharacterLabel.Text = "[ NOT ASSIGNED ]";
            this.CharacterLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CharacterLabel_MouseClick);
            this.CharacterLabel.Leave += new System.EventHandler(this.CharacterLabel_Leave);
            // 
            // RemoveCharacterButton
            // 
            this.RemoveCharacterButton.Location = new System.Drawing.Point(475, 7);
            this.RemoveCharacterButton.Name = "RemoveCharacterButton";
            this.RemoveCharacterButton.Size = new System.Drawing.Size(32, 29);
            this.RemoveCharacterButton.TabIndex = 4;
            this.RemoveCharacterButton.Text = "X";
            this.RemoveCharacterButton.UseVisualStyleBackColor = true;
            this.RemoveCharacterButton.Click += new System.EventHandler(this.RemoveCharacterButton_Click);
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
            this.DragSelect.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragSelect_DragDrop);
            // 
            // ConfiguredCharacter
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DragSelect);
            this.Controls.Add(this.CharacterHotkeyButton);
            this.Controls.Add(this.RemoveCharacterButton);
            this.Controls.Add(this.CharacterLabel);
            this.Name = "ConfiguredCharacter";
            this.Size = new System.Drawing.Size(512, 45);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ConfiguredCharacter_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ConfiguredCharacter_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ConfiguredCharacter_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.DragSelect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button CharacterHotkeyButton;
        private System.Windows.Forms.RichTextBox CharacterLabel;
        private System.Windows.Forms.Button RemoveCharacterButton;
        private System.Windows.Forms.PictureBox DragSelect;
    }
}
