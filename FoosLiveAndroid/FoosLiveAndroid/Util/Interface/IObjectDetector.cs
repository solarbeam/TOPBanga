using Android.Graphics;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IObjectDetector
    {
        bool Detect(Canvas canvas, Hsv ballHsv, Bitmap bitmap, Bitmap bgBitmap);
    }
}
