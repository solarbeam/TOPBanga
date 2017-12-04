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
    class ColorDetector : IDetector
    {
        public static string Tag = "ColorDetector";
        private const int VerticeCount = 4;

        private readonly int DefaultThreshold = PropertiesManager.GetIntProperty("default_threshold");
        private readonly int DefaultContourArea = PropertiesManager.GetIntProperty("default_contour_area");
        private readonly double CannyThreshold = PropertiesManager.GetDoubleProperty("canny_threshold");
        private readonly double CannyThresholdLinking = PropertiesManager.GetDoubleProperty("canny_threshold_linking");
        private readonly int MinAngle = PropertiesManager.GetIntProperty("min_angle");
        private readonly int MaxAngle = PropertiesManager.GetIntProperty("max_angle");

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
        private int boxWidth = PropertiesManager.GetIntProperty("starting_box_width");
        /// <summary>
        /// Defines the starting box's height
        /// </summary>
        private int boxHeight = PropertiesManager.GetIntProperty("starting_box_height");
        /// <summary>
        /// Count how many frames a blob was not detected
        /// </summary>
        private int framesLost = 0;
        /// <summary>
        /// Defines the count of frames a blob is allowed to not be detected
        /// </summary>
        private readonly int framesLostToNewBoundingBox = PropertiesManager.GetIntProperty("frames_lost_to_new_bounding_box");
        /// <summary>
        /// Defines the last calculated size of the blob
        /// </summary>
        private PointF lastBlob;
        /// <summary>
        /// Defines the last known size of the blob
        /// </summary>
        private int lastSize = 0;
        /// <summary>
        /// Defines the permitted size difference between blobs
        /// </summary>
        private int SizeDiff = PropertiesManager.GetIntProperty("size_difference");
        /// <summary>
        /// Defines the limit for the bounding box sizing algorithm
        /// </summary>
        private readonly int MinBlobSize = PropertiesManager.GetIntProperty("min_blob_size");

        /// <summary>
        /// Defines the multipliers for the bounding box sizing algorithm
        /// </summary>
        private readonly int MulDeltaX = PropertiesManager.GetIntProperty("multiplier_delta_x");
        private readonly int MulDeltaY = PropertiesManager.GetIntProperty("multiplier_delta_y");
        private readonly int MulDeltaWidth = PropertiesManager.GetIntProperty("multiplier_delta_width");
        private readonly int MulDeltaHeight = PropertiesManager.GetIntProperty("multiplier_delta_height");
        private readonly int MinWidth = PropertiesManager.GetIntProperty("min_width");
        private readonly int MinHeight = PropertiesManager.GetIntProperty("min_height");
        private readonly double MinTableSize = PropertiesManager.GetDoubleProperty("min_table_size");

        /// <summary>
        /// The detector's image, used for calculations
        /// </summary>
        public Image<Hsv, byte> image { get; set; }

        private BlobDetector blobDetector;

        /// <summary>
        /// The threshold, which defines the range of colors
        /// </summary>
        public int Threshold { get; set; }

        private int minContourArea;
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
        public ColorDetector()
        {
            Threshold = DefaultThreshold;
            MinContourArea = DefaultContourArea;
            box = new Rectangle();
            blobDetector = new BlobDetector();
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

            // Cleanup
            cannyEdges.Dispose();
            uimage.Dispose();

            return success;
        }

        /// <summary>
        /// Detects a ball using the predefined image, stored in this class,
        /// and the specific Hsv
        /// </summary>
        /// <param name="ballHsv">The Hsv values, which are to be used in calculations</param>
        /// <param name="rect">The rectangle, which holds the information about the blob, if such was found</param>
        /// <param name="blobBox">Defines the rectangle, in which we search for the blob</param>
        /// <returns>True if a ball was detected. False otherwise</returns>
        public bool DetectBall(Hsv ballHsv, out Rectangle rect, out Rectangle blobBox)
        {
            //default returns
            rect = new Rectangle();

            // Define the upper and lower limits of the Hue and Saturation values
            Hsv lowerLimit = new Hsv(ballHsv.Hue - Threshold, ballHsv.Satuation - Threshold, ballHsv.Value - Threshold);
            Hsv upperLimit = new Hsv(ballHsv.Hue + Threshold, ballHsv.Satuation + Threshold, ballHsv.Value + Threshold);

            Image<Gray, byte> imgFiltered = image.InRange(lowerLimit, upperLimit);

            // Define the class, which will store information about blobs found
            var points = new CvBlobs();

            // Get the blobs found out of the filtered image and the count
            var count = blobDetector.GetBlobs(imgFiltered, points);

            // If the blob was lost for an amount of frames, reset the bounding box
            if (framesLost > framesLostToNewBoundingBox || !boxSet)
            {
                box.Width = 0;
                box.Height = 0;
                box.X = image.Size.Width / 2;
                box.Y = image.Size.Height / 2;
                box.Inflate(new System.Drawing.Size(boxWidth, boxHeight / 2));
                framesLost = 0;
                boxSet = true;
            }

            // Cleanup the filtered image, as it will not be needed anymore
            imgFiltered.Dispose();

            blobBox = box;

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
                if (box.Contains((int)pair.Value.Centroid.X, (int)pair.Value.Centroid.Y))
                {
                    // It is, so we pressume it to be the ball
                    biggestBlob = pair.Value;
                    UpdateBox(biggestBlob);
                    framesLost = 0;
                    lastSize = biggestBlob.Area;
                    break;
                }
            }

            // If a blob wasn't found, find the one with the area in a range close to the last one
            if (biggestBlob == null)
            {
                foreach (var blob in points)
                {
                    if (blob.Value.Area > ( lastSize - SizeDiff ) && blob.Value.Area < ( lastSize + SizeDiff ) )
                    {
                        biggestBlob = blob.Value;
                        lastBlob = biggestBlob.Centroid;
                        UpdateBox(blob.Value);
                        break;
                    }
                }
            }

            // Check if a blob was found
            var success = biggestBlob != null;
            blobBox = box;

            if (success)
            {
                // Deep copy the blob's information
                rect = new Rectangle(new Point(biggestBlob.BoundingBox.X, biggestBlob.BoundingBox.Y),
                                        new System.Drawing.Size(biggestBlob.BoundingBox.Size.Width, biggestBlob.BoundingBox.Height));
                this.lastBlob = biggestBlob.Centroid;
            }
            else
            {
                // Welp, we tried to find the ball
                framesLost++;
            }

            // Cleanup
            points.Dispose();

            return success;
        }
        private void UpdateBox(CvBlob newBlob)
        {
            box = newBlob.BoundingBox;
            float toAddX = 0, toAddY = 0;
            if (lastBlob != null) // Todo: check why it's always true
            {
                toAddX = lastBlob.X - newBlob.Centroid.X;
                toAddY = lastBlob.Y - newBlob.Centroid.Y;

                if (toAddX < 0)
                    toAddX *= -1;
                if (toAddY < 0)
                    toAddY *= -1;
            }

            System.Drawing.Size toInflate = new System.Drawing.Size();
            if (newBlob.Area > MinBlobSize)
            {
                toInflate = new System.Drawing.Size(newBlob.BoundingBox.Width * MulDeltaWidth + (int)toAddX * MulDeltaX,
                                        newBlob.BoundingBox.Height * MulDeltaHeight + (int)toAddY * MulDeltaY);
            }
            else
            {
                toInflate = new System.Drawing.Size(MinWidth + (int)toAddX * MulDeltaX, MinHeight + (int)toAddY * MulDeltaY);
            }

            box.Inflate(toInflate);
        }
    }
}
