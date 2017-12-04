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
        /// Defines the goal zones, which hold the point of no return for the ball
        /// </summary>
        private RectF zoneOne;
        public RectF zoneTwo;

        /// <summary>
        /// Defines the maximum number of edges a table can have
        /// </summary>
        private const int TablePointNumber = 4;

        /// <summary>
        /// Defines the height of the precalculated goal zone
        ///  using the table's side as reference
        /// </summary>
        private const float percentageOfSide = 0.20f;

        /// <summary>
        /// Defines the amount of frames to skip between goal checks
        /// </summary>
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

                    temp?.Dispose();
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
        /// Set the table, which will be used for the definition of
        /// the goal zones
        /// It is pressumed, that the first point is the top left one, the second
        /// is the top right, the third is the bottom left and the bottom is the
        /// bottom right
        /// </summary>
        /// <param name="points">The coordinates of the table</param>
        public void SetTable(PointF[] points)
        {
            if (points.Length != TablePointNumber)
                return;

            // Calculate the different zones, using the points given
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
        /// Defines the goal checking mechanism, which is called whenever
        /// a new position is added to the queue
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
                // Check if this particular point signals that the ball is lost
                if (point == null)
                {
                    // It is, so check if a goal is about to occur
                    if (ballInFirstGoalZone && framesLost == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        // Fire the goal event for the first team
                        BlueScore++;
                        GoalEvent(this, EventArgs.Empty);
                        cooldown = MAXIMUM_BALL_COORDINATE_NUMBER;
                        return;
                    }
                    else
                        if (ballInSecondGoalZone && framesLost == GOAL_FRAMES_TO_COUNT_GOAL)
                    {
                        // Fire the goal event for the second team
                        RedScore++;
                        GoalEvent(this, EventArgs.Empty);
                        cooldown = MAXIMUM_BALL_COORDINATE_NUMBER;
                        return;
                    }

                    framesLost++;
                    continue;
                }
                else
                    // It isn't, so reset the counter
                    framesLost = 0;

                // Check if the ball is in the first zone
                if ( zoneOne.Contains(point.X, point.Y) )
                {
                    ballInFirstGoalZone = true;
                    ballInSecondGoalZone = false;
                    continue;
                }
                else
                // Check if the ball is in the second zone
                    if ( zoneTwo.Contains(point.X, point.Y) )
                {
                    ballInSecondGoalZone = true;
                    ballInFirstGoalZone = false;
                    continue;
                }

                // The ball is in neither of the zones, so set the appropriate values
                ballInFirstGoalZone = false;
                ballInSecondGoalZone = false;
            }

            // To avoid repetetive calculations, set a cooldown counter
            cooldown = MAXIMUM_BALL_COORDINATE_NUMBER;
        }
    }
}
