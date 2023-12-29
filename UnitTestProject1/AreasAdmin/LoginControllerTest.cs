using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using OnlineCourse.Areas.Admin.Models;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class LoginControllerTest
    {
        LoginController ctrl;
        UserLogin userLogin;
        LoginModel loginModel;
        User user;

        Mock<IUserDao> userDao;
        Mock<IUserLoginManager> _userLoginManager;

        public LoginControllerTest() 
        {
            ctrl = new LoginController();
            
            loginModel = new LoginModel() { UserName = "vinh", Password = "1" };
            user = new User() { UserName = loginModel.UserName, Password = loginModel.Password, Name = "vinh", LinkImage = "UserAvatar.png" };

            userDao = new Mock<IUserDao>();
            userDao.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);

            _userLoginManager = new Mock<IUserLoginManager>();
            _userLoginManager.Setup(x => x.AddUserLogin(It.IsAny<UserLogin>()));
            _userLoginManager.Setup(x => x.GetUserLogin()).Returns(userLogin);
            _userLoginManager.Setup(x => x.RemoveUserLogin());


            ctrl._userLoginManager = _userLoginManager.Object;
            ctrl._userDao = userDao.Object;

        }

        [TestMethod]
        public void TestShowViewLogin()
        {
            // this function checks the return view index

            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void TestLoginSuccessful()
        {
            userDao.Setup(x=>x.isAdminRole(It.IsAny<int>())).Returns(true);
            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(1);

            RedirectToRouteResult result = ctrl.Login(loginModel) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void TestBannedAcount()
        {
            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(-1);

            var actionResult = ctrl.Login(loginModel);

            var modelState = ctrl.ModelState.Where(x => x.Key == "").SingleOrDefault();

            string msg = modelState.Value.Errors[0].ErrorMessage.ToString();

            Assert.AreEqual<string>("Tài khoản đang bị khóa", msg);
        }

        [TestMethod]
        public void TestWrongPassword()
        {
            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(-2);

            var actionResult = ctrl.Login(loginModel);

            var modelState = ctrl.ModelState.Where(x => x.Key == "").SingleOrDefault();

            string msg = modelState.Value.Errors[0].ErrorMessage.ToString();

            Assert.AreEqual<string>("Mật khẩu không đúng", msg);
        }

        [TestMethod]
        public void TestAccountDoesntExist()
        {
            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(0);

            var actionResult = ctrl.Login(loginModel);

            var modelState = ctrl.ModelState.Where(x => x.Key == "").SingleOrDefault();

            string msg = modelState.Value.Errors[0].ErrorMessage.ToString();

            Assert.AreEqual<string>("Tài khoản không tồn tại", msg);
        }

        [TestMethod]
        public void TestAccountNoPermit()
        {
            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(-3);

            var actionResult = ctrl.Login(loginModel);

            var modelState = ctrl.ModelState.Where(x => x.Key == "").SingleOrDefault();

            string msg = modelState.Value.Errors[0].ErrorMessage.ToString();

            Assert.AreEqual<string>("Tài khoản không có quyền đăng nhập", msg);
        }

        [TestMethod]
        public void TestAccountNoPermitAtAdmin()
        {
            userDao.Setup(x => x.isAdminRole(It.IsAny<int>())).Returns(false);
            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(1);

            var actionResult = ctrl.Login(loginModel);

            var modelState = ctrl.ModelState.Where(x => x.Key == "").SingleOrDefault();

            string msg = modelState.Value.Errors[0].ErrorMessage.ToString();

            Assert.AreEqual<string>("Tài khoản không có quyền đăng nhập", msg);
        }
    }
}
