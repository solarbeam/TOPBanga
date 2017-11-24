using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace FoosLiveAndroid.TOPBanga.Detection
{
    class ColorContainer
    {
        public List<Hsv> List { get; private set; }

        public ColorContainer()
        {
            List = new List<Hsv>();
        }

        public Boolean Add(Hsv hsv)
        {
            /**
             * Check if the specific value exists already
             * in the list
             */
            foreach(Hsv i in this.List)
            {
                if ( hsv.Hue == i.Hue && hsv.Satuation == i.Satuation && hsv.Value == i.Value )
                {
                    return false;
                }
            }

            List.Add(hsv);

            return true;
        }
    }
}
