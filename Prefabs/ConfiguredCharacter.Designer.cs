
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
            this.CharacterHotkeyButton = new System.Windows.Forms.Button();
            this.CharacterLabel = new System.Windows.Forms.RichTextBox();
            this.RemoveCharacterButton = new System.Windows.Forms.Button();
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
            this.CharacterLabel.Location = new System.Drawing.Point(7, 10);
            this.CharacterLabel.Multiline = false;
            this.CharacterLabel.Name = "CharacterLabel";
            this.CharacterLabel.Size = new System.Drawing.Size(272, 25);
            this.CharacterLabel.TabIndex = 3;
            this.CharacterLabel.Text = "[ NOT ASSIGNED ]";
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
            // ConfiguredCharacter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CharacterHotkeyButton);
            this.Controls.Add(this.RemoveCharacterButton);
            this.Controls.Add(this.CharacterLabel);
            this.Name = "ConfiguredCharacter";
            this.Size = new System.Drawing.Size(512, 45);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button CharacterHotkeyButton;
        private System.Windows.Forms.RichTextBox CharacterLabel;
        private System.Windows.Forms.Button RemoveCharacterButton;
    }
}
