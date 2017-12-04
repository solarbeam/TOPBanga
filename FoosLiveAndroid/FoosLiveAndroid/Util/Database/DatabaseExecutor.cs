using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace FoosLiveAndroid.Util.Database
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

        public static List<History> GetHistory() {
            List<History> toReturn = new List<History>();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(CONNECTION_URL);
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write("GetHistory");
            streamWriter.Flush();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response;
            while ((response = streamReader.ReadLine()) != null)
            {
                string[] splitted = response.Split(';');
                toReturn.Add(new History(splitted));
            }
            return toReturn;
        }
    }

    public class History
    {
        public string blueTeamName;
        public string redTeamName;
        public int blueTeamPoints;
        public int redTeamPoints;

        public History(string[] input)
        {
            this.blueTeamName = input[0];
            this.redTeamName = input[1];
            this.blueTeamPoints = int.Parse(input[2]);
            this.redTeamPoints = int.Parse(input[3]);
        }
    }
}