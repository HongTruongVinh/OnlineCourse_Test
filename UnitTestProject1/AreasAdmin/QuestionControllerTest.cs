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
    public class QuestionControllerTest
    {
        QuestionController ctrl;

        Mock<IProductDao> _productDao;
        Mock<IQuestionDao> _questionDao;

        public QuestionControllerTest() 
        {
            ctrl = new QuestionController();
            _productDao = new Mock<IProductDao>();
            _questionDao = new Mock<IQuestionDao>();

            ctrl._questionDao = _questionDao.Object;
            ctrl._productDao = _productDao.Object;
        }


        [TestMethod]
        public void Index()
        {
            _productDao.Setup(x => x.ListAllProduct()).Returns(new List<Product>());
            _questionDao.Setup(x => x.ListAllPaging(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Question>());

            ViewResult v = ctrl.Index("Toeic 700") as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Delete()
        {
            _questionDao.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            RedirectToRouteResult result = ctrl.Delete(It.IsAny<int>()) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void AddQuestionAjax_Successfull()
        {
            _questionDao.Setup(x => x.Insert(It.IsAny<Question>())).Returns(1);

            JsonResult result = ctrl.AddQuestionAjax(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>()
                ) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddQuestionAjax_Failure()
        {
            _questionDao.Setup(x => x.Insert(It.IsAny<Question>())).Returns(0);

            JsonResult result = ctrl.AddQuestionAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                ) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void UpdateQuestionAjax_Successfull()
        {
            _questionDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Question() { ID = 1 });
            _questionDao.Setup(x => x.Update(It.IsAny<Question>())).Returns(true);

            JsonResult result = ctrl.UpdateQuestionAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void UpdateQuestionAjax_Failure()
        {
            _questionDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Question() { ID = 1 });
            _questionDao.Setup(x => x.Update(It.IsAny<Question>())).Returns(false);

            JsonResult result = ctrl.UpdateQuestionAjax(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }
    }
}
