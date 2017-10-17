using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    struct Coordinates
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public Coordinates(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
