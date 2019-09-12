using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using TextEditorMVC.Controllers;
using TextEditorMVC.Services;
using TextEditorMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Threading;
using System.Security.Claims;

namespace TextEditorTests
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        [TestMethod]
        public void Register_Is_Null_ThrowException()
        {
            var serviceMock = new Mock<IAuthenticationService>();
            //serviceMock.Setup(x => x.Register(null)).Throws<ArgumentNullException>();

            var controller = new AuthenticationController(serviceMock.Object);
            var result = controller.Register(null);
            var viewResult = (ViewResult)result;

            Assert.AreEqual("Message", viewResult.ViewName);

            var message = (Message)viewResult.Model;
            Assert.AreEqual("Unexpected error.", message.Text);
        }

        [TestMethod]
        public void Register_UsernameNotValid_ThrowException()
        {
            var serviceMock = new Mock<IAuthenticationService>();
            var userForRegistration = new UserForRegistration();

            serviceMock.Setup(x => x.Register(userForRegistration))
            .Throws(new InvalidOperationException("Not allowed registration. Username already exist."));

            var controller = new AuthenticationController(serviceMock.Object);

            var result = controller.Register(userForRegistration);
            var viewResult = (ViewResult)result;

            Assert.AreEqual("Message", viewResult.ViewName);

            var model = (Message)viewResult.Model;
            Assert.AreEqual("Not allowed registration. Username already exist.", model.Text);
        }

        [TestMethod]
        public void Register_EmailNotExisting_ThrowException()
        {
            var serviceMock = new Mock<IAuthenticationService>();
            var userForRegistration = new UserForRegistration();

            serviceMock.Setup(x => x.Register(userForRegistration))
            .Throws(new InvalidOperationException("Not allowed registration. Email is already used."));

            var controller = new AuthenticationController(serviceMock.Object);

            var result = controller.Register(userForRegistration);
            var viewResult = (ViewResult)result;

            Assert.AreEqual("Message", viewResult.ViewName);

            var model = (Message)viewResult.Model;
            Assert.AreEqual("Not allowed registration. Email is already used.", model.Text);
        }

        [TestMethod]
        public void Register_EmailNotValid_ThrowException()
        {
            var serviceMock = new Mock<IAuthenticationService>();
            var userForRegistration = new UserForRegistration();

            serviceMock.Setup(x => x.Register(userForRegistration))
            .Throws(new InvalidOperationException("Not allowed registration. Email is not valid."));

            var controller = new AuthenticationController(serviceMock.Object);

            var result = controller.Register(userForRegistration);
            var viewResult = (ViewResult)result;

            Assert.AreEqual("Message", viewResult.ViewName);

            var model = (Message)viewResult.Model;
            Assert.AreEqual("Not allowed registration. Email is not valid.", model.Text);
        }

        [TestMethod]
        public void Register_PasswordAndRepeatedPasswordNotSame_ThrowException()
        {
            var serviceMock = new Mock<IAuthenticationService>();
            var userForRegistration = new UserForRegistration();

            serviceMock.Setup(x => x.Register(userForRegistration))
            .Throws(new InvalidOperationException("Not allowed registration. Password and repeated password are not the same."));

            var controller = new AuthenticationController(serviceMock.Object);

            var result = controller.Register(userForRegistration);
            var viewResult = (ViewResult)result;

            Assert.AreEqual("Message", viewResult.ViewName);

            var model = (Message)viewResult.Model;
            Assert.AreEqual("Not allowed registration. Password and repeated password are not the same.", model.Text);
        }

        //[TestMethod]
        //public void ChangeForPassword_Success_Test()
        //{
        //    var firstServiceMock = new Mock<IUserFunctionality>();
        //    var secondServiceMock = new Mock<IAuthenticationService>();
        //    var thirdServiceMock = new Mock<IWordRecognition>();
        //    var fourthServiceMock = new Mock<ITextRecognition>();
        //    var changeForPassword = new ChangeForPassword();

        //    firstServiceMock.Setup(x => x.ChangePassword("", "", ""))
        //    .Throws(new InvalidOperationException("Not allowed change. Invalid password."));

        //    var controller = new FunctionalityController(firstServiceMock.Object, secondServiceMock.Object, thirdServiceMock.Object, fourthServiceMock.Object);

        //    var cp = new Mock<ClaimsPrincipal>();
        //    cp.Setup(m => m.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        //    var contextMock = new Mock<ControllerContext>();
        //    contextMock.Setup(ctx => ctx.HttpContext.User).Returns(cp.Object);

        //    var result = controller.ChangePassword(changeForPassword);
        //    var viewResult = (ViewResult)result;

        //    Assert.AreEqual("Message", viewResult.ViewName);

        //    var model = (Message)viewResult.Model;
        //    Assert.AreEqual("Not allowed registration. Invalid password.", model.Text);
        //}


    }
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims) : base(new TestIdentity(claims))
        {
        }
    }

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims)
        {
        }
    }

}
