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

        private readonly int DefaultThreshold = PropertiesManager.GetIntProperty("default_threshold");

        /// <summary>
        /// False if the box field is null
        /// True if the box field is not null
        /// </summary>
        private bool _boxSet = false;
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
        /// Defines the limit for the bounding box sizing algorithm
        /// </summary>
        private readonly int MinBlobSize = PropertiesManager.GetIntProperty("min_blob_size");
        private readonly int HsvDivisor = 4;
        private readonly float SaturationMultiplier = 1.3f;
        private readonly float ValueMultiplier = 1.3f;

        /// <summary>
        /// Defines the multipliers for the bounding box sizing algorithm
        /// </summary>
        private readonly int MulDeltaX = PropertiesManager.GetIntProperty("multiplier_delta_x");
        private readonly int MulDeltaY = PropertiesManager.GetIntProperty("multiplier_delta_y");
        private readonly int MulDeltaWidth = PropertiesManager.GetIntProperty("multiplier_delta_width");
        private readonly int MulDeltaHeight = PropertiesManager.GetIntProperty("multiplier_delta_height");
        private readonly int MinWidth = PropertiesManager.GetIntProperty("min_width");
        private readonly int MinHeight = PropertiesManager.GetIntProperty("min_height");

        private readonly int MinAddition = 5;
        private readonly int MaxAddition = 5;

        /// <summary>
        /// The detector's image, used for calculations
        /// </summary>
        public Image<Hsv, byte> image { get; set; }

        private BlobDetector _blobDetector;

        /// <summary>
        /// The threshold, which defines the range of colors
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// Defines the minimum size allowed for the blob to pass
        /// </summary>
        private int _minAllowed;
        /// <summary>
        /// Defines the maximum size allowed for the blob to pass
        /// </summary>
        private int _maxAllowed;

        /// <summary>
        /// Creates the ColorDetector class with the appropriate threshold
        /// </summary>
        public ColorDetector()
        {
            Threshold = DefaultThreshold;
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

            /**
             * In order to boost tracking accuracy in all lighting conditions, I reduce the base color
             * min and max values and boost the black and white intensities ( in this case, its 
             * the Saturation and Value values ). This in turn helps us detect the ball, even in poor conditions, 
             * but not all of the lighting conditions are touched. This will be addressed sometime in the future
             */
            Hsv lowerLimit = new Hsv(ballHsv.Hue - Threshold / HsvDivisor,
                ballHsv.Satuation - Threshold * SaturationMultiplier,
                ballHsv.Value - Threshold * ValueMultiplier);
            Hsv upperLimit = new Hsv(ballHsv.Hue + Threshold / HsvDivisor,
                ballHsv.Satuation + Threshold * SaturationMultiplier,
                ballHsv.Value + Threshold * ValueMultiplier);

            Image<Gray, byte> imgFiltered = image.InRange(lowerLimit, upperLimit);

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
                if (_box.Contains((int)pair.Value.Centroid.X, (int)pair.Value.Centroid.Y))
                {
                    // It is, so we pressume it to be the ball

                    /**
                     * The blob can differ in size when detected by the tracking algorithm. 
                     * In this case, I check whether the blob, accepted by the algorithm, is lower than
                     * the previous minimum. This in turn lets us "remember" all of the blob sizes, should
                     * we detect them in the future, and also allows to detect pretty fast shots
                     */
                    if (_minAllowed == 0)
                        _minAllowed = pair.Value.Area;
                    else
                        if (_minAllowed > pair.Value.Area)
                        _minAllowed = pair.Value.Area - MinAddition;

                    if (_maxAllowed < pair.Value.Area)
                        _maxAllowed = pair.Value.Area + MaxAddition;

                    biggestBlob = pair.Value;
                    UpdateBox(biggestBlob);
                    _framesLost = 0;
                    _lastSize = biggestBlob.Area;
                    break;
                }
                else
                    if (pair.Value.Area > _minAllowed && pair.Value.Area < _maxAllowed)
                {
                    biggestBlob = pair.Value;
                    _lastBlob = biggestBlob.Centroid;
                    UpdateBox(pair.Value);
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

            _box.Inflate(toInflate);
        }
    }
}