using Android.Graphics;
using Emgu.CV;
using Emgu.CV.Structure;
using FoosLiveAndroid.Util.GameControl;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Detection
{
    /// <summary>
    /// A class, which detects the table and ball, and, if detected,
    /// draws them on the given canvas
    /// </summary>
    class ObjectDetector : IObjectDetector
    {
        /// <summary>
        /// Defines the detector 
        /// </summary>
        private IColorDetector _detector;
        private GameController _controller;
        private float _mulX;
        private float _mulY;
        private Paint _paintBall;
        private static readonly float BallStrokeWidth = PropertiesManager.GetFloatProperty("ball_stroke_width");
        private static readonly float RectStrokeWidth = PropertiesManager.GetFloatProperty("rect_stroke_width");

        /// <summary>
        /// The default constructor for the ObjectDetector class
        /// </summary>
        /// <param name="mulX">The upscaling multiplier for the X axis</param>
        /// <param name="mulY">The upscaling multiplier for the Y axis</param>
        /// <param name="detector">The detector used to detect the ball</param>
        /// <param name="controller">The Game Controller, which fires specific events, related to the game</param>
        public ObjectDetector(float mulX, float mulY, IColorDetector detector, GameController controller, Hsv color)
        {
            _controller = controller;
            _detector = detector;
            _mulX = mulX;
            _mulY = mulY;

            // Declare the outline style for the ball
            _paintBall = new Paint
            {
                Color = Color.HSVToColor(new float[]
                {
                    (float)(color.Hue * 2),
                    (float)(color.Satuation / 180),
                    (float)(color.Value / 180)
                })
            };
            _paintBall.SetStyle(Paint.Style.Stroke);
            _paintBall.StrokeWidth = RectStrokeWidth;
        }
        public bool Detect(Canvas canvas, Hsv ballHsv, Bitmap bitmap, Bitmap bgBitmap)
        {
            // Preliminary checks
            if (canvas == null || _detector == null || bitmap == null)
                return false;

            // Declare temporary variables
            bool ballDetected = false;

            // Refresh the detector's image
            _detector.image = new Image<Hsv, byte>(bitmap);

            // Clear the image
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            canvas.DrawBitmap(bgBitmap, 0, 0, null);

            // Try to detect the ball
            ballDetected = _detector.DetectBall(ballHsv, out var ball, out var bBox);

            // Free unused resources
            _detector.image.Dispose();

            if (ballDetected)
            {
                // Update the GameController class with new coordinates
                _controller.LastBallCoordinates = new PointF(((ball.Left + ball.Right) / 2) * _mulX,
                                                             ((ball.Top + ball.Bottom) / 2) * _mulY);
            }
            else
                // No ball was detected, so we let the GameController know that we lost it
                _controller.LastBallCoordinates = null;

            // Paint the trail
            Path path = new Path();

            PointF[] points = _controller.ballCoordinates.ToArray();
            int toPaint = 10;
            bool startSet = false;
            for (int i = points.Length - 1; i > 0; i--)
            {
                if (points[i] == null)
                {
                    toPaint--;

                    if (toPaint == 0)
                        break;
                    else
                        continue;
                }

                if (startSet)
                {
                    if (i < points.Length - 1 && points[i + 1] != null)
                    {
                        // Apply quadratic beziers in order to smooth the path
                        path.QuadTo(points[i].X, points[i].Y,
                                    points[i + 1].X, points[i + 1].Y);
                    }
                    else
                        path.LineTo(points[i].X, points[i].Y);

                    _paintBall.Alpha = 200 * (toPaint / 9) + 50;
                    canvas.DrawPath(path, _paintBall);
                }
                else
                {
                    path.MoveTo(points[i].X,
                                points[i].Y);
                    startSet = true;
                }

                toPaint--;

                if (toPaint == 0)
                    break;
            }

            path.Dispose();

            return true;
        }
    }
}