using System.Data.SqlClient;
using System.Data;

namespace SpaceShooter.Utilities
{
    internal class DatabaseConnection
    {
        // Connection string to connect to the SQL Server database
        private static readonly string connectionString = "Data Source=MARWAN\\SQLEXPRESS;Initial Catalog=SpaceShooter;Integrated Security=True;TrustServerCertificate=True";

        // Static instance of SqlConnection using the connection string
        private static readonly SqlConnection connection = new(connectionString);

        // Method to get the static SqlConnection instance
        public static SqlConnection GetConnection() => connection;

        // Method to open the database connection if it's not already open
        public static void Open()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        // Method to close the database connection if it's not already closed
        public static void Close()
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        // Method to create a SqlCommand with the given query and the static connection
        public static SqlCommand CreateCommand(string query)
        {
            SqlCommand command = new(query, connection);
            return command;
        }

        // Method to create a SqlDataAdapter with the given query and the static connection
        public static SqlDataAdapter CreateDataAdapter(string query)
        {
            SqlDataAdapter adapter = new(query, connection);
            return adapter;
        }

        // Method to execute a query that returns a SqlDataReader
        public static SqlDataReader ExecuteReader(string query)
        {
            SqlCommand command = CreateCommand(query);
            return command.ExecuteReader();
        }
    }
}
