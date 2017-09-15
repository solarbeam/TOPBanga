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
        private const int min_radius = 5;
        private const int max_radius = 150;
        private const double resolution = 0.0001f;
        private IImage image;
        private Tesseract t;
        private VideoCapture video;
        private System.Threading.Timer timer;
        private const int canny = 60;
        private const int accumulator = 28;
        public MainForm()
        {
            InitializeComponent();
            t = new Tesseract("", "eng", OcrEngineMode.Default);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Loaded");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //MessageBox.Show("Resize", "Event");
        }
        private void Image()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(image.Bitmap);
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
            Gray cannyThreshold = new Gray(canny);
            Gray cannyThresholdLinking = new Gray(120);
            Gray circleAccumulatorThreshold = new Gray(accumulator);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                resolution, //Resolution of the accumulator used to detect centers of the circles
                50.0, //min distance 
                min_radius, //min radius
                max_radius //max radius
                )[0]; //Get the circles from the first channel
            for (int i = 0; i != circles.Count(); i++)
                img.Draw(circles[i], new Bgr(), 1, LineType.EightConnected, 0);

            Picture.Image = img.ToBitmap();
            System.GC.Collect();
        }

        private void frame(Object stateInfo) {
            //video.Grab().ToString();
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(video.QueryFrame().Bitmap);
            Image<Gray, Byte> gray = img.Convert<Gray,Byte>();
            Gray cannyThreshold = new Gray(canny);
            Gray cannyThresholdLinking = new Gray(120);
            Gray circleAccumulatorThreshold = new Gray(accumulator);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                resolution, //Resolution of the accumulator used to detect centers of the circles
                50.0, //min distance 
                min_radius, //min radius
                max_radius //max radius
                )[0]; //Get the circles from the first channel
            for (int i = 0; i != circles.Count(); i++)
                img.Draw(circles[i], new Bgr(), 1, LineType.EightConnected, 0);

            Picture.Image = img.ToBitmap();
        } 
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog() {
                Filter = "File|*.*", 
                InitialDirectory = "C:\\"
            };
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                String extension = Openfile.FileName;
                if ( extension.Contains("png") || extension.Contains("jpg") )
                {
                    image = new Image<Bgr, Byte>(Openfile.FileName);
                    Image();
                    return;
                }

                video = new VideoCapture(Openfile.FileName);
                timer = new System.Threading.Timer(this.frame, null, 0, 100);
            }
        }

        private void BarValueChange(object sender, EventArgs e)
        {
            //resolution = trackBar1.Value / 4.0;
            textBox1.Text = "Resolution: " + resolution.ToString();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            //canny = trackBar2.Value;
            textBox2.Text = "Canny: " + canny.ToString();
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            //accumulator = trackBar3.Value;
            textBox3.Text = "Accumulator: " + accumulator.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (image != null) Image();
        }
    }
}
