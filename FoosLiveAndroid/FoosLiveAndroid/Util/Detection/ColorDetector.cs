using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Detection
{
    /// <summary>
    /// The class contains functions to detect a table by contours and a blob by color
    /// </summary>
    class ColorDetector
    {
        private const double CannyThreshold = 180.0;
        private const double CannyThresholdLinking = 120.0;
        private const int ContourArea = 250;
        private const int VerticeCount = 4;
        private const int MinAngle = 80;
        private const int MaxAngle = 100;

        /// <summary>
        /// False if the box field is null
        /// True if the box field is not null
        /// </summary>
        private bool boxSet = false;
        private bool started = false;
        private Rectangle preliminaryBlob;
        /// <summary>
        /// Defines the bounding box, in which we search for the blob
        /// </summary>
        private Rectangle box;
        /// <summary>
        /// Defines the starting box's width
        /// </summary>
        private int boxWidth = 20;
        /// <summary>
        /// Defines the starting box's height
        /// </summary>
        private int boxHeight = 20;
        /// <summary>
        /// Count how many frames a blob was not detected
        /// </summary>
        private int framesLost = 0;
        /// <summary>
        /// Defines the count of frames a blob is allowed to not be detected
        /// </summary>
        private const int framesLostToNewBoundingBox = 30;
        /// <summary>
        /// Defines the last calculated size of the blob
        /// </summary>
        private int lastBlobSize;
        /// <summary>
        /// Defines the preliminary size of the blob
        /// </summary>
        private int firstBlobSize;
        /// <summary>
        /// Defines the range, in which the size of the blob is permitted to be
        /// </summary>
        private const float rangeMultiplier = 3.0f;

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
            Threshold = 35; // default threshold
            this.box = new Rectangle();
            this.box.Width = boxWidth;
            this.box.Height = boxHeight;
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
            var boxList = new List<RotatedRect>();
            var cannyEdges = new UMat();
            var uimage = new UMat();
            CvInvoke.CvtColor(image, uimage, ColorConversion.Bgr2Gray);
            CvInvoke.Canny(uimage, cannyEdges, CannyThreshold, CannyThresholdLinking);
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    using (var contour = contours[i])
                    using (var approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour) > ContourArea) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == VerticeCount) //The contour has 4 vertices.
                            {
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < MinAngle || angle > MaxAngle)
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
        public bool DetectBall(Hsv ballHsv, out Rectangle rect, out Rectangle bBox)
        {
            //default returns
            rect = new Rectangle();
            bBox = this.box;

            // Will change this in order to optimize
            Image<Hsv, byte> hsvImg = image.Convert<Hsv, byte>();
            hsvImg.SmoothBlur(hsvImg.Size.Width, hsvImg.Size.Height);

            // Define the upper and lower limits of the Hue and Saturation values
            Hsv lowerLimit = new Hsv(ballHsv.Hue - Threshold, ballHsv.Satuation - Threshold, ballHsv.Value - Threshold);
            Hsv upperLimit = new Hsv(ballHsv.Hue + Threshold, ballHsv.Satuation + Threshold, ballHsv.Value + Threshold);

            Image<Gray, byte> imgFiltered = hsvImg.InRange(lowerLimit, upperLimit);

            // Will be added as an attribute to this class
            var detector = new BlobDetector();

            // Define the class, which will store information about blobs found
            var points = new CvBlobs();

            // Get the blobs found out of the filtered image and the count
            var count = detector.GetBlobs(imgFiltered, points);

            // If the blob was lost for an amount of frames, reset the bounding box
            if (framesLost > framesLostToNewBoundingBox || !this.boxSet)
            {
                this.box.X = image.Size.Width / 2;
                this.box.Y = image.Size.Height / 2;
                this.box.Width = boxWidth;
                this.box.Height = boxWidth;
                this.boxSet = true;
            }

            // Cleanup the filtered image, as it will not be needed anymore
            imgFiltered.Dispose();

            // If there were 0 blobs, return false
            if (count == 0)
            {
                points.Dispose();
                framesLost++;
                return false;
            }
            
            CvBlob biggestBlob = null;
            foreach (var pair in points.OrderByDescending(e => e.Value.Area))
            {
                // Check if the blob is within the predefined bounding box and is of a given size
                if (this.box.Contains((int)pair.Value.Centroid.X, (int)pair.Value.Centroid.Y))
                {
                    // It is, so we pressume it to be the ball
                    biggestBlob = pair.Value;
                    this.box = biggestBlob.BoundingBox;
                    Size toInflate = new Size((int)(biggestBlob.BoundingBox.Width * 0.5f),
                                                (int)(biggestBlob.BoundingBox.Height * 0.5f));
                    this.box.Inflate(toInflate);
                    break;
                }
            }

            var success = biggestBlob != null;
            bBox = this.box;

            if (success)
            {
                // Deep copy the blob's information
                rect = new Rectangle(new Point(biggestBlob.BoundingBox.X, biggestBlob.BoundingBox.Y),
                                        new Size(biggestBlob.BoundingBox.Size.Width, biggestBlob.BoundingBox.Height));
                this.lastBlobSize = biggestBlob.Area;
            }
            else
                framesLost++;

            points.Dispose();

            return success;
        }
    }
}
