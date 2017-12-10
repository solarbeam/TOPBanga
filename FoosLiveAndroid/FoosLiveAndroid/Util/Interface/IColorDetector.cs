using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IColorDetector
    {
        Image<Hsv, byte> image { get; set; }
        int Threshold { get; set; }
        int MinContourArea { get; set; }
        bool DetectBall(Hsv ballHsv, out Rectangle rect, out Rectangle blobBox);
    }
}
