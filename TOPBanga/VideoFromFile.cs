using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using TOPBanga.Detection.GameUtil;
using System.Collections.Generic;

namespace TOPBanga
{
    public partial class VideoFromFile : Form
    {
        private const int videoInterval = 30;
        private const int webcamInterval = 80;
        private IDetector detector;
        private VideoCapture video;
        private Mat currentFrame;
        private System.Timers.Timer videoTickTimer;
        private bool videoLoaded;
        private VideoCapture webcam;
        private List<GoalZone> goals = new List<GoalZone>();
        private List<Coordinates> tempCoords = new List<Coordinates>();
        private bool markingMode = false;

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
                this.videoLoaded = true;
                this.videoTickTimer.Interval = videoInterval;
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
            if (!markingMode)
            {
                this.detector.SetBallColorHSVFromCoords(x, y);
                Image<Hsv, byte> colorImage = new Image<Hsv, byte>(25, 25, this.detector.ballHsv);
                this.ColorBox.Image = colorImage.Bitmap;
            }
            else
            {
                this.tempCoords.Add(new Coordinates(x, y));
                if (this.tempCoords.Count == 4)
                {
                    this.markingMode = false;
                    this.Toggle_Buttons_Except_Mark_Goals();
                    this.goals.Add(new GoalZone(this.tempCoords[0], this.tempCoords[1],
                        this.tempCoords[2], this.tempCoords[3], goalSide.Right));
                    this.tempCoords = new List<Coordinates>();

                }
            }
        }

        private void DetectionButton_Click(object sender, EventArgs e)
        {
            this.videoTickTimer.Stop();
            this.videoTickTimer = new System.Timers.Timer();
            this.videoTickTimer.Elapsed += new ElapsedEventHandler(delegate (object o, ElapsedEventArgs args) {
                if (this.videoLoaded)
                    this.currentFrame = this.video.QueryFrame();
                else if (this.webcam != null)
                    this.currentFrame = this.webcam.QueryFrame();
                if (this.currentFrame == null)
                {
                    this.videoTickTimer.Stop();
                    return;
                }
                Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
                this.detector.image = currentImage;
                if (this.detector.DetectBall(out float x, out float y, out float radius, out Bitmap bitmap))
                {
                    Coordinates ballCoordinates = new Coordinates(x, y);
                    foreach(GoalZone zone in this.goals)
                    {
                        bitmap = GoalChecker.PaintGoalOn(bitmap, zone);
                        //bool hit = GoalChecker.Check(zone, ballCoordinates);
                    }
                        
                    this.Picture.Image = bitmap;

                }
                else
                    this.Picture.Image = currentImage.Bitmap;
            });
            this.videoTickTimer.Start();
        }

        private void switchCam_Click(object sender, EventArgs e)
        {
            this.videoTickTimer.Interval = webcamInterval;
            if (this.videoLoaded)
            {
                this.videoTickTimer.Stop();
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
                this.markingMode = false;
        }
    }
}