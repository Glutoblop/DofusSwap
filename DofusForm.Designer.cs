
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
            this.SuspendLayout();
            // 
            // AddCharacterButton
            // 
            this.AddCharacterButton.Location = new System.Drawing.Point(12, 12);
            this.AddCharacterButton.Name = "AddCharacterButton";
            this.AddCharacterButton.Size = new System.Drawing.Size(106, 36);
            this.AddCharacterButton.TabIndex = 1;
            this.AddCharacterButton.Text = "Add Character";
            this.AddCharacterButton.UseVisualStyleBackColor = true;
            this.AddCharacterButton.Click += new System.EventHandler(this.AddCharacterButton_Click);
            // 
            // ActiveCharacters
            // 
            this.ActiveCharacters.AutoScroll = true;
            this.ActiveCharacters.Location = new System.Drawing.Point(12, 54);
            this.ActiveCharacters.Name = "ActiveCharacters";
            this.ActiveCharacters.Size = new System.Drawing.Size(580, 416);
            this.ActiveCharacters.TabIndex = 2;
            // 
            // DofusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 479);
            this.Controls.Add(this.ActiveCharacters);
            this.Controls.Add(this.AddCharacterButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DofusForm";
            this.Text = "DofusSwap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DofusForm_FormClosing);
            this.Load += new System.EventHandler(this.DofusForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button AddCharacterButton;
        private System.Windows.Forms.FlowLayoutPanel ActiveCharacters;
    }
}

