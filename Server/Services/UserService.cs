using System.Security.Cryptography;
using System.Text;
using gamershop.Server.Repositories;
using gamershop.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace gamershop.Server.Services;

public class UserService
{
    private const int _saltSize = 16;
    private const int _hashSize = 20;
    private const int _iterations = 10000;

    private readonly UserRepository _userRepository;
    private readonly string secretKey;


    public UserService(UserRepository userRepository, IConfiguration configuration)
    {
        secretKey = configuration.GetValue<string>("SecretKey");
        _userRepository = userRepository;
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


    public async Task StoreUser(string username, HashedPassword password)
    {
        User newUser = new User
        {
            UserName = username,
            StoredPassword =  password
        };

        await _userRepository.InsertUser(newUser);
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


    public async Task<string> GenerateJWT(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }


    public async Task<User> GetUser(string username)
    {
        return await _userRepository.GetUser(username);
    }
}