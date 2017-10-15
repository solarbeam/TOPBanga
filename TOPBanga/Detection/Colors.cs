using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection
{
    /**
     * To be renamed
     */
    class Colors
    {
        public List<Hsv> list { get; private set; }

        public Colors()
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
