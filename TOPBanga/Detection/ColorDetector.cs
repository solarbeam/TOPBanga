using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace TOPBanga
{
    class ColorDetector : IDetector
    {

        public Image<Bgr, byte> image { get; set; }

        public Hsv ballHsv {  get; set; }

        public int threshold { get; set; }

        public ColorDetector()
        {
            this.threshold = 35;
        }

        public void SetBallColorHSV(int h, int s, int v)
        {
            this.ballHsv = new Hsv(h, s, v);
        }

        public bool DetectBall(out float x, out float y, out float radius, out Bitmap bitmap)
        {
            bool success = false;
            x = 0;
            y = 0;
            radius = 0;
            bitmap = null;
            Image<Hsv, byte> hsvImg = this.image.Convert<Hsv, byte>();

            Hsv lowerLimit = new Hsv(ballHsv.Hue - this.threshold, ballHsv.Satuation - this.threshold, ballHsv.Value - this.threshold);
            Hsv upperLimit = new Hsv(ballHsv.Hue + this.threshold, ballHsv.Satuation + this.threshold, ballHsv.Value + this.threshold);

            Image<Gray, byte> imgFiltered = hsvImg.InRange(lowerLimit, upperLimit);

            CircleF[] circles = imgFiltered.HoughCircles(new Gray(12), new Gray(26), 1.9, 10, 0, 0)[0];

            foreach (CircleF c in circles)
            {
                this.image.Draw(c, new Bgr(1, 1, 255), 1);
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

        public void SetBallColorHSVFromCoords(int x, int y)
        {
            Image<Hsv, byte> hsvImage = this.image.Convert<Hsv, byte>();
            this.ballHsv = new Hsv(hsvImage.Data[y, x, 0], hsvImage.Data[y, x, 1], hsvImage.Data[y, x, 2]);
        }
    }
}
