using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Util.Model;

namespace FoosLiveAndroid.Util.Sounds
{
    public class SoundAlerts
    {
        public IAlert Team1Goal { get; set; }
        public IAlert Team1Win { get; set; }

        public IAlert Team2Goal { get; set; }
        public IAlert Team2Win { get; set; }
       
        public IAlert BallLost { get; set; }

        public void Play(EAlert alertType)
        {
            switch(alertType)
            {
                case EAlert.Team2Goal:
                    Team2Goal.Play();
                    break;
                case EAlert.Team2Win:
                    Team2Win.Play();
                    break;
                case EAlert.Team1Goal:
                    Team1Goal.Play();
                    break;
                case EAlert.Team1Win:
                    Team1Win.Play();
                    break;
                case EAlert.BallLost:
                    BallLost.Play();
                    break;
            }
        }
    }
}