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

        private float _zoneWidth;
        private float _zoneHeight;
        private float _topLeftX;
        private float _topLeftY;

        public ZoneInfo(RectF tableInfo, int width, int height)
        {
            values = new int[height, width];
            PointF topLeftCorner = new PointF(tableInfo.Left, tableInfo.Top);
            this.width = width;
            this.height = height;
            _zoneHeight = (( tableInfo.Bottom - tableInfo.Top ) / width);
            _zoneWidth = (( tableInfo.Right - tableInfo.Left ) / height);
            _topLeftX = tableInfo.Left;
            _topLeftY = tableInfo.Top;
        }

        public void AssignValue(PointF point)
        {
            if (point == null)
                return;

            float x = point.X - _topLeftX;
            float y = point.Y - _topLeftY;

            int posX = (int)(x / _zoneWidth);
            int posY = (int)(y / _zoneHeight);

            if (posX < width && posY < height)
            {
                values[posY, posX] += 3;

                int toAddY = -1, toAddX = -1;
                for (int i = 0; i < 2; i ++)
                {
                    for (int j = 0; j < 2; j ++)
                    {
                        if ((posX + toAddX < width && posX + toAddX > 0) &&
                            (posY + toAddY < height && posY + toAddY > 0))
                        {
                            values[posX + toAddX, posY + toAddY]++;
                        }
                        toAddX ++;
                    }
                    toAddX = 0;
                    toAddY++;
                }
            }
        }
    }
}