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

namespace FoosLiveAndroid.Util.GameControl
{
    /// <summary>
    /// Defines the primary class for heatmap info generation
    /// </summary>
    class RowDrawer
    {
        private RectF[,] zones;
        private int[,] values;
        private int height;
        private int width;

        public RowDrawer(Rect tableInfo, int width, int height)
        {
            zones = new RectF[height,width];
            values = new int[height, width];
            Point topLeftCorner = new Point(tableInfo.Left, tableInfo.Top);
            this.width = width;
            this.height = height;
            int toAddX = 0, toAddY = 0;
            int zoneHeight = (( tableInfo.Bottom - tableInfo.Top ) / width );
            int zoneWidth = (( tableInfo.Right - tableInfo.Left ) / height);

            for (int i = 0; i < height; i ++, toAddY += zoneHeight)
            {
                for (int j = 0; j < width; j ++, toAddX += zoneWidth)
                {
                    zones[i, j] = new RectF(topLeftCorner.X + toAddX, topLeftCorner.Y + toAddY,
                                            topLeftCorner.X + zoneWidth, topLeftCorner.Y + zoneHeight);
                }
                toAddX = 0;
            }
        }

        public void AssignValue(PointF point)
        {
            if (point == null)
                return;

            for (int i = 0; i < height; i ++)
            {
                for (int j = 0; j < width; j ++)
                {
                    if (zones[i,j].Contains(point.X, point.Y))
                    {
                        values[i, j]++;
                        break;
                    }
                }
            }
        }
    }
}