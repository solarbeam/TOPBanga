﻿using System;
using System.Collections.Generic;
using Android.Graphics;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Util.GameControl
{
    public class Goal
    {
        // Defines the real height of the table in meters
        private readonly double RealWidth = PropertiesManager.GetDoubleProperty("real_width");

        // Defines the real width of the table in meters
        private readonly double RealHeight = PropertiesManager.GetDoubleProperty("real_height");

        /// <summary>
        /// Defines the amount of centimeters in a meter
        /// </summary>
        private const int CentimetersInMeter = 100;

        private PointF[] _points;

        public readonly double[] Speeds;
        public readonly double MaxSpeed;
        private readonly long timestampStart;
        private readonly long timestampEnd;

        // Which team scored: true if blue, false if 
        public TeamColor TeamColor { get; }

        public long Duration { get; }

        public Goal(Queue<PointF> positions, RectF tablePoints, long start, long end, TeamColor team)
        {
            _points = new PointF[positions.Count];
            Speeds = new double[positions.Count];
            timestampStart = start;
            timestampEnd = end;
            TeamColor = team;
            
            Duration = TimeSpan.FromMilliseconds(timestampEnd - timestampStart).Seconds;

            // Fill the points array with positions
            var i = 0;
            foreach (var point in positions)
            {
                _points[i] = point;
            }

            var mulX = RealWidth / (tablePoints.Right - tablePoints.Left);
            var mulY = RealHeight / (tablePoints.Bottom - tablePoints.Top);

            // Convert meters into centimeters
            mulX *= CentimetersInMeter;
            mulY *= CentimetersInMeter;

            // Calculate the speeds
            PointF lastPoint = null;
            var lostFrameCounter = 0;
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
                
                Speeds[i] = Math.Sqrt( (point.X * mulX - lastPoint.X * mulX) * (point.X * mulX - lastPoint.X * mulX) + 
                                        (point.Y * mulY - lastPoint.Y * mulY) * (point.Y * mulY - lastPoint.Y * mulY) );
                Speeds[i] /= lostFrameCounter + 1.0f;

                if (MaxSpeed < Speeds[i])
                    MaxSpeed = Speeds[i];

                i++;

                lastPoint = point;
                lostFrameCounter = 0;
            }
        }
    }
}