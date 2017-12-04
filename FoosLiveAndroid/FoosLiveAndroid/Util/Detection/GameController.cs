using Android.Graphics;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using Android.Graphics;

namespace FoosLiveAndroid.Util.Detection
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
        private const int MAXIMUM_BALL_COORDINATE_NUMBER = 100;
        /// <summary>
        /// The minimum amount of frames in the goal zone in order for
        /// the goal to be accepted
        /// </summary>
        private const int GOAL_FRAMES_TO_COUNT_GOAL = 10;
        /// <summary>
        /// Holds the coordinates of the last position of the ball
        /// </summary>
        private PointF last_ball_coordinates;

        /// <summary>
        /// Defines the zones, which hold the goals ( the point of no return for the ball ) and the middle
        /// </summary>
        private RectF zoneOne;
        public RectF zoneTwo;
        private const float percentageOfSide = 0.10f;
        private int cooldown = 0;

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
                    PointF temp = ballCoordinates.Dequeue();
                    
                    if (temp != null)
                        temp.Dispose();
                }
                last_ball_coordinates = value;
                ballCoordinates.Enqueue(last_ball_coordinates);
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
        /// It is pressumed, that the first point is the top left one, the second
        /// is the top right, the third is the bottom left and the bottom is the
        /// bottom right
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

            // Calculate the different zones, using the values given
            this.zoneOne = new RectF(points[0].X,
                                    points[0].Y,
                                    points[1].X,
                                    (points[2].Y - points[0].Y) * percentageOfSide);
            this.zoneTwo = new RectF(points[0].X, points[2].Y - (points[2].Y - points[0].Y) * percentageOfSide,
                                        points[3].X,
                                        points[3].Y);
        }
        /// <summary>
        /// The default constructor for the GameController class
        /// </summary>
        public GameController()
        {
            ballCoordinates = new Queue<PointF>();
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
            if (cooldown != 0)
            {
                cooldown--;
                return;
            }

            // Check if there was a goal event for either team
            bool ballInFirstGoalZone = false;
            bool ballInSecondGoalZone = false;
            int framesLost = 0;
            foreach (var point in ballCoordinates)
            {
                if (point == null)
                {
                    if (ballInFirstGoalZone == true && framesLost == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        // Fire the goal event for the first team
                        this.BlueScore++;
                        GoalEvent(this, EventArgs.Empty);
                        cooldown = MAXIMUM_BALL_COORDINATE_NUMBER;
                        return;
                    }
                    else
                        if (ballInSecondGoalZone == true && framesLost == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        // Fire the goal event for the second team
                        this.RedScore++;
                        GoalEvent(this, EventArgs.Empty);
                        cooldown = MAXIMUM_BALL_COORDINATE_NUMBER;
                        return;
                    }

                    framesLost++;
                    continue;
                }
                else
                    framesLost = 0;

                if ( zoneOne.Contains(point.X, point.Y) )
                {
                    ballInFirstGoalZone = true;
                    ballInSecondGoalZone = false;
                    continue;
                }
                else
                    if ( zoneTwo.Contains(point.X, point.Y) )
                {
                    ballInSecondGoalZone = true;
                    ballInFirstGoalZone = false;
                    continue;
                }

                ballInFirstGoalZone = false;
                ballInSecondGoalZone = false;
            }

            cooldown = MAXIMUM_BALL_COORDINATE_NUMBER;
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
