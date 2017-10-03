using Emgu.CV;
using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using TOPBanga.Detection;

namespace TOPBanga
{
    public partial class SURF_Form : Form
    {

        private VideoCapture webcam; // video stream to webcam
        private Mat target; // target image to track
        private PositionLogger logger;
        private Mat lastImage;

        private bool track = false;

        private System.Timers.Timer webcam_frame_timer;
        private System.Timers.Timer logger_timer;
        public SURF_Form()
        {
            InitializeComponent();
        }

        private void SURF_Form_Load(object sender, EventArgs e)
        {
            this.webcam = new VideoCapture();
            this.webcam_frame_timer = new System.Timers.Timer();
            this.webcam_frame_timer.Interval = 50;
            this.webcam_frame_timer.Elapsed += new ElapsedEventHandler(Frame_Tick);
            this.webcam_frame_timer.Start();
            this.logger = new PositionLogger(this);
            logger_timer = new System.Timers.Timer();
            logger_timer.Interval = 500;
            logger_timer.Elapsed += new ElapsedEventHandler(this.logger.Update);
            logger_timer.Start();
        }

        public void Frame_Tick(object o, EventArgs e)
        {
            if(!this.track)
                this.Webcam_Picture.Image = this.webcam.QueryFrame().Bitmap;
            else
            {
                Mat img = this.webcam.QueryFrame();
                this.lastImage = DrawMatches.Draw(target, img, out long took_time, out Point objectPos);
                this.Webcam_Picture.Image = this.lastImage.Bitmap;
            }
        }

        private void Capture_Button_Click(object sender, EventArgs e)
        {
            this.target = this.webcam.QueryFrame();
            this.Capture_Picture.Image = this.target.Bitmap;
            this.track = true;
        }

        private void FPS_Change_Button_Click(object sender, EventArgs e)
        {
            if (Int32.TryParse(FPSTextBox.Text, out int result))
            {
                this.webcam_frame_timer.Interval = result;
                this.logger_timer.Interval = result * 10;
            }
        }
        public void setDeltaText(String text)
        {
            this.relativeDelta.Text = text;
        }
    }
}
