using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Util
{
    class EventLog
    {
        private StreamWriter file;
        public EventLog(String fileName)
        {
            this.file = new StreamWriter(fileName);
            this.file.WriteLine("Start of Event Log");
            this.file.Flush();
        }
        public void WriteEvent(String ev)
        {
            this.file.WriteLine(DateTime.Now.ToString() + ": " + ev);
            this.file.Flush();
        }
        public void CloseFile()
        {
            this.file.Close();
        }
    }
}
