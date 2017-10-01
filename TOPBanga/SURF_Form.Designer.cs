namespace TOPBanga
{
    partial class SURF_Form
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
            this.Capture_Button = new System.Windows.Forms.Button();
            this.Webcam_Picture = new System.Windows.Forms.PictureBox();
            this.Capture_Picture = new System.Windows.Forms.PictureBox();
            this.FPSTextBox = new System.Windows.Forms.TextBox();
            this.FPS_Change_Button = new System.Windows.Forms.Button();
            this.relativeDelta = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Webcam_Picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Capture_Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // Capture_Button
            // 
            this.Capture_Button.Location = new System.Drawing.Point(875, 372);
            this.Capture_Button.Name = "Capture_Button";
            this.Capture_Button.Size = new System.Drawing.Size(91, 60);
            this.Capture_Button.TabIndex = 0;
            this.Capture_Button.Text = "Capture";
            this.Capture_Button.UseVisualStyleBackColor = true;
            this.Capture_Button.Click += new System.EventHandler(this.Capture_Button_Click);
            // 
            // Webcam_Picture
            // 
            this.Webcam_Picture.Location = new System.Drawing.Point(0, 0);
            this.Webcam_Picture.Name = "Webcam_Picture";
            this.Webcam_Picture.Size = new System.Drawing.Size(566, 445);
            this.Webcam_Picture.TabIndex = 1;
            this.Webcam_Picture.TabStop = false;
            // 
            // Capture_Picture
            // 
            this.Capture_Picture.Location = new System.Drawing.Point(572, 12);
            this.Capture_Picture.Name = "Capture_Picture";
            this.Capture_Picture.Size = new System.Drawing.Size(394, 299);
            this.Capture_Picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Capture_Picture.TabIndex = 2;
            this.Capture_Picture.TabStop = false;
            // 
            // FPSTextBox
            // 
            this.FPSTextBox.Location = new System.Drawing.Point(573, 385);
            this.FPSTextBox.Name = "FPSTextBox";
            this.FPSTextBox.Size = new System.Drawing.Size(82, 20);
            this.FPSTextBox.TabIndex = 3;
            this.FPSTextBox.Text = "30";
            // 
            // FPS_Change_Button
            // 
            this.FPS_Change_Button.Location = new System.Drawing.Point(675, 372);
            this.FPS_Change_Button.Name = "FPS_Change_Button";
            this.FPS_Change_Button.Size = new System.Drawing.Size(139, 60);
            this.FPS_Change_Button.TabIndex = 4;
            this.FPS_Change_Button.Text = "Change mills/frame";
            this.FPS_Change_Button.UseVisualStyleBackColor = true;
            this.FPS_Change_Button.Click += new System.EventHandler(this.FPS_Change_Button_Click);
            // 
            // relativeDelta
            // 
            this.relativeDelta.AutoSize = true;
            this.relativeDelta.Location = new System.Drawing.Point(572, 318);
            this.relativeDelta.Name = "relativeDelta";
            this.relativeDelta.Size = new System.Drawing.Size(35, 13);
            this.relativeDelta.TabIndex = 5;
            this.relativeDelta.Text = "label1";
            // 
            // SURF_Form
            // 
            this.ClientSize = new System.Drawing.Size(978, 444);
            this.Controls.Add(this.relativeDelta);
            this.Controls.Add(this.FPS_Change_Button);
            this.Controls.Add(this.FPSTextBox);
            this.Controls.Add(this.Capture_Picture);
            this.Controls.Add(this.Webcam_Picture);
            this.Controls.Add(this.Capture_Button);
            this.Name = "SURF_Form";
            this.Load += new System.EventHandler(this.SURF_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Webcam_Picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Capture_Picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Capture_Button;
        private System.Windows.Forms.PictureBox Webcam_Picture;
        private System.Windows.Forms.PictureBox Capture_Picture;
        private System.Windows.Forms.TextBox FPSTextBox;
        private System.Windows.Forms.Button FPS_Change_Button;
        private System.Windows.Forms.Label relativeDelta;
    }
}