using Android.Graphics;
using Emgu.CV.Structure;
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
        /// The amount of positions to hold in the queue
        /// </summary>
        private const int MAXIMUM_BALL_COORDINATE_NUMBER = 50;
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
        /// A get and set function to assign the last position of the ball
        /// </summary>
        public PointF LastBallCoordinates
        {
            get => last_ball_coordinates;

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

            Table = new Path();
            Table.MoveTo(points[0].X, points[0].Y);
            Table.LineTo(points[1].X, points[1].Y);
            Table.LineTo(points[2].X, points[2].Y);
            Table.LineTo(points[3].X, points[3].Y);
            Table.Close();
        }
        /// <summary>
        /// The default constructor for the GameController class
        /// </summary>
        public GameController()
        {
            ballCoordinates = new Queue<PointF>();
            LastBallCoordinates = new PointF(0, 0);
        }

        /// <summary>
        /// Add a goal to the queue
        /// </summary>
        /// <param name="points">The points before a goal was made</param>
        public void AddGoal(PointF[] points)
        {
            if (goals.Count == 2)
            {
                Goal toDispose = goals.Dequeue();
                toDispose.Dispose();
            }
            var temp = new Path();
            temp.MoveTo(points[0].X, points[0].Y);
            temp.LineTo(points[1].X, points[1].Y);
            temp.LineTo(points[2].X, points[2].Y);
            temp.LineTo(points[3].X, points[3].Y);
            temp.Close();
            goals.Enqueue(new Goal(temp));
        }

        /// <summary>
        /// TODO Add documentation
        /// </summary>
        private void OnNewFrame()
        {
            foreach (var goal in goals)
            {
                var goalConvertion = new RectF();

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
            Path = path;
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
