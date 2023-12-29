using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class UserControllerTest
    {
        UserController ctrl;

        Mock<IUserDao> _userDao;
        Mock<IRoleDao> _roleDao;

        public UserControllerTest() 
        {
            ctrl = new UserController();
            _userDao = new Mock<IUserDao>();
            _roleDao = new Mock<IRoleDao>();

            ctrl._roleDao = _roleDao.Object;
            ctrl._userDao = _userDao.Object;
        }

        [TestMethod]
        public void Index()
        {
            _userDao.Setup(x => x.ListAllPaging(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<User>());

            ViewResult v = ctrl.Index("Vinh") as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void AddUserAjax_Successfull()
        {
            _userDao.Setup(x => x.Insert(It.IsAny<User>())).Returns(1);

            JsonResult result = ctrl.AddUserAjax(It.IsAny<string>(),
                It.IsAny<string>(), "password", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddUserAjax_Failure()
        {
            _userDao.Setup(x => x.Insert(It.IsAny<User>())).Returns(0);

            JsonResult result = ctrl.AddUserAjax(It.IsAny<string>(),
                It.IsAny<string>(), "password", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }


        [TestMethod]
        public void UpdateUser_Successfull()
        {
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new User() { ID = 1 });
            _userDao.Setup(x => x.Update(It.IsAny<User>())).Returns(true);

            JsonResult result = ctrl.UpdateUser(new User() { ID = 1}) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void UpdateUser_Failure()
        {
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new User());
            _userDao.Setup(x => x.Update(It.IsAny<User>())).Returns(false);

            JsonResult result = ctrl.UpdateUser(new User() { ID = 1 }) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void UpdateUser_PartialView()
        {
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new User() { ID = 1 });
            _roleDao.Setup(x => x.GetRoleUser(It.IsAny<int>())).Returns("Admin");

            PartialViewResult result = ctrl.UpdateUser(It.IsAny<int>()) as PartialViewResult;

            Assert.AreEqual<string>("_UpdateUserPartial", result.ViewName);
        }

        [TestMethod]
        public void Delete()
        {
            _userDao.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            RedirectToRouteResult result = ctrl.Delete(new User() { ID = 1 }) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void Delete_PartialView()
        {
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new User());

            PartialViewResult result = ctrl.Delete(It.IsAny<int>()) as PartialViewResult;

            Assert.AreEqual<string>("_ConfirmDeleteModelPartial", result.ViewName);
        }
    }
}
