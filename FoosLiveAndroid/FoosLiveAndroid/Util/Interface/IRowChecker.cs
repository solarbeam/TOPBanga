using Android.Graphics;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IRowChecker
    {
        void CheckRow(PointF lastBallCoordinates, ref CurrentEvent currentEvent);
        void CalculateRows(System.Drawing.Rectangle tableZone, CaptureMode mode);
    }
}
