using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using TOPBanga.Detection;
using System.Threading;

namespace TOPBanga
{
    public partial class VideoFromFile : Form
    {

        private IDetector detector;
        private VideoCapture video;
        private Mat currentFrame;
        private System.Timers.Timer videoTickTimer;
        private bool videoLoaded;
        private bool colorNeeded = false;

        private const int videoInterval = 30;

        private ColorContainer colorContainer = new ColorContainer();

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
            openFileDialog.Filter = "MP4 file|*.mp4|AVI file|*.avi";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ( this.video != null )
                {
                    this.video.Dispose();
                }
                this.videoLoaded = true;
                this.video = new VideoCapture(openFileDialog.FileName);
                this.currentFrame = this.video.QueryFrame();
                CvInvoke.Resize(this.currentFrame, this.currentFrame, new Size(Picture.Width, Picture.Height));
                this.Picture.Image = this.currentFrame.Bitmap;
                Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
                this.detector.image = currentImage;
                this.colorNeeded = true;
            }
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            if (this.colorNeeded)
            {
                MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
                int x = mouseEventArgs.X;
                int y = mouseEventArgs.Y;
                colorContainer.Add(this.detector.GetBallColorHSVFromCoords(x, y));
                Image<Hsv, byte> colorImage = new Image<Hsv, byte>(this.ColorBox.Width, this.ColorBox.Height, colorContainer.list[0]);
                this.ColorBox.Image = colorImage.Bitmap;
                this.colorNeeded = false;
            }
        }

        public void setDeltaText(String text)
        {
            this.label1.Text = text;
        }

        private void DetectionButton_Click(object sender, EventArgs e)
        {
            if (!videoLoaded || colorNeeded)
                return;
            this.video.ImageGrabbed += ImageGrabbed;
            this.video.Start();
        }

        private void ImageGrabbed(object o, EventArgs e)
        {
            bool circleFound = false;
            this.video.Retrieve(this.currentFrame);
            CvInvoke.Resize(this.currentFrame, this.currentFrame, new Size(Picture.Width, Picture.Height));
            if (this.currentFrame == null)
            {
                this.videoTickTimer.Stop();
                return;
            }
            Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
            this.detector.image = currentImage;
            foreach (Hsv i in colorContainer.list)
            {
                if (this.detector.DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, i))
                {
                    this.Picture.Image = bitmap;
                    circleFound = true;
                    break;
                }
            }
            currentImage.Dispose();
            if (!circleFound)
            {
                /**
                 * TODO
                 * 
                 * Pause the video and ask the user to select the ball
                 */
                this.videoTickTimer.Stop();
                MessageBox.Show("Please select the ball and press Start Detection");
                this.colorNeeded = true;
            }
            Thread.Sleep((int) video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
        }

        private void skipFrame_Click(object sender, EventArgs e)
        {
            Mat temp = video.QueryFrame();
            CvInvoke.Resize(temp, temp, new Size(this.Picture.Width, this.Picture.Height));
            this.Picture.Image = temp.Bitmap;
            this.detector.image = temp.ToImage<Bgr, byte>();
        }
    }
}
