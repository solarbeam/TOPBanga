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
        //Todo: handle redundant variable
        private static readonly float BallStrokeWidth = PropertiesManager.GetFloatProperty("ball_stroke_width");
        private static readonly float RectStrokeWidth = PropertiesManager.GetFloatProperty("rect_stroke_width");

        private static readonly int TraceMaxAlpha = PropertiesManager.GetIntProperty("trace_max_alpha");
        private static readonly int TraceDivisor = PropertiesManager.GetIntProperty("trace_divisor");
        private static readonly int TraceToAdd = PropertiesManager.GetIntProperty("trace_to_add");

        /// <summary>
        /// The default constructor for the ObjectDetector class
        /// </summary>
        /// <param name="mulX">The upscaling multiplier for the X axis</param>
        /// <param name="mulY">The upscaling multiplier for the Y axis</param>
        /// <param name="detector">The detector used to detect the ball</param>
        /// <param name="controller">The Game Controller, which fires specific events, related to the game</param>
        /// <param name="color">Defines the color of the ball trace effect</param>
        public ObjectDetector(float mulX, float mulY, IColorDetector detector, GameController controller)
        {
            _controller = controller;
            _detector = detector;
            _mulX = mulX;
            _mulY = mulY;
        }

        public void SetColor(Hsv color)
        {
            // Declare the outline style for the ball
            _paintBall = new Paint
            {
                /*
                 * EmguCv holds the HSV values as follows : 0 <= Hue <= 180,
                 * 0 <= Saturation <= 180 and 0 <= Value <= 180, while Android holds the 
                 * values like so: 0 <= Hue <= 360, 0 <= Saturation <= 1 and 0 <= Value <= 1. 
                 * The multiplication and division is just a conversion taking place between
                 * the different frameworks
                 */
                Color = Color.HSVToColor(new[]
                {
                    (float)(color.Hue * 2),
                    (float)(color.Satuation / 180),
                    (float)(color.Value / 180)
                })
            };

            _paintBall.SetStyle(Paint.Style.Stroke);
            _paintBall.StrokeWidth = RectStrokeWidth;
        }

        public bool Detect(Canvas canvas, Hsv ballHsv, Bitmap bitmap)
        {
            // Preliminary checks
            if (canvas == null || _detector == null || bitmap == null)
                return false;

            // Declare temporary variables
            var ballDetected = false;

            // Refresh the detector's image
            _detector.image = new Image<Hsv, byte>(bitmap);

            // Try to detect the ball
            ballDetected = _detector.DetectBall(ballHsv, out var ball, out var bBox);

            // Free unused resources
            _detector.image.Dispose();

            // Clear the image
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);

            if (ballDetected)
            {
                // Update the GameController class with new coordinates
                
                _controller.LastBallCoordinates = new PointF((ball.Left + ball.Right) / 2 * _mulX,
                                                             (ball.Top + ball.Bottom) / 2 * _mulY);
            }
            else
                // No ball was detected, so we let the GameController know that we lost it
                _controller.LastBallCoordinates = null;

            // Paint the trail
            var path = new Path();

            var points = _controller.BallCoordinates.ToArray();
            var toPaint = 10;
            var startSet = false;
            for (var i = points.Length - 1; i > 0; i--)
            {
                if (points[i] == null)
                {
                    toPaint--;

                    if (toPaint == 0)
                        break;
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

                    _paintBall.Alpha = TraceMaxAlpha * (toPaint / TraceDivisor) + TraceToAdd;
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