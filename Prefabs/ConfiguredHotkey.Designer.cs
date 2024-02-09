
namespace DofusSwap.Prefabs
{
    partial class ConfiguredHotkey
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
            this.RemoveCharacterButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CharacterHotkeyButton
            // 
            this.CharacterHotkeyButton.Location = new System.Drawing.Point(3, 8);
            this.CharacterHotkeyButton.Name = "CharacterHotkeyButton";
            this.CharacterHotkeyButton.Size = new System.Drawing.Size(175, 29);
            this.CharacterHotkeyButton.TabIndex = 7;
            this.CharacterHotkeyButton.Text = "[HOT_KEY]";
            this.CharacterHotkeyButton.UseVisualStyleBackColor = true;
            this.CharacterHotkeyButton.Click += new System.EventHandler(this.CharacterHotkeyButton_Click);
            // 
            // RemoveCharacterButton
            // 
            this.RemoveCharacterButton.Location = new System.Drawing.Point(185, 8);
            this.RemoveCharacterButton.Name = "RemoveCharacterButton";
            this.RemoveCharacterButton.Size = new System.Drawing.Size(32, 29);
            this.RemoveCharacterButton.TabIndex = 8;
            this.RemoveCharacterButton.Text = "X";
            this.RemoveCharacterButton.UseVisualStyleBackColor = true;
            this.RemoveCharacterButton.Click += new System.EventHandler(this.RemoveConfig_Click);
            // 
            // ConfiguredHotkey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CharacterHotkeyButton);
            this.Controls.Add(this.RemoveCharacterButton);
            this.Name = "ConfiguredHotkey";
            this.Size = new System.Drawing.Size(226, 45);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button CharacterHotkeyButton;
        private System.Windows.Forms.Button RemoveCharacterButton;
    }
}
