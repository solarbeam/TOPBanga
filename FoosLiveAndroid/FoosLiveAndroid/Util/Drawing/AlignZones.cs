using Android.Graphics;
using FoosLiveAndroid.Util.GameControl;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.Drawing
{
    class AlignZones
    {
        private static readonly float AlignZonesStrokeWidth = PropertiesManager.GetFloatProperty("align_zones_stroke_width");

        public static Canvas DrawZones(Canvas canvas, GameController gameController)
        {
            // Set up variables for alignment figure drawing
            Paint paint = new Paint
            {
                Color = new Color(255, 255, 255)
            };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = AlignZonesStrokeWidth;
            paint.SetPathEffect(new DashPathEffect(new float[] { 30, 20 }, 0));
            Path contour = new Path();

            // Calculate drawint points based on screen size
            float bottomLeftX = canvas.Width * PropertiesManager.GetFloatProperty("bottom_left_x_mul");
            float bottomRightX = canvas.Width * PropertiesManager.GetFloatProperty("bottom_right_x_mul");
            float bottomY = canvas.Height * PropertiesManager.GetFloatProperty("bottom_y_mul");

            float upperBottomLeftX = canvas.Width * PropertiesManager.GetFloatProperty("upper_bottom_left_x_mul");
            float upperBottomRightX = canvas.Width * PropertiesManager.GetFloatProperty("upper_bottom_right_x_mul");
            float upperBottomY = canvas.Height * PropertiesManager.GetFloatProperty("upper_bottom_y_mul");

            float lowerTopLeftX = canvas.Width * PropertiesManager.GetFloatProperty("lower_top_left_x_mul");
            float lowerTopRightX = canvas.Width * PropertiesManager.GetFloatProperty("lower_top_right_x_mul");
            float lowerTopY = canvas.Height * PropertiesManager.GetFloatProperty("lower_top_y_mul");

            float topLeftX = canvas.Width * PropertiesManager.GetFloatProperty("top_left_x_mul");
            float topRightX = canvas.Width * PropertiesManager.GetFloatProperty("top_right_x_mul");
            float topY = canvas.Height * PropertiesManager.GetFloatProperty("top_y_mul");
                
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
            gameController.SetTable(new PointF[]
            {
                new PointF(0, topY),
                new PointF(canvas.Width, topY),
                new PointF(0, bottomY),
                new PointF(canvas.Width, bottomY)
            }, CaptureMode.Camera);

            return canvas;
        }
    }
}