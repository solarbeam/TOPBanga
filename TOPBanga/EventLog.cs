using System;
using System.IO;

namespace TOPBanga.Util
{
    public class EventLog : IWrite
    {
        private StreamWriter file;
        public EventLog(String fileName)
        {
            try
            {
                this.file = new StreamWriter(fileName);
                this.file.WriteLine("Start of Event Log");
                this.file.Flush();
            }
            catch(Exception e) { }
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
        public void Close()
        {
            this.file.Close();
        }
    }
}
