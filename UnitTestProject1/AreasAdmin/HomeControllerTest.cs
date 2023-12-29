using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Models;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using OnlineCourse.Common;
using System;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class HomeControllerTest
    {
        HomeController ctrl;
        Mock<IUserLoginManager> _userLoginManager;
        User user;
        UserLogin userLogin;

        public HomeControllerTest() 
        {
            ctrl = new HomeController();

            user = new User()
            {
                ID = 1,
                UserName = "vinh",
                Password = "1",
                Name = "vinh",
                LinkImage = "UserAvatar.png",

            };
            userLogin = new UserLogin()
            {
                UserID = user.ID,
                FullName = user.Name,
                UserName = user.UserName,
                Password = user.Password,
                WishListIdProduct = new System.Collections.Generic.Dictionary<string, bool>(),
            };

            _userLoginManager = new Mock<IUserLoginManager>();
            ctrl._userLoginManager = _userLoginManager.Object;
        }

        [TestMethod]
        public void Index()
        {
            // this function checks the return view index

            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void LogUot()
        {
            _userLoginManager.Setup(x => x.RemoveUserLogin());
            ctrl.isUnitTest = true;

            RedirectToRouteResult result = ctrl.LogUot() as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }
    }
}
