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
        public Image<Hsv, byte> image { get; set; }

        public Hsv ballHsv { private get; set; }

        public ColorDetector(string path) {
            this.image = new Image<Hsv, byte>(path);
        }

        public ColorDetector(Image<Hsv, byte> image)
        {
            this.image = image;
        }

        public Bitmap img()
        {
            Hsv lowerLimit = new Hsv(ballHsv.Hue - 25, ballHsv.Satuation - 25, ballHsv.Value - 25);
            Hsv upperLimit = new Hsv(ballHsv.Hue + 25, ballHsv.Satuation + 25, ballHsv.Value + 25);

            Image<Gray, byte> imageHSVDest = this.image.InRange(lowerLimit, upperLimit);

            return imageHSVDest.Bitmap;
        }


        public bool DetectBall(out float x, out float y, out float radius)
        {

            Hsv lowerLimit = new Hsv(ballHsv.Hue - 25, ballHsv.Satuation - 25, ballHsv.Value - 25);
            Hsv upperLimit = new Hsv(ballHsv.Hue + 25, ballHsv.Satuation + 25, ballHsv.Value + 25);

            CircleF[] circle = CvInvoke.HoughCircles(this.image.InRange(lowerLimit, upperLimit), Emgu.CV.CvEnum.HoughType.Gradient, 1, 10);

            if (circle.Length != 0)
            {
                x = circle[0].Center.X;
                y = circle[0].Center.Y;
                radius = circle[0].Radius;
                return true;
            }
            else
            {
                x = 0;
                y = 0;
                radius = 0;
                return false;
            }
        }
    }
}
