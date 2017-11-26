using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Cvb;
using FoosLiveAndroid.TOPBanga.Interface;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace FoosLiveAndroid.TOPBanga.Detection
{
    class ColorDetector
    {
        public Image<Bgr, byte> image { get; set; }

        public int Threshold { get; set; }

        public Bgr CircleColor { get; set; }

        public int CircleWidth { get; set; }


        public ColorDetector()
        {
            Threshold = 15; // default threshold
        }

        public ColorDetector(int threshold)
        {
            Threshold = threshold;
        }

        public bool DetectTable(out RotatedRect rect)
        {
            bool success = false;
            rect = new RotatedRect();
            List<RotatedRect> boxList = new List<RotatedRect>();
            UMat cannyEdges = new UMat();
            UMat uimage = new UMat();
            double cannyThreshold = 180.0;
            double cannyThresholdLinking = 120.0;
            CvInvoke.CvtColor(this.image, uimage, ColorConversion.Bgr2Gray);
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == 4) //The contour has 4 vertices.
                            {
                                bool isRectangle = true;
                                System.Drawing.Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }

                                if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                            }
                        }
                    }
                }
            }
            if (boxList.Count > 0)
            {
                success = true;
                boxList.OrderByDescending(b => b.Size);
                rect = boxList[0];
            }   
            return success;
        }

        public bool DetectBall(Bgr ballBgr, out Rectangle rect)
        {
            //default returns
            bool success = false;
            rect = new Rectangle();
            Image<Bgr, byte> bgrImg = image;
            int primaryChannel;

            /**
             * Find the primary channel
             */
            Gray lowerLimit, upperLimit;
            if (ballBgr.Blue > ballBgr.Green && ballBgr.Blue > ballBgr.Red)
            {
                lowerLimit = new Gray(ballBgr.Blue - Threshold);
                upperLimit = new Gray(ballBgr.Blue + Threshold);
                primaryChannel = 0;
            }
            else
                if (ballBgr.Green >= ballBgr.Blue && ballBgr.Green > ballBgr.Red)
            {
                lowerLimit = new Gray(ballBgr.Green - Threshold);
                upperLimit = new Gray(ballBgr.Green + Threshold);
                primaryChannel = 1;
            }
            else
            {
                lowerLimit = new Gray(ballBgr.Red - Threshold);
                upperLimit = new Gray(ballBgr.Red + Threshold);
                primaryChannel = 2;
            }

            Image<Gray, byte>[] imgFiltered = bgrImg.Split();

            for(int i = 0; i < 3; i ++)
            {
                if (i == primaryChannel)
                    continue;

                imgFiltered[i].Dispose();
            }

            imgFiltered[primaryChannel] = imgFiltered[primaryChannel].InRange(lowerLimit, upperLimit);

            BlobDetector detector = new BlobDetector();
            CvBlobs points = new CvBlobs();
            CvBlob blob;
            uint count;

            count = detector.GetBlobs(imgFiltered[primaryChannel], points);

            imgFiltered[primaryChannel].Dispose();

            if (count == 0)
            {
                success = false;
                points.Dispose();
                return false;
            }

            /**
             * Get the biggest blob
             */
            var enumerator = points.GetEnumerator();
            CvBlob biggestBlob = null;
            int biggestArea = 0;
            foreach(var pair in points)
            {
                if ( biggestArea < pair.Value.Area )
                {
                    biggestArea = pair.Value.Area;
                    biggestBlob = pair.Value;
                }
            }

            if (points.Count != 0)
            {
                //this.image.Draw(points[1].BoundingBox, new Bgr(255,255,255), 2);
                success = true;
            }  

            if (success)
            {
                /**
                 * Deep copy the blob
                 */
                rect = new Rectangle(new Point(biggestBlob.BoundingBox.X, biggestBlob.BoundingBox.Y),
                                        new Size(biggestBlob.BoundingBox.Size.Width, biggestBlob.BoundingBox.Height));
            }

            points.Dispose();

            return success;
        }
    }
}
