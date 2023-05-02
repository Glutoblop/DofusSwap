
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
            this.SaveButton = new System.Windows.Forms.Button();
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
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(483, 12);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(109, 36);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DofusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 479);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.ActiveCharacters);
            this.Controls.Add(this.AddCharacterButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DofusForm";
            this.Text = "DofusSwap";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button AddCharacterButton;
        private System.Windows.Forms.FlowLayoutPanel ActiveCharacters;
        private System.Windows.Forms.Button SaveButton;
    }
}

