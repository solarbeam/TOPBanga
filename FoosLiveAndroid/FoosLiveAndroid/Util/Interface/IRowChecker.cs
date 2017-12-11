using Android.Graphics;
using FoosLiveAndroid.Util.Model;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IRowChecker
    {
        void CheckRow(PointF lastBallCoordinates);
        void CalculateRows(System.Drawing.Rectangle tableZone, ECaptureMode mode);
    }
}
