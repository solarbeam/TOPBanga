using Android.Graphics;
using System;
using System.Collections.Generic;

namespace TOPBanga.Detection.GameUtil
{
    public class GameController
    {

        public event EventHandler<EventArgs> GoalEvent;
        public int RedScore { get; private set; }
        public int BlueScore { get; private set; }
        private const int SPACE_FOR_GOALS = 25;

        private Queue<Goal> goals = new Queue<Goal>();

        private const int MAXIMUM_BALL_COORDINATE_NUMBER = 20;
        private const int GOAL_FRAMES_TO_COUNT_GOAL = 3;
        private PointF last_ball_coordinates;

        public PointF LastBallCoordinates
        {
            get
            {
                return last_ball_coordinates;
            }

            set
            {
                if (ballCoordinates.Count == MAXIMUM_BALL_COORDINATE_NUMBER)
                {
                    ballCoordinates.Dequeue();
                }
                ballCoordinates.Enqueue(last_ball_coordinates);
                last_ball_coordinates = value;
                OnNewFrame();
            }
        }

        public Queue<PointF> ballCoordinates;


        public Path Table { get; private set; }
        public void SetTable(PointF[] points)
        {
            if (points.Length != 4)
                return;
            this.Table = new Path();
            this.Table.MoveTo(points[0].X, points[0].Y);
            this.Table.LineTo(points[1].X, points[1].Y);
            this.Table.LineTo(points[2].X, points[2].Y);
            this.Table.LineTo(points[3].X, points[3].Y);
            this.Table.Close();
            //PointF[] goalPoint = new PointF[4];
            //float xMiddle = (points[0].X + points[1].X) / 2;
            //goalPoint[0] = new PointF(xMiddle - SPACE_FOR_GOALS, points[0].Y);
            //goalPoint[1] = new PointF(xMiddle + SPACE_FOR_GOALS, points[0].Y);
            //goalPoint[2] = new PointF(xMiddle + SPACE_FOR_GOALS, points[0].Y + SPACE_FOR_GOALS);
            //goalPoint[3] = new PointF(xMiddle - SPACE_FOR_GOALS, points[0].Y + SPACE_FOR_GOALS);
            //this.AddGoal(goalPoint);
            //goalPoint = new PointF[4];
            //xMiddle = (points[2].X + points[3].X) / 2;
            //goalPoint[0] = new PointF(xMiddle - SPACE_FOR_GOALS, points[2].Y);
            //goalPoint[1] = new PointF(xMiddle + SPACE_FOR_GOALS, points[2].Y);
            //goalPoint[2] = new PointF(xMiddle + SPACE_FOR_GOALS, points[2].Y - SPACE_FOR_GOALS);
            //goalPoint[3] = new PointF(xMiddle - SPACE_FOR_GOALS, points[2].Y - SPACE_FOR_GOALS);
            //this.AddGoal(goalPoint);
        }
        public GameController()
        {
            this.ballCoordinates = new Queue<PointF>();
            this.LastBallCoordinates = new PointF(0, 0);
            //this.GoalEvent += ((obj, args) => System.Console.WriteLine("GOAL")); // for preview
        }

        public Bitmap PaintGoals(Bitmap bitmap)
        {
            Canvas graphics = new Canvas(bitmap);
            Paint bluePaint = new Paint();
            bluePaint.SetARGB(255, 0, 0, 255);
            Paint redPaint = new Paint();
            redPaint.SetARGB(255, 255, 0, 0);
            graphics.DrawPath(Table, bluePaint);
            foreach (Goal goal in goals)
            {
                RectF goalConvertion = new RectF();
                goal.Path.ComputeBounds(goalConvertion, true);
                if (goalConvertion.Contains(LastBallCoordinates.X, LastBallCoordinates.Y))
                    graphics.DrawPath(Table, redPaint);
                else
                    graphics.DrawPath(Table, bluePaint);
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
            Path temp = new Path();
            temp.MoveTo(points[0].X, points[0].Y);
            temp.LineTo(points[1].X, points[1].Y);
            temp.LineTo(points[2].X, points[2].Y);
            temp.LineTo(points[3].X, points[3].Y);
            temp.Close();
            this.goals.Enqueue(new Goal(temp));
        }

        private void OnNewFrame()
        {
            foreach (Goal goal in goals)
            {
                RectF goalConvertion = new RectF();
                if (goalConvertion.Contains(LastBallCoordinates.X, LastBallCoordinates.Y))
                {
                    goal.FramesBallInGoal++;
                    if (goal.FramesBallInGoal == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        GoalEvent?.Invoke(this, new EventArgs());
                    }
                }
                else
                {
                    goal.FramesBallInGoal = 0;

                }
            }
        }
    }

    internal class Goal : IDisposable
    {
        public Path Path { get; set; }
        public int FramesBallInGoal { get; set; }

        internal Goal(Path path)
        {
            this.Path = path;
            FramesBallInGoal = 0;
        }

        public void Dispose()
        {
            Path.Dispose();
        }
    }
}
