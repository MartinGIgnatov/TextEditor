using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TextEditorMVC.Models;
using TextEditorMVC.Services;

namespace TextEditorMVC.Controllers
{
    [Authorize]
    public class FunctionalityController : Controller
    {

        private TextEditorMVC.Services.IAuthenticationService _authenticationService;
        private TextEditorMVC.Services.IUserFunctionality _userFunctionality;
        private TextEditorMVC.Services.IWordRecognition _wordRecognition;
        private ITextRecognition _textRecognition;

        public FunctionalityController(IUserFunctionality userFunctionality, TextEditorMVC.Services.IAuthenticationService authenticationService,
            TextEditorMVC.Services.IWordRecognition wordRecognition, ITextRecognition textRecognition)
        {
            _authenticationService = authenticationService;
            _userFunctionality = userFunctionality;
            _wordRecognition = wordRecognition;
            _textRecognition = textRecognition;
        }

        /// <summary>
        /// Changes email.
        /// </summary>
        /// <param name="user">The parameter needed for the change.</param>
        /// <returns>Redirects to MyProfile.</returns>
        public IActionResult ChangeEmail(ChangeForEmail user)
        {
            var a = HttpContext.User.Identity.Name;

            _userFunctionality.ChangeEmail(a, user.NewEmail, user.Password);

            return RedirectToAction("MyProfile");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the form for Email change.</returns>
        public IActionResult ChangeEmailForm()
        {
            return View();
        }

        /// <summary>
        /// Changes Name.
        /// </summary>
        /// <param name="user">The parameter needed for the change.</param>
        /// <returns>Redirects to MyProfile.</returns>
        public IActionResult ChangeName(ChangeForName user)
        {
            var a = HttpContext.User.Identity.Name;

            _userFunctionality.ChangeName(a, user.NewName, user.Password);

            return RedirectToAction("MyProfile");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the form for Name change.</returns>
        public IActionResult ChangeNameForm()
        {
            return View();
        }

        /// <summary>
        /// Changes Password.
        /// </summary>
        /// <param name="user">The parameter needed for the change.</param>
        /// <returns>Redirects to MyProfile.</returns>
        public IActionResult ChangePassword(ChangeForPassword user)
        {
            var a = HttpContext.User.Identity.Name;

            _userFunctionality.ChangePassword(a, user.NewPassword, user.Password);

            return RedirectToAction("MyProfile");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the form for Password change.</returns>
        public IActionResult ChangePasswordForm()
        {
            return View();
        }

        /// <summary>
        /// Changes Username.
        /// </summary>
        /// <param name="user">The parameter needed for the change.</param>
        /// <returns>Redirects to MyProfile.</returns>
        public async Task<IActionResult> ChangeUsername(ChangeForUsername user)
        {
            var a = HttpContext.User.Identity.Name;

            _userFunctionality.ChangeUsername(a, user.NewUsername, user.Password);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var result = _authenticationService.TokenString(new UserForLogin { Username = user.NewUsername });

            ClaimsIdentity identity = new ClaimsIdentity(new[]
            {
                   new Claim(ClaimTypes.Name, result),
                }, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("MyProfile");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the form for Username change.</returns>
        public IActionResult ChangeUsernameForm()
        {
            return View();
        }

        /// <summary>
        /// Creates an instance of TextToView, with it changes the text in the database by replacing it with the given proposition.
        /// </summary>
        /// <param name="id">The id of the current text.</param>
        /// <param name="title">The title of the current text.</param>
        /// <param name="text">The body of the current text.</param>
        /// <param name="erorWordIndex">The index of the Error desired to be changed.</param>
        /// <param name="propositionIndex">The index of the proposition for the appropriate Error.</param>
        /// <returns>Redirects to RedactText for the current TextToView.</returns>
        public IActionResult ChangeWord(Guid id, string title, string text, int erorWordIndex, int propositionIndex)
        {
            ;
            var textForView = new TextForView
            {
                Id = id,
                Title = title,
                Text = text
            };

            textForView = _textRecognition.ChangeWord(textForView, erorWordIndex, propositionIndex);

            return RedirectToAction("SaveAndRedact", textForView);
        }

        /// <summary>
        /// Deletes the chosen instance of TextForView from the Database.
        /// </summary>
        /// <param name="id">The Id of the instance of TextForView.</param>
        /// <returns>Redirects to MyTexts.</returns>
        public IActionResult DeleteText(Guid id)
        {
            _userFunctionality.DeleteText(id);
            return RedirectToAction("MyTexts");
        }

        /// <summary>
        /// If a word is given it searches for propositions.
        /// </summary>
        /// <param name="dictionaryForView">The model, in which you give the word for search.</param>
        /// <returns>Sends to view and displays the propositions if they are given.</returns>
        public IActionResult Dictionary(DictionaryForView dictionaryForView)
        {

            if (dictionaryForView != null && dictionaryForView.SearchWord != null)
            {
                dictionaryForView.wordPropositions = _wordRecognition.FindWordPropositions(dictionaryForView.SearchWord, 10);
            }
            else
            {
                dictionaryForView = new DictionaryForView();
            }

            return View(dictionaryForView);
        }

        /// <summary>
        /// Creates an instance of TextForView and gives it to the view.
        /// </summary>
        /// <param name="id">The Id of the current Text</param>
        /// <param name="title">The Title of the current Text</param>
        /// <param name="text">The Body of the current Text</param>
        /// <returns>Returns the View where the text is edited by the user.</returns>
        public IActionResult EditText(Guid id, string title, string text)
        {
            var textForView = new TextForView
            {
                Id = id,
                Title = title,
                Text = text
            };
            ;
            return View(textForView);
        }

        public IActionResult Message(Message message)
        {
            return View(message);
        }

        /// <summary>
        /// Gets the username from the claims of the current user.
        /// </summary>
        /// <returns>Returns the view with the current user's data.</returns>
        public IActionResult MyProfile()
        {
            var a = HttpContext.User.Identity.Name;

            return View(_userFunctionality.GetUserInfo(a));
        }

        /// <summary>
        /// Gets the username from the claims of the current user and fets all the texts from the database of the current user.
        /// </summary>
        /// <returns>Returns the view with all the texts.</returns>
        public IActionResult MyTexts()
        {
            var token = HttpContext.User.Identity.Name;
            var textsForView = _userFunctionality.GetUsersTexts(token);
            return View(textsForView);
        }

        /// <summary>
        /// Creates an instance of TextForView and gives it to the view.
        /// </summary>
        /// <param name="id">The Id of the current Text</param>
        /// <param name="title">The Title of the current Text</param>
        /// <param name="text">The Body of the current Text</param>
        /// <returns>Returns the View where the text is redacted and the user can confirm the changes.</returns>
        public IActionResult RedactText(Guid id,string title, string text)
        {
            ;
            var textForView = new TextForView
            {
                Id = id,
                Title = title,
                Text = text
            };

            var a = _textRecognition.Process(textForView);

            return View(a);
        }

        /// <summary>
        /// If the given model has Id updates the instance of the model in the database, if not creates new instance of Text.
        /// </summary>
        /// <param name="textForView">The Model requred to make a save.</param>
        /// <returns>Redirects to the RedactText Action or the Message if there is an error.</returns>
        public IActionResult SaveAndRedact(TextForView textForView)
        {
            ;
            try
            {
                var tokenString = HttpContext.User.Identity.Name;
                if (textForView.Id == Guid.Empty)
                {
                    _userFunctionality.SaveText(textForView, tokenString);
                }
                else
                {
                    _userFunctionality.UpdateText(textForView);
                }

                return RedirectToAction("RedactText", new { id = textForView.Id, title = textForView.Title, text = textForView.Text });
            }
            catch(InvalidOperationException ex)
            {
                return RedirectToAction("Message", new Message { Text = ex.Message });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Message", new Message { Text = ex.Message });
            }
        }

    }
}