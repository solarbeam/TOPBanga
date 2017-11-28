using Android.Graphics;
using Emgu.CV;
using Emgu.CV.Structure;
using FoosLiveAndroid.TOPBanga.Detection;

namespace FoosLiveAndroid.Util.Detection
{
    /// <summary>
    /// A class, which detects the table and ball, and, if detected,
    /// draws them on the given canvas
    /// </summary>
    class ObjectDetector
    {
        private ColorDetector detector;
        private float mul;
        public ObjectDetector(float mul, ColorDetector detector)
        {
            this.detector = detector;
            this.mul = mul;
        }
        public bool Detect(Canvas canvas, Hsv ballHsv, Bitmap bitmap, Bitmap bgBitmap)
        {
            // Preliminary checks
            if (canvas == null || detector == null || bitmap == null)
                return false;

            // Declare temporary variables
            bool tableDetected = false;
            bool ballDetected = false;

            // Refresh the detector's image
            detector.image = new Image<Bgr, byte>(bitmap);

            // Clear the image
            canvas.DrawColor(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            canvas.DrawBitmap(bgBitmap, 0, 0, null);

            // Try to detect a table
            tableDetected = detector.DetectTable(out var table);

            ballDetected = detector.DetectBall(ballHsv, out var ball);
            // Declare the outline style for the table
            var paintRect = new Paint
            {
                Color = new Android.Graphics.Color(255, 0, 0)
            };
            paintRect.SetStyle(Paint.Style.Stroke);

            // Declare the outline style for the ball
            var paintBall = new Paint
            {
                Color = new Android.Graphics.Color(0, 255, 0)
            };
            paintBall.SetStyle(Paint.Style.Stroke);

            // Free unused resources
            detector.image.Dispose();

            if (tableDetected)
            {
                // Get the table points
                var tablePoints = new float[8];

                var j = 0;

                // Assign them values
                for (var i = 0; i < 8; i += 2)
                {
                    tablePoints[i] = table.GetVertices()[j].X * mul;
                    tablePoints[i + 1] = table.GetVertices()[j].Y * mul;
                    j++;
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
                canvas.DrawRect((int)(ball.X * mul),
                                 (int)(ball.Y * mul),
                                 (int)((ball.X + ball.Width) * mul),
                                 (int)((ball.Y + ball.Height) * mul),
                                 paintBall);
            }

            return true;
        }
    }
}