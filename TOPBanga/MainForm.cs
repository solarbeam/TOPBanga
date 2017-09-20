using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Linq;
using System.Threading;
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
        private Image<Bgr,Byte> image;
        public MainForm()
        {
            InitializeComponent();
            t = new Tesseract("", "eng", OcrEngineMode.Default);
            DetectionMain test = new DetectionMain();
            
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
                if ( extension.Contains("png") || extension.Contains("jpg") )
                {
                    image = new Image<Bgr, Byte>(Openfile.FileName);
                    Picture.Image = image.ToBitmap();
                    return;
                }
            }
        }

        private void Refresh_Button_Click(object sender, EventArgs e)
        {
            
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            if (clicked == true) return;
            Picture.Image = null;
            MouseEventArgs test = (MouseEventArgs)e;
            Point testLocation = test.Location;
            MessageBox.Show(testLocation.ToString());
            DetectionMain testDetection = new DetectionMain();
            Picture.Image = testDetection.CalcImage(image, testLocation).ToBitmap();
            clicked = true;
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
