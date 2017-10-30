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
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Picture
            // 
            this.Picture.Location = new System.Drawing.Point(1, 0);
            this.Picture.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(480, 292);
            this.Picture.TabIndex = 0;
            this.Picture.TabStop = false;
            this.Picture.Click += new System.EventHandler(this.Picture_Click);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(539, 43);
            this.BrowseButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(220, 31);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // DetectionButton
            // 
            this.DetectionButton.Location = new System.Drawing.Point(539, 9);
            this.DetectionButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.DetectionButton.Name = "DetectionButton";
            this.DetectionButton.Size = new System.Drawing.Size(220, 31);
            this.DetectionButton.TabIndex = 2;
            this.DetectionButton.Text = "Start Detection";
            this.DetectionButton.UseVisualStyleBackColor = true;
            this.DetectionButton.Click += new System.EventHandler(this.DetectionButton_Click);
            // 
            // ColorBox
            // 
            this.ColorBox.Location = new System.Drawing.Point(493, 9);
            this.ColorBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.ColorBox.Name = "ColorBox";
            this.ColorBox.Size = new System.Drawing.Size(42, 66);
            this.ColorBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ColorBox.TabIndex = 4;
            this.ColorBox.TabStop = false;
            // 
            // switchCam
            // 
            this.switchCam.AccessibleName = "";
            this.switchCam.Location = new System.Drawing.Point(539, 78);
            this.switchCam.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.switchCam.Name = "switchCam";
            this.switchCam.Size = new System.Drawing.Size(220, 36);
            this.switchCam.TabIndex = 5;
            this.switchCam.Text = "Switch to Webcam";
            this.switchCam.UseVisualStyleBackColor = true;
            this.switchCam.Click += new System.EventHandler(this.switchCam_Click);
            // 
            // Mark_Goals_Button
            // 
            this.Mark_Goals_Button.Location = new System.Drawing.Point(539, 119);
            this.Mark_Goals_Button.Margin = new System.Windows.Forms.Padding(2);
            this.Mark_Goals_Button.Name = "Mark_Goals_Button";
            this.Mark_Goals_Button.Size = new System.Drawing.Size(220, 36);
            this.Mark_Goals_Button.TabIndex = 7;
            this.Mark_Goals_Button.Text = "Mark Goals";
            this.Mark_Goals_Button.UseVisualStyleBackColor = true;
            this.Mark_Goals_Button.Click += new System.EventHandler(this.Mark_Goals_Button_Click);
            // 
            // skipFrame
            // 
            this.skipFrame.Location = new System.Drawing.Point(539, 161);
            this.skipFrame.Name = "skipFrame";
            this.skipFrame.Size = new System.Drawing.Size(219, 31);
            this.skipFrame.TabIndex = 8;
            this.skipFrame.Text = "Skip frame";
            this.skipFrame.UseVisualStyleBackColor = true;
            this.skipFrame.Click += new System.EventHandler(this.skipFrame_Click);
            // 
            // VideoFromFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 417);
            this.Controls.Add(this.skipFrame);
            this.Controls.Add(this.Mark_Goals_Button);
            this.Controls.Add(this.switchCam);
            this.Controls.Add(this.ColorBox);
            this.Controls.Add(this.DetectionButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.Picture);
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "VideoFromFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VideoFromFile";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
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
    }
}