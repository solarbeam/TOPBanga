using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TOPBanga
{
    public class DatabaseConnection : IDisposable
    {
        public bool initSuccess { get; set; }
        private MySqlConnection connection;

        public DatabaseConnection(string connectionString)
        {
            this.connection = new MySqlConnection(connectionString);
            try
            {
                this.connection.Open();
            }
            catch(MySqlException e) {
                Console.WriteLine(e);
                this.initSuccess = false;
            }
            this.initSuccess = true;
        }

        public void Dispose() {
            this.connection.Dispose();
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

    }
}
