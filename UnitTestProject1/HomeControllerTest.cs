using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Moq;
using OnlineCourse.Controllers;
using System;
using System.Web.Mvc;

namespace UnitTestProject1
{
    [TestClass]
    public class HomeControllerTest
    {
        HomeController ctrl;

        public Mock<IProductCategoryDao> _productCategoryDao;
        public Mock<IProductDao> _productDao;
        public Mock<IExamDao> _examDao;
        public Mock<IGetInforDao> _getInforDao;

        public HomeControllerTest() 
        {
            ctrl = new HomeController();

            _productCategoryDao = new Mock<IProductCategoryDao>();
            _productDao = new Mock<IProductDao>();
            _examDao = new Mock<IExamDao>();
            _getInforDao = new Mock<IGetInforDao>();

            ctrl._productCategoryDao = _productCategoryDao.Object;
            ctrl._productDao = _productDao.Object;
            ctrl._examDao = _examDao.Object;
            ctrl._getInforDao = _getInforDao.Object;
        }

        [TestMethod]
        public void Index()
        {
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new System.Collections.Generic.List<Model.Models.ProductCategory>());
            _productDao.Setup(x=>x.ListAllProduct()).Returns(new System.Collections.Generic.List<Model.Models.Product>());
            _examDao.Setup(x => x.ListAllExam()).Returns(new System.Collections.Generic.List<Model.Models.Exam>());
            _getInforDao.Setup(x => x.GetHomeInfor()).Returns(new HomeInfor());

            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

    }
}
