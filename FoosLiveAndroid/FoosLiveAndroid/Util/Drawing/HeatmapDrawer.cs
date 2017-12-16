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
        private static readonly int MaxAlphaValue = PropertiesManager.GetIntProperty("trail_alpha_max");
        private static readonly int MaxHue = PropertiesManager.GetIntProperty("max_hue");
        private static readonly int MaxSaturation = PropertiesManager.GetIntProperty("max_saturation");
        private static readonly int MaxValue = PropertiesManager.GetIntProperty("max_value");

        public static Canvas DrawZones(Canvas canvas, ZoneInfo zones)
        {
            System.Drawing.Size sizeOfBitmap = new System.Drawing.Size(canvas.Width, canvas.Height);
            PointF topLeftCorner = new PointF(0,0);

            // Find the max value
            int max = 0;
            for (int i = 0; i < zones.Height; i ++)
            {
                for (int j = 0; j < zones.Width; j ++)
                {
                    if (max < zones.Values[i, j])
                        max = zones.Values[i, j];
                }
            }

            // Initialize the colorspace
            Color[] colours = {
                Color.Argb(MaxAlphaValue, 0, 0, 0) ,
                Color.Argb(MaxAlphaValue, 0, 0, 0xFF) ,
                Color.Argb(MaxAlphaValue, 0, 0xFF, 0xFF) ,
                Color.Argb(MaxAlphaValue, 0, 0xFF, 0) ,
                Color.Argb(MaxAlphaValue, 0xFF, 0xFF, 0) ,
                Color.Argb(MaxAlphaValue, 0xFF, 0, 0) ,
                Color.Argb(MaxAlphaValue, 0xFF, 0xFF, 0xFF)
            };

            // Draw the zones
            Paint paint = new Paint();
            float zoneWidth = sizeOfBitmap.Width / (float)zones.Width;
            float zoneHeight = sizeOfBitmap.Height / (float)zones.Height;
            float toAddX = 0, toAddY = 0;
            for (int i = 0; i < zones.Height; i ++)
            {
                for (int j = 0; j < zones.Width; j ++)
                {
                    float multiplier = zones.Values[i, j] / (float)max;

                    paint.Color = CalculateColor(zones.Values[i, j], max, colours);

                    canvas.DrawRect(new RectF(topLeftCorner.X + toAddX,
                                        topLeftCorner.Y + toAddY,
                                        topLeftCorner.X + toAddX + zoneWidth,
                                        topLeftCorner.Y + toAddY + zoneHeight),
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

            return Color.Argb(MaxAlphaValue, (byte)red, (byte)green, (byte)blue);
        }
    }
}