using System;
using Android.Graphics;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IGameController
    {
        event EventHandler<EventArgs> GoalEvent;
        event EventHandler<EventArgs> PositionEvent;

        int RedScore { get; }
        int BlueScore { get; }
        PointF LastBallCoordinates { get; set; }

        void SetTable(PointF[] points, CaptureMode mode);
    }
}
