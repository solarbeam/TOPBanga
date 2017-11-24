using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Android.Graphics;

namespace FoosLiveAndroid.TOPBanga.Interface
{
    public interface IDetector
    {
        Image<Bgr, byte> image { get; set; }
        bool DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, Hsv ballHsv);
        Hsv GetBallColorHSVFromCoords(int x, int y);
    }
}
