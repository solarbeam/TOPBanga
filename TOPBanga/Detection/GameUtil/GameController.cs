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

        public event EventHandler<EventArgs> GoalEvent;
        public int redScore { get; private set; }
        public int blueScore { get; private set; }

        private Queue<Goal> goals = new Queue<Goal>();

        private const int MAXIMUM_BALL_COORDINATE_NUMBER = 20;
        private const int GOAL_FRAMES_TO_COUNT_GOAL = 3;
        private PointF last_ball_coordinates;
        public PointF lastBallCoordinates {

            get {
                return last_ball_coordinates;
            }

            set {
                if (ballCoordinates.Count == MAXIMUM_BALL_COORDINATE_NUMBER)
                    ballCoordinates.Dequeue();
                ballCoordinates.Enqueue(last_ball_coordinates);
                last_ball_coordinates = value;
                OnNewFrame();
            }
        }

        public Queue<PointF> ballCoordinates;

        public GameController()
        {
            this.ballCoordinates = new Queue<PointF>();
            this.lastBallCoordinates = new PointF(0, 0);
            this.GoalEvent += ((obj, args) => System.Console.WriteLine("WEW lad"));
        }

        public Bitmap PaintGoals(Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen bluePen = new Pen(Color.Blue);
            Pen redPen = new Pen(Color.Red);
            foreach (Goal goal in goals)
            {
                if (goal.graphicsPath.IsVisible(this.lastBallCoordinates))
                    graphics.DrawPath(redPen, goal.graphicsPath);
                else
                    graphics.DrawPath(bluePen, goal.graphicsPath);
            }
            graphics.Dispose();
            return bitmap;
        }

        public void AddGoal(PointF[] points)
        {
            if(this.goals.Count == 2)
            {
                Goal toDispose = this.goals.Dequeue();
                toDispose.Dispose();
            }
            GraphicsPath temp = new GraphicsPath();
            temp.AddPolygon(points);
            this.goals.Enqueue(new Goal(temp));
        }

        private void OnNewFrame()
        {
            foreach (Goal goal in goals)
            {
                if (goal.graphicsPath.IsVisible(this.lastBallCoordinates))
                {
                    goal.timesTouched++;
                    if(goal.timesTouched == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        GoalEvent(this, new EventArgs());
                    }
                }
            }
        }
    }

    internal class Goal : IDisposable
    {
        public GraphicsPath graphicsPath { get; set; }
        public int timesTouched { get; set; }

        internal Goal(GraphicsPath graphicsPath)
        {
            this.graphicsPath = graphicsPath;
            this.timesTouched = 0;
        }

        public void Dispose() {
            this.graphicsPath.Dispose();
        }
    }
}
