using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Linq;
using System.Threading;
using Emgu.CV.Tracking;
using System.Windows.Forms;
using Detection;
using System.Drawing;
using TOPBanga.Detection;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;

namespace Test
{
    public partial class MainForm : Form
    {
        private Boolean clicked = false;
        private Tesseract t;
        private VideoCapture video;
        private System.Timers.Timer timer;
        private Mat template;
        private DetectionMain detect;

        private VideoCapture capture;
        private Mat target;
        public MainForm()
        {
            InitializeComponent();
            t = new Tesseract("", "eng", OcrEngineMode.Default);
            capture = new VideoCapture();
            target = capture.QueryFrame();
            timer = new System.Timers.Timer() {
                Interval = 200
                
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            timer.Start();
            
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
        }
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog() {
                Filter = "PNG file|*.png|JPG file|*.jpg|MP4 file|*.mp4|File|*.*",
                InitialDirectory = "C:\\"
            };
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                String extension = Openfile.FileName;

                video = new VideoCapture(Openfile.FileName);
                Picture.Image = video.QueryFrame().Bitmap;
                detect = new DetectionMain(Openfile.FileName);
            }
        }

        public void Tick(object o, EventArgs e) {
            this.setImage(capture.QueryFrame().Bitmap);
            this.setImage(DrawMatches.Draw(target, capture.QueryFrame(), out long timmmmme).Bitmap);
        }

        private void Refresh_Button_Click(object sender, EventArgs e)
        {
            
        }

        private void Picture_Click(object sender, EventArgs e)
        {
        }

        private void minDist_Click(object sender, EventArgs e)
        {
            
        }

        private void changeResolution_Click(object sender, EventArgs e)
        {
            
        }

        private void changeMinRadius_Click(object sender, EventArgs e)
        {
           
        }

        private void changeMaxRadius_Click(object sender, EventArgs e)
        {
            
        }

        public void setImage ( Image image)
        {
            Picture.Image = image;
        }

        private void Capture_Click(object sender, EventArgs e)
        {
            target = capture.QueryFrame();
            pictureBox1.Image = target.Bitmap;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
