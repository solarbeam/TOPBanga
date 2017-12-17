using Android.Graphics;

namespace FoosLiveAndroid.Util.GameControl
{
    /// <summary>
    /// Defines the primary class for heatmap info generation
    /// </summary>
    public class ZoneInfo
    {
        private const int _toAddZone1 = 8;
        private const int _toAddZone2 = 4;
        private const int _toAddZone3 = 2;

        public int[,] values
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
            values = new int[height, width];
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

            if (posX >= 0 && posY >= 0  && posX < Width && posY < Height)
            {
                values[posY, posX] += 8;

                for (int i = -2; i < 3; i ++)
                {
                    for (int j = -2; j < 3; j ++)
                    {
                        if ((posX + i < Width && posX + i > 0) &&
                            (posY + j < Height && posY + j > 0))
                        {
                            // Defines the outermost points from the center
                            if ((i == -2 || i == 2) && (j == -2 || j == 2))
                                values[posY + j, posX + i] += _toAddZone3;
                            else
                            // Defines the points, which surround the center point
                                if ((i == -1 || i == 1) && (j == -1 || j == 1))
                                values[posY + j, posX + i] += _toAddZone2;
                            else
                                // Defines the center point
                                values[posY + j, posX + i] += _toAddZone3;
                        }
                    }
                }
            }
        }
    }
}