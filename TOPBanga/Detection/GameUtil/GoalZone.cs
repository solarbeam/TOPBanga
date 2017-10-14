using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    public enum goalSide { Left , Right };
    class GoalZone
    {
        public Coordinates topLeft { get; private set; }
        public Coordinates topRight { get; private set; }
        public Coordinates bottomLeft { get; private set; }
        public Coordinates bottomRight { get; private set; }
        private goalSide side;
        public GoalZone(Coordinates topLeft, Coordinates topRight, Coordinates bottomLeft, Coordinates bottomRight, goalSide side)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.side = side;
        }
    }
}
