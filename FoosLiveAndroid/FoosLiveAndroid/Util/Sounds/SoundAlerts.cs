using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Sounds
{
    public class SoundAlerts
    {
        public IAlert RedTeamGoal { get; set; }
        public IAlert RedTeamWins { get; set; }

        public IAlert BlueTeamGoal { get; set; }
        public IAlert BlueTeamWins { get; set; }
       
        public IAlert BallLost { get; set; }

        public void Play(EAlert alertType)
        {
            switch(alertType)
            {
                case EAlert.RedGoal:
                    RedTeamGoal.Play();
                    break;
                case EAlert.RedWin:
                    RedTeamWins.Play();
                    break;
                case EAlert.BlueGoal:
                    BlueTeamGoal.Play();
                    break;
                case EAlert.BlueWin:
                    BlueTeamWins.Play();
                    break;
                case EAlert.BallLost:
                    BallLost.Play();
                    break;
            }
        }
    }
}