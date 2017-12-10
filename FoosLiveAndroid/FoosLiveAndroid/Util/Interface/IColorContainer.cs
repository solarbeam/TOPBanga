using System.Collections.Generic;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Interface
{
    public interface IColorContainer
    {
        List<Hsv> List { get; }
        bool Add(Hsv hsv);
    }
}
