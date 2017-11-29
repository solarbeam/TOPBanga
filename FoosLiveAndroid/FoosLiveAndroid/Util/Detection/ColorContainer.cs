using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Structure;

namespace FoosLiveAndroid.Util.Detection
{
    class ColorContainer
    {
        public List<Hsv> List { get; }

        public ColorContainer()
        {
            List = new List<Hsv>();
        }

        public bool Add(Hsv hsv)
        {
            /**
             * Check if the specific value exists already
             * in the list
             */
            if (List.Any(i => hsv.Hue == i.Hue && hsv.Satuation == i.Satuation && hsv.Value == i.Value))
            {
                return false;
            }

            List.Add(hsv);

            return true;
        }
    }
}
