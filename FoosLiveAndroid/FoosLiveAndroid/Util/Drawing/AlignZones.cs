using Android.Graphics;
using FoosLiveAndroid.Util.GameControl;
using FoosLiveAndroid.Util.Model;

namespace FoosLiveAndroid.Util.Drawing
{
    class AlignZones
    {
        private static readonly float AlignZonesStrokeWidth = PropertiesManager.GetFloatProperty("align_zones_stroke_width");

        public static Canvas DrawZones(Canvas canvas, GameController gameController)
        {
            // Set up variables for alignment figure drawing
            var paint = new Paint
            {
                Color = new Color(255, 255, 255)
            };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = AlignZonesStrokeWidth;
            paint.SetPathEffect(new DashPathEffect(new float[] { 30, 20 }, 0));
            var contour = new Path();

            // Calculate drawint points based on screen size
            var bottomLeftX = canvas.Width * PropertiesManager.GetFloatProperty("bottom_left_x_mul");
            var bottomRightX = canvas.Width * PropertiesManager.GetFloatProperty("bottom_right_x_mul");
            var bottomY = canvas.Height * PropertiesManager.GetFloatProperty("bottom_y_mul");

            var upperBottomLeftX = canvas.Width * PropertiesManager.GetFloatProperty("upper_bottom_left_x_mul");
            var upperBottomRightX = canvas.Width * PropertiesManager.GetFloatProperty("upper_bottom_right_x_mul");
            var upperBottomY = canvas.Height * PropertiesManager.GetFloatProperty("upper_bottom_y_mul");

            var lowerTopLeftX = canvas.Width * PropertiesManager.GetFloatProperty("lower_top_left_x_mul");
            var lowerTopRightX = canvas.Width * PropertiesManager.GetFloatProperty("lower_top_right_x_mul");
            var lowerTopY = canvas.Height * PropertiesManager.GetFloatProperty("lower_top_y_mul");

            var topLeftX = canvas.Width * PropertiesManager.GetFloatProperty("top_left_x_mul");
            var topRightX = canvas.Width * PropertiesManager.GetFloatProperty("top_right_x_mul");
            var topY = canvas.Height * PropertiesManager.GetFloatProperty("top_y_mul");
                
            // Draw alignment figure
            contour.MoveTo(bottomLeftX, bottomY);
            contour.LineTo(upperBottomLeftX, upperBottomY);
            contour.LineTo(lowerTopLeftX, lowerTopY);
            contour.LineTo(topLeftX, topY);
            contour.LineTo(topRightX, topY);
            contour.LineTo(lowerTopRightX, lowerTopY);
            contour.LineTo(upperBottomRightX, upperBottomY);
            contour.LineTo(bottomRightX, bottomY);
            contour.LineTo(bottomLeftX, bottomY);
            canvas.DrawPath(contour, paint);

            // Calculate the table based on the outline
            gameController.SetTable(new[]
            {
                new PointF(lowerTopLeftX, topY),
                new PointF(lowerTopRightX, topY),
                new PointF(bottomLeftX, bottomY),
                new PointF(bottomRightX, bottomY)
            }, ECaptureMode.Live);

            return canvas;
        }
    }
}