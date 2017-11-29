using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IDetector
    {
        Image<Bgr, byte> image { get; set; }
        bool DetectBall(Hsv ballHsv, out Rectangle rect);
    }
}
