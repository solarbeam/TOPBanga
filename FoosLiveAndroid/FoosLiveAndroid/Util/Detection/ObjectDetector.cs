using Android.Graphics;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Detection
{
    /// <summary>
    /// A class, which detects the table and ball, and, if detected,
    /// draws them on the given canvas
    /// </summary>
    class ObjectDetector
    {
        private ColorDetector _detector;
        private GameController _controller;
        private float _mulX;
        private float _mulY;
        /// <summary>
        /// The default constructor for the ObjectDetector class
        /// </summary>
        /// <param name="mulX">The upscaling multiplier for the X axis</param>
        /// <param name="mulY">The upscaling multiplier for the Y axis</param>
        /// <param name="detector">The detector used to detect the ball</param>
        /// <param name="controller">The Game Controller, which fires specific events, related to the game</param>
        public ObjectDetector(float mulX, float mulY, ColorDetector detector, GameController controller)
        {
            _controller = controller;
            _detector = detector;
            _mulX = mulX;
            _mulY = mulY;
        }
        public bool Detect(Canvas canvas, Hsv ballHsv, Bitmap bitmap, Bitmap bgBitmap)
        {
            // Preliminary checks
            if (canvas == null || _detector == null || bitmap == null)
                return false;

            // Declare temporary variables
            bool tableDetected = false;
            bool ballDetected = false;

            // Refresh the detector's image
            _detector.image = new Image<Bgr, byte>(bitmap);

            // Clear the image
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            canvas.DrawBitmap(bgBitmap, 0, 0, null);

            // Try to detect a table
            tableDetected = _detector.DetectTable(out var table);
            tableDetected = false;

            ballDetected = _detector.DetectBall(ballHsv, out var ball, out var bBox);
            // Declare the outline style for the table
            var paintRect = new Paint
            {
                Color = new Color(255, 0, 0)
            };
            paintRect.SetStyle(Paint.Style.Stroke);

            // Declare the outline style for the ball
            var paintBall = new Paint
            {
                Color = new Color(0, 255, 0)
            };
            paintBall.SetStyle(Paint.Style.Stroke);

            // Free unused resources
            _detector.image.Dispose();

            if (tableDetected)
            {
                // Get the table points
                var tablePoints = new float[8];

                // Assign them values
                for (int i = 0, j = 0; i < tablePoints.Length; i += 2, j++)
                {
                    tablePoints[i] = table.GetVertices()[j].X * _mulX;
                    tablePoints[i + 1] = table.GetVertices()[j].Y * _mulY;
                    j++;
                }

                // Finally, draw the rectangle
                canvas.DrawLine(tablePoints[0], tablePoints[1], tablePoints[2], tablePoints[3], paintRect);
                canvas.DrawLine(tablePoints[2], tablePoints[3], tablePoints[4], tablePoints[5], paintRect);
                canvas.DrawLine(tablePoints[4], tablePoints[5], tablePoints[6], tablePoints[7], paintRect);
                canvas.DrawLine(tablePoints[6], tablePoints[7], tablePoints[0], tablePoints[1], paintRect);
            }

            if (ballDetected)
            {
                canvas.DrawRect((int)(ball.Left * _mulX),
                                 (int)(ball.Top * _mulY),
                                 (int)(ball.Right * _mulX),
                                 (int)(ball.Bottom * _mulY),
                                 paintBall);
                // Update the GameController class with new coordinates
                _controller.LastBallCoordinates = new PointF(ball.X, ball.Y);
            }

            canvas.DrawRect((int)(bBox.Left * _mulX),
                                 (int)(bBox.Top * _mulY),
                                 (int)(bBox.Right * _mulX),
                                 (int)(bBox.Bottom * _mulY),
                                 paintRect);

            return true;
        }
    }
}