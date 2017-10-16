using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    /**
     * For now it's a class, but we really should
     * convert this class to a struct
     */
    struct Coordinates
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public Coordinates(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
