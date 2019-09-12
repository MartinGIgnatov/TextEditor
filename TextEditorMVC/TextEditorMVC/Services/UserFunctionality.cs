using Dapper;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditorMVC.Models;
using TextEditorMVC.Services;

namespace TextEditorMVC.Services
{
    public class UserFunctionality : IUserFunctionality
    {
        private string _connectionString;
        private string _privateKey;
        private AuthenticationService _authenticationService;

        public UserFunctionality(IConfiguration configuration)
        {
            _authenticationService = new AuthenticationService(configuration);
#if DEBUG 
            _connectionString = configuration.GetConnectionString("Development");
            _privateKey = configuration.GetValue<string>("PrivateKey");
#else
            _connectionString = configuration.GetConnectionString("Production");
            _privateKey = configuration.GetValue<string>("PrivateKey");
#endif
        }

        /// <summary>
        /// Changes the Email of the current user if the validation of the password and the given new instance of the Email is available. 
        /// </summary>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        /// <param name="newEmail">The new Email.</param>
        /// <param name="password">The password given from the user.</param>
        public void ChangeEmail(string tokenString, string newEmail, string password)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;
            _authenticationService.CheckPassword(username, password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE [User] SET [Email] = @newEmail WHERE [Username] = @username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@username", username));
                command.Parameters.Add(new SqlParameter("@newEmail", newEmail));

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new Exception("More than one user have been affected.");
                }
            }
        }

        /// <summary>
        /// Changes the Name of the current user if the validation of the password and the given new instance of the Name is available. 
        /// </summary>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        /// <param name="newEmail">The new Name.</param>
        /// <param name="password">The password given from the user.</param>
        public void ChangeName(string tokenString, string newName, string password)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;
            _authenticationService.CheckPassword(username, password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE [User] SET [Name] = @newName WHERE [Username] = @username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@username", username));
                command.Parameters.Add(new SqlParameter("@newName", newName));

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new Exception("More than one user have been affected.");
                }
            }
        }

        /// <summary>
        /// Changes the Password of the current user if the validation of the password and the given new instance of the Password is available. 
        /// </summary>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        /// <param name="newEmail">The new Password.</param>
        /// <param name="password">The password given from the user.</param>
        public void ChangePassword(string tokenString, string newPassword, string password)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;
            _authenticationService.CheckPassword(username, password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE [User] SET [Password] = @newPassword WHERE [Username] = @username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@username", username));
                command.Parameters.Add(new SqlParameter("@newPassword", BCrypt.Net.BCrypt.HashPassword(newPassword)));

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                {
                    throw new Exception("More than one user have been affected.");
                }
            }
        }

        /// <summary>
        /// Changes the Username of the current user if the validation of the password and the given new instance of the Username is available.
        /// And removes the old coockey and adds a new one.
        /// </summary>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        /// <param name="newEmail">The new Username.</param>
        /// <param name="password">The password given from the user.</param>
        public void ChangeUsername(string tokenString, string newUsername, string password)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;

            _authenticationService.CheckPassword(username, password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = "UPDATE [User] SET [Username] = @newUsername WHERE [Username] = @username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@username", username));
                command.Parameters.Add(new SqlParameter("@newUsername", newUsername));

                var rowsAffested = command.ExecuteNonQuery();
                if (rowsAffested != 1)
                {
                    throw new Exception("More than one user have been affected.");
                }
            }
        }

        /// <summary>
        /// Deletes and instance of Text from the database by given Id. Does not validate the owner of the Text.
        /// </summary>
        /// <param name="Id">The Is of the Text in the database.</param>
        public void DeleteText(Guid Id)
        {
            var queryDeleteTextFromText = @"DELETE FROM [Text] WHERE [Id]=@id;";
            var queryDeleteTextFromUserText = @"DELETE FROM [User_Text] WHERE [Id_Text]=@id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@id", Id);

                var rowsChanged = connection.Execute(queryDeleteTextFromUserText, dynamicParameters);

                if (rowsChanged != 1)
                {
                    throw new Exception("More than one texts or none have been deleted.");
                }

                rowsChanged = connection.Execute(queryDeleteTextFromText, dynamicParameters);

                if (rowsChanged != 1)
                {
                    throw new Exception("More than one texts or none have been deleted.");
                }
            }
        }

        /// <summary>
        /// Gets the user info from the database.
        /// </summary>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        /// <returns>Returns an instance of UserToView, which contains all data for the user available to him.</returns>
        public UserToView GetUserInfo(string tokenString)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;

            var user = _authenticationService.GetUser(username);

            return new UserToView
            {
                Username = user.Username,
                Email = user.Email,
                CreationDate = user.CreationDate,
                Name = user.Name
            };
        }

        /// <summary>
        /// Gets all text in the database belonging to the current user.
        /// </summary>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        /// <returns>Returns an instance of TextsForView with all the texts.</returns>
        public TextsForView GetUsersTexts(string tokenString)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;

            var texts = new List<TextForView>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var queryGetUsersTexts = @"select [Text].[Id], [Text].[Text], [Text],[Title]
                    from[Text]
                    join[User_Text]
                    on[User_Text].[Id_Text] = [Text].Id
                    join[User]
                    on[User_Text].[Id_User] = [User].Id
                    where[User].Username = @username";

                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@username", username);

                texts = connection.Query<TextForView>(queryGetUsersTexts, dynamicParameters).ToList();
            }

            return new TextsForView
            {
                Texts = texts
            };
        }

        /// <summary>
        /// Saves the given instance of TextForView in the database by creating an instance there. 
        /// </summary>
        /// <param name="textForView">The instance of TextForView.</param>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        public void SaveText(TextForView textForView, string tokenString)
        {
            ValidateToken(tokenString, _privateKey, out JwtSecurityToken token);
            var username = token.Claims.First(claim => claim.Type == "unique_name").Value;

            if (!IsTitleUnique(textForView.Title))
            {
                throw new InvalidOperationException("Title already exists.");
            }

            var queryInsertText = @"Insert into [Text] values (@id, @text, GETDATE(), GETDATE(), @title)";
            var queryInsertUserText = @"Insert into [User_Text] values (@userId, @textId)";
            textForView.Id = Guid.NewGuid();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();


                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@text", textForView.Text);
                dynamicParameters.Add("@title", textForView.Title);
                dynamicParameters.Add("@id", textForView.Id);

                var rowsChanged = connection.Execute(queryInsertText, dynamicParameters);

                if (rowsChanged != 1)
                {
                    throw new Exception("Text has not been inserted.");
                }

                var userId = _authenticationService.GetUser(username).Id;

                dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@userId", userId);
                dynamicParameters.Add("@textId", textForView.Id);

                rowsChanged = connection.Execute(queryInsertUserText, dynamicParameters);

                if (rowsChanged != 1)
                {
                    throw new Exception("Cannot insert into User_Text.");
                }
            }
        }

        /// <summary>
        /// Saves the given instance of TextForView in the database by updating an existing instance there. 
        /// </summary>
        /// <param name="textForView">The instance of TextForView.</param>
        /// <param name="tokenString">The token string containing the username of the current user.</param>
        public void UpdateText(TextForView textForView)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var queryUpdateText = @"update [Text] set [Text] = @text, [Title] = @title, [LastVisited] = GETDATE() where [Id] = @id";

                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@text", textForView.Text);
                dynamicParameters.Add("@title", textForView.Title);
                dynamicParameters.Add("@id", textForView.Id);

                var rowsChanged = connection.Execute(queryUpdateText, dynamicParameters);

                if (rowsChanged != 1)
                {
                    throw new Exception("More than one texts or none have been changed.");
                }
            }
        }

        private static TokenValidationParameters GetValidationParameters(string key)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }

        private bool IsTitleUnique(string title)
        {
            var querySelectWithTitleText = @"select * from [Text] where [Title] = @title";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@title", title);

                var rowsChanged = connection.Query<TextForView>(querySelectWithTitleText, dynamicParameters).ToList();

                if (rowsChanged == null || rowsChanged.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static bool ValidateToken(string authToken, string key, out JwtSecurityToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(key);

            try
            {
                tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken resultToken);
                token = (JwtSecurityToken)resultToken;
                return true;
            }
            catch
            {
                token = null;
                return false;
            }
        }
    }
}
