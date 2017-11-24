using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.TOPBanga.Detection
{
    class BlobDetector
    {
        private CvBlobDetector detector;
        public BlobDetector()
        {
            this.detector = new CvBlobDetector();
            CvBlobs blobs = new CvBlobs();
        }
        public uint GetBlobs(Image<Gray,byte> image, CvBlobs blobs)
        {
            return this.detector.Detect(image,blobs);
        }
    }
}
