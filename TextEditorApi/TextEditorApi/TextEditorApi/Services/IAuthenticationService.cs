using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorApi.Models;

namespace TextEditorApi.Services
{
    public interface IAuthenticationService
    {
        void Register(UserForRegistration userForRegistration);
        string LogIn(UserForLogIn userForLogIn);
    }
}
