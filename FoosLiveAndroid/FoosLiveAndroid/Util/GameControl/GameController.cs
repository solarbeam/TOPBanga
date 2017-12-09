using Android.Graphics;
using Android.Util;
using FoosLiveAndroid.Util.GameControl;
using System;
using System.Collections.Generic;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.GameControl
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
        public event EventHandler<EventArgs> PositionEvent;

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
        
        public CurrentEvent currentEvent;

        private PositionChecker posChecker;
        private RowChecker rowChecker;

        /// <summary>
        /// Holds the coordinates of the last position of the ball
        /// </summary>
        private PointF lastBallCoordinates;
        private PointF lastLastBallCoordinates;

        /// <summary>
        /// Defines the current speed of the ball in centimeters per second
        /// </summary>
        public double CurrentSpeed;

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

                lastLastBallCoordinates = lastBallCoordinates;
                lastBallCoordinates = value;

                // Check which row has the ball
                if (lastBallCoordinates != null && rowChecker.rows != null)
                {
                    rowChecker.CheckRow(lastBallCoordinates, ref currentEvent);
                }

                ballCoordinates.Enqueue(lastBallCoordinates);

                posChecker.OnNewFrame(lastBallCoordinates,
                                        BlueScore,
                                        RedScore,
                                        currentEvent,
                                        (blue, red, ev) =>
                                        {
                                            BlueScore = blue;
                                            RedScore = red;
                                            currentEvent = ev;
                                        },
                                        GoalEvent,
                                        ballCoordinates);

                CurrentSpeed = posChecker.calculateSpeed(lastBallCoordinates, lastLastBallCoordinates, PositionEvent);
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
        public void SetTable(PointF[] points, CaptureMode mode)
        {
            if (points.Length != TablePointNumber)
                return;

            // Calculate the different zones, using the points given
            posChecker.zoneOne = new RectF(points[0].X,
                                    points[0].Y,
                                    points[1].X,
                                    (points[2].Y - points[0].Y) * percentageOfSide);

            posChecker.zoneTwo = new RectF(points[0].X, posChecker.zoneOne.Bottom + (1.0f - percentageOfSide * 2) * (points[2].Y - points[0].Y),
                                        points[3].X,
                                        points[3].Y);

            rowChecker.CalculateRows(new System.Drawing.Rectangle((int)posChecker.zoneOne.Left, (int)posChecker.zoneOne.Top,
                                                (int)posChecker.zoneTwo.Right, (int)posChecker.zoneTwo.Bottom), mode);
        }
        /// <summary>
        /// The default constructor for the GameController class
        /// </summary>
        public GameController()
        {
            ballCoordinates = new Queue<PointF>();
            rowChecker = new RowChecker();
            posChecker = new PositionChecker();
        }
    }
}
