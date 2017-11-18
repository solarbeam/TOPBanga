using System;
using System.IO;
using TOPBanga.Detection.GameUtil;

namespace TOPBanga.Util
{
    public class EventLog : IWrite
    {
        private const string redTeamWin = "Red Team Wins";
        private const string blueTeamWin = "Blue Team Wins";
        private StreamWriter file;
        
        public EventLog(String fileName)
        {
            try
            {
                this.file = new StreamWriter(fileName);
                this.file.WriteLine("Start of Event Log");
                this.file.Flush();
            }
            catch(Exception e) {  }
        }
        public void Write(String ev)
        {
            try
            {
                this.file.WriteLine(DateTime.Now.ToString() + ": " + ev);
                this.file.Flush();
            }
            catch (Exception e) { }
        }

        public string WinAnnouncement()
        {
            GameController score = new GameController();
            if (score.redScore == 10)
                return redTeamWin;
            if (score.blueScore == 10)
                return blueTeamWin;
            else
                return null;

        }

        public void Close()
        {
            this.file.Close();
        }
    }
}
