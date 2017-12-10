using System;
using System.Collections.Generic;
using Android.Graphics;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IPositionChecker
    {
        RectF ZoneOne { get; set; }
        RectF ZoneTwo { get; set; }
        bool BallInFirstGoalZone { get; set; }
        bool BallInSecondGoalZone { get; set; }

        void OnNewFrame(PointF lastBallCoordinates, int BlueScore, int RedScore,
                        CurrentEvent currentEvent, Action<int, int, CurrentEvent> setter,
                        EventHandler<EventArgs> GoalEvent, Queue<PointF> ballCoordinates);
        double CalculateSpeed(PointF one, PointF two, EventHandler<EventArgs> PositionEvent);
    }
}
