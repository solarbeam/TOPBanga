﻿using System;
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
    /// <summary>
    /// The class contains functions to detect a table by contours and a blob by color
    /// </summary>
    class ColorDetector : IDetector
    {
        /// <summary>
        /// The detector's image, used for calculations
        /// </summary>
        public Image<Bgr, byte> image { get; set; }

        /// <summary>
        /// The threshold, which defines the range of colors
        /// </summary>
        public int Threshold { get; set; }

        [Obsolete("Not used anymore")]
        public Bgr CircleColor { get; set; }

        [Obsolete("Not used anymore")]
        public int CircleWidth { get; set; }

        /// <summary>
        /// The default constructor for the ColorDetector class
        /// The threshold is assumed to be 15
        /// </summary>
        public ColorDetector()
        {
            Threshold = 15; // default threshold
        }

        /// <summary>
        /// Creates the ColorDetector class with the appropriate threshold
        /// </summary>
        /// <param name="threshold">The threshold, which will be used to define the range of colors</param>
        public ColorDetector(int threshold)
        {
            Threshold = threshold;
        }

        /// <summary>
        /// Detect a table, using the predefined image, stored in this class
        /// </summary>
        /// <param name="rect">Creates the rectangle, holding the positions</param>
        /// <returns>True if a table was detected. False otherwise</returns>
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

        /// <summary>
        /// Detects a ball using the predefined image, stored in this class,
        /// and the specific Hsv
        /// </summary>
        /// <param name="ballHsv">The Hsv values, which are to be used in calculations</param>
        /// <param name="rect">The rectangle, which holds the information about the blob, if such was found</param>
        /// <returns></returns>
        public bool DetectBall(Hsv ballHsv, out Rectangle rect)
        {
            //default returns
            bool success = false;
            rect = new Rectangle();

            // Will change this in order to optimize
            Image<Hsv, byte> hsvImg = image.Convert<Hsv,byte>();

            // Define the upper and lower limits of the Hue and Saturation values
            Hsv lowerLimit = new Hsv(ballHsv.Hue - Threshold, ballHsv.Satuation - Threshold, ballHsv.Value - Threshold);
            Hsv upperLimit = new Hsv(ballHsv.Hue + Threshold, ballHsv.Satuation + Threshold, ballHsv.Value + Threshold);

            // Use an intermediary to filter on different channels
            Image<Gray, byte>[] intermediary = hsvImg.Split();
            intermediary[0] = intermediary[0].InRange(new Gray(lowerLimit.Hue), new Gray(upperLimit.Hue));
            intermediary[1] = intermediary[1].InRange(new Gray(lowerLimit.Satuation), new Gray(upperLimit.Satuation));

            // Join the two channels together
            Image<Gray, byte> imgFiltered = intermediary[0].And(intermediary[1]);

            // Cleanup
            intermediary[0].Dispose();
            intermediary[1].Dispose();
            intermediary[2].Dispose();

            //imgFiltered = hsvImg.InRange(lowerLimit,upperLimit);

            // Will be added as an attribute to this class
            BlobDetector detector = new BlobDetector();

            // Define the class, which will store information about blobs found
            CvBlobs points = new CvBlobs();
            uint count;

            // Get the blobs found out of the filtered image and the count
            count = detector.GetBlobs(imgFiltered, points);

            // Cleanup the filtered image, as it will not be needed anymore
            imgFiltered.Dispose();

            // If there were 0 blobs, return false
            if (count == 0)
            {
                success = false;
                points.Dispose();
                return false;
            }

            // Get the biggest blob by going through all of them
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
                // Deep copy the blob's information
                rect = new Rectangle(new Point(biggestBlob.BoundingBox.X, biggestBlob.BoundingBox.Y),
                                        new Size(biggestBlob.BoundingBox.Size.Width, biggestBlob.BoundingBox.Height));
            }

            points.Dispose();

            return success;
        }
    }
}
