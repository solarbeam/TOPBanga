using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPBanga.Interface;

namespace TOPBanga.Util
{
    public class EventLog : IWrite
    {
        private StreamWriter file;
        public EventLog(String fileName)
        {
            this.file = new StreamWriter(fileName);
            this.file.WriteLine("Start of Event Log");
            this.file.Flush();
        }
        public void Write(String ev)
        {
            this.file.WriteLine(DateTime.Now.ToString() + ": " + ev);
            this.file.Flush();
        }
        public void Close()
        {
            this.file.Close();
        }
    }
}
