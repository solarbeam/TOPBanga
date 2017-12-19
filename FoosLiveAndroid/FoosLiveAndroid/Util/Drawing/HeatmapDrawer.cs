using System;
using Android.Graphics;
using FoosLiveAndroid.Util.GameControl;

namespace FoosLiveAndroid.Util.Drawing
{
    class HeatmapDrawer
    {
        private static readonly int MaxAlphaValue = PropertiesManager.GetIntProperty("trail_alpha_max");
        //Todo: handle redundant configuration values
        private static readonly int MaxHue = PropertiesManager.GetIntProperty("max_hue");
        private static readonly int MaxSaturation = PropertiesManager.GetIntProperty("max_saturation");
        private static readonly int MaxValue = PropertiesManager.GetIntProperty("max_value");

        
        //Todo: handle redundant return value
        public static Canvas DrawZones(Canvas canvas, ZoneInfo zones)
        {
            var sizeOfBitmap = new System.Drawing.Size(canvas.Width, canvas.Height);
            var topLeftCorner = new PointF(0,0);

            // Find the max value
            var max = 0;
            for (var i = 0; i < zones.Height; i ++)
            {
                for (var j = 0; j < zones.Width; j ++)
                {
                    if (max < zones.values[i, j])
                        max = zones.values[i, j];
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
            var paint = new Paint();
            var zoneWidth = sizeOfBitmap.Width / (float)zones.Width;
            var zoneHeight = sizeOfBitmap.Height / (float)zones.Height;
            float toAddX = 0, toAddY = 0;
            for (var i = 0; i < zones.Height; i ++)
            {
                for (var j = 0; j < zones.Width; j ++)
                {
                    //Todo: handle redundant variable
                    var multiplier = zones.values[i, j] / (float)max;

                    paint.Color = CalculateColor(zones.values[i, j], max, colours);

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
            var percentage = value / (double)(maxValue + 1);
            var colorPercentage = 1d / (colours.Length - 1);
            var colorBlock = percentage / colorPercentage;
            var which = (int)Math.Truncate(colorBlock);
            var residue = percentage - which * colorPercentage;
            var percOfColor = residue / colorPercentage;

            var targetColor = colours[which];
            var nextColor = colours[which + 1];

            var redDelta = nextColor.R - targetColor.R;
            var greenDelta = nextColor.G - targetColor.G;
            var blueDelta = nextColor.B - targetColor.B;

            var red = targetColor.R + redDelta * percOfColor;
            var green = targetColor.G + greenDelta * percOfColor;
            var blue = targetColor.B + blueDelta * percOfColor;

            return Color.Argb(MaxAlphaValue, (byte)red, (byte)green, (byte)blue);
        }
    }
}