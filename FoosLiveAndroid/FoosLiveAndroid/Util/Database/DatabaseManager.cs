using System.Net;
using System.IO;
using System.Collections.Generic;
using FoosLiveAndroid.Model.Interface;
using FoosLiveAndroid.Model;
using System.Threading.Tasks;
using System;
using Android.Util;

namespace FoosLiveAndroid.Util.Database
{
    public static class DatabaseManager
    {
        // Request properties
        private static readonly string ConnectionUrl = PropertiesManager.GetProperty("connection_url");
        private static readonly string OperationSuccess = PropertiesManager.GetProperty("operation_success");
        private static readonly int Timeout = PropertiesManager.GetIntProperty("db_timeout");

        // Request statements formats
        private static readonly string InsertGameFormat = PropertiesManager.GetProperty("insert_game_format");
        private static readonly string InsertGoalFormat = PropertiesManager.GetProperty("insert_goal_format");
        private static readonly string InsertEventFormat = PropertiesManager.GetProperty("insert_event_format");
        private static readonly string GetHistoryFormat = PropertiesManager.GetProperty("get_history_format");

        /// <summary>
        /// Inserts a game into the remote database. 
        /// </summary>
        /// <param name="blueTeamName"> Blue team name</param>
        /// <param name="redTeamName">Red team name</param>
        /// <returns>The id of the inserted game, or -1 if error happens.
        /// This id is used to specify which game to add goals and events to.</returns>
        public static async Task<int> InsertGame(string blueTeamName, string redTeamName, string ownersId) {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = Timeout;

                var streamWriter = new StreamWriter(request.GetRequestStream());
                // Prepare query statement
                streamWriter.Write(InsertGameFormat, blueTeamName, redTeamName, ownersId);

                streamWriter.Flush();
                var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();

                var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                var idUnconverted = await streamReader.ReadToEndAsync();
                if (int.TryParse(idUnconverted, out var id))
                    return id;
                return -1;
            }
            catch(Exception e)
            {
                Log.Error("Exception in db", e.ToString());
            }
            return -1;
        }
        /// <summary>
        /// Inserts a goal into the remote database.
        /// </summary>
        /// <param name="gameId">The game id in the database. It is recommended to use the int returned by InsertGame,
        /// otherwise the method might not work correctly or at all.</param>
        /// <param name="teamName">The team name of the team who scored.</param>
        /// <returns>Request result</returns>
        public static async Task<bool> InsertGoal(int gameId, string teamName)
        {
            // Set up request 
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = Timeout;

            // Set up request statement
            var streamWriter = new StreamWriter(await request.GetRequestStreamAsync());
            streamWriter.Write(InsertGoalFormat, gameId, teamName);
            streamWriter.Flush();

            // Get response
            var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response = await streamReader.ReadToEndAsync();

            return response.Equals(OperationSuccess);
        }

        /// <summary>
        /// Inserts an event into the remote database.
        /// </summary>
        /// <param name="gameId">The game id in the database. It is recommended to use the int returned by InsertGame,
        /// otherwise the method might not work correctly or at all.</param>
        /// <param name="details">The details of the event, in string form.</param>
        /// <returns>Request result</returns>
        public static async Task<bool> InsertEvent(int gameId, string details)
        {
            // Set up request 
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = Timeout;

            // Set up request statement
            var streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write(InsertEventFormat, gameId, details);
            streamWriter.Flush();

            // Get response
            var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response = await streamReader.ReadToEndAsync();

            return response.Equals(OperationSuccess);
        }

        /// <summary>
        /// Gets the full history of games played.
        /// </summary>
        /// <returns>A task that returns a list of IHistory objects.</returns>
        public static async Task<List<IHistory>> GetHistory() {
            // Set up request 
            var request = (HttpWebRequest)WebRequest.Create(ConnectionUrl);
            request.Method = WebRequestMethods.Http.Post;
            request.Timeout = Timeout;
            try
            {
                // Set up request statement
                var streamWriter = new StreamWriter(await request.GetRequestStreamAsync());
                streamWriter.Write(GetHistoryFormat);
                streamWriter.Flush();

                // Get response
                var httpWebResponse = (HttpWebResponse)await request.GetResponseAsync();
                var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                // Parse response
                string response;
                var historyData = new List<IHistory>();
                while ((response = streamReader.ReadLine()) != null)
                {
                    historyData.Add(new History(response.Split(';')));
                }
                return historyData;
            }
            catch (Exception we) {
                Log.Error("Exception", we.ToString()); 
            }
            return null;
        }
    }
}
