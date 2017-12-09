using Android.Graphics;
using FoosLiveAndroid.Util.GameControl;
using System.Drawing;

namespace FoosLiveAndroid.Util.Drawing
{
    class AlignZones
    {
        private static readonly int RowCount = 8;
        public static Canvas DrawZones(Canvas canvas, GameController gameController)
        {
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

            float bottomLeftX = canvas.Width * PropertiesManager.GetFloatProperty("bottomLeftXMul");
            float bottomRightX = canvas.Width * PropertiesManager.GetFloatProperty("bottomRightXMul");
            float bottomY = canvas.Height * PropertiesManager.GetFloatProperty("bottomYMul");

            float upperBottomLeftX = canvas.Width * PropertiesManager.GetFloatProperty("upperBottomLeftXMul");
            float upperBottomRightX = canvas.Width * PropertiesManager.GetFloatProperty("upperBottomRightXMul");
            float upperBottomY = canvas.Height * PropertiesManager.GetFloatProperty("upperBottomYMul");

            float lowerTopLeftX = canvas.Width * PropertiesManager.GetFloatProperty("lowerTopLeftXMul");
            float lowerTopRightX = canvas.Width * PropertiesManager.GetFloatProperty("lowerTopRightXMul");
            float lowerTopY = canvas.Height * PropertiesManager.GetFloatProperty("lowerTopYMul");

            float topLeftX = canvas.Width * PropertiesManager.GetFloatProperty("topLeftXMul");
            float topRightX = canvas.Width * PropertiesManager.GetFloatProperty("topRightXMul");
            float topY = canvas.Height * PropertiesManager.GetFloatProperty("topYMul");
                
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

            // Calculate the table based on the outline
            gameController.SetTable(new Android.Graphics.PointF[]
            {
                new Android.Graphics.PointF(0, topY),
                new Android.Graphics.PointF(canvas.Width, topY),
                new Android.Graphics.PointF(0, bottomY),
                new Android.Graphics.PointF(canvas.Width, bottomY)
            });

            return canvas;
        }
    }
}