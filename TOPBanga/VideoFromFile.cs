using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;
using TOPBanga.Detection;
using System.Threading;
using System.Collections.Generic;
using TOPBanga.Detection.GameUtil;
using TOPBanga.Util;

namespace TOPBanga
{
    public partial class VideoFromFile : Form
    {
        private IDetector detector;
        private VideoCapture video;
        private Mat currentFrame;
        private bool videoLoaded;
        private bool colorNeeded = false;
        private bool colorNeededFromThread = false;
        private ColorContainer colorContainer = new ColorContainer();
        private VideoCapture webcam;
        private GameController gameController;
        private List<PointF> tempCoords = new List<PointF>();
        private Hsv initialHsv;
        private bool markingMode = false;
        private bool added = false;
        private IWrite EventLog;
        private string filepath;

        public VideoFromFile(IDetector detector)
        {
            InitializeComponent();

            this.detector = detector;

            this.gameController = new GameController();

            EventLog = new EventLog(filepath);
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
                if (this.video != null)
                {
                    this.video.Dispose();
                }
                this.webcam = null;
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
            MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
            int x = mouseEventArgs.X;
            int y = mouseEventArgs.Y;
            if (!markingMode)
            {
                if (this.colorNeeded || this.colorNeededFromThread)
                {
                    colorContainer.Add(this.detector.GetBallColorHSVFromCoords(x, y));
                    Image<Hsv, byte> colorImage = new Image<Hsv, byte>(this.ColorBox.Width, this.ColorBox.Height, colorContainer.list[0]);
                    this.ColorBox.Image = colorImage.Bitmap;
                    this.colorNeeded = false;
                }
            }
            else
            {
                this.tempCoords.Add(new PointF(x, y));
                if (this.tempCoords.Count == 4)
                {
                    this.markingMode = false;
                    this.Toggle_Buttons_Except_Mark_Goals();
                    this.gameController.AddGoal(this.tempCoords.ToArray());
                    this.tempCoords = new List<PointF>();

                }
            }
        }

        private void DetectionButton_Click(object sender, EventArgs e)
        {
            if (!videoLoaded || this.ColorBox.Image == null)
                return;

            if (this.colorNeededFromThread)
            {
                this.detector.image.Dispose();
                this.colorContainer.Add(initialHsv);
                this.colorNeededFromThread = false;
            }
            this.colorNeeded = false;
            this.colorContainer.Add(initialHsv);
            if (!added)
            {
                this.video.ImageGrabbed += ImageGrabbed;
                added = true;
            }
            this.video.Start();
        }

        private void ImageGrabbed(object o, EventArgs e)
        {
            bool circleFound = false;
            if (this.videoLoaded)
                this.video.Retrieve(this.currentFrame);
            if (this.webcam != null)
                this.currentFrame = this.webcam.QueryFrame();
            if (this.currentFrame != null)
                CvInvoke.Resize(this.currentFrame, this.currentFrame, new Size(Picture.Width, Picture.Height));
            if (this.currentFrame == null)
            {
                //this.videoTickTimer.Stop();
                return;
            }
            Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
            this.detector.image = currentImage;
            foreach (Hsv i in colorContainer.list)
            {
                if (this.detector.DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, i))
                {
                    this.gameController.lastBallCoordinates = new PointF(x, y);
                    bitmap = this.gameController.PaintGoals(bitmap);
                    this.Picture.Image = bitmap;
                    circleFound = true;
                    break;
                }
                else
                {
                    this.Picture.Image = currentImage.Bitmap;
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
                // this.video.Pause();
                // MessageBox.Show("Please select the ball and press Start Detection");
                //this.colorNeeded = true;
            }
            Thread.Sleep((int)video.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
        }

        private void skipFrame_Click(object sender, EventArgs e)
        {
            if (this.videoLoaded)
            {
                Mat temp = video.QueryFrame();
                CvInvoke.Resize(temp, temp, new Size(this.Picture.Width, this.Picture.Height));
                this.Picture.Image = temp.Bitmap;
                this.detector.image = temp.ToImage<Bgr, byte>();
                temp.Dispose();
            }
        }
        [System.Obsolete("Will be moved elsewhere shortly")]
        private void switchCam_Click(object sender, EventArgs e)
        {
            if (this.videoLoaded)
            {
                //this.videoTickTimer.Stop();
                this.videoLoaded = false;
            }
            if (this.webcam == null)
            {
                this.webcam = new VideoCapture();
            }
            this.currentFrame = this.webcam.QueryFrame();
            this.Picture.Image = this.currentFrame.Bitmap;
            Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
            this.detector.image = currentImage;
        }

        private void Toggle_Buttons_Except_Mark_Goals()
        {
            this.DetectionButton.Enabled = !this.DetectionButton.Enabled;
            this.BrowseButton.Enabled = !this.BrowseButton.Enabled;
            this.switchCam.Enabled = !this.switchCam.Enabled;
        }

        private void Mark_Goals_Button_Click(object sender, EventArgs e)
        {
            this.Toggle_Buttons_Except_Mark_Goals();
            if (!this.markingMode)
                this.markingMode = true;
            else
            {
                this.markingMode = false;
            }

        }

        private void SaveFile_Click(object sender, EventArgs e)
        {    
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filepath = saveFileDialog.FileName;
                this.gameController = new GameController(EventLog, filepath);

            }
        }

        private void settings_Click(object sender, EventArgs e)
        {

        }
    }
}


