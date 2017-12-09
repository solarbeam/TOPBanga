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
        /// <summary>
        /// Defines the goal zones, which hold the point of no return for the ball
        /// </summary>
        public RectF zoneOne;
        public RectF zoneTwo;

        /// <summary>
        /// Holds the goals, which occured during the session
        /// </summary>
        private Queue<Goal> goals;

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
        /// The minimum amount of frames in the goal zone in order for
        /// the goal to be accepted
        /// </summary>
        private readonly int GoalFramesToCountGoal = PropertiesManager.GetIntProperty("goal_frames_to_count_goal");
        public PositionChecker()
        {
            goals = new Queue<Goal>();
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
                if (framesLost == GoalFramesToCountGoal)
                {
                    // It is, so check if a goal is about to occur
                    if (ballInFirstGoalZone)
                    {
                        // Fire the goal event for the first team
                        setter(BlueScore + 1, RedScore, CurrentEvent.BlueGoalOccured);
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
                        setter(BlueScore, RedScore + 1, CurrentEvent.BlueGoalOccured);
                        GoalEvent(this, EventArgs.Empty);

                        goals.Enqueue(new Goal(ballCoordinates, new RectF(zoneOne.Left, zoneOne.Top, zoneTwo.Right, zoneTwo.Bottom)));

                        // Reset variables to their starting values
                        framesLost = 0;
                        ballInFirstGoalZone = false;
                        ballInSecondGoalZone = false;

                        return;
                    }
                }
                else
                    framesLost++;
            }
            else
            {
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

            return;
        }

        public double calculateSpeed(PointF one, PointF two, EventHandler<EventArgs> PositionEvent)
        {
            if (one != null && two != null)
            {
                PositionEvent(this, EventArgs.Empty);
                return Math.Sqrt(
                    (one.X - two.X) * (one.X - two.X) +
                    (one.Y - two.Y) * (one.Y - two.Y)
                    );
            }
            else
                return 0;
        }
    }
}