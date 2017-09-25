﻿using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Tracking;
using Emgu.CV.Structure;
using System.Linq;
using System.Drawing;
using Emgu.CV.Tracking;
using Emgu.CV.Cvb;
using System.Windows.Forms;

namespace Detection
{
    class DetectionMain
    {
        private Tracker tracker;
        private VideoCapture video;
        private Rectangle boundingBox;
        private int sleep;
        private bool isInit;
        public static Mat frame;
        public DetectionMain( String videoFileName )
        {
            tracker = new Tracker("MIL");
            video = new VideoCapture(videoFileName);
            
            this.video = video;
            int refreshRate = Convert.ToInt32(video.GetCaptureProperty(CapProp.Fps));
            sleep = 1000 / refreshRate; //Calculate the time needed to put the Thread to sleep
            MessageBox.Show(sleep.ToString());
        }
        public void Init(Point point)
        {
            if (point == null || isInit) return;
            boundingBox = new Rectangle(point, new Size(100, 100));
            tracker.Init(video.QueryFrame(), boundingBox);
            isInit = true;
        }
        public void CalcImage(Object stateInfo)
        {
            video.Read(frame);
            tracker.Update(video.QueryFrame(),out boundingBox);
            CvInvoke.Rectangle(frame, boundingBox, new MCvScalar(255, 0, 0), 2);
        }
    }
}
