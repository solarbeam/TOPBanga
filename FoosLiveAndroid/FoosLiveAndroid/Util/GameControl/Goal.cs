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
        private const double realWidth = 1.4224f;

        // Defines the real width of the table in meters
        private const double realHeight = 0.7620f;

        private int startingFrame;
        private int endingFrame;

        private PointF[] points;

        public double[] speeds;
        public double maxSpeed;
        public Goal(Queue<PointF> positions, RectF tablePoints)
        {
            points = new PointF[positions.Count];
            speeds = new double[positions.Count];

            // Fill the points array with positions
            int i = 0;
            foreach (var point in positions)
            {
                points[i] = point;
            }

            double mulX = realWidth / (tablePoints.Right - tablePoints.Left);
            double mulY = realHeight / (tablePoints.Bottom - tablePoints.Top);

            // Convert meters into centimeters
            mulX *= 100;
            mulY *= 100;

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
                    speeds[i] = Math.Sqrt( ( ( (point.X - lastPoint.X) * (point.X - lastPoint.X) ) * mulX + 
                                            ( (point.Y - lastPoint.Y) * (point.Y - lastPoint.Y) ) * mulY ) );
                    speeds[i] /= ((double)lostFrameCounter + 1.0f);

                    i++;

                    lastPoint = point;
                    lostFrameCounter = 0;
                }
            }
        }
    }
}