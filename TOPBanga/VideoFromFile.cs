using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOPBanga.Detection;

namespace TOPBanga
{
    public partial class VideoFromFile : Form
    {

        ColorDetector colorDetector;
        VideoCapture video;
        Mat currentFrame;
        private PositionLogger logger;
        public Point lastPos;

        public VideoFromFile()
        {
            InitializeComponent();
        }

        private void VideoFromFile_Load(object sender, EventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.video = new VideoCapture(openFileDialog.FileName);
                this.currentFrame = this.video.QueryFrame();
                this.Picture.Image = this.currentFrame.Bitmap;
                this.logger = new PositionLogger(this);
                this.colorDetector = new ColorDetector(this.currentFrame.ToImage<Hsv, byte>());
            }
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
            int x = mouseEventArgs.X;
            int y = mouseEventArgs.Y;
            this.colorDetector.ballHsv = new Hsv(this.colorDetector.image.Data[y, x, 0],
                                                this.colorDetector.image.Data[y, x, 1],
                                                this.colorDetector.image.Data[y, x, 2]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(delegate (object o, EventArgs args) {
                this.currentFrame = this.video.QueryFrame();
                this.colorDetector.image = this.currentFrame.ToImage<Hsv, byte>();
                this.Picture.Image = this.colorDetector.img();
            });
            Application.Idle += new EventHandler(logger.Update);
        }
        public void setDeltaText(String text)
        {
            this.label1.Text = text;
        }
    }
}
