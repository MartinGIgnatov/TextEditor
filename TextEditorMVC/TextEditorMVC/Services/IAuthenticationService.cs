using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.CustomClasses;
using TextEditorMVC.Models;

namespace TextEditorMVC.Services
{
    public interface IAuthenticationService
    {
        void CheckPassword(string username, string password);

        User GetUser(string username);

        string Login(UserForLogin userForLogin);

        void Register(UserForRegistration userForRegistration);

        string TokenString(UserForLogin userForLogin);
        
    }
}

