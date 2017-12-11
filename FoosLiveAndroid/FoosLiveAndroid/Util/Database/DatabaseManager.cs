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

        /// <summary>
        /// Inserts a game into the remote database. 
        /// </summary>
        /// <param name="blueTeamName"> Blue team name</param>
        /// <param name="redTeamName">Red team name</param>
        /// <returns>A task that returns int. The int is the id of the inserted game.
        /// This id is used to specify which game to add goals and events to.</returns>
        public static async Task<int> InsertGame(string blueTeamName, string redTeamName) {
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = GetTimeout;
            var streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write($"InsertGame;{blueTeamName};{redTeamName}");
            streamWriter.Flush();
            var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string idUnconverted = await streamReader.ReadToEndAsync();
            return int.Parse(idUnconverted);
        }
        /// <summary>
        /// Inserts a goal into the remote database.
        /// </summary>
        /// <param name="gameId">The game id in the database. It is recommended to use the int returned by InsertGame,
        /// otherwise the method might not work correctly or at all.</param>
        /// <param name="teamName">The team name of the team who scored.</param>
        /// <returns></returns>
        public static async Task InsertGoal(int gameId, string teamName)
        {
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = GetTimeout;
            var streamWriter = new StreamWriter(await request.GetRequestStreamAsync());
            streamWriter.Write($"InsertGoal;{gameId};{teamName}");
            streamWriter.Flush();
            /* The following two lines are only invoked because the request does not get handled
             * if the caller does not open a response steam. The stream is otherwise unused.
             */
            var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
        }
        /// <summary>
        /// Inserts an event into the remote database.
        /// </summary>
        /// <param name="gameId">The game id in the database. It is recommended to use the int returned by InsertGame,
        /// otherwise the method might not work correctly or at all.</param>
        /// <param name="details">The details of the event, in string form.</param>
        /// <returns></returns>
        public static async Task InsertEvent(int gameId, string details)
        {
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = GetTimeout;
            var streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write($"InsertEvent;{gameId};{details}");
            streamWriter.Flush();
            /* The following two lines are only invoked because the request does not get handled
             * if the caller does not open a response steam. The stream is otherwise unused.
             */
            var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
        }
        /// <summary>
        /// Gets the full history of games played.
        /// </summary>
        /// <returns>A task that returns a list of IHistory objects.</returns>
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
                string[] splitted = response.Split(';');
                historyData.Add(new History(splitted));
            }
            return historyData;
        }
    }
}