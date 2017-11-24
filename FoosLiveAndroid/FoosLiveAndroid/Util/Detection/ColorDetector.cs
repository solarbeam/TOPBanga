﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Cvb;
using FoosLiveAndroid.TOPBanga.Interface;
using Android.Graphics;
using FoosLiveAndroid.TOPBanga.Detection;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace FoosLiveAndroid.TOPBanga.Detection
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

        public bool DetectBall(Hsv ballHsv, out Rectangle rect)
        {
            //default returns
            bool success = false;
            rect = new Rectangle();
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
                success = false;
                return false;
            }

            /**
             * Sort blobs by the amount of pixels in them
             */
            points.OrderByDescending(b => b.Value.Area);

            if (points.Count != 0)
            {
                //this.image.Draw(points[1].BoundingBox, new Bgr(255,255,255), 2);
                success = true;
            }  

            if (success)
            {
                rect = points[1].BoundingBox;

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
