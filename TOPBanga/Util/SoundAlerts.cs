using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using TOPBanga.Interface;

namespace TOPBanga.Util
{
    public enum EAlert
    {
        RedGoal,
        RedWin,
        BlueGoal,
        BlueWin,
        BallLost
    }
    public class SoundAlerts
    {
        public IAlert redTeamGoal { get; set; }
        public IAlert redTeamWins { get; set; }

        public IAlert blueTeamGoal { get; set; }
        public IAlert blueTeamWins { get; set; }

        public IAlert ballLost { get; set; }

        public SoundAlerts()
        {

        }
        public void Play(EAlert alertType)
        {
            /**
             * #TODO
             * Add exception handling
             */
            switch(alertType)
            {
                case EAlert.RedGoal:
                    if (this.redTeamGoal.pathSet == true)
                        this.redTeamGoal.Play();
                    break;
                case EAlert.RedWin:
                    if (this.redTeamWins.pathSet == true)
                        this.redTeamWins.Play();
                    break;
                case EAlert.BlueGoal:
                    if (this.blueTeamGoal.pathSet == true)
                        this.blueTeamGoal.Play();
                    break;
                case EAlert.BlueWin:
                    if (this.blueTeamWins.pathSet == true)
                        this.blueTeamWins.Play();
                    break;
                case EAlert.BallLost:
                    if (this.ballLost.pathSet == true)
                        this.ballLost.Play();
                    break;
            }
        }
    }
}
