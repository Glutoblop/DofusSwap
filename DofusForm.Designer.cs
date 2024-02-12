
namespace DofusSwap
{
    partial class DofusForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DofusForm));
            this.AddCharacterButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ConfigToolMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ActiveCharacters = new System.Windows.Forms.Panel();
            this.ActiveHotkeys = new System.Windows.Forms.Panel();
            this.checkUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddCharacterButton
            // 
            this.AddCharacterButton.Location = new System.Drawing.Point(12, 27);
            this.AddCharacterButton.Name = "AddCharacterButton";
            this.AddCharacterButton.Size = new System.Drawing.Size(106, 36);
            this.AddCharacterButton.TabIndex = 1;
            this.AddCharacterButton.Text = "Add Character";
            this.AddCharacterButton.UseVisualStyleBackColor = true;
            this.AddCharacterButton.Click += new System.EventHandler(this.AddCharacterButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConfigToolMenuStripItem,
            this.checkUpdate});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(604, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ConfigToolMenuStripItem
            // 
            this.ConfigToolMenuStripItem.Name = "ConfigToolMenuStripItem";
            this.ConfigToolMenuStripItem.Size = new System.Drawing.Size(52, 22);
            this.ConfigToolMenuStripItem.Text = "Folder";
            this.ConfigToolMenuStripItem.Click += new System.EventHandler(this.ConfigToolStrip_OnClick);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(486, 27);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(106, 36);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ActiveCharacters
            // 
            this.ActiveCharacters.Location = new System.Drawing.Point(12, 69);
            this.ActiveCharacters.Name = "ActiveCharacters";
            this.ActiveCharacters.Size = new System.Drawing.Size(280, 430);
            this.ActiveCharacters.TabIndex = 5;
            // 
            // ActiveHotkeys
            // 
            this.ActiveHotkeys.Location = new System.Drawing.Point(298, 69);
            this.ActiveHotkeys.Name = "ActiveHotkeys";
            this.ActiveHotkeys.Size = new System.Drawing.Size(280, 430);
            this.ActiveHotkeys.TabIndex = 6;
            // 
            // checkUpdate
            // 
            this.checkUpdate.Name = "checkUpdate";
            this.checkUpdate.Size = new System.Drawing.Size(57, 22);
            this.checkUpdate.Text = "Update";
            this.checkUpdate.Click += new System.EventHandler(this.checkUpdate_Click);
            // 
            // DofusForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 511);
            this.Controls.Add(this.ActiveHotkeys);
            this.Controls.Add(this.ActiveCharacters);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.AddCharacterButton);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "DofusForm";
            this.Text = "DofusSwap";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DofusForm_FormClosing);
            this.Load += new System.EventHandler(this.DofusForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DofusForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button AddCharacterButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ConfigToolMenuStripItem;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Panel ActiveCharacters;
        private System.Windows.Forms.Panel ActiveHotkeys;
        private System.Windows.Forms.ToolStripMenuItem checkUpdate;
    }
}

