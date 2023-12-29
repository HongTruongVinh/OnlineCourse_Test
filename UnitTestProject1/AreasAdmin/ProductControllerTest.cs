using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class ProductControllerTest
    {
        ProductController ctrl;

        Mock<IProductDao> _productDao;
        Mock<IProductCategoryDao> _productCategoryDao;

        public ProductControllerTest() 
        {
            ctrl = new ProductController();

            _productDao = new Mock<IProductDao>();
            _productCategoryDao = new Mock<IProductCategoryDao>();

            ctrl._productDao = _productDao.Object;
            ctrl._productCategoryDao = _productCategoryDao.Object;
        }

        [TestMethod]
        public void Index()
        {
            // this function checks the return view index
            _productDao.Setup(x => x.ListAllPaging(It.IsAny<long>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Product>());


            ViewResult v = ctrl.Index(null, "Toeic") as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Delete()
        {
            _productDao.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            RedirectToRouteResult result = ctrl.Delete(It.IsAny<int>()) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void AddProductAjax_Successfull()
        {
            _productDao.Setup(x => x.Insert(It.IsAny<Product>())).Returns(1);

            JsonResult result = ctrl.AddProductAjax(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                ) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddProductAjax_Failure()
        {
            _productDao.Setup(x => x.Insert(It.IsAny<Product>())).Returns(0);

            JsonResult result = ctrl.AddProductAjax(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                ) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void UpdateProductAjax_Successfull()
        {
            _productDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new Product() { ID = 1});
            _productDao.Setup(x => x.Update(It.IsAny<Product>())).Returns(true);

            JsonResult result = ctrl.UpdateProductAjax(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), "Mo ta khoa hoc",
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                ) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void UpdateProductAjax_Failure()
        {
            _productDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new Product());
            _productDao.Setup(x => x.Update(It.IsAny<Product>())).Returns(false);

            JsonResult result = ctrl.UpdateProductAjax(It.IsAny<int>(),  It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), "Mo ta khoa hoc",
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                ) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }
    }
}
