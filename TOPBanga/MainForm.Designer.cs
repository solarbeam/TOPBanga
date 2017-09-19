namespace Test
{
    partial class MainForm
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
            this.BrowseButton = new System.Windows.Forms.Button();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.Resolution = new System.Windows.Forms.TextBox();
            this.MinDistValue = new System.Windows.Forms.TextBox();
            this.MinRadValue = new System.Windows.Forms.TextBox();
            this.changeResolution = new System.Windows.Forms.Button();
            this.MaxRadValue = new System.Windows.Forms.TextBox();
            this.minDist = new System.Windows.Forms.Button();
            this.changeMinRadius = new System.Windows.Forms.Button();
            this.changeMaxRadius = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(947, 0);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(175, 50);
            this.BrowseButton.TabIndex = 0;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // Picture
            // 
            this.Picture.Location = new System.Drawing.Point(1, 0);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(940, 679);
            this.Picture.TabIndex = 1;
            this.Picture.TabStop = false;
            this.Picture.Click += new System.EventHandler(this.Picture_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(947, 56);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(175, 55);
            this.RefreshButton.TabIndex = 8;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.Refresh_Button_Click);
            // 
            // Resolution
            // 
            this.Resolution.Location = new System.Drawing.Point(947, 140);
            this.Resolution.Name = "Resolution";
            this.Resolution.Size = new System.Drawing.Size(175, 27);
            this.Resolution.TabIndex = 9;
            // 
            // MinDistValue
            // 
            this.MinDistValue.Location = new System.Drawing.Point(947, 167);
            this.MinDistValue.Name = "MinDistValue";
            this.MinDistValue.Size = new System.Drawing.Size(175, 27);
            this.MinDistValue.TabIndex = 10;
            // 
            // MinRadValue
            // 
            this.MinRadValue.Location = new System.Drawing.Point(947, 194);
            this.MinRadValue.Name = "MinRadValue";
            this.MinRadValue.Size = new System.Drawing.Size(175, 27);
            this.MinRadValue.TabIndex = 11;
            // 
            // changeResolution
            // 
            this.changeResolution.Location = new System.Drawing.Point(1128, 139);
            this.changeResolution.Name = "changeResolution";
            this.changeResolution.Size = new System.Drawing.Size(162, 21);
            this.changeResolution.TabIndex = 12;
            this.changeResolution.Text = "Change Resolution";
            this.changeResolution.UseVisualStyleBackColor = true;
            this.changeResolution.Click += new System.EventHandler(this.changeResolution_Click);
            // 
            // MaxRadValue
            // 
            this.MaxRadValue.Location = new System.Drawing.Point(947, 221);
            this.MaxRadValue.Name = "MaxRadValue";
            this.MaxRadValue.Size = new System.Drawing.Size(175, 27);
            this.MaxRadValue.TabIndex = 13;
            // 
            // minDist
            // 
            this.minDist.Location = new System.Drawing.Point(1128, 167);
            this.minDist.Name = "minDist";
            this.minDist.Size = new System.Drawing.Size(162, 21);
            this.minDist.TabIndex = 14;
            this.minDist.Text = "Change Min. distance";
            this.minDist.UseVisualStyleBackColor = true;
            this.minDist.Click += new System.EventHandler(this.minDist_Click);
            // 
            // changeMinRadius
            // 
            this.changeMinRadius.Location = new System.Drawing.Point(1128, 193);
            this.changeMinRadius.Name = "changeMinRadius";
            this.changeMinRadius.Size = new System.Drawing.Size(162, 21);
            this.changeMinRadius.TabIndex = 15;
            this.changeMinRadius.Text = "Change Min. Radius";
            this.changeMinRadius.UseVisualStyleBackColor = true;
            this.changeMinRadius.Click += new System.EventHandler(this.changeMinRadius_Click);
            // 
            // changeMaxRadius
            // 
            this.changeMaxRadius.Location = new System.Drawing.Point(1128, 221);
            this.changeMaxRadius.Name = "changeMaxRadius";
            this.changeMaxRadius.Size = new System.Drawing.Size(162, 21);
            this.changeMaxRadius.TabIndex = 16;
            this.changeMaxRadius.Text = "Change Max. Radius";
            this.changeMaxRadius.UseVisualStyleBackColor = true;
            this.changeMaxRadius.Click += new System.EventHandler(this.changeMaxRadius_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1397, 738);
            this.Controls.Add(this.changeMaxRadius);
            this.Controls.Add(this.changeMinRadius);
            this.Controls.Add(this.minDist);
            this.Controls.Add(this.MaxRadValue);
            this.Controls.Add(this.changeResolution);
            this.Controls.Add(this.MinRadValue);
            this.Controls.Add(this.MinDistValue);
            this.Controls.Add(this.Resolution);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.Picture);
            this.Controls.Add(this.BrowseButton);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.TextBox Resolution;
        private System.Windows.Forms.TextBox MinDistValue;
        private System.Windows.Forms.TextBox MinRadValue;
        private System.Windows.Forms.Button changeResolution;
        private System.Windows.Forms.TextBox MaxRadValue;
        private System.Windows.Forms.Button minDist;
        private System.Windows.Forms.Button changeMinRadius;
        private System.Windows.Forms.Button changeMaxRadius;
    }
}

