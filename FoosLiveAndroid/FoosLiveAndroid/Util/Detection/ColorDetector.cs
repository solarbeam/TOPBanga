using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Android.Util;
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
        public static string Tag = "ColorDetector";
        private const double CannyThreshold = 180.0;
        private const double CannyThresholdLinking = 120.0;
        private const int DefaultContourArea = 250;
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
        private int boxWidth = 40;
        /// <summary>
        /// Defines the starting box's height
        /// </summary>
        private int boxHeight = 40;
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
        private PointF lastBlob;
        private int lastSize = 0;
        private int sizeDiff = 5;
        /// <summary>
        /// Defines the range, in which the size of the blob is permitted to be
        /// </summary>
        private const float rangeMultiplier = 1.3f;
        private const int DefaultThreshold = 35;
        private const double MinTableSize = 0.6;

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

        private int minContourArea = DefaultContourArea;
        public int MinContourArea 
        {
            get
            {
                return minContourArea;
            }
            set
            {
                if (value > DefaultThreshold)
                    minContourArea = value;
                else
                    Log.Warn(Tag, "minContourArea remains default");
            }
        }



        public void SetSceenSize(int screenWidth, int screenHeight) 
        {
            MinContourArea = (int)(screenWidth * screenHeight * MinTableSize);
        }

        /// <summary>
        /// Creates the ColorDetector class with the appropriate threshold
        /// </summary>
        /// <param name="threshold">The threshold, which will be used to define the range of colors</param>
        public ColorDetector(int threshold = DefaultThreshold)
        {
            Threshold = threshold;
            MinContourArea = DefaultContourArea;
            this.box = new Rectangle();
            //minContourArea = (int)(screenWidth * screenHeight * MinTableSize);
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

                        // Todo: patikrinti ar ContourArea pakeista į minContourArea veikia gerai
                        if (CvInvoke.ContourArea(approxContour) > MinContourArea)
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
            if (success = (boxList.Count > 0))
            {
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

            // Will change this in order to optimize
            Image<Hsv, byte> hsvImg = image.Convert<Hsv, byte>();

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
                this.box.Width = image.Size.Width;
                this.box.Height = boxHeight;
                this.box.X = 0;
                this.box.Y = image.Size.Height / 2 - boxHeight / 2;
                framesLost = 0;
                this.boxSet = true;
            }

            // Cleanup the filtered image, as it will not be needed anymore
            imgFiltered.Dispose();

            bBox = this.box;

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
                    updateBox(biggestBlob);
                    framesLost = 0;
                    break;
                }
            }

            // If a blob wasn't found, find the one with the area in a range close to the last one
            if (biggestBlob == null)
            {
                foreach (var blob in points)
                {
                    if (blob.Value.Area > lastSize - sizeDiff && blob.Value.Area < lastSize + sizeDiff)
                    {
                        biggestBlob = blob.Value;
                        lastBlob = biggestBlob.Centroid;
                        updateBox(blob.Value);
                        break;
                    }
                }
            }
            var success = biggestBlob != null;
            bBox = this.box;

            if (success)
            {
                // Deep copy the blob's information
                rect = new Rectangle(new Point(biggestBlob.BoundingBox.X, biggestBlob.BoundingBox.Y),
                                        new System.Drawing.Size(biggestBlob.BoundingBox.Size.Width, biggestBlob.BoundingBox.Height));
                this.lastBlob = biggestBlob.Centroid;
                this.lastSize = biggestBlob.Area;
            }
            else
            {
                // Welp, we tried to find the ball
                framesLost++;
            }

            points.Dispose();

            return success;
        }
        private void updateBox(CvBlob newBlob)
        {
            float toAddX = 0, toAddY = 0;
            if (this.lastBlob != null)
            {
                toAddX = lastBlob.X - newBlob.Centroid.X;
                toAddY = lastBlob.Y - newBlob.Centroid.Y;

                if (toAddX < 0)
                    toAddX *= -1;
                if (toAddY < 0)
                    toAddY *= -1;
            }

            System.Drawing.Size toInflate = new System.Drawing.Size();
            if (newBlob.Area > 10)
            {
                toInflate = new System.Drawing.Size(newBlob.BoundingBox.Width * 2 + (int)toAddX * 4,
                                        newBlob.BoundingBox.Height * 2 + (int)toAddY * 4);
            }
            else
            {
                toInflate = new System.Drawing.Size(30 + (int)toAddX * 4, 20 + (int)toAddY * 4);
            }

            this.box.Inflate(toInflate);
        }
    }
}
