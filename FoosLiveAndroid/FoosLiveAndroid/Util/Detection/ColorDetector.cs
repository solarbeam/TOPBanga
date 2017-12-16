using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Detection
{
    /// <summary>
    /// The class contains functions to detect a table by contours and a blob by color
    /// </summary>
    class ColorDetector : IColorDetector
    {
        private static readonly string Tag = typeof(MenuActivity).Name;
        private const int VerticeCount = 4;

        private readonly int DefaultThreshold = PropertiesManager.GetIntProperty("default_threshold");
        private readonly int DefaultContourArea = PropertiesManager.GetIntProperty("default_contour_area");
        private readonly double CannyThreshold = PropertiesManager.GetDoubleProperty("canny_threshold");
        private readonly double CannyThresholdLinking = PropertiesManager.GetDoubleProperty("canny_threshold_linking");
        private readonly int MinAngle = PropertiesManager.GetIntProperty("min_angle");
        private readonly int MaxAngle = PropertiesManager.GetIntProperty("max_angle");

        private const int Iterations = 1;

        /// <summary>
        /// False if the box field is null
        /// True if the box field is not null
        /// </summary>
        private bool _boxSet = false;
        // Todo: redundant variable
        private bool _started = false;
        // Todo: redundant variable
        private Rectangle _preliminaryBlob;
        /// <summary>
        /// Defines the bounding box, in which we search for the blob
        /// </summary>
        private Rectangle _box;
        /// <summary>
        /// Defines the starting box's width
        /// </summary>
        private int BoxWidth = PropertiesManager.GetIntProperty("starting_box_width");
        /// <summary>
        /// Defines the starting box's height
        /// </summary>
        private int BoxHeight = PropertiesManager.GetIntProperty("starting_box_height");
        /// <summary>
        /// Count how many frames a blob was not detected
        /// </summary>
        private int _framesLost = 0;
        /// <summary>
        /// Defines the count of frames a blob is allowed to not be detected
        /// </summary>
        private readonly int FramesLostToNewBoundingBox = PropertiesManager.GetIntProperty("frames_lost_to_new_bounding_box");
        /// <summary>
        /// Defines the last calculated size of the blob
        /// </summary>
        private PointF _lastBlob;
        /// <summary>
        /// Defines the last known size of the blob
        /// </summary>
        private int _lastSize = 0;
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

        /// <summary>
        /// The detector's image, used for calculations
        /// </summary>
        public Image<Hsv, byte> image { get; set; }

        private BlobDetector _blobDetector;

        /// <summary>
        /// The threshold, which defines the range of colors
        /// </summary>
        public int Threshold { get; set; }
        // Todo: redundant variable
        private int minContourArea;
        public int MinContourArea { get; set; }

        /// <summary>
        /// Creates the ColorDetector class with the appropriate threshold
        /// </summary>
        public ColorDetector()
        {
            Threshold = DefaultThreshold;
            MinContourArea = DefaultContourArea;
            _box = new Rectangle();
            _blobDetector = new BlobDetector();
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
            var lowerLimit = new Hsv(ballHsv.Hue - Threshold / 2, ballHsv.Satuation - Threshold * 1.3f, ballHsv.Value - Threshold * 1.3f);
            var upperLimit = new Hsv(ballHsv.Hue + Threshold / 2, ballHsv.Satuation + Threshold * 1.3f, ballHsv.Value + Threshold * 1.3f);

            var imgFiltered = image.InRange(lowerLimit, upperLimit);

            // Define the class, which will store information about blobs found
            var points = new CvBlobs();

            // Get the blobs found out of the filtered image and the count
            var count = _blobDetector.GetBlobs(imgFiltered, points);

            // If the blob was lost for an amount of frames, reset the bounding box
            if (_framesLost > FramesLostToNewBoundingBox || !_boxSet)
            {
                _box.Width = 0;
                _box.Height = 0;
                _box.X = image.Size.Width / 2;
                _box.Y = image.Size.Height / 2;
                _box.Inflate(new Size(BoxWidth, BoxHeight / 2));
                _framesLost = 0;
                _boxSet = true;
            }

            // Cleanup the filtered image, as it will not be needed anymore
            imgFiltered.Dispose();

            blobBox = _box;

            // If there were 0 blobs, return false
            if (count == 0)
            {
                points.Dispose();
                _framesLost++;
                return false;
            }
            
            CvBlob biggestBlob = null;
            foreach (var pair in points.OrderByDescending(e => e.Value.Area))
            {
                // Check if the blob is within the predefined bounding box and is of a given size
                if (!_box.Contains((int) pair.Value.Centroid.X, (int) pair.Value.Centroid.Y)) continue;
                // It is, so we pressume it to be the ball
                biggestBlob = pair.Value;
                UpdateBox(biggestBlob);
                _framesLost = 0;
                _lastSize = biggestBlob.Area;
                break;
            }

            // If a blob wasn't found, find the one with the area in a range close to the last one
            if (biggestBlob == null)
            {
                foreach (var blob in points)
                {
                    if (blob.Value.Area <= _lastSize - SizeDiff || blob.Value.Area >= _lastSize + SizeDiff)
                        continue;
                    biggestBlob = blob.Value;
                    _lastBlob = biggestBlob.Centroid;
                    UpdateBox(blob.Value);
                    _framesLost = 0;
                    break;
                }
            }

            // Check if a blob was found
            var success = biggestBlob != null;
            blobBox = _box;

            if (success)
            {
                // Deep copy the blob's information
                rect = new Rectangle(new Point(biggestBlob.BoundingBox.X, biggestBlob.BoundingBox.Y),
                                        new System.Drawing.Size(biggestBlob.BoundingBox.Size.Width, biggestBlob.BoundingBox.Height));
                _lastBlob = biggestBlob.Centroid;
            }
            else
            {
                // Welp, we tried to find the ball
                _framesLost++;
            }

            // Cleanup
            points.Dispose();

            return success;
        }
        private void UpdateBox(CvBlob newBlob)
        {
            _box = newBlob.BoundingBox;
            float toAddX = 0, toAddY = 0;
            if (_lastBlob != null) // Todo: check why it's always true
            {
                toAddX = _lastBlob.X - newBlob.Centroid.X;
                toAddY = _lastBlob.Y - newBlob.Centroid.Y;

                if (toAddX < 0)
                    toAddX *= -1;
                if (toAddY < 0)
                    toAddY *= -1;
            }

            var toInflate = new Size();
            if (newBlob.Area > MinBlobSize)
            {
                toInflate = new Size(newBlob.BoundingBox.Width * MulDeltaWidth + (int)toAddX * MulDeltaX,
                                        newBlob.BoundingBox.Height * MulDeltaHeight + (int)toAddY * MulDeltaY);
            }
            else
            {
                toInflate = new Size(MinWidth + (int)toAddX * MulDeltaX, MinHeight + (int)toAddY * MulDeltaY);
            }

            _box.Inflate(toInflate);
        }
    }
}