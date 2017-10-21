using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace TOPBanga
{
    public interface IDetector
    {
        Image<Bgr, byte> image { get; set; }
        bool DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, Hsv ballHsv, int minRadius = 1,
            int cannyThreshold = 12, int accumulatorThreshold = 26, double resolution = 1.9, double minDist = 10, int HoughMinRadius = 0, int HoughMaxRadius = 0);
        Hsv GetBallColorHSVFromCoords(int x, int y);
    }
}
