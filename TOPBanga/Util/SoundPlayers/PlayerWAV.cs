using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using TOPBanga.Interface;

namespace TOPBanga.Util.SoundPlayers
{
    class PlayerWAV : IAlert
    {
        private SoundPlayer player;
        public bool pathSet { get; private set; }
        public PlayerWAV()
        {
            this.player = new SoundPlayer();
        }
        public void SetPath(String URL)
        {
            if (URL == null)
                return;

            this.player.SoundLocation = URL;
            this.pathSet = true;
        }
        public void Play()
        {
            if ( this.pathSet )
            {
                this.player.Play();
            }
        }
    }
}
