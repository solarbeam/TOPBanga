using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IBlobDetector
    {
        uint GetBlobs(Image<Gray, byte> image, CvBlobs blobs);
    }
}
