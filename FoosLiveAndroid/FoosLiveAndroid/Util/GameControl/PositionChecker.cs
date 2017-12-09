using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.GameControl
{
    class PositionChecker
    {
        // Defines the real height of the table in meters
        private readonly double RealWidth = PropertiesManager.GetDoubleProperty("realWidth");

        // Defines the real width of the table in meters
        private readonly double RealHeight = PropertiesManager.GetDoubleProperty("realHeight");

        private readonly int CentimetersInAMeter = PropertiesManager.GetIntProperty("centimetersInAMeter");

        private double _mulX;
        private double _mulY;

        private RectF _zoneOne;
        private RectF _zoneTwo;

        /// <summary>
        /// Defines the goal zones, which hold the point of no return for the ball
        /// </summary>
        public RectF zoneOne
        {
            get => _zoneOne;

            set
            {
                _zoneOne = value;
                CalculateMultipliers();
            }
        }
        public RectF zoneTwo
        {
            get => _zoneTwo;

            set
            {
                _zoneTwo = value;
                CalculateMultipliers();
            }
        }

        public bool ballInFirstGoalZone
        {
            get => _ballInFirstGoalZone;

            set
            {
                if (value)
                    _ballInSecondGoalZone = false;

                _ballInFirstGoalZone = value;
            }
        }

        public bool ballInSecondGoalZone
        {
            get => _ballInSecondGoalZone;

            set
            {
                if (value)
                    _ballInFirstGoalZone = false;

                _ballInSecondGoalZone = value;
            }
        }

        /// <summary>
        /// Holds the goals, which occured during the session
        /// </summary>
        private Queue<Goal> _goals;

        /// <summary>
        /// Defines whether the ball is in the first goal zone
        /// </summary>
        private bool _ballInFirstGoalZone = false;
        /// <summary>
        /// Defines whether the ball is in the second ball zone
        /// </summary>
        private bool _ballInSecondGoalZone = false;
        /// <summary>
        /// Defines the lost frame counter
        /// </summary>
        private int _framesLost = 0;

        /// <summary>
        /// The minimum amount of frames in the goal zone in order for
        /// the goal to be accepted
        /// </summary>
        private readonly int GoalFramesToCountGoal = PropertiesManager.GetIntProperty("goal_frames_to_count_goal");
        public PositionChecker()
        {
            _goals = new Queue<Goal>();
        }

        private void CalculateMultipliers()
        {
            if (_zoneOne == null || _zoneTwo == null)
                return;

            _mulX = CentimetersInAMeter * ( RealWidth / (_zoneTwo.Right - _zoneTwo.Left) );
            _mulY = CentimetersInAMeter * ( RealHeight / (_zoneTwo.Bottom - _zoneOne.Top) );
        }

        /// <summary>
        /// Defines the goal checking mechanism, which is called whenever
        /// a new position is added to the queue
        /// </summary>
        /// <param name="lastBallCoordinates">Defines the last point of the ball</param>
        /// <param name="BlueScore">Defines the current score for the blue team</param>
        /// <param name="RedScore">Defines the current score for the red team</param>
        /// <param name="currentEvent">Defines the current event</param>
        /// <param name="setter">Defines the setter function for the GameController class's attributes</param>
        /// <param name="GoalEvent">Defines the goal event, which is fired whenever a goal occurs</param>
        /// <param name="ballCoordinates">Defines the queue, holding the historical points of the ball</param>
        public void OnNewFrame(PointF lastBallCoordinates, int BlueScore, int RedScore,
                                CurrentEvent currentEvent, Action<int,int,CurrentEvent> setter,
                                EventHandler<EventArgs> GoalEvent, Queue<PointF> ballCoordinates)
        {
            // Check if this particular point signals that the ball is lost
            if (lastBallCoordinates == null)
            {
                if (_framesLost == GoalFramesToCountGoal)
                {
                    // It is, so check if a goal is about to occur
                    if (_ballInFirstGoalZone)
                    {
                        // Fire the goal event for the first team
                        setter(BlueScore + 1, RedScore, CurrentEvent.BlueGoalOccured);
                        GoalEvent(this, EventArgs.Empty);

                        _goals.Enqueue(new Goal(ballCoordinates, new RectF(zoneOne.Left, zoneOne.Top, zoneTwo.Right, zoneTwo.Bottom)));

                        // Reset variables to their starting values
                        _framesLost = 0;
                        _ballInFirstGoalZone = false;
                        _ballInSecondGoalZone = false;

                        return;
                    }
                    else
                        if (_ballInSecondGoalZone)
                    {
                        // Fire the goal event for the second team
                        setter(BlueScore, RedScore + 1, CurrentEvent.BlueGoalOccured);
                        GoalEvent(this, EventArgs.Empty);

                        _goals.Enqueue(new Goal(ballCoordinates, new RectF(zoneOne.Left, zoneOne.Top, zoneTwo.Right, zoneTwo.Bottom)));

                        // Reset variables to their starting values
                        _framesLost = 0;
                        _ballInFirstGoalZone = false;
                        _ballInSecondGoalZone = false;

                        return;
                    }
                }
                else
                    _framesLost++;
            }
            else
            {
                // It isn't, so reset the counter
                _framesLost = 0;

                // Check if the ball is in the first zone
                if (zoneOne.Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    ballInFirstGoalZone = true;
                }
                else
                    // Check if the ball is in the second zone
                    if (zoneTwo.Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    ballInSecondGoalZone = true;
                }
                else
                {
                    ballInFirstGoalZone = false;
                }
            }

            return;
        }

        public double calculateSpeed(PointF one, PointF two, EventHandler<EventArgs> PositionEvent)
        {
            if (one != null && two != null)
            {
                PositionEvent(this, EventArgs.Empty);
                return Math.Sqrt(
                    (one.X * _mulX - two.X * _mulX) * (one.X * _mulX - two.X * _mulX) +
                    (one.Y * _mulY - two.Y * _mulY) * (one.Y * _mulY - two.Y * _mulY)
                    );
            }
            else
                return 0;
        }
    }
}