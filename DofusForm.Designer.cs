
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
            this.RefreshConfigButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RefreshConfigButton
            // 
            this.RefreshConfigButton.Location = new System.Drawing.Point(12, 12);
            this.RefreshConfigButton.Name = "RefreshConfigButton";
            this.RefreshConfigButton.Size = new System.Drawing.Size(137, 34);
            this.RefreshConfigButton.TabIndex = 0;
            this.RefreshConfigButton.Text = "Refresh Config";
            this.RefreshConfigButton.UseVisualStyleBackColor = true;
            this.RefreshConfigButton.Click += new System.EventHandler(this.RefreshConfigButton_Click);
            // 
            // DofusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.RefreshConfigButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DofusForm";
            this.Text = "DofusSwap";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button RefreshConfigButton;
    }
}

