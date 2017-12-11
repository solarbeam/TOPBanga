using System;
using Android.Graphics;
using FoosLiveAndroid.Util.Model;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IGameController
    {
        event EventHandler<EventArgs> GoalEvent;
        event EventHandler<EventArgs> PositionEvent;

        int RedScore { get; }
        int BlueScore { get; }
        PointF LastBallCoordinates { get; set; }

        void SetTable(PointF[] points, ECaptureMode mode);
    }
}
