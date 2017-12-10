using Android.Graphics;
using System;
using System.Collections.Generic;
using static FoosLiveAndroid.Util.GameControl.Enums;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.GameControl
{

    /// <summary>
    /// The class holds the primary functions, required for goal detection
    /// and the predefined attributes for them
    /// </summary>
    public class GameController : IGameController
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

        private PositionChecker _posChecker;
        private RowChecker _rowChecker;

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
                if (lastBallCoordinates != null && _rowChecker.rows != null)
                {
                    _rowChecker.CheckRow(lastBallCoordinates, ref currentEvent);
                }

                ballCoordinates.Enqueue(lastBallCoordinates);

                _posChecker.OnNewFrame(lastBallCoordinates,
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

                CurrentSpeed = _posChecker.CalculateSpeed(lastBallCoordinates, lastLastBallCoordinates, PositionEvent);
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
            _posChecker.ZoneOne = new RectF(points[0].X,
                                    points[0].Y,
                                    points[1].X,
                                    (points[2].Y - points[0].Y) * percentageOfSide);

            _posChecker.ZoneTwo = new RectF(points[0].X, _posChecker.ZoneOne.Bottom + (1.0f - percentageOfSide * 2) * (points[2].Y - points[0].Y),
                                        points[3].X,
                                        points[3].Y);

            _rowChecker.CalculateRows(new System.Drawing.Rectangle((int)_posChecker.ZoneOne.Left, (int)_posChecker.ZoneOne.Top,
                                                (int)_posChecker.ZoneTwo.Right, (int)_posChecker.ZoneTwo.Bottom), mode);
        }
        /// <summary>
        /// The default constructor for the GameController class
        /// </summary>
        public GameController()
        {
            ballCoordinates = new Queue<PointF>();
            _rowChecker = new RowChecker();
            _posChecker = new PositionChecker();
        }
    }
}