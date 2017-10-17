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
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Picture
            // 
            this.Picture.Location = new System.Drawing.Point(2, 0);
            this.Picture.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(640, 360);
            this.Picture.TabIndex = 0;
            this.Picture.TabStop = false;
            this.Picture.Click += new System.EventHandler(this.Picture_Click);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(719, 53);
            this.BrowseButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(293, 38);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // DetectionButton
            // 
            this.DetectionButton.Location = new System.Drawing.Point(719, 11);
            this.DetectionButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DetectionButton.Name = "DetectionButton";
            this.DetectionButton.Size = new System.Drawing.Size(293, 38);
            this.DetectionButton.TabIndex = 2;
            this.DetectionButton.Text = "Start Detection";
            this.DetectionButton.UseVisualStyleBackColor = true;
            this.DetectionButton.Click += new System.EventHandler(this.DetectionButton_Click);
            // 
            // ColorBox
            // 
            this.ColorBox.Location = new System.Drawing.Point(657, 11);
            this.ColorBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ColorBox.Name = "ColorBox";
            this.ColorBox.Size = new System.Drawing.Size(56, 82);
            this.ColorBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ColorBox.TabIndex = 4;
            this.ColorBox.TabStop = false;
            this.ColorBox.Click += new System.EventHandler(this.ColorBox_Click);
            // 
            // switchCam
            // 
            this.switchCam.AccessibleName = "";
            this.switchCam.Location = new System.Drawing.Point(719, 96);
            this.switchCam.Name = "switchCam";
            this.switchCam.Size = new System.Drawing.Size(293, 45);
            this.switchCam.TabIndex = 5;
            this.switchCam.Text = "Switch to Webcam";
            this.switchCam.UseVisualStyleBackColor = true;
            this.switchCam.Click += new System.EventHandler(this.switchCam_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(966, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "label1";
            // 
            // VideoFromFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 513);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.switchCam);
            this.Controls.Add(this.ColorBox);
            this.Controls.Add(this.DetectionButton);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.Picture);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "VideoFromFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VideoFromFile";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.VideoFromFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button DetectionButton;
        private System.Windows.Forms.PictureBox ColorBox;
        private System.Windows.Forms.Button switchCam;
        private System.Windows.Forms.Label label1;
    }
}