using System.Collections.Generic;
using System.Drawing;

namespace TOPBanga
{
    public class GameTracker : IGameTracker
    {

        public Queue<Point> points { get; private set; }

        public int pointMaximum { get; set; }

        public event Delegates.GameReceiver GameEvent;

        public GameTracker() {
            this.points = new Queue<Point>();
        }

        public void AddCoords(float x, float y)
        {
            //this.points.Enqueue(new Point(x, y));
        }

        public void DefineGoal(Point[] p)
        {
            throw new System.NotImplementedException();
        }
    }
}