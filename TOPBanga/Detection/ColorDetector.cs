﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using Emgu.CV.Cvb;

namespace TOPBanga.Detection
{
    class ColorDetector : IDetector
    {
        public Image<Bgr, byte> image { get; set; }

        public int threshold { get; set; }

        public Bgr drawColor { get; set; }

        public int drawWidth { get; set; }

        private const int MAX_BLOB_LIMIT = 5;

        public ColorDetector()
        {
            this.threshold = 40; // default threshold, was raised due to the increase of reliability
            this.drawColor = new Bgr(255, 1, 1); // the default circle draw color is blue
            this.drawWidth = 1;
        }

        public ColorDetector(int threshold)
        {
            this.threshold = threshold;
        }

        public bool DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, Hsv ballHsv)
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

            BlobDetector detector = new BlobDetector();
            CvBlobs points = new CvBlobs();
            uint count;

            count = detector.GetBlobs(imgFiltered, points);


            if (count == 0)
            {
                /**
                 * Will change this in the future
                 */
                success = false;
                bitmap = this.image.Bitmap;
                return false;
            }
            if (points.Count > MAX_BLOB_LIMIT)
                return false;
            /**
             * Sort blobs by the amount of pixels in them
             */
            points.OrderByDescending(b => b.Value.Area);

            if (points.Count != 0)
            {
                /**
                 * Paint the blob with the highest area
                 */
                this.image.Draw(points[1].BoundingBox, this.drawColor, this.drawWidth);
                x = points[1].Centroid.X;
                y = points[1].Centroid.Y;
                success = true;
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
