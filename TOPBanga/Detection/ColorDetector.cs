using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace TOPBanga.Detection
{
    class ColorDetector : IDetector
    {
        public Image<Bgr, byte> image { get; set; }

        public int threshold { get; set; }

        public Bgr circleColor { get; set; }

        public int circleWidth { get; set; }


        public ColorDetector()
        {
            this.threshold = 35; // default threshold
            this.circleColor = new Bgr(1, 1, 255); // the default circle draw color is red
            this.circleWidth = 1;
        }

        public ColorDetector(int threshold)
        {
            this.threshold = threshold;
        }

        public bool DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, Hsv ballHsv, int minRadius = 1,
            int cannyThreshold = 12, int accumulatorThreshold = 26, double resolution = 1.9, double minDist = 10, int HoughMinRadius = 0, int HoughMaxRadius = 0)
        {
            //default returns
            bool success = false;
            x = 0;
            y = 0;
            radius = 0;
            bitmap = null;
            Image<Hsv, byte> hsvImg = this.image.Convert<Hsv, byte>();

            Hsv lowerLimit = new Hsv(ballHsv.Hue - this.threshold, ballHsv.Satuation - this.threshold, ballHsv.Value - this.threshold);
            Hsv upperLimit = new Hsv(ballHsv.Hue + this.threshold, ballHsv.Satuation + this.threshold, ballHsv.Value + this.threshold);

            Image<Gray, byte> imgFiltered = hsvImg.InRange(lowerLimit, upperLimit);

            CircleF[] circles = imgFiltered.HoughCircles(new Gray(cannyThreshold), new Gray(accumulatorThreshold), resolution, minDist, HoughMinRadius, HoughMaxRadius)[0];

            IEnumerable < CircleF > circlesFiltered =
                from circle in circles
                where circle.Radius > minRadius
                select circle;

            foreach (CircleF c in circlesFiltered)
            {
                this.image.Draw(c, this.circleColor, circleWidth);
                success = true;
                x = c.Center.X;
                y = c.Center.Y;
                radius = c.Radius;
            }

            if (success)
            {
                bitmap = this.image.Bitmap;
            }

            return success;
        }

        public Hsv GetBallColorHSVFromCoords(int x, int y)
        {
            Image<Hsv, byte> hsvImage = this.image.Convert<Hsv, byte>();
            return new Hsv(hsvImage.Data[y, x, 0], hsvImage.Data[y, x, 1], hsvImage.Data[y, x, 2]);
        }
    }
}
