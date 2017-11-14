using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace TOPBanga
{
    public class DatabaseConnection : IDisposable
    {
        public bool initSuccess { get; set; }
        private MySqlConnection connection;
        private Lazy<MySqlCommand> insertIntoHistory;

        public DatabaseConnection(string connectionString)
        {
            this.initSuccess = true;
            this.connection = new MySqlConnection(connectionString);
            try
            {
                this.connection.Open();
            }
            catch(MySqlException e) {
                this.initSuccess = false;
            }
            this.insertIntoHistory = new Lazy<MySqlCommand>(() => 
                new MySqlCommand(System.Configuration.ConfigurationManager.AppSettings["InsertIntoHistory"], connection));
        }

        public void Dispose() {
            this.connection.Dispose();
            this.insertIntoHistory.Value.Dispose();
        }

        public int ExecuteCommand (string command)
        {
            MySqlCommand sqlCommand = new MySqlCommand(command, this.connection);
            int rowsAffected = -1;
            try
            {
                rowsAffected = sqlCommand.ExecuteNonQuery();
            }
            catch(MySqlException e)
            {
                return -1;
            }
            finally
            {
                sqlCommand.Dispose();
            }
            return rowsAffected;
        }

        public MySqlDataReader ExecuteQuery(string command)
        {
            MySqlCommand sqlCommand = new MySqlCommand(command, this.connection);
            MySqlDataReader reader;
            try
            {
                reader = sqlCommand.ExecuteReader();
            }
            catch (MySqlException e)
            {
                return null;
            }
            finally
            {
                sqlCommand.Dispose();
            }
            return reader;
        }

        public int InsertIntoHistory(string blueTeamName, string redTeamName, int blueTeamPoints, int redTeamPoints)
        {
            this.insertIntoHistory.Value.Parameters.AddWithValue("@blueTeamName", blueTeamName);
            this.insertIntoHistory.Value.Parameters.AddWithValue("@redTeamName", redTeamName);
            this.insertIntoHistory.Value.Parameters.AddWithValue("@blueTeamPoints", blueTeamPoints);
            this.insertIntoHistory.Value.Parameters.AddWithValue("@redTeamPoints", redTeamPoints);
            int rowsAffected = -1;
            try
            {
                rowsAffected = this.insertIntoHistory.Value.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                return -1;
            }
            return rowsAffected;
        }
    }
}
