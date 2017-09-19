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
        private double min_dist = 40.0;
        private int min_radius = 8;
        private int max_radius = 150;
        private double resolution = 0.0001f;
        private IImage image;
        private Tesseract t;
        private VideoCapture video;
        private System.Threading.Timer timer;
        private const int canny = 60;
        private const int accumulator = 28;
        public MainForm()
        {
            InitializeComponent();
            this.Resolution.Text = "" + resolution.ToString("0.00000") ;
            this.MinDistValue.Text = "" + min_dist;
            this.MinRadValue.Text = "" + min_radius;
            this.MaxRadValue.Text = "" + max_radius;
            t = new Tesseract("", "eng", OcrEngineMode.Default);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
        }
        private void calcImage()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(image.Bitmap);
            /*
             * This part, surprisingly, helps!
             */
            CvInvoke.MedianBlur(img, img, 3);
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
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
            System.GC.Collect();
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
                Filter = "PNG file|.png|JPG file|.jpg|MP4 file|.mp4",
                InitialDirectory = "C:\\"
            };
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                String extension = Openfile.FileName;
                if ( extension.Contains("png") || extension.Contains("jpg") )
                {
                    image = new Image<Bgr, Byte>(Openfile.FileName);
                    calcImage();
                    return;
                }

                video = new VideoCapture(Openfile.FileName);
                timer = new System.Threading.Timer(this.calcFrame, null, 0, 100);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (image != null) calcImage();
        }

        private void Picture_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            min_dist = int.Parse(textBox2.Text);
            if (image != null) calcImage();
        }

        private void changeResolution_Click(object sender, EventArgs e)
        {
            resolution = Double.Parse(textBox1.Text);
            if (image != null) calcImage();
        }

        private void changeMinRadius_Click(object sender, EventArgs e)
        {
            min_radius = int.Parse(textBox3.Text);
            if (image != null) calcImage();
        }

        private void changeMaxRadius_Click(object sender, EventArgs e)
        {
            max_radius = int.Parse(textBox4.Text);
            if (image != null) calcImage();
        }
    }
}
