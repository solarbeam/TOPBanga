using Android.Graphics;
using FoosLiveAndroid.Util.Detection;
using System.Drawing;

namespace FoosLiveAndroid.Util.Drawing
{
    class AlignZones
    {
        private static int RowCount = 8;
        public static Canvas DrawZones(Canvas canvas, GameController gameController)
        {
            Rectangle[] rows = new Rectangle[RowCount];

            // Declare the paint style of the guidelines
            Paint paint = new Paint
            {
                Color = new Android.Graphics.Color(255, 255, 255)
            };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 4.0f;
            paint.SetPathEffect(new DashPathEffect(new float[] { 30, 20 }, 0));

            // Paint the guideline
            Path contour = new Path();

            float bottomLeftX = canvas.Width * 0.25f;             float bottomRightX = canvas.Width * 0.75f;             float bottomY = canvas.Height * 0.9209f;              float upperBottomLeftX = canvas.Width * 0.03f;
            float upperBottomRightX = canvas.Width * 0.97f;
            float upperBottomY = canvas.Height * 0.8023f;

            float lowerTopLeftX = canvas.Width * 0.20f;             float lowerTopRightX = canvas.Width * 0.80f;             float lowerTopY = canvas.Height * 0.35f;

            float topLeftX = canvas.Width * 0.42f;             float topRightX = canvas.Width * 0.58f;             float topY = canvas.Height * 0.2994f;
                
            // apacios vidurio taskas
            contour.MoveTo(bottomLeftX, bottomY);
            // kairinis apacios kampas
            //contour.LineTo(bottomLeftPointX, bottomCenterPoint[1]);
            // kairesnis kampas nuo apacios kaires
            contour.LineTo(upperBottomLeftX, upperBottomY);
            //kairesnis kampas nuo virsaus kaires
            contour.LineTo(lowerTopLeftX, lowerTopY);
            //kairinis virsaus kampas
            contour.LineTo(topLeftX, topY);
            // desininis virsaus kampas
            contour.LineTo(topRightX, topY);
            // desinesnis virsaus kampas
            contour.LineTo(lowerTopRightX, lowerTopY);
            // desinys apacios kampas
            contour.LineTo(upperBottomRightX, upperBottomY);
            // desinesnis apacios kampas
            contour.LineTo(bottomRightX, bottomY);
            // iki centro atgal
            contour.LineTo(bottomLeftX, bottomY);
            // Finally, draw the path
            canvas.DrawPath(contour, paint);

            return canvas;
        }
    }
}