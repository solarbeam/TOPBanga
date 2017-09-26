using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Tracking;
using Emgu.CV.Structure;
using System.Linq;
using System.Drawing;
using Emgu.CV.Cvb;
using System.Windows.Forms;

namespace Detection
{
    class DetectionMain
    {
        public event EventHandler FrameChange;
        private Tracker tracker;
        private VideoCapture video;
        private Rectangle boundingBox;
        private bool isInit;
        private MCvScalar scalar = new MCvScalar(255, 0, 0);
        public static Mat frame;
        private System.Threading.Timer timer;
        public DetectionMain( String videoFileName )
        {
            tracker = new Tracker("KCF");
            video = new VideoCapture(videoFileName);
            video.Grab();
            frame = video.QueryFrame();
            /**
             * Initialize a rectangle at coordinates
             * x - 580, y - 520
             * These coordinates give the position
             * of roughly where the ball can be found
             * at the starting frame of the test video
             * 720p.mp4
             */
            Init(new Point(580,520));
            timer = new System.Threading.Timer(this.CalcImage, null, 0, 60);
        }
        public void Init(Point point)
        {
            /**
             * Create a rectangle of size
             * width - 100 pixels
             * height - 100 pixels
             */
            boundingBox = new Rectangle(point, new Size(100, 100));
            
            tracker.Init(frame, boundingBox);
            isInit = true;
        }
        protected virtual void OnFrameChange(EventArgs e)
        {
            FrameChange?.Invoke(this, e);
        }
        public int getPosX()
        {
            return boundingBox.X;
        }
        public int getPosY()
        {
            return boundingBox.Y;
        }
        public void CalcImage(Object state)
        {
            if (!video.Grab()) return;
            frame = video.QueryFrame();
            if (frame == null || boundingBox == null)
            {
                return;
            }
            tracker.Update(frame,out boundingBox);
            CvInvoke.Rectangle(frame, boundingBox, scalar, 2);
            this.OnFrameChange(EventArgs.Empty);
            FrameChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
