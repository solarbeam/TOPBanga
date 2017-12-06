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
            Paint paint = new Paint();
            paint.Color = new Android.Graphics.Color(0, 255, 100);
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 2.0f;
            paint.SetPathEffect(new DashPathEffect(new float[] { 30, 20 }, 0));

            // Paint the guideline
            Path contour = new Path();
            contour.MoveTo(canvas.Width / 2, canvas.Height);
            contour.LineTo(canvas.Width / 2 - canvas.Width / 8, canvas.Height);
            contour.LineTo(canvas.Width / 16, canvas.Height * 0.85f);
            contour.LineTo(canvas.Width / 16 + canvas.Width / 12, canvas.Height * 0.15f);
            contour.LineTo(canvas.Width / 2 - canvas.Width / 10, canvas.Height * 0.08f);
            contour.LineTo(canvas.Width / 2 + canvas.Width / 10, canvas.Height * 0.08f);
            contour.LineTo(canvas.Width - (canvas.Width / 16 + canvas.Width / 12), canvas.Height * 0.15f);
            contour.LineTo(canvas.Width - canvas.Width / 16, canvas.Height * 0.85f);
            contour.LineTo(canvas.Width / 2 + canvas.Width / 8, canvas.Height);

            // Finally, draw the path
            canvas.DrawPath(contour, paint);

            return canvas;
        }
    }
}