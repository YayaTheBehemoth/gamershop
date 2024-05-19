using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using gamershop.Shared.Models;
using Npgsql;

namespace gamershop.Server.Services;

public class UserService
{
    private const int _saltSize = 16;
    private const int _hashSize = 20;
    private const int _iterations = 10000;

    private DbConnection _dbConnection;

    public UserService(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<HashedPassword> EncryptPassword(string password)
    {
        HashedPassword hashedPassword = new HashedPassword();
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] salt = new byte[_saltSize];
            rng.GetBytes(salt);

            // Generate the hash
            using (var pbkdf2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, _iterations, HashAlgorithmName.SHA1))
            {
                byte[] hash = pbkdf2.GetBytes(_hashSize);

                // Convert to base64 for storage
                string base64Salt = Convert.ToBase64String(salt);
                string base64Hash = Convert.ToBase64String(hash);

                hashedPassword.Password = base64Hash;
                hashedPassword.Salt = base64Salt;

                return hashedPassword;
            }
        }
    }

    public async Task<bool> CheckPassword(HashedPassword storedpassword, string password)
    {
        // Extract salt and hash from stored password
        byte[] salt = Convert.FromBase64String(storedpassword.Salt);
        byte[] storedPassword = Convert.FromBase64String(storedpassword.Password);

        // Generate the hash for the input password
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _iterations, HashAlgorithmName.SHA1))
        {
            byte[] hash = pbkdf2.GetBytes(_hashSize);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < _hashSize; i++)
            {
                if (storedPassword[i] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    public async Task CreateUser(string userName, HashedPassword password)
    {
        string sql = "INSERT INTO users (username, password, salt) VALUES (@username, @password, @salt)";
        using var cmd = new NpgsqlCommand(sql, _dbConnection as NpgsqlConnection);
        cmd.Parameters.AddWithValue("username", userName);
        cmd.Parameters.AddWithValue("password",  password.Password);
        cmd.Parameters.AddWithValue("salt", password.Salt);
        await cmd.ExecuteNonQueryAsync();
    }
}