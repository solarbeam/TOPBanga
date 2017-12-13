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
using Android.Graphics;
using FoosLiveAndroid.Util.GameControl;

namespace FoosLiveAndroid.Util.Drawing
{
    class HeatmapDrawer
    {
        private static readonly int maxAlphaValue = PropertiesManager.GetIntProperty("trail_alpha_max");
        private const int maxHue = 255;
        private const int maxSaturation = 180;
        private const int maxValue = 0;
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

            // Initialize the colorspace
            Color[] colours = new Color[]
            {
                Color.Argb(maxAlphaValue, 0, 0, 0) ,
                Color.Argb(maxAlphaValue, 0, 0, 0xFF) ,
                Color.Argb(maxAlphaValue, 0, 0xFF, 0xFF) ,
                Color.Argb(maxAlphaValue, 0, 0xFF, 0) ,
                Color.Argb(maxAlphaValue, 0xFF, 0xFF, 0) ,
                Color.Argb(maxAlphaValue, 0xFF, 0, 0) ,
                Color.Argb(maxAlphaValue, 0xFF, 0xFF, 0xFF)
            };

            // Draw the zones
            Paint paint = new Paint();
            float zoneWidth = sizeOfBitmap.Width / (float)zones.width;
            float zoneHeight = sizeOfBitmap.Height / (float)zones.height;
            float toAddX = 0, toAddY = 0;
            for (int i = 0; i < zones.height; i ++)
            {
                for (int j = 0; j < zones.width; j ++)
                {
                    float multiplier = zones.values[i, j] / (float)max;

                    paint.Color = CalculateColor(zones.values[i, j], max, colours);

                    canvas.DrawRect(topLeftCorner.X + toAddX,
                                    topLeftCorner.Y + toAddY,
                                    topLeftCorner.X + zoneWidth + toAddX,
                                    topLeftCorner.Y + zoneHeight + toAddY,
                                    paint);
                    toAddX += zoneWidth;
                }
                toAddX = 0;
                toAddY += zoneHeight;
            }

            return canvas;
        }

        private static Color CalculateColor(int value, int maxValue, Color[] colours)
        {
            double percentage = value / (double)(maxValue + 1);
            double colorPercentage = 1d / (colours.Length - 1);
            double colorBlock = percentage / colorPercentage;
            int which = (int)Math.Truncate(colorBlock);
            double residue = percentage - (which * colorPercentage);
            double percOfColor = residue / colorPercentage;

            Color target = colours[which];
            Color next = colours[which + 1];

            int redDelta = next.R - target.R;
            int greenDelta = next.G - target.G;
            int blueDelta = next.B - target.B;

            double red = target.R + (redDelta * percOfColor);
            double green = target.G + (greenDelta * percOfColor);
            double blue = target.B + (blueDelta * percOfColor);

            return Color.Argb(maxAlphaValue, (byte)red, (byte)green, (byte)blue);
        }
    }
}