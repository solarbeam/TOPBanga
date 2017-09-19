using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Test
{
    public partial class MainForm : Form
    {
        private IImage image;
        private Tesseract t;
        private VideoCapture video;
        private System.Threading.Timer timer;
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

        private void calcFrame(Object stateInfo) {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(video.QueryFrame().Bitmap);
            Image<Gray, Byte> gray = img.Convert<Gray,Byte>();
            Gray cannyThreshold = new Gray(canny);
            Gray cannyThresholdLinking = new Gray(120);
            Gray circleAccumulatorThreshold = new Gray(accumulator);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                resolution, //Resolution of the accumulator used to detect centers of the circles
                min_dist, //min distance 
                min_radius, //min radius
                max_radius //max radius
                )[0]; //Get the circles from the first channel
            for (int i = 0; i != circles.Count(); i++)
                img.Draw(circles[i], new Bgr(), 1, LineType.EightConnected, 0);

            Picture.Image = img.ToBitmap();
            /*
             * Collect garbage after each calcFrame call
             */
            System.GC.Collect();
        } 
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog() {
                /*
                 * TODO
                 *  add more extensions
                 */
                Filter = "PNG file|*.png|JPG file|*.jpg|MP4 file|*.mp4|File|*.*",
                InitialDirectory = "C:\\"
            };
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                String extension = Openfile.FileName;
                if ( extension.Contains("png") || extension.Contains("jpg") )
                {
                    image = new Image<Bgr, Byte>(Openfile.FileName);
                    
                    return;
                }

                video = new VideoCapture(Openfile.FileName);
                timer = new System.Threading.Timer(this.calcFrame, null, 0, 100);
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

    }
}
