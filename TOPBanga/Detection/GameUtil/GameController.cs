using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    public class GameController
    {


        private List<GraphicsPath> goals = new List<GraphicsPath>();

        public PointF lastBallCoordinates { get; set; }

        public GameController()
        {
            lastBallCoordinates = new PointF(0, 0);
        }

        public Bitmap PaintGoals(Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen bluePen = new Pen(Color.Blue);
            Pen redPen = new Pen(Color.Red);
            foreach (GraphicsPath path in goals)
            {
                if (path.IsVisible(this.lastBallCoordinates))
                    graphics.DrawPath(redPen, path);
                else
                    graphics.DrawPath(bluePen, path);
            }
            graphics.Dispose();
            return bitmap;
        }

        public void AddGoal(PointF[] points)
        {
            GraphicsPath temp = new GraphicsPath();
            temp.AddPolygon(points);
            this.goals.Add(temp);
        }


    }
}