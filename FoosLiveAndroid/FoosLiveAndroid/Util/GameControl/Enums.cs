using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid.Util.GameControl
{
    public class Enums
    {
        public enum Row
        {
            RedGoalie,
            RedDefence,
            BlueAttack,
            RedMidfield,
            BlueMidfield,
            RedAttack,
            BlueDefence,
            BlueGoalie
        }
        public enum CurrentEvent
        {
            PositionChanged,
            RedGoalOccured,
            BlueGoalOccured,
            None
        }
    }
}