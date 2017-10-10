using System;
using System.Drawing;
using static TOPBanga.Delegates;

namespace TOPBanga
{

    public interface IGameTracker
    {
        void AddCoords(float x, float y);
        void DefineGoal(Point[] p);

        event GameReceiver GameEvent;
    }
}
