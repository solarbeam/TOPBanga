﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using FoosLiveAndroid.Util.GameControl;

namespace FoosLiveAndroid.Util.Drawing
{
    class HeatmapDrawer
    {
        private const int maxAlphaValue = 100;
        private const int maxRValue = 200;
        private const int maxGValue = 200;
        private const int maxBValue = 0;
        public static Canvas DrawZones(Canvas canvas, ZoneInfo zones)
        {
            System.Drawing.Size sizeOfBitmap = new System.Drawing.Size(canvas.Width, canvas.Height);
            PointF topLeftCorner = new PointF(0,0);

            // Find the max value
            int max = 0;
            for (int i = 0; i < zones.height; i ++)
            {
                for (int j = 0; j < zones.width; j ++)
                {
                    if (max < zones.values[i, j])
                        max = zones.values[i, j];
                }
            }

            // Draw the zones
            float zoneWidth = sizeOfBitmap.Width / (float)zones.width;
            float zoneHeight = sizeOfBitmap.Height / (float)zones.height;
            float toAddX = 0, toAddY = 0;
            for (int i = 0; i < zones.height; i ++)
            {
                for (int j = 0; j < zones.width; j ++)
                {
                    float multiplier = zones.values[i,j] / (float)max;

                    Paint paint = new Paint()
                    {
                        Color = new Color((int)(maxRValue * multiplier),
                                            (int)(maxGValue * (1 - multiplier)),
                                            (int)(maxBValue * multiplier))
                    };

                    canvas.DrawRect(topLeftCorner.X + toAddX,
                                    topLeftCorner.Y + toAddY,
                                    topLeftCorner.X + zoneWidth + toAddX,
                                    topLeftCorner.Y + zoneHeight + toAddY,
                                    paint);
                    paint.Dispose();
                    toAddX += zoneWidth;
                }
                toAddX = 0;
                toAddY += zoneHeight;
            }

            return canvas;
        }
    }
}