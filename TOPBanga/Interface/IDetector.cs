using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace TOPBanga
{
    public interface IDetector
    {
        Image<Bgr, byte> image { get; set; }
        Hsv ballHsv { get; set; }
        bool DetectBall(out float x, out float y, out float radius, out Bitmap b, int minRadius = 1);
        void SetBallColorHSV(int h, int s, int v);
        void SetBallColorHSVFromCoords(int x, int y);
    }
}
