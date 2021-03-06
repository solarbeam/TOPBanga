﻿using System;
using System.Collections.Generic;
using Android.Graphics;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Util.Model;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Util.GameControl
{
    public class PositionChecker : IPositionChecker
    {
        // Defines the real height of the table in meters
        private readonly double RealWidth = PropertiesManager.GetDoubleProperty("real_width");

        // Defines the real width of the table in meters
        private readonly double RealHeight = PropertiesManager.GetDoubleProperty("real_height");

        private const int CentimetersInMeter = 100;

        private double _mulX;
        private double _mulY;

        public RectF zoneOne;
        public RectF zoneTwo;

        private bool goalOccured;
        private long timestampStart;

        /// <summary>
        /// Defines the goal zones, which hold the point of no return for the ball
        /// </summary>
        public RectF ZoneOne
        {
            get => zoneOne;

            set
            {
                zoneOne = value;
                CalculateMultipliers();
            }
        }
        public RectF ZoneTwo
        {
            get => zoneTwo;

            set
            {
                zoneTwo = value;
                CalculateMultipliers();
            }
        }

        public bool BallInFirstGoalZone
        {
            get => ballInFirstGoalZone;

            set
            {
                ballInSecondGoalZone &= !value;

                ballInFirstGoalZone = value;
            }
        }

        public bool BallInSecondGoalZone
        {
            get => ballInSecondGoalZone;

            set
            {
                ballInFirstGoalZone &= !value;

                ballInSecondGoalZone = value;
            }
        }

        /// <summary>
        /// Holds the goals, which occured during the session
        /// </summary>
        public Queue<Goal> _goals;

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
            if (zoneOne == null || zoneTwo == null)
                return;

            _mulX = CentimetersInMeter * ( RealWidth / (zoneTwo.Right - zoneTwo.Left) );
            _mulY = CentimetersInMeter * ( RealHeight / (zoneTwo.Bottom - zoneOne.Top) );
        }

        /// <summary>
        /// Defines the goal checking mechanism, which is called whenever
        /// a new position is added to the queue
        /// </summary>
        /// <param name="lastBallCoordinates">Defines the last point of the ball</param>
        /// <param name="BlueScore">Defines the current score for the blue team</param>
        /// <param name="RedScore">Defines the current score for the red team</param>
        /// <param name="setter">Defines the setter function for the GameController class's attributes</param>
        /// <param name="GoalEvent">Defines the goal event, which is fired whenever a goal occurs</param>
        /// <param name="ballCoordinates">Defines the queue, holding the historical points of the ball</param>
        public void OnNewFrame(PointF lastBallCoordinates, int BlueScore, int RedScore,
                               Action<int,int> setter,
                               EventHandler<CurrentEvent> GoalEvent, Queue<PointF> ballCoordinates)
        {
            // Check if this particular point signals that the ball is lost
            if (lastBallCoordinates == null)
            {
                if (_framesLost == GoalFramesToCountGoal)
                {
                    // It is, so check if a goal is about to occur
                    if (ballInFirstGoalZone)
                    {
                        // Fire the goal event for the first team
                        setter(BlueScore + 1, RedScore);
                        GoalEvent(this, CurrentEvent.BlueGoalOccured);

                        _goals.Enqueue(new Goal(ballCoordinates, new RectF(ZoneOne.Left, ZoneOne.Top, ZoneTwo.Right,
                                                ZoneTwo.Bottom), timestampStart, GameTimer.Time, TeamColor.Blue));

                        // Reset variables to their starting values
                        _framesLost = 0;
                        ballInFirstGoalZone = false;
                        ballInSecondGoalZone = false;
                        goalOccured = true;

                        return;
                    }
                    if (!ballInSecondGoalZone) return;
                    // Fire the goal event for the second team
                    setter(BlueScore, RedScore + 1);
                    GoalEvent(this, CurrentEvent.RedGoalOccured);

                    _goals.Enqueue(new Goal(ballCoordinates, new RectF(ZoneOne.Left, ZoneOne.Top, ZoneTwo.Right,
                        ZoneTwo.Bottom), timestampStart, GameTimer.Time, TeamColor.Red));

                    // Reset variables to their starting values
                    _framesLost = 0;
                    ballInFirstGoalZone = false;
                    ballInSecondGoalZone = false;
                    goalOccured = true;
                }
                else
                    _framesLost++;
            }
            else
            {
                if (goalOccured)
                {
                    timestampStart = GameTimer.Time;
                    goalOccured = false;
                }

                // It isn't, so reset the counter
                _framesLost = 0;

                // Check if the ball is in the first zone
                if (ZoneOne.Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    BallInFirstGoalZone = true;
                }
                else
                    // Check if the ball is in the second zone
                    if (ZoneTwo.Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    BallInSecondGoalZone = true;
                }
                else
                {
                    BallInFirstGoalZone = false;
                }
            }
        }

        public double CalculateSpeed(PointF one, PointF two, EventHandler<EventArgs> positionEvent)
        {
            if (one == null || two == null) return 0;
            positionEvent(this, EventArgs.Empty);
            return Math.Sqrt(
                (one.X * _mulX - two.X * _mulX) * (one.X * _mulX - two.X * _mulX) +
                (one.Y * _mulY - two.Y * _mulY) * (one.Y * _mulY - two.Y * _mulY));
        }
    }
}