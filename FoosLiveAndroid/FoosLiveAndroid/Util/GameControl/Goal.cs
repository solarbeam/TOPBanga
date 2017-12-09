using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections;
using Android.Graphics;
using Android.Util;

namespace FoosLiveAndroid.Util.GameControl
{
    class Goal
    {
        // Defines the real height of the table in meters
        private readonly double RealWidth = PropertiesManager.GetDoubleProperty("realWidth");

        // Defines the real width of the table in meters
        private readonly double RealHeight = PropertiesManager.GetDoubleProperty("realHeight");

        /// <summary>
        /// Defines the amount of centimeters in a meter
        /// </summary>
        private readonly int CentimetersInAMeter = PropertiesManager.GetIntProperty("centimetersInAMeter");

        private PointF[] _points;

        public double[] _speeds;
        public double _maxSpeed;
        public Goal(Queue<PointF> positions, RectF tablePoints)
        {
            _points = new PointF[positions.Count];
            _speeds = new double[positions.Count];

            // Fill the points array with positions
            int i = 0;
            foreach (var point in positions)
            {
                _points[i] = point;
            }

            double mulX = RealWidth / (tablePoints.Right - tablePoints.Left);
            double mulY = RealHeight / (tablePoints.Bottom - tablePoints.Top);

            // Convert meters into centimeters
            mulX *= CentimetersInAMeter;
            mulY *= CentimetersInAMeter;

            // Calculate the speeds
            PointF lastPoint = null;
            int lostFrameCounter = 0;
            i = 0;
            foreach (var point in positions)
            {
                if (lastPoint == null)
                {
                    lastPoint = point;
                    continue;
                }

                if (point == null)
                {
                    lostFrameCounter++;
                    i++;
                    continue;
                }
                else
                {
                    _speeds[i] = Math.Sqrt( (point.X * mulX - lastPoint.X * mulX) * (point.X * mulX - lastPoint.X * mulX) + 
                                            (point.Y * mulY - lastPoint.Y * mulY) * (point.Y * mulY - lastPoint.Y * mulY) );
                    _speeds[i] /= (lostFrameCounter + 1.0f);

                    i++;

                    lastPoint = point;
                    lostFrameCounter = 0;
                }
            }
        }
    }
}