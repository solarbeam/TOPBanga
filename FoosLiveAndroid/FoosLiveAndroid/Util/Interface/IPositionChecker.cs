using System;
using System.Collections.Generic;
using Android.Graphics;
using FoosLiveAndroid.Util.Model;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IPositionChecker
    {
        RectF ZoneOne { get; set; }
        RectF ZoneTwo { get; set; }
        bool BallInFirstGoalZone { get; set; }
        bool BallInSecondGoalZone { get; set; }

        void OnNewFrame(PointF lastBallCoordinates, int BlueScore, int RedScore,
                        Action<int, int> setter,
                        EventHandler<CurrentEvent> GoalEvent, Queue<PointF> ballCoordinates);
        double CalculateSpeed(PointF one, PointF two, EventHandler<EventArgs> PositionEvent);
    }
}
