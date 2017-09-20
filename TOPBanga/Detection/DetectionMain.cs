using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System.Linq;
using System.Drawing;

namespace Detection
{
    class DetectionMain
    {
        private double min_dist = 40.0;
        private int min_radius = 8;
        private int max_radius = 150;
        private double resolution = 0.0001f;
        private const int canny = 60;
        private const int accumulator = 28;
        public DetectionMain()
        {

        }
        public Image<Bgr,Byte> CalcImage( Image<Bgr,Byte> img , Point point )
        {
            CvInvoke.MedianBlur(img, img, 3);
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
            Gray cannyThreshold = new Gray(canny);
            Gray cannyThresholdLinking = new Gray(120);
            Gray circleAccumulatorThreshold = new Gray(accumulator);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                resolution,
                min_dist,
                min_radius,
                max_radius
                )[0];
            for (int i = 0; i != circles.Count(); i++)
                img.Draw(circles[i], new Bgr(), 1, LineType.EightConnected, 0);

            return img;
        }
    }
}
