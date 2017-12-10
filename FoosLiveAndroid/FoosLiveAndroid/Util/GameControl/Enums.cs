namespace FoosLiveAndroid.Util.GameControl
{
    public class Enums
    {
        public enum CaptureMode
        {
            Video,
            Camera
        }
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