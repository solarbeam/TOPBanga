using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;

namespace TOPBanga
{
    public partial class VideoFromFile : Form
    {
        private int fileInterval = 30;
        private int webcamInterval = 80;
        private IDetector detector;
        private VideoCapture video;
        private Mat currentFrame;
        private System.Timers.Timer videoTickTimer;
        private bool videoLoaded;
        private VideoCapture webcam;
        private bool webcamOn;
        public VideoFromFile(IDetector detector)
        {
            InitializeComponent();

            this.detector = detector;

            videoTickTimer = new System.Timers.Timer();
        }

        private void VideoFromFile_Load(object sender, EventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 file|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.webcam = null;
                this.webcamOn = false;
                this.videoLoaded = true;
                this.videoTickTimer.Interval = this.fileInterval;
                this.video = new VideoCapture(openFileDialog.FileName);
                this.currentFrame = this.video.QueryFrame();
                this.Picture.Image = this.currentFrame.Bitmap;
                Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
                this.detector.image = currentImage;
            }
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
            int x = mouseEventArgs.X;
            int y = mouseEventArgs.Y;
            this.detector.SetBallColorHSVFromCoords(x, y);
            Image<Hsv, byte> colorImage = new Image<Hsv, byte>(25, 25, this.detector.ballHsv);
            this.ColorBox.Image = colorImage.Bitmap;
        }

        public void setDeltaText(String text)
        {
            this.label1.Text = text;
        }

        private void DetectionButton_Click(object sender, EventArgs e)
        {
            this.videoTickTimer.Stop();
            this.videoTickTimer = new System.Timers.Timer();             
            this.videoTickTimer.Elapsed += new ElapsedEventHandler(delegate (object o, ElapsedEventArgs args) {
                if (this.videoLoaded)
                   this.currentFrame = this.video.QueryFrame();
                else if (this.webcamOn)
                   this.currentFrame = this.webcam.QueryFrame();
                if (this.currentFrame == null)
                {
                    this.videoTickTimer.Stop();
                    return;
                }
                Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
                this.detector.image = currentImage;
                if (this.detector.DetectBall(out float x, out float y, out float radius, out Bitmap bitmap))
                    this.Picture.Image = bitmap;
                else
                    this.Picture.Image = currentImage.Bitmap;
            });
            this.videoTickTimer.Start();  
        }

        private void switchCam_Click(object sender, EventArgs e)
        {
            this.videoTickTimer.Interval = this.webcamInterval;
            if (this.videoLoaded)
            {
                this.videoTickTimer.Stop();
                this.videoLoaded = false;
            }
            if (webcam == null)
            {
                this.webcamOn = true;
                this.webcam = new VideoCapture(0); //0 is default camera
            }
            this.currentFrame = this.webcam.QueryFrame();
            this.Picture.Image = this.currentFrame.Bitmap;
            Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
            this.detector.image = currentImage;
        }
    }
}
