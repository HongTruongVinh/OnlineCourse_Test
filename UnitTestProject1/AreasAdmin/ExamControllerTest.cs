using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class ExamControllerTest
    {
        ExamController ctrl;

        Mock<IProductDao> _productDao;
        Mock<IExamDao> _examDao;

        public ExamControllerTest() 
        {
            ctrl = new ExamController();

            _examDao = new Mock<IExamDao>();
            _productDao = new Mock<IProductDao>();

            ctrl._examDao = _examDao.Object;
            ctrl._productDao = _productDao.Object;
        }

        [TestMethod]
        public void Index()
        {
            // this function checks the return view index

            _examDao.Setup(x => x.ListAllPaging("Thi Toeic", 2, 20)).Returns(new List<Exam>());
            _productDao.Setup(x=>x.ListAllProduct()).Returns(new List<Product>());

            ViewResult v = ctrl.Index("Thi Toeic") as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Delete()
        {
            _examDao.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            RedirectToRouteResult result = ctrl.Delete(It.IsAny<int>()) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void AddExamAjax_Seccessfull()
        {
            _examDao.Setup(x => x.Insert(It.IsAny<Exam>())).Returns(1);

            JsonResult result = ctrl.AddExamAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                  It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                   It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddExamAjax_Failure()
        {
            _examDao.Setup(x => x.Insert(It.IsAny<Exam>())).Returns(0);

            JsonResult result = ctrl.AddExamAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                  It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                   It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void UpdateExamAjax_Success() 
        {
            _examDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Exam());
            _examDao.Setup(x => x.Update(It.IsAny<Exam>())).Returns(true);

            JsonResult result = ctrl.UpdateExamAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                  It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                   It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void UpdateExamAjax_Failure()
        {
            _examDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Exam());
            _examDao.Setup(x => x.Update(It.IsAny<Exam>())).Returns(false);

            JsonResult result = ctrl.UpdateExamAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                 It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                  It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                   It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }
    }
}