using System.Net;
using System.IO;
using System.Collections.Generic;
using FoosLiveAndroid.Model.Interface;
using FoosLiveAndroid.Model;
using System.Threading.Tasks;
using Android.Util;

namespace FoosLiveAndroid.Util.Database
{
    public static class DatabaseManager
    {

        private static readonly string ConnectionUrl = PropertiesManager.GetProperty("connection_url");
        private static readonly string OperationSuccess = PropertiesManager.GetProperty("operation_success");
        private static readonly int GetTimeout = PropertiesManager.GetIntProperty("get_timeout");

        public static bool InsertIntoHistory(string blueTeamName, string redTeamName, int bluePoints, int redPoints)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write($"InsertIntoHistory;{blueTeamName};{redTeamName};{bluePoints};{redPoints}");
            streamWriter.Flush();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response = streamReader.ReadLine();
            return (response != null && response.Equals(OperationSuccess));
        }

        public static async Task<List<IHistory>> GetHistory() {
            var historyData = new List<IHistory>();
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = GetTimeout;
            var streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write("GetHistory");
            streamWriter.Flush();
            var httpWebResponse = (HttpWebResponse) await request.GetResponseAsync();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response;
            while ((response = streamReader.ReadLine()) != null)
            {
                //Log.Debug("", response);
                string[] splitted = response.Split(';');
                historyData.Add(new History(splitted));
            }
            return historyData;
        }

        //Todo: transfer & replace
        public static List<IHistory> tempDataStorage;
    }
}