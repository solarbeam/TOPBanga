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
        private VideoCapture video;
        private DetectionMain detect;
        public MainForm()
        {
            InitializeComponent();
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
                Filter = "MP4 file|*.mp4|File|*.*",
                InitialDirectory = "./"
            };
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                String extension = Openfile.FileName;
                this.detect = new DetectionMain(Openfile.FileName);
                detect.FrameChange += new EventHandler(this.frameChange);
            }
        }

        public void frameChange (object sender, EventArgs e)
        {
            Console.WriteLine("AAA");
            Picture.Image = DetectionMain.frame.Bitmap;
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
    }
}
