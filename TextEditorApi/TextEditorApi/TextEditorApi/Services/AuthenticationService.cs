using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TextEditorApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace TextEditorApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private string _connectionString;
        private string _privateKey;

        public AuthenticationService(IConfiguration configuration)
        {
#if DEBUG 
            _connectionString = configuration.GetConnectionString("Development");
#else
            _connectionString = configuration.GetConnectionString("Production");
#endif
        }

        private bool IsValidUsername(SqlConnection connection, string username)
        {
            string query = @"SELECT Username FROM [User] WHERE Username = @Username";

            return connection.QueryFirstOrDefault<string>(query, new { Username = username }) == null;
        }

        private bool IsValidEmail(SqlConnection connection, string email)
        {
            string query = @"SELECT Email FROM [User] WHERE Email = @Email";

            return connection.QueryFirstOrDefault<string>(query, new { Email = email }) == null;
        }

        private bool DoPasswordsMatch(string password, string repeatedPassword)
        {
            if (password.Equals(repeatedPassword))
            {
                return true;
            }
            return false;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public void Register(UserForRegistration userForRegistration)
        {
            string query = @"Insert into [User] (Id, Username, Password, Email) Values (newid(), @Username, @Password, @Email);";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                if (IsValidUsername(connection, userForRegistration.Username) &&
                    DoPasswordsMatch(userForRegistration.Password, userForRegistration.RepeatedPassword) &&
                    IsValidEmail(connection, userForRegistration.Email))
                {
                    string hashedPassword = HashPassword(userForRegistration.Password);
                    connection.Query(query, new
                    {
                        Username = userForRegistration.Username,
                        Password = hashedPassword,
                        Email = userForRegistration.Email
                    });
                }
                else
                {
                    throw new InvalidOperationException("Unable to register user!");
                }
            }
        }

        public string LogIn(UserForLogIn userForLogIn)
        {
            if (userForLogIn == null)
            {
                throw new ArgumentNullException(nameof(userForLogIn));
            }

            string passwordFromSql = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Password FROM [User] WHERE Username = @Username";
                passwordFromSql = connection.QueryFirstOrDefault<string>(query, new
                {
                    Username = userForLogIn.Username
                });
            }

            if (passwordFromSql == null)
            {
                throw new InvalidOperationException("There is no user with that username.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userForLogIn.Password, passwordFromSql))
            {
                throw new InvalidOperationException("Wrong password.");
            }

            return TokenString(userForLogIn);
        }       

        private string TokenString(UserForLogIn userForLogIn)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_privateKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userForLogIn.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}

