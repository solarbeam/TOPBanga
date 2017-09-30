using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection
{
    class PositionLogger
    {
        /**
         * Defines the object's position on the x axis
         **/
        private int posX;
        /**
         * Defines the object's position on the y axis
         **/
        private int posY;
        /**
         * Defines the relative delta between two different
         * positions. Will be used for statistics ( Speed and etc. )
         **/
        private int delta;
        public PositionLogger()
        {

        }
        public void Update ( object o, EventArgs e)
        {
            delta = Math.Abs((this.posX - DrawMatches.objectPos.X)^2 + (this.posY - DrawMatches.objectPos.Y)^2);
            this.posX = DrawMatches.objectPos.X;
            this.posY = DrawMatches.objectPos.Y;
        }
    }
}
