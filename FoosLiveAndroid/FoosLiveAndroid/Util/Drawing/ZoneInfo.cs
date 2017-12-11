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
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Android.Util;

namespace FoosLiveAndroid.Util.Drawing
{
    class ZoneInfo
    {
        public static Canvas DrawZones(int[] zoneInfo, Canvas toDraw)
        {
            if (zoneInfo.Length != 8)
                return toDraw;

            float[] values = new float[zoneInfo.Length];
            int total = 0;

            // calculate the total
            foreach (var value in zoneInfo)
                total += value;

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = zoneInfo[i] / (float)total;
            }

            System.Drawing.Size sizeOfBitmap = new System.Drawing.Size((int)(toDraw.Width * 0.4f),
                                                                        (int)(toDraw.Height * 0.26f));
            Android.Graphics.PointF topLeftCorner = new Android.Graphics.PointF((toDraw.Width / 2) - sizeOfBitmap.Width / 2,
                                                                                toDraw.Height * 0.48f);

            float toAdd = 0;
            for (int i = 0; i < zoneInfo.Length; i ++)
            {
                Paint paint = new Paint();
                paint.Color = new Android.Graphics.Color((int)(200 * values[i]), (int)(200 * (1.0f - values[i])), 0, 100);
                toDraw.DrawRect(topLeftCorner.X, topLeftCorner.Y + toAdd,
                                topLeftCorner.X + sizeOfBitmap.Width, topLeftCorner.Y + toAdd + sizeOfBitmap.Height / 8, paint);
                toAdd += sizeOfBitmap.Height / 8.0f;
                paint.Dispose();
            }

            return toDraw;
        }
    }
}