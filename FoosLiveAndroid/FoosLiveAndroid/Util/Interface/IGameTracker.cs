using System;
using System.Drawing;

namespace FoosLiveAndroid.Util.Interface
{

    public interface IGameTracker
    {
        void AddCoords(float x, float y);
        void DefineGoal(Point[] p);

        event EventHandler GameEvent;
    }
}
