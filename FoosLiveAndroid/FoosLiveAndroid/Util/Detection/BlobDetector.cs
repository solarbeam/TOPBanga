using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Detection
{
    class BlobDetector : IBlobDetector
    {
        private readonly CvBlobDetector _detector;
        public BlobDetector()
        {
            _detector = new CvBlobDetector();
        }
        public uint GetBlobs(Image<Gray,byte> image, CvBlobs blobs)
        {
            return _detector.Detect(image,blobs);
        }   
    }
}
