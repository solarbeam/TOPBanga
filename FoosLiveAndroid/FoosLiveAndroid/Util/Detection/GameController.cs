using Android.Graphics;
using System;
using System.Collections.Generic;

namespace TOPBanga.Detection.GameUtil
{
    /// <summary>
    /// The class holds the primary functions, required for goal detection
    /// and the predefined attributes for them
    /// </summary>
    public class GameController
    {
        /// <summary>
        /// Fired whenever a goal event occurs
        /// </summary>
        public event EventHandler<EventArgs> GoalEvent;
        /// <summary>
        /// Defines the current score for the red team
        /// </summary>
        public int RedScore { get; private set; }
        /// <summary>
        /// Defines the current score for the blue team
        /// </summary>
        public int BlueScore { get; private set; }
        /// <summary>
        /// TODO Add documentation
        /// </summary>
        private const int SPACE_FOR_GOALS = 25;

        /// <summary>
        /// TODO Add documentation
        /// </summary>
        private Queue<Goal> goals = new Queue<Goal>();

        /// <summary>
        /// TODO Add documentation
        /// </summary>
        private const int MAXIMUM_BALL_COORDINATE_NUMBER = 20;
        /// <summary>
        /// The minimum amount of frames in the goal zone in order for
        /// the goal to be accepted
        /// </summary>
        private const int GOAL_FRAMES_TO_COUNT_GOAL = 3;
        /// <summary>
        /// Holds the coordinates of the last position of the ball
        /// </summary>
        private PointF last_ball_coordinates;

        /// <summary>
        /// TODO Add documentation
        /// </summary>
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

        /// <summary>
        /// Holds the ball coordinates in a queue
        /// </summary>
        public Queue<PointF> ballCoordinates;

        /// <summary>
        /// TODO Add documentation
        /// </summary>
        public Path Table { get; private set; }
        /// <summary>
        /// Set the table, which will be used for the definition of
        /// the goal zones
        /// </summary>
        /// <param name="points">The coordinates of the table</param>
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
        /// <summary>
        /// The default constructor for the GameController class
        /// </summary>
        public GameController()
        {
            this.ballCoordinates = new Queue<PointF>();
            this.LastBallCoordinates = new PointF(0, 0);
            //this.GoalEvent += ((obj, args) => System.Console.WriteLine("GOAL")); // for preview
        }

        /// <summary>
        /// Draw the goal zone if a goal was made
        /// </summary>
        /// <param name="bitmap">The bitmap, to which a path will be drawn</param>
        /// <returns>The drawn bitmap</returns>
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

        /// <summary>
        /// Add a goal to the queue
        /// </summary>
        /// <param name="points">The points before a goal was made</param>
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

        /// <summary>
        /// TODO Add documentation
        /// </summary>
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

    /// <summary>
    /// TODO Add documentation
    /// </summary>
    internal class Goal : IDisposable
    {
        /// <summary>
        /// TODO Add documentation
        /// </summary>
        public Path Path { get; set; }
        /// <summary>
        /// TODO Add documentation
        /// </summary>
        public int FramesBallInGoal { get; set; }

        /// <summary>
        /// TODO Add documentation
        /// </summary>
        /// <param name="path">TODO</param>
        internal Goal(Path path)
        {
            this.Path = path;
            FramesBallInGoal = 0;
        }

        /// <summary>
        /// A function that is called when the object is to be
        /// given to the garbage collector
        /// </summary>
        public void Dispose()
        {
            Path.Dispose();
        }
    }
}
