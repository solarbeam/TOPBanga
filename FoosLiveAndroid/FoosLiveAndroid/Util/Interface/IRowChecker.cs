using Android.Graphics;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IRowChecker
    {
        void CheckRow(PointF lastBallCoordinates);
        void CalculateRows(System.Drawing.Rectangle tableZone, CaptureMode mode);
    }
}
