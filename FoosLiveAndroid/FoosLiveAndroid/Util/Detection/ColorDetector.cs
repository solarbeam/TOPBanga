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
    /// <summary>
    /// The class contains functions to detect a table by contours and a blob by color
    /// </summary>
    class ColorDetector : IDetector
    {
        private double cannyThreshold = 180.0;
        private double cannyThresholdLinking = 120.0;
        private int contourArea = 250;
        private int verticeCount = 4;
        private int minAngle = 80;
        private int maxAngle = 100;
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
            CvInvoke.CvtColor(this.image, uimage, ColorConversion.Bgr2Gray);
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > contourArea) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == verticeCount) //The contour has 4 vertices.
                            {
                                bool isRectangle = true;
                                System.Drawing.Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < minAngle || angle > maxAngle)
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
        /// <returns>True if a ball was detected. False otherwise</returns>
        public bool DetectBall(Hsv ballHsv, out Rectangle rect)
        {
            //default returns
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
            var detector = new BlobDetector();

            // Define the class, which will store information about blobs found
            var points = new CvBlobs();

            // Get the blobs found out of the filtered image and the count
            var count = detector.GetBlobs(imgFiltered, points);

            // Cleanup the filtered image, as it will not be needed anymore
            imgFiltered.Dispose();

            // If there were 0 blobs, return false
            if (count == 0)
            {
                points.Dispose();
                return false;
            }

            // Get the biggest blob by going through all of them
            CvBlob biggestBlob = null;
            var biggestArea = 0;
            foreach(var pair in points)
            {
                if ( biggestArea < pair.Value.Area )
                {
                    biggestArea = pair.Value.Area;
                    biggestBlob = pair.Value;
                }
            }
            //this.image.Draw(points[1].BoundingBox, new Bgr(255,255,255), 2);
            var success = points.Count != 0; 

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
