using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace gamershop.Server.Database
{
    public class DbConnectionFactory
    {
        private  string _connectionString;

        private string _connectionString1;

        

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("ConnectionString");
             _connectionString1 = configuration.GetValue<string>("ConnectionString1");
      
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
                Console.WriteLine($"Error creating database connection for reading: {ex.Message}");
                throw;
            }
        }
          public NpgsqlConnection CreateConnection1()
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString1);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database connection for writing: {ex.Message}");
                throw;
            }
        }
    }
}
