using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Cuda;
using Emgu.CV.XFeatures2D;
using TOPBanga;
using Test;

namespace TOPBanga.Detection
{
    public class SURFDetection
    {
        private SURF surf;
        private VectorOfKeyPoint modelKeyPoints;
        private VectorOfKeyPoint observedKeyPoints;
        private VectorOfVectorOfDMatch matches;
        private MainForm form;

        public SURFDetection(MainForm mm) {
            this.form = mm;
            Mat m1 = new Mat("C:/Users/Liutauras/Desktop/a.jpg");
            Mat m2 = new Mat("C:/Users/Liutauras/Desktop/b.jpg");
            UMat um1 = m1.GetUMat(AccessType.Read);
            UMat um2 = m2.GetUMat(AccessType.Read);
            UMat modelDescriptors = new UMat();
            UMat observedDescriptors = new UMat();

            this.surf = new SURF(300);
            this.modelKeyPoints = new VectorOfKeyPoint();
            this.observedKeyPoints = new VectorOfKeyPoint();
            this.matches = new VectorOfVectorOfDMatch();

            this.surf.DetectAndCompute(um1, null, modelKeyPoints, modelDescriptors, false);
            this.surf.DetectAndCompute(um2, null, observedKeyPoints, observedDescriptors, false);
            BFMatcher matcher = new BFMatcher(DistanceType.L2);
            matcher.Add(modelDescriptors);

            matcher.KnnMatch(observedDescriptors, matches, 2, null);

            Mat mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(255));
            Features2DToolbox.VoteForUniqueness(matches, 0.8, mask);

            Mat result = new Mat();
            Features2DToolbox.DrawMatches(m2, modelKeyPoints, m1, observedKeyPoints,
               matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);
            form.setImage(result.Bitmap);
        }
    }
}
