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
            List<IHistory> toReturn = new List<IHistory>();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write("GetHistory");
            streamWriter.Flush();
            HttpWebResponse httpWebResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response;
            while ((response = streamReader.ReadLine()) != null)
            {
                Log.Debug("", response);
                string[] splitted = response.Split(';');
                toReturn.Add(new History(splitted));
            }
            return toReturn;
        }

        //Todo: transfer & replace
        public static List<IHistory> tempDataStorage;
    }
}