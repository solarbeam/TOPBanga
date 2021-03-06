using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPBanga.Util;

namespace TOPBanga.Detection.GameUtil
{

    public class GameController
    {

        public event EventHandler<EventArgs> GoalEvent;
        public int redScore { get; private set; }
        public int blueScore { get; private set; }
        private const int SPACE_FOR_GOALS = 25;

        public EventLog EventLog { get; set; }
        private Goal first;
        private Goal second;

        private Queue<Goal> goals = new Queue<Goal>();
        private string goalAnnouncement = "Goal!";
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


        public GraphicsPath table { get; private set; }
        public void SetTable(PointF[] points)
        {
            if (points.Length != 4)
                return;
            this.table = new GraphicsPath();
            this.table.AddPolygon(points);
            PointF[] goalPoint = new PointF[4];
            float xMiddle = (points[0].X + points[1].X)/2;
            goalPoint[0] = new PointF(xMiddle - SPACE_FOR_GOALS, points[0].Y);
            goalPoint[1] = new PointF(xMiddle + SPACE_FOR_GOALS, points[0].Y);
            goalPoint[2] = new PointF(xMiddle + SPACE_FOR_GOALS, points[0].Y + SPACE_FOR_GOALS);
            goalPoint[3] = new PointF(xMiddle - SPACE_FOR_GOALS, points[0].Y + SPACE_FOR_GOALS);
            this.AddGoal(goalPoint);
            goalPoint = new PointF[4];
            xMiddle = (points[2].X + points[3].X) / 2;
            goalPoint[0] = new PointF(xMiddle - SPACE_FOR_GOALS, points[2].Y);
            goalPoint[1] = new PointF(xMiddle + SPACE_FOR_GOALS, points[2].Y);
            goalPoint[2] = new PointF(xMiddle + SPACE_FOR_GOALS, points[2].Y - SPACE_FOR_GOALS);
            goalPoint[3] = new PointF(xMiddle - SPACE_FOR_GOALS, points[2].Y - SPACE_FOR_GOALS);
            this.AddGoal(goalPoint);
        }
        public GameController()
        {
            this.ballCoordinates = new Queue<PointF>();
            this.lastBallCoordinates = new PointF(0, 0);
            //this.GoalEvent += ((obj, args) => System.Console.WriteLine("GOAL")); // for preview
        }

        public GameController(IWrite EventLog, string filepath)
        {
            this.ballCoordinates = new Queue<PointF>();
            this.lastBallCoordinates = new PointF(0, 0);
            this.EventLog = new EventLog(filepath);
            this.GoalEvent += ((obj, args) => this.EventLog.Write(goalAnnouncement + " " + blueScore + ":" + redScore + " " + this.EventLog.WinAnnouncement(redScore, blueScore)));
        }

        public Bitmap PaintGoals(Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen bluePen = new Pen(Color.Blue);
            Pen redPen = new Pen(Color.Red);
            graphics.DrawPath(bluePen, this.table);
            foreach (Goal goal in goals)
            {
                if (first.graphicsPath.IsVisible(this.lastBallCoordinates))
                {
                    graphics.DrawPath(redPen, first.graphicsPath);
                }
                else
                    graphics.DrawPath(bluePen, first.graphicsPath);


                if (second.graphicsPath.IsVisible(this.lastBallCoordinates))
                {
                    graphics.DrawPath(redPen, second.graphicsPath);
                }
                else
                    graphics.DrawPath(bluePen, second.graphicsPath);
            }
            graphics.Dispose();
            return bitmap;
        }

        public void AddGoal(PointF[] points)
        {
            if (this.goals.Count == 2)
            {
                Goal toDispose = this.goals.Dequeue();
                toDispose.Dispose();
            }
            GraphicsPath temp = new GraphicsPath();
            temp.AddPolygon(points);
            if (this.goals.Count == 0)
            {
                first = new Goal(temp);
            }
            if (this.goals.Count == 1)
            {
                second = new Goal(temp);
            }
            this.goals.Enqueue(new Goal(temp));
        }

        private void OnNewFrame()
        {
            foreach (Goal goal in goals)
            {
                if (first.graphicsPath.IsVisible(this.lastBallCoordinates))
                {
                    first.framesBallInGoal++;
                    if (first.framesBallInGoal == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        this.redScore++;
                        GoalEvent(this, new EventArgs());
                    }
                }
                else
                {
                    first.framesBallInGoal = 0;
                }
                
                if (second.graphicsPath.IsVisible(this.lastBallCoordinates))
                {
                    second.framesBallInGoal++;
                    if (second.framesBallInGoal == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        this.blueScore++;
                        GoalEvent(this, new EventArgs());
                    }
                }
                else {
                    goal.framesBallInGoal = 0;

                }
            }
        }
    }

    internal class Goal : IDisposable
    {
        public GraphicsPath graphicsPath { get; set; }
        public int framesBallInGoal { get; set; }

        internal Goal(GraphicsPath graphicsPath)
        {
            this.graphicsPath = graphicsPath;
            this.framesBallInGoal = 0;
        }

        public void Dispose() {
            this.graphicsPath.Dispose();
        }
    }
}