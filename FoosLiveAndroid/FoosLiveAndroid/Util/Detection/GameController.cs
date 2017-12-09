using Android.Graphics;
using Android.Util;
using FoosLiveAndroid.Util.GameControl;
using System;
using System.Collections.Generic;

namespace FoosLiveAndroid.Util.Detection
{
    public enum Row
    {
        RedGoalie,
        RedDefence,
        BlueAttack,
        RedMidfield,
        BlueMidfield,
        RedAttack,
        BlueDefence,
        BlueGoalie
    }
    public enum CurrentEvent
    {
        PositionChanged,
        RedGoalOccured,
        BlueGoalOccured,
        None
    }
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
        public event EventHandler<EventArgs> PositionEvent;

        /// <summary>
        /// Defines the row the ball is currently in
        /// </summary>
        public Row currentRow;
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
        private readonly int MaximumBallCoordinatesNumber = PropertiesManager.GetIntProperty("maximum_ball_coordinate_number");
        /// <summary>
        /// The minimum amount of frames in the goal zone in order for
        /// the goal to be accepted
        /// </summary>
        private readonly int GoalFramesToCountGoal = PropertiesManager.GetIntProperty("goal_frames_to_count_goal");
        /// <summary>
        /// Holds the coordinates of the last position of the ball
        /// </summary>
        private PointF lastBallCoordinates;

        public CurrentEvent currentEvent;

        /// <summary>
        /// Holds the goals, which occured during the session
        /// </summary>
        private Queue<Goal> goals;

        /// <summary>
        /// Defines the goal zones, which hold the point of no return for the ball
        /// </summary>
        private RectF zoneOne;
        public RectF zoneTwo;

        /// <summary>
        /// Defines the rows of foosmen
        /// </summary>
        public RectF[] rows;

        /// <summary>
        /// Defines whether the ball is in the first goal zone
        /// </summary>
        private bool ballInFirstGoalZone = false;
        /// <summary>
        /// Defines whether the ball is in the second ball zone
        /// </summary>
        private bool ballInSecondGoalZone = false;
        /// <summary>
        /// Defines the lost frame counter
        /// </summary>
        private int framesLost = 0;

        /// <summary>
        /// Defines the maximum number of edges a table can have
        /// </summary>
        private const int TablePointNumber = 4;

        /// <summary>
        /// Defines the height of the precalculated goal zone
        ///  using the table's side as reference
        /// </summary>
        private readonly float percentageOfSide = PropertiesManager.GetFloatProperty("percentage_of_side");

        /// <summary>
        /// A get and set function to assign the last position of the ball
        /// </summary>
        public PointF LastBallCoordinates
        {
            get => lastBallCoordinates;

            set
            {
                if (ballCoordinates.Count == MaximumBallCoordinatesNumber)
                {
                    PointF temp = ballCoordinates.Dequeue();

                    temp?.Dispose();
                }
                
                // Check which row has the ball

                lastBallCoordinates = value;

                if (lastBallCoordinates != null && rows != null)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (rows[i].Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                        {
                            switch (i)
                            {
                                case 0:
                                    currentRow = Row.RedGoalie;
                                    break;
                                case 1:
                                    currentRow = Row.RedDefence;
                                    break;
                                case 2:
                                    currentRow = Row.BlueAttack;
                                    break;
                                case 3:
                                    currentRow = Row.RedMidfield;
                                    break;
                                case 4:
                                    currentRow = Row.BlueMidfield;
                                    break;
                                case 5:
                                    currentRow = Row.RedAttack;
                                    break;
                                case 6:
                                    currentRow = Row.BlueDefence;
                                    break;
                                case 7:
                                    currentRow = Row.BlueGoalie;
                                    break;
                            }

                            if (currentEvent == CurrentEvent.None)
                                currentEvent = CurrentEvent.PositionChanged;

                            PositionEvent(this, EventArgs.Empty);
                            break;
                        }
                    }
                }

                ballCoordinates.Enqueue(lastBallCoordinates);
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
            zoneOne = new RectF(points[0].X,
                                    points[0].Y,
                                    points[1].X,
                                    (points[2].Y - points[0].Y) * percentageOfSide);

            zoneTwo = new RectF(points[0].X, zoneOne.Bottom + (1.0f - percentageOfSide * 2) * (points[2].Y - points[0].Y),
                                        points[3].X,
                                        points[3].Y);
        }
        /// <summary>
        /// The default constructor for the GameController class
        /// </summary>
        public GameController()
        {
            ballCoordinates = new Queue<PointF>();
            goals = new Queue<Goal>();
        }

        public void CalculateRows(System.Drawing.Rectangle tableZone)
        {
            // Declare constants
            float[] multipliers = new float[8];
            multipliers[0] = 0.1158f;
            multipliers[1] = 0.0623f;
            multipliers[2] = 0.0865f;
            multipliers[3] = 0.1005f;
            multipliers[4] = 0.1145f;
            multipliers[5] = 0.1348f;
            multipliers[6] = 0.1832f;
            multipliers[7] = 0.2124f;

            rows = new RectF[8];

            rows[0] = new RectF(tableZone.Left, tableZone.Top,
                                tableZone.Right, tableZone.Top + (tableZone.Height * multipliers[0]));
            
            for (int i = 1; i < 8; i ++)
            {
                float toAdd = 0;
                for(int j = i; j != 0; j --)
                {
                    toAdd += rows[j - 1].Height();
;                }
                rows[i] = new RectF(tableZone.Left, rows[i - 1].Bottom,
                                    tableZone.Right, tableZone.Top + toAdd + (tableZone.Height * multipliers[i]));
            }
        }

        /// <summary>
        /// Defines the goal checking mechanism, which is called whenever
        /// a new position is added to the queue
        /// </summary>
        private void OnNewFrame()
        {
            // Check if this particular point signals that the ball is lost
            if (lastBallCoordinates == null)
            {
                if (framesLost == GoalFramesToCountGoal)
                {
                    // It is, so check if a goal is about to occur
                    if (ballInFirstGoalZone)
                    {
                        // Fire the goal event for the first team
                        BlueScore++;
                        currentEvent = CurrentEvent.BlueGoalOccured;
                        GoalEvent(this, EventArgs.Empty);

                        goals.Enqueue(new Goal(ballCoordinates, new RectF(zoneOne.Left, zoneOne.Top, zoneTwo.Right, zoneTwo.Bottom)));

                        // Reset variables to their starting values
                        framesLost = 0;
                        ballInFirstGoalZone = false;
                        ballInSecondGoalZone = false;

                        return;
                    }
                    else
                        if (ballInSecondGoalZone)
                    {
                        // Fire the goal event for the second team
                        RedScore++;
                        currentEvent = CurrentEvent.RedGoalOccured;
                        GoalEvent(this, EventArgs.Empty);

                        goals.Enqueue(new Goal(ballCoordinates, new RectF(zoneOne.Left, zoneOne.Top, zoneTwo.Right, zoneTwo.Bottom)));

                        // Reset variables to their starting values
                        framesLost = 0;
                        ballInFirstGoalZone = false;
                        ballInSecondGoalZone = false;

                        return;
                    }
                }

                framesLost++;
                return;
            }
            else
                // It isn't, so reset the counter
                framesLost = 0;

            // Check if the ball is in the first zone
            if (zoneOne.Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
            {
                ballInFirstGoalZone = true;
                ballInSecondGoalZone = false;
            }
            else
                // Check if the ball is in the second zone
                if (zoneTwo.Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
            {
                ballInSecondGoalZone = true;
                ballInFirstGoalZone = false;
            }
            else
            {
                ballInFirstGoalZone = false;
                ballInSecondGoalZone = false;
            }
        }
    }
}
