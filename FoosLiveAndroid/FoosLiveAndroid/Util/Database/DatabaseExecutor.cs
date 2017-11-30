using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace FoosLiveAndroid.Util
{
    public static class DatabaseExecutor
    {

        private const string CONNECTION_URL = "http://yu-gi-oh.lt/a.php";
        private const string OPERATION_SUCCESS = "SUCCESS";

        public static bool InsertIntoHistory(string blueTeamName, string redTeamName, int bluePoints, int redPoints)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(CONNECTION_URL);
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write($"InsertIntoHistory;{blueTeamName};{redTeamName};{bluePoints};{redPoints}");
            streamWriter.Flush();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response = streamReader.ReadLine();
            return (response != null && response.Equals(OPERATION_SUCCESS));
        }
    }
}