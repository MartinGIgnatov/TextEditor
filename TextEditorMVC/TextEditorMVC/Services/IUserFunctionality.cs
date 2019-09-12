using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.Models;

namespace TextEditorMVC.Services
{
    public interface IUserFunctionality
    {
        void ChangeEmail(string email, string newEmail, string password);

        void ChangeName(string name, string newName, string password);

        void ChangePassword(string name, string newPassword, string password);

        void ChangeUsername(string username, string newUsername, string password);

        void DeleteText(Guid Id);

        UserToView GetUserInfo(string tokenString);

        TextsForView GetUsersTexts(string tokenString);

        void SaveText(TextForView textForView, string tokenString);

        void UpdateText(TextForView textForView);

    }
}
