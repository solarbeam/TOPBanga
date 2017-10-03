using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOPBanga.Util;
using System.Configuration;

namespace TOPBanga.UnitTests
{
    [TestClass]
    public class EventLogUnitTest
    {
        [TestMethod]
        [ExpectedException (typeof(System.IO.DirectoryNotFoundException))]
        public void EventLogWriteBadPath()
        {
            string path = ConfigurationManager.AppSettings["fakePath"];

            EventLog eventLog = new EventLog(path);

            eventLog.Write("");
        }
    }
}
