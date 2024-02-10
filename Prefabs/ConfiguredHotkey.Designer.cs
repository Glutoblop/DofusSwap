
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
            this.ShiftOn = new System.Windows.Forms.CheckBox();
            this.ControlOn = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CharacterHotkeyButton
            // 
            this.CharacterHotkeyButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CharacterHotkeyButton.Location = new System.Drawing.Point(3, 8);
            this.CharacterHotkeyButton.Name = "CharacterHotkeyButton";
            this.CharacterHotkeyButton.Size = new System.Drawing.Size(122, 29);
            this.CharacterHotkeyButton.TabIndex = 7;
            this.CharacterHotkeyButton.Text = "[HOT_KEY]";
            this.CharacterHotkeyButton.UseVisualStyleBackColor = true;
            this.CharacterHotkeyButton.Click += new System.EventHandler(this.CharacterHotkeyButton_Click);
            // 
            // RemoveCharacterButton
            // 
            this.RemoveCharacterButton.Location = new System.Drawing.Point(249, 8);
            this.RemoveCharacterButton.Name = "RemoveCharacterButton";
            this.RemoveCharacterButton.Size = new System.Drawing.Size(32, 29);
            this.RemoveCharacterButton.TabIndex = 8;
            this.RemoveCharacterButton.Text = "X";
            this.RemoveCharacterButton.UseVisualStyleBackColor = true;
            this.RemoveCharacterButton.Click += new System.EventHandler(this.RemoveConfig_Click);
            // 
            // ShiftOn
            // 
            this.ShiftOn.AutoSize = true;
            this.ShiftOn.Location = new System.Drawing.Point(131, 15);
            this.ShiftOn.Name = "ShiftOn";
            this.ShiftOn.Size = new System.Drawing.Size(47, 17);
            this.ShiftOn.TabIndex = 9;
            this.ShiftOn.Text = "Shift";
            this.ShiftOn.UseVisualStyleBackColor = true;
            // 
            // ControlOn
            // 
            this.ControlOn.AutoSize = true;
            this.ControlOn.Location = new System.Drawing.Point(184, 15);
            this.ControlOn.Name = "ControlOn";
            this.ControlOn.Size = new System.Drawing.Size(59, 17);
            this.ControlOn.TabIndex = 10;
            this.ControlOn.Text = "Control";
            this.ControlOn.UseVisualStyleBackColor = true;
            // 
            // ConfiguredHotkey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ControlOn);
            this.Controls.Add(this.ShiftOn);
            this.Controls.Add(this.CharacterHotkeyButton);
            this.Controls.Add(this.RemoveCharacterButton);
            this.Name = "ConfiguredHotkey";
            this.Size = new System.Drawing.Size(284, 45);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button CharacterHotkeyButton;
        private System.Windows.Forms.Button RemoveCharacterButton;
        private System.Windows.Forms.CheckBox ShiftOn;
        private System.Windows.Forms.CheckBox ControlOn;
    }
}
