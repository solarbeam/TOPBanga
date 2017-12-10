﻿using Android.Graphics;
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
        private Paint _paintRect;
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
        public ObjectDetector(float mulX, float mulY, ColorDetector detector, GameController controller)
        {
            _controller = controller;
            _detector = detector;
            _mulX = mulX;
            _mulY = mulY;

            // Declare the outline style for the table
            _paintRect = new Paint
            {
                Color = new Color(255, 0, 0)
            };
            _paintRect.SetStyle(Paint.Style.Stroke);
            _paintRect.StrokeWidth = RectStrokeWidth;

            // Declare the outline style for the ball
            _paintBall = new Paint
            {
                Color = new Color(0, 255, 0)
            };
            _paintBall.SetStyle(Paint.Style.Stroke);
            _paintBall.StrokeWidth = BallStrokeWidth;
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
            
            canvas.DrawRect((int)(bBox.Left * _mulX),
                                 (int)(bBox.Top * _mulY),
                                 (int)(bBox.Right * _mulX),
                                 (int)(bBox.Bottom * _mulY),
                                 _paintRect);

            // Free unused resources
            _detector.image.Dispose();

            if (ballDetected)
            {
                // The ball was detected, so we draw it
                canvas.DrawRect((int)(ball.Left * _mulX),
                                 (int)(ball.Top * _mulY),
                                 (int)(ball.Right * _mulX),
                                 (int)(ball.Bottom * _mulY),
                                 _paintBall);

                // Update the GameController class with new coordinates
                _controller.LastBallCoordinates = new PointF(ball.X * _mulX, ball.Y * _mulY);
            }
            else
                // No ball was detected, so we let the GameController know that we lost it
                _controller.LastBallCoordinates = null;

            return true;
        }
    }
}