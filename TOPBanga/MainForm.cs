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

namespace Test
{
    public partial class MainForm : Form
    {
        private Boolean clicked = false;
        private Tesseract t;
        private VideoCapture video;
        private System.Threading.Timer timer;
        private Mat template;
        private DetectionMain detect;
        public MainForm()
        {
            InitializeComponent();
            t = new Tesseract("", "eng", OcrEngineMode.Default);
            
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
    }
}
