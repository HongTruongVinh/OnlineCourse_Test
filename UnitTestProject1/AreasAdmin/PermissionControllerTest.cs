using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class PermissionControllerTest
    {
        PermissionController ctrl;
        Mock<IRoleDao> _roleDao;

        public PermissionControllerTest() 
        {
            ctrl = new PermissionController();
            _roleDao = new Mock<IRoleDao>();

            ctrl._roleDao = _roleDao.Object;

        }

        [TestMethod]
        public void Index()
        {
            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void ViewPermissionDetail()
        {
            _roleDao.Setup(x => x.GetListPer_IHave(It.IsAny<int>())).Returns(new System.Collections.Generic.Dictionary<string, string>());
            _roleDao.Setup(x => x.GetListPer_IHaveNo(new System.Collections.Generic.Dictionary<string, string>())).Returns(new System.Collections.Generic.Dictionary<string, string>());

            JsonResult jsonResult = ctrl.ViewPermissionDetail(It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ listPer_IHave = System.Collections.Generic.Dictionary`2[System.String,System.String], listPer_IHaveNo = System.Collections.Generic.Dictionary`2[System.String,System.String] }", respone);
        }

        [TestMethod]
        public void UpdatePermissionDetail()
        {
            _roleDao.Setup(x => x.UpdateMyPermission(It.IsAny<int>(), It.IsAny<List<string>>())).Returns(true);

            JsonResult jsonResult = ctrl.UpdatePermissionDetail(It.IsAny<int>(), It.IsAny<List<string>>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ result = True }", respone);
        }

    }
}
