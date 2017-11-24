using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace FoosLiveAndroid.TOPBanga.Interface
{
    public interface IDetector
    {
        Image<Bgr, byte> image { get; set; }
        bool DetectBall(Hsv ballHsv, out Rectangle rect);
        Hsv GetBallColorHSVFromCoords(int x, int y);
    }
}
