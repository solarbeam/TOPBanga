using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoosLiveAndroid.TOPBanga.Detection
{
    class ColorContainer
    {
        public List<Hsv> list { get; private set; }

        public ColorContainer()
        {
            this.list = new List<Hsv>();
        }

        public Boolean Add(Hsv hsv)
        {
            /**
             * Check if the specific value exists already
             * in the list
             */
            foreach(Hsv i in this.list)
            {
                if ( hsv.Hue == i.Hue && hsv.Satuation == i.Satuation && hsv.Value == i.Value )
                {
                    return false;
                }
            }

            this.list.Add(hsv);

            return true;
        }
    }
}
