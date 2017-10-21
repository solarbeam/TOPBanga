﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using TOPBanga.Detection;

namespace TOPBanga
{
    public partial class VideoFromFile : Form
    {

        private IDetector detector;
        private VideoCapture video;
        private Mat currentFrame;
        private System.Timers.Timer videoTickTimer;
        private bool videoLoaded;

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
            openFileDialog.Filter = "MP4 file|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.videoLoaded = true;
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
            colorContainer.Add(this.detector.GetBallColorHSVFromCoords(x, y));
            Image<Hsv, byte> colorImage = new Image<Hsv, byte>(this.ColorBox.Width, this.ColorBox.Height, colorContainer.list[0]);
            this.ColorBox.Image = colorImage.Bitmap;
        }

        public void setDeltaText(String text)
        {
            this.label1.Text = text;
        }

        private void DetectionButton_Click(object sender, EventArgs e)
        {
            if (!videoLoaded)
                return;
            this.videoTickTimer.Stop();
            this.videoTickTimer = new System.Timers.Timer();
            this.videoTickTimer.Interval = videoInterval;
            this.videoTickTimer.Elapsed += new ElapsedEventHandler(delegate (object o, ElapsedEventArgs args) {
                bool circleFound = false;
                this.currentFrame = this.video.QueryFrame();
                if (this.currentFrame == null)
                {
                    this.videoTickTimer.Stop();
                    return;
                }
                Image<Bgr, byte> currentImage = this.currentFrame.ToImage<Bgr, byte>();
                this.detector.image = currentImage;
                foreach(Hsv i in colorContainer.list)
                {
                    if (this.detector.DetectBall(out float x, out float y, out float radius, out Bitmap bitmap,i))
                    {
                        this.Picture.Image = bitmap;
                        circleFound = true;
                        break;
                    }
                }
                if ( !circleFound )
                {
                    /**
                     * TODO
                     * 
                     * Pause the video and ask the user to select the ball
                     */
                }
            });
            this.videoTickTimer.Start();
        }
    }
}
