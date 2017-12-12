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
using Android.Util;

namespace FoosLiveAndroid.Util.GameControl
{
    /// <summary>
    /// Defines the primary class for heatmap info generation
    /// </summary>
    public class ZoneInfo
    {
        public int[,] values
        {
            get;
            private set;
        }
        public int height
        {
            get;
            private set;
        }
        public int width
        {
            get;
            private set;
        }

        private float zoneWidth;
        private float zoneHeight;
        private float topLeftX;
        private float topLeftY;

        public ZoneInfo(RectF tableInfo, int width, int height)
        {
            values = new int[height, width];
            PointF topLeftCorner = new PointF(tableInfo.Left, tableInfo.Top);
            this.width = width;
            this.height = height;
            zoneHeight = (( tableInfo.Bottom - tableInfo.Top ) / width);
            zoneWidth = (( tableInfo.Right - tableInfo.Left ) / height);
            topLeftX = tableInfo.Left;
            topLeftY = tableInfo.Top;
        }

        public void AssignValue(PointF point)
        {
            if (point == null)
                return;

            float x = point.X - topLeftX;
            float y = point.Y - topLeftY;

            int posX = (int)(x / zoneWidth);
            int posY = (int)(y / zoneHeight);

            if (posX < width && posY < height)
                values[posY, posX]++;
        }
    }
}