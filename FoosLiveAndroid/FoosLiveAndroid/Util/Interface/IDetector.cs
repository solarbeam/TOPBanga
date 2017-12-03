using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IDetector
    {
        Image<Hsv, byte> image { get; set; }
        bool DetectTable(out RotatedRect rect);
        bool DetectBall(Hsv ballHsv, out Rectangle rect, out Rectangle bBox);
    }
}
