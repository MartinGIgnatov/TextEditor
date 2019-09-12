using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Net.Mail;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private string _connectionString;
        private string _privateKey;

        /// <summary>
        /// Creates the instance of this service and gets the private key and the connection string.
        /// </summary>
        /// <param name="configuration">The configuration, from which it gets the the required parameters.</param>
        public AuthenticationService(IConfiguration configuration)
        {

#if DEBUG 
            _connectionString = configuration.GetConnectionString("Development");
            _privateKey = configuration.GetValue<string>("PrivateKey");
#else
            _connectionString = configuration.GetConnectionString("Production");
            _privateKey = configuration.GetValue<string>("PrivateKey");
#endif
        }

        /// <summary>
        /// Check if the password of the current user is a valid type password.
        /// </summary>
        /// <param name="username"> Username of the user. </param>
        /// <param name="password"> Password of the user. </param>
        public void CheckPassword(string username, string password)
        {
            string passwordFromSql;
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Password FROM [User] WHERE Username = @Username";
                passwordFromSql = connection.QueryFirstOrDefault<string>(query, new
                {
                    username
                });
            }

            if (passwordFromSql == null)
            {
                throw new InvalidOperationException("Username or password do not match with existing account.");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, passwordFromSql))
            {
                throw new InvalidOperationException("Username or password do not match with existing account.");
            }
        }

        /// <summary>
        /// Get the user and the whole information about him by his username.
        /// </summary>
        /// <param name="username"> Username of the user. </param>
        /// <returns></returns>
        public User GetUser(string username)
        {
            string queryGetUser = @"SELECT * FROM [User] WHERE [Username] = @Username";

            User user;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                user = connection.QueryFirstOrDefault<User>(queryGetUser, new { Username = username });
                if (user != null)
                {
                    UpdateLastTimeOnline(connection, username);
                }
            }

            return user;
        }

        /// <summary>
        /// Loggin the current user if he exists.
        /// </summary>
        /// <param name="userForLogin"></param>
        /// <returns></returns>
        public string Login(UserForLogin userForLogin)
        {

            if (userForLogin == null)
            {
                throw new ArgumentNullException(nameof(userForLogin));
            }

            CheckPassword(userForLogin.Username, userForLogin.Password);

            return TokenString(userForLogin);

        }

        /// <summary>
        /// Registering the current user by fullfil all the information which is need to be registered.
        /// </summary>
        /// <param name="userForRegistration"></param>
        public void Register(UserForRegistration userForRegistration)
        {
            string query = @"Insert into [User] (Id, Username, Password, Email, Name, CreationDate, LastTimeOnline) Values (newid(), @Username, @Password, @Email, @Name, @CreationDate, @LastTimeOnline);";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                if (!IsValidUsername(connection, userForRegistration.Username))
                {
                    throw new InvalidOperationException("Not allowed registration. Username already exist.");
                }

                if (!IsPasswordsMatching(userForRegistration.Password, userForRegistration.RepeatedPassword))
                {
                    throw new InvalidOperationException("Not allowed registration. Password and repeated password are not the same.");
                }

                if (!IsEmailExisting(connection, userForRegistration.Email))
                {
                    throw new InvalidOperationException("Not allowed registration. Email is already used.");
                }

                DateTime dateTime = DateTime.Now;
                string sqlFormattedDate = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string hashedPassword = HashPassword(userForRegistration.Password);
                connection.Query(query, new
                {
                    Username = userForRegistration.Username,
                    Password = hashedPassword,
                    Email = userForRegistration.Email,
                    Name = userForRegistration.Name,
                    CreationDate = sqlFormattedDate,
                    LastTimeOnline = sqlFormattedDate
                });
            }
        }

        /// <summary>
        /// A token which is used for log in and to be known when the user is authenticated and logged in his own account.
        /// </summary>
        /// <param name="userForLogin"></param>
        /// <returns></returns>
        public string TokenString(UserForLogin userForLogin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_privateKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userForLogin.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool IsValidUsername(SqlConnection connection, string username)
        {
            string query = @"SELECT Username FROM [User] WHERE Username = @Username";

            return connection.QueryFirstOrDefault<string>(query, new { Username = username }) == null;
        }

        private bool IsEmailExisting(SqlConnection connection, string email)
        {
            string query = @"SELECT Email FROM [User] WHERE Email = @Email";

            return connection.QueryFirstOrDefault<string>(query, new { Email = email }) == null;
        }

        private bool IsPasswordsMatching(string password, string repeatedPassword)
        {
            if (password.Equals(repeatedPassword))
            {
                return true;
            }

            return false;
        }

        private void UpdateLastTimeOnline(SqlConnection connection, string username)
        {
            var newLastTimeOnline = DateTime.Now;

            connection.Open();
            var query = "UPDATE [User] SET [LastTimeOnline] = @newLastTimeOnline WHERE [Username] = @username";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@username", username));
            command.Parameters.Add(new SqlParameter("@newLastTimeOnline", newLastTimeOnline));

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected != 1)
            {
                throw new Exception("More than one user have been affected.");
            }
        }
    }
}
