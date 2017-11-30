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
        private float _scaleMultiplier;

        public ObjectDetector(float scaleMultiplier, ColorDetector detector)
        {
            _detector = detector;
            _scaleMultiplier = scaleMultiplier;
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

            ballDetected = _detector.DetectBall(ballHsv, out var ball);
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
                    tablePoints[i] = table.GetVertices()[j].X * _scaleMultiplier;
                    tablePoints[i + 1] = table.GetVertices()[j].Y * _scaleMultiplier;
                }

                // Finally, draw the rectangle
                canvas.DrawLine(tablePoints[0], tablePoints[1], tablePoints[2], tablePoints[3], paintRect);
                canvas.DrawLine(tablePoints[2], tablePoints[3], tablePoints[4], tablePoints[5], paintRect);
                canvas.DrawLine(tablePoints[4], tablePoints[5], tablePoints[6], tablePoints[7], paintRect);
                canvas.DrawLine(tablePoints[6], tablePoints[7], tablePoints[0], tablePoints[1], paintRect);
            }
            else
                return false;

            if (ballDetected)
            {
                canvas.DrawRect((int)(ball.X * _scaleMultiplier),
                                 (int)(ball.Y * _scaleMultiplier),
                                 (int)((ball.X + ball.Width) * _scaleMultiplier),
                                 (int)((ball.Y + ball.Height) * _scaleMultiplier),
                                 paintBall);
            }

            return true;
        }
    }
}