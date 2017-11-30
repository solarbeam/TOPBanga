using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace FoosLiveAndroid.Util
{
    public static class DatabaseExecutor
    {

        private const string CONNECTION_URL = "http://yu-gi-oh.lt/a.php";

        public static bool InsertIntoHistory(string blueTeamName, string redTeamName, int bluePoints, int redPoints)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(CONNECTION_URL);
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write($"InsertIntoHistory;{blueTeamName};{redTeamName};{bluePoints};{redPoints}");
            streamWriter.Flush();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string s = streamReader.ReadLine();
            if(s != null && s.Equals("SUCCESS"))
                return true;
            return false;
        }
    }
}