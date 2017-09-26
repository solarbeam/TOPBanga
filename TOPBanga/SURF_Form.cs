using Emgu.CV;
using System;
using System.Timers;
using System.Windows.Forms;
using TOPBanga.Detection;

namespace TOPBanga
{
    public partial class SURF_Form : Form
    {

        private VideoCapture webcam; // video stream to webcam
        private Mat target; // target image to track

        private bool track = false;

        System.Timers.Timer webcam_frame_timer;
        public SURF_Form()
        {
            InitializeComponent();
        }

        private void SURF_Form_Load(object sender, EventArgs e)
        {
            this.webcam = new VideoCapture();
            this.webcam_frame_timer = new System.Timers.Timer();
            this.webcam_frame_timer.Interval = 30;
            this.webcam_frame_timer.Elapsed += new ElapsedEventHandler(Frame_Tick);
            this.webcam_frame_timer.Start();
        }

        public void Frame_Tick(object o, EventArgs e)
        {
            if(!this.track)
                this.Webcam_Picture.Image = this.webcam.QueryFrame().Bitmap;
            else
            {
                Mat img = this.webcam.QueryFrame();
                this.Webcam_Picture.Image = DrawMatches.Draw(target, img, out long took_time).Bitmap;
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
                this.webcam_frame_timer.Interval = result;
        }
    }
}
