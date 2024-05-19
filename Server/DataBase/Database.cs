using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace gamershop.Server.Database
{
    public class DbConnectionFactory
    {
        private  string _connectionString;

        

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("ConnectionString");
      
            if (_connectionString == null)
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }
        }

        public NpgsqlConnection CreateConnection()
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database connection: {ex.Message}");
                throw;
            }
        }
    }
}
