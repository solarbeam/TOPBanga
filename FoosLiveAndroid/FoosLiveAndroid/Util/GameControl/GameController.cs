using Android.Graphics;
using System;
using System.Collections.Generic;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Util.Model;
using Android.Util;

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
        public event EventHandler<CurrentEvent> GoalEvent;
        public event EventHandler<EventArgs> PositionEvent;

        public int[] zones
        {
            get => _rowChecker.GetRowInformation();
        }

        public ZoneInfo heatmapZones
        {
            get => _heatmapGen;
        }

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

        private PositionChecker _posChecker;
        private RowChecker _rowChecker;
        private ZoneInfo _heatmapGen;

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
        /// Defines the average speed during the match at any given moment
        /// </summary>
        public double AverageSpeed;
        private int _avgSpeedCounter;

        public double MaxSpeed;

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
                    _rowChecker.CheckRow(lastBallCoordinates);
                }

                // Update heatmap info
                _heatmapGen.AssignValue(lastBallCoordinates);

                ballCoordinates.Enqueue(lastBallCoordinates);

                _posChecker.OnNewFrame(lastBallCoordinates,
                                        BlueScore,
                                        RedScore,
                                        (blue, red) =>
                                        {
                                            BlueScore = blue;
                                            RedScore = red;
                                        },
                                        GoalEvent,
                                        ballCoordinates);

                // Calculate the speed
                CurrentSpeed = _posChecker.CalculateSpeed(lastBallCoordinates, lastLastBallCoordinates, PositionEvent);

                // Calculate the average speed
                if (lastBallCoordinates != null)
                {
                    AverageSpeed = ((AverageSpeed * _avgSpeedCounter) + CurrentSpeed) / (_avgSpeedCounter + 1);
                    _avgSpeedCounter++;
                }

                if (MaxSpeed < CurrentSpeed)
                    MaxSpeed = CurrentSpeed;
            }
        }

        /// <summary>
        /// Holds the ball coordinates in a queue
        /// </summary>
        public Queue<PointF> ballCoordinates;

        public Queue<Goal> Goals => _posChecker._goals;

        /// <summary>
        /// Set the table, which will be used for the definition of
        /// the goal zones
        /// It is pressumed, that the first point is the top left one, the second
        /// is the top right, the third is the bottom left and the bottom is the
        /// bottom right
        /// </summary>
        /// <param name="points">The coordinates of the table</param>
        /// <param name="mode"></param>
        public void SetTable(PointF[] points, ECaptureMode mode)
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

            _heatmapGen = new ZoneInfo(new RectF(_posChecker.ZoneOne.Left, _posChecker.ZoneOne.Top,
                                                _posChecker.ZoneTwo.Right, _posChecker.ZoneTwo.Bottom), 30, 30);
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
