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

        IDetector detector;
        VideoCapture video;
        Mat currentFrame;
        System.Timers.Timer videoTickTimer;


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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.videoTickTimer.Stop();
            this.videoTickTimer = new System.Timers.Timer();
            this.videoTickTimer.Interval = 30;
            this.videoTickTimer.Elapsed += new ElapsedEventHandler(delegate (object o, ElapsedEventArgs args) {
                this.currentFrame = this.video.QueryFrame();
                if(this.currentFrame == null)
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
        public void setDeltaText(String text)
        {
            this.label1.Text = text;
        }
    }
}
