using Android.Graphics;

namespace FoosLiveAndroid.Util.Drawing
{
    //Todo: handle redundant class
    class RowDrawer
    {
        //Todo: handle redundant method
        public static Canvas DrawZones(int[] zoneInfo, Canvas toDraw)
        {
            if (zoneInfo.Length != 8)
                return toDraw;

            var values = new float[zoneInfo.Length];
            var total = 0;

            // calculate the total
            foreach (var value in zoneInfo)
                total += value;

            for (var i = 0; i < values.Length; i++)
            {
                values[i] = zoneInfo[i] / (float)total;
            }

            var sizeOfBitmap = new System.Drawing.Size((int)(toDraw.Width * 0.4f),
                                                                        (int)(toDraw.Height * 0.26f));
            var topLeftCorner = new PointF(toDraw.Width / 2 - sizeOfBitmap.Width / 2,
                                                                                toDraw.Height * 0.48f);

            float toAdd = 0;
            for (var i = 0; i < zoneInfo.Length; i ++)
            {
                var paint = new Paint
                {
                    Color = new Color((int) (200 * values[i]), (int) (200 * (1.0f - values[i])), 0, 100)
                };
                toDraw.DrawRect(topLeftCorner.X, topLeftCorner.Y + toAdd,
                                topLeftCorner.X + sizeOfBitmap.Width, topLeftCorner.Y + toAdd + sizeOfBitmap.Height / 8, paint);
                toAdd += sizeOfBitmap.Height / 8.0f;
                paint.Dispose();
            }

            topLeftCorner.Dispose();

            return toDraw;
        }
    }
}