using System;
using System.IO;
using TOPBanga.Detection.GameUtil;

namespace TOPBanga.Util
{
    public class EventLog : IWrite
    {
        private const string redTeamWin = "Red Team Wins";
        private const string blueTeamWin = "Blue Team Wins";
        private const string eventLogStart = "Start of Event Log";
        private StreamWriter file;
        
        public EventLog(String fileName)
        {
            
                this.file = new StreamWriter(fileName);
                this.file.WriteLine(eventLogStart);
                this.file.Flush();
           
        }
        public void Write(String ev)
        {
                this.file.WriteLine(DateTime.Now.ToString() + ": " + ev);
                this.file.Flush();
        }

        public string WinAnnouncement(int redScore,int blueScore)
        {
            if (redScore == 10)
                return redTeamWin;
            if (blueScore == 10)
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
