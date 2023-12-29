using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnlineCourse.Controllers;
using OnlineCourse.Common;
using OnlineCourse.Models;
using System;
using System.Web.Mvc;
using Moq;
using Model.Dao;
using Model.Models;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace UnitTestProject1
{
    [TestClass]
    public class UserControllerTest
    {
        UserLogin userLogin;
        UserController ctrl;
        LoginModel loginModel;
        User user;

        Mock<IUserDao> userDao;
        Mock<IUserLoginManager> _userLoginManager;
        Mock<IProductDao> _productDao;

        Dictionary<string, bool> wishListProduct;

        public UserControllerTest() 
        {
            ctrl = new UserController();
            ctrl.isUnitTest = true;
            loginModel = new LoginModel() { UserName = "vinh", Password = "1" };
            user = new User() { UserName = loginModel.UserName, Password = loginModel.Password, Name = "vinh", LinkImage = "UserAvatar.png" };

            userDao = new Mock<IUserDao>();
            userDao.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);
            ctrl._userDao = userDao.Object;

            _userLoginManager = new Mock<IUserLoginManager>();
            _userLoginManager.Setup(x => x.AddUserLogin(It.IsAny<UserLogin>()));
            _userLoginManager.Setup(x => x.GetUserLogin()).Returns(userLogin);
            _userLoginManager.Setup(x => x.RemoveUserLogin());
            ctrl._userLoginManager = _userLoginManager.Object;

            _productDao = new Mock<IProductDao>();
            wishListProduct = new Dictionary<string, bool>();
            wishListProduct.Add("productId1", true);
            wishListProduct.Add("productId2", false);
            _productDao.Setup(x => x.GetWishListProduct(It.IsAny<int>())).Returns(wishListProduct);
            ctrl._productDao = _productDao.Object;
        }

        [TestMethod]
        public void TestShowViewLogin()
        {
            // this function checks the return view index

            Mock<IGetInforDao> _getInforDao = new Mock<IGetInforDao>();
            HomeInfor homeInfor = new HomeInfor() { CountStudent = 50, CountTeacher = 10, CountProduct = 30, CountCertification = 5 };
            _getInforDao.Setup(x => x.GetHomeInfor()).Returns(homeInfor);
            ctrl._getInforDao = _getInforDao.Object;

            ViewResult v = ctrl.Login() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);

        }

        [TestMethod]
        public void TestLoginSuccessful()
        {
            // this function checks the user has an avatar 

            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(1);

            RedirectToRouteResult result = ctrl.Login(loginModel) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
            Assert.IsTrue(result.RouteValues.ContainsValue("Home"));
        }

        [TestMethod]
        public void TestLoginSuccessfulWithUserAvatarless()
        {
            // this function checks the user has no avatar 
            var user = new User() { UserName = loginModel.UserName, Password = loginModel.Password, Name = "vinh", LinkImage = null };

            userDao.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(user);

            userDao.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(1);

            ViewResult v = ctrl.Login(loginModel) as ViewResult;

            Assert.AreEqual<object>(null, v);
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

            Assert.AreEqual<string>("Tài khoản không tồn tai", msg);
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
        public void ProfileUser()
        {
            ViewResult v = ctrl.ProfileUser() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);

        }

        [TestMethod]
        public void LogOut()
        {
            RedirectToRouteResult result = ctrl.LogUot() as RedirectToRouteResult;

            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
            Assert.IsTrue(result.RouteValues.ContainsValue("Home"));
        }

        [TestMethod]
        public void SetUserSession_NoAvatarUser()
        {
            user.LinkImage = null;
            var result = ctrl.SetUserSession(user);

            Assert.AreEqual<string>("/assets/client/images/avatar/00.jpg", result.Image);
            Assert.AreEqual<string>("Học viên", result.Role);
        }

        [TestMethod]
        public void SetUserSession_AvatarUser()
        {
            user.LinkImage = "userAvatarLink.jpg";
            var result = ctrl.SetUserSession(user);

            Assert.AreEqual<string>("userAvatarLink.jpg", result.Image);
            Assert.AreEqual<string>("Học viên", result.Role);
        }

        [TestMethod]
        public void Constructer_Test()
        {
            ctrl = new UserController();
            Assert.IsNotNull(ctrl._productDao);
            Assert.IsNotNull(ctrl._userDao);
            Assert.IsNotNull(ctrl._userLoginManager);
            Assert.IsNotNull(ctrl._getInforDao);

            Assert.IsFalse(ctrl.isUnitTest);
        }


    }
}
