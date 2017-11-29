using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Detection
{
    class BlobDetector
    {
        private CvBlobDetector detector;
        public BlobDetector()
        {
            detector = new CvBlobDetector();
            var blobs = new CvBlobs();
        }
        public uint GetBlobs(Image<Gray,byte> image, CvBlobs blobs)
        {
            return detector.Detect(image,blobs);
        }
    }
}
