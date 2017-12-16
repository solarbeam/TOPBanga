using Android.Graphics;

namespace FoosLiveAndroid.Util.GameControl
{
    /// <summary>
    /// Defines the primary class for heatmap info generation
    /// </summary>
    public class ZoneInfo
    {
        public int[,] Values
        {
            get;
        }
        public int Height
        {
            get;
        }
        public int Width
        {
            get;
        }

        private float _zoneWidth;
        private float _zoneHeight;
        private float _topLeftX;
        private float _topLeftY;

        public ZoneInfo(RectF tableInfo, int width, int height)
        {
            Values = new int[height, width];
            // Todo: redudantant variable
            var topLeftCorner = new PointF(tableInfo.Left, tableInfo.Top);
            Width = width;
            Height = height;
            _zoneHeight = (( tableInfo.Bottom - tableInfo.Top ) / width);
            _zoneWidth = (( tableInfo.Right - tableInfo.Left ) / height);
            _topLeftX = tableInfo.Left;
            _topLeftY = tableInfo.Top;
        }

        public void AssignValue(PointF point)
        {
            if (point == null)
                return;

            var x = point.X - _topLeftX;
            var y = point.Y - _topLeftY;

            var posX = (int)(x / _zoneWidth);
            var posY = (int)(y / _zoneHeight);

            if (posX < 0 || posY < 0 || posX >= Width || posY >= Height) return;
            Values[posY, posX] += 3;

            int toAddY = -1, toAddX = -1;
            for (var i = 0; i < 2; i ++)
            {
                for (var j = 0; j < 2; j ++)
                {
                    if ((posX + toAddX < Width && posX + toAddX > 0) &&
                        (posY + toAddY < Height && posY + toAddY > 0))
                    {
                        Values[posX + toAddX, posY + toAddY]++;
                    }
                    toAddX ++;
                }
                toAddX = 0;
                toAddY++;
            }
        }
    }
}