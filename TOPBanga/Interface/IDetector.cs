using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga
{
    interface IDetector
    {
        bool DetectBall(out float x, out float y, out float radius);
    }
}
