namespace TOPBanga
{
    partial class VideoFromFile
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
            this.Picture = new System.Windows.Forms.PictureBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.DetectionButton = new System.Windows.Forms.Button();
            this.ColorBox = new System.Windows.Forms.PictureBox();
            this.switchCam = new System.Windows.Forms.Button();
            this.Mark_Goals_Button = new System.Windows.Forms.Button();
            this.skipFrame = new System.Windows.Forms.Button();
            this.settings = new System.Windows.Forms.Button();
            this.SaveFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Picture
            // 

            this.Picture.Location = new System.Drawing.Point(1, 0);
            this.Picture.Margin = new System.Windows.Forms.Padding(1);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(616, 512);

            this.Picture.TabIndex = 0;
            this.Picture.TabStop = false;
            this.Picture.Click += new System.EventHandler(this.Picture_Click);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(620, 48);
            this.BrowseButton.Margin = new System.Windows.Forms.Padding(1);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(260, 38);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // DetectionButton
            // 

            this.DetectionButton.Location = new System.Drawing.Point(698, 9);
            this.DetectionButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DetectionButton.Name = "DetectionButton";
            this.DetectionButton.Size = new System.Drawing.Size(292, 48);
            this.DetectionButton.TabIndex = 2;
            this.DetectionButton.Text = "Start Detection";
            this.DetectionButton.UseVisualStyleBackColor = true;
            this.DetectionButton.Click += new System.EventHandler(this.DetectionButton_Click);
            // 
            // ColorBox
            // 

            this.ColorBox.Location = new System.Drawing.Point(621, 292);
            this.ColorBox.Margin = new System.Windows.Forms.Padding(1);
            this.ColorBox.Name = "ColorBox";
            this.ColorBox.Size = new System.Drawing.Size(56, 76);
            this.ColorBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ColorBox.TabIndex = 4;
            this.ColorBox.TabStop = false;
            // 
            // switchCam
            // 
            this.switchCam.AccessibleName = "";
            this.switchCam.Enabled = false;

            this.switchCam.Location = new System.Drawing.Point(620, 90);
            this.switchCam.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.switchCam.Name = "switchCam";
            this.switchCam.Size = new System.Drawing.Size(260, 38);
            this.switchCam.TabIndex = 5;
            this.switchCam.Text = "Switch to Webcam";
            this.switchCam.UseVisualStyleBackColor = true;
            this.switchCam.Click += new System.EventHandler(this.switchCam_Click);
            // 
            // Mark_Goals_Button
            // 

            this.Mark_Goals_Button.Location = new System.Drawing.Point(620, 132);
            this.Mark_Goals_Button.Margin = new System.Windows.Forms.Padding(1);
            this.Mark_Goals_Button.Name = "Mark_Goals_Button";
            this.Mark_Goals_Button.Size = new System.Drawing.Size(260, 41);

            this.Mark_Goals_Button.TabIndex = 7;
            this.Mark_Goals_Button.Text = "Mark Table";
            this.Mark_Goals_Button.UseVisualStyleBackColor = true;
            this.Mark_Goals_Button.Click += new System.EventHandler(this.Mark_Goals_Button_Click);
            // 
            // skipFrame
            // 
            this.skipFrame.Location = new System.Drawing.Point(621, 177);
            this.skipFrame.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.skipFrame.Name = "skipFrame";
            this.skipFrame.TabIndex = 8;
            this.skipFrame.Text = "Skip frame";
            this.skipFrame.UseVisualStyleBackColor = true;
            this.skipFrame.Click += new System.EventHandler(this.skipFrame_Click);
            // 
            // settings
            // 

            this.settings.Location = new System.Drawing.Point(1, 5);
            this.settings.Margin = new System.Windows.Forms.Padding(4);
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(66, 11);
            this.settings.TabIndex = 0;
            this.settings.Text = "Settings";
            this.settings.UseVisualStyleBackColor = true;
            this.settings.Click += new System.EventHandler(this.settings_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.Location = new System.Drawing.Point(622, 223);
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(259, 41);
            this.SaveFile.TabIndex = 9;
            this.SaveFile.Text = "Save File As...";
            this.SaveFile.UseVisualStyleBackColor = true;
            this.SaveFile.Click += new System.EventHandler(this.SaveFile_Click);
            // 
            // VideoFromFile
            // 

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 564);
            this.Controls.Add(this.SaveFile);
            this.Controls.Add(this.settings);
            this.Controls.Add(this.skipFrame);
            this.Controls.Add(this.Mark_Goals_Button);
            this.Controls.Add(this.switchCam);
            this.Controls.Add(this.ColorBox);
            this.Controls.Add(this.DetectionButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.Picture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(911, 611);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(911, 611);
            this.Name = "VideoFromFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VideoFromFile";
            this.Load += new System.EventHandler(this.VideoFromFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button DetectionButton;
        private System.Windows.Forms.PictureBox ColorBox;
        private System.Windows.Forms.Button switchCam;
        private System.Windows.Forms.Button Mark_Goals_Button;
        private System.Windows.Forms.Button skipFrame;
        private System.Windows.Forms.Button settings;
        private System.Windows.Forms.Button SaveFile;
    }
}