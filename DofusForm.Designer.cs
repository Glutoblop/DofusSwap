
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
            this.ActiveCharacters = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ConfigToolMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddCharacterButton
            // 
            this.AddCharacterButton.Location = new System.Drawing.Point(24, 52);
            this.AddCharacterButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.AddCharacterButton.Name = "AddCharacterButton";
            this.AddCharacterButton.Size = new System.Drawing.Size(212, 69);
            this.AddCharacterButton.TabIndex = 1;
            this.AddCharacterButton.Text = "Add Character";
            this.AddCharacterButton.UseVisualStyleBackColor = true;
            this.AddCharacterButton.Click += new System.EventHandler(this.AddCharacterButton_Click);
            // 
            // ActiveCharacters
            // 
            this.ActiveCharacters.AutoScroll = true;
            this.ActiveCharacters.Location = new System.Drawing.Point(24, 133);
            this.ActiveCharacters.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ActiveCharacters.Name = "ActiveCharacters";
            this.ActiveCharacters.Size = new System.Drawing.Size(1160, 827);
            this.ActiveCharacters.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConfigToolMenuStripItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1208, 44);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ConfigToolMenuStripItem
            // 
            this.ConfigToolMenuStripItem.Name = "ConfigToolMenuStripItem";
            this.ConfigToolMenuStripItem.Size = new System.Drawing.Size(102, 36);
            this.ConfigToolMenuStripItem.Text = "Folder";
            this.ConfigToolMenuStripItem.Click += new System.EventHandler(this.ConfigToolStrip_OnClick);
            // 
            // DofusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1208, 983);
            this.Controls.Add(this.ActiveCharacters);
            this.Controls.Add(this.AddCharacterButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "DofusForm";
            this.Text = "DofusSwap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DofusForm_FormClosing);
            this.Load += new System.EventHandler(this.DofusForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button AddCharacterButton;
        private System.Windows.Forms.FlowLayoutPanel ActiveCharacters;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ConfigToolMenuStripItem;
    }
}

