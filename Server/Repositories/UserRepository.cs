using gamershop.Shared.Models;
using gamershop.Server.Database;
using Dapper;
using AutoMapper.Internal.Mappers;

namespace gamershop.Server.Repositories;
public class UserRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InsertUser(User user)
    {
        using (var connection = _connectionFactory.CreateConnection())
        {
            try
            {
                // Insert transaction into database
                await connection.ExecuteAsync(
                    "INSERT INTO users (username, password, salt) VALUES (@username, @password, @salt)",
                    new { user.UserName, user.StoredPassword.Password, user.StoredPassword.Salt });
            }
            catch (Exception ex)
            {
                // Rollback transaction on failure
                string message = "Lol it failed with exception " + ex.Message;
                Console.WriteLine(message);
                throw;
            }
        }
    }


    public async Task<User> GetUser(string username)
    {
        try
        {
            using (var connection = _connectionFactory.CreateConnection())
            {

                var userData = await connection.QuerySingleOrDefaultAsync<dynamic>(
                    "SELECT * FROM users WHERE username = @username",
                    new { username });
                if (userData != null)
                {
                    var user = new User
                    {
                        UserName = userData.username,
                        StoredPassword = new HashedPassword 
                        { 
                            Password = userData.password, 
                            Salt = userData.salt != null ? userData.salt : string.Empty
                        }
                    };
                    return user;
                }
            }
            throw new Exception("No user was found :O)");
        }
        catch (Exception ex)
        {
            // Rollback transaction on failure
            string message = "Lol it failed with exception " + ex.Message;
            Console.WriteLine(message);
            throw;
        }
    }
}