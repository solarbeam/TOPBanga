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
            tracker = new Tracker("MIL");
            video = new VideoCapture(videoFileName);
            video.Grab();
            frame = video.QueryFrame();
            Init(new Point(580,550));
            timer = new System.Threading.Timer(this.CalcImage, null, 0, 33);
        }
        public void Init(Point point)
        {
            boundingBox = new Rectangle(point, new Size(50, 50));
            
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
