//using System;

//namespace FoosLiveAndroid.Util.Database
//{
//    public class DatabaseConnection : IDisposable
//    {
//        public DatabaseConnection(string connectionString)
//        {
//            this.InitSuccess = true;
//            this.connection = new MySqlConnection(connectionString);
//            try
//            {
//                this.connection.Open();
//            }
//            catch (MySqlException e)
//            {
//                this.InitSuccess = false;
//            }
//            this.insertIntoHistory = new Lazy<MySqlCommand>(() =>
//                new MySqlCommand(System.Configuration.ConfigurationManager.AppSettings["InsertIntoHistory"], connection));
//        }

//        public bool InitSuccess { get; set; }
//        private MySqlConnection connection;
//        private Lazy<MySqlCommand> insertIntoHistory;



//        public void Dispose()
//        {
//            this.connection.Dispose();
//            this.insertIntoHistory.Value.Dispose();
//        }

//        public int ExecuteCommand(string command)
//        {
//            MySqlCommand sqlCommand = new MySqlCommand(command, this.connection);
//            int rowsAffected = -1;
//            try
//            {
//                rowsAffected = sqlCommand.ExecuteNonQuery();
//            }
//            catch (MySqlException e)
//            {
//                return -1;
//            }
//            finally
//            {
//                sqlCommand.Dispose();
//            }
//            return rowsAffected;
//        }

//        public MySqlDataReader ExecuteQuery(string command)
//        {
//            MySqlCommand sqlCommand = new MySqlCommand(command, this.connection);
//            MySqlDataReader reader;
//            try
//            {
//                reader = sqlCommand.ExecuteReader();
//            }
//            catch (MySqlException e)
//            {
//                return null;
//            }
//            finally
//            {
//                sqlCommand.Dispose();
//            }
//            return reader;
//        }

//        public int InsertIntoHistory(string blueTeamName, string redTeamName, int blueTeamPoints, int redTeamPoints)
//        {
//            this.insertIntoHistory.Value.Parameters.AddWithValue("@blueTeamName", blueTeamName);
//            this.insertIntoHistory.Value.Parameters.AddWithValue("@redTeamName", redTeamName);
//            this.insertIntoHistory.Value.Parameters.AddWithValue("@blueTeamPoints", blueTeamPoints);
//            this.insertIntoHistory.Value.Parameters.AddWithValue("@redTeamPoints", redTeamPoints);
//            int rowsAffected = -1;
//            try
//            {
//                rowsAffected = this.insertIntoHistory.Value.ExecuteNonQuery();
//            }
//            catch (MySqlException e)
//            {
//                return -1;
//            }
//            return rowsAffected;
//        }
//    }
//}
