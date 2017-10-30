using System;
using System.Drawing;

namespace TOPBanga
{

    public interface IGameTracker
    {
        void AddCoords(float x, float y);
        void DefineGoal(Point[] p);

        event EventHandler GameEvent;
    }
}
