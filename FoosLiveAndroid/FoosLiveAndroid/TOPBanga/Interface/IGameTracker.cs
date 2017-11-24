using System;
using System.Drawing;

namespace FoosLiveAndroid.TOPBanga.Interface
{

    public interface IGameTracker
    {
        void AddCoords(float x, float y);
        void DefineGoal(Point[] p);

        event EventHandler GameEvent;
    }
}
