using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Common;
using OnlineCourse.Controllers;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace UnitTestProject1
{
    [TestClass]
    public class ManagementCourseControllerTest
    {
        ManagementCourseController ctrl;

        User user;
        UserLogin userLogin;
        Dictionary<string, bool> wishListProduct;
        List<Product> listProducts;


        Mock<IUserLoginManager> _userLoginManager;
        Mock<IProductDao> _productDao;
        Mock<IProductCategoryDao> _productCategoryDao;
        Mock<IManagementCourseDao> _managementCourseDao;
        Mock<ICourseVideoDao> _courseVideoDao;
        Mock<ICourseDocumentDao> _courseDocumentDao;
        Mock<IFileManager> _fileManager;

        public ManagementCourseControllerTest() 
        {
            ctrl = new ManagementCourseController();

            user = new User()
            {
                ID = 1,
                UserName = "vinh",
                Password = "1",
                Name = "vinh",
                LinkImage = "UserAvatar.png",

            };

            listProducts = new List<Product>()
            {
                new Product()
                {
                    ID = 1,
                    CategoryID = 1,
                    Name = "Khoa hoc Toeic",
                    ListFile = "doc1.docx*doc2.docx"
                },
                new Product()
                {
                    ID = 2,
                    CategoryID = 2,
                    Name = "Khoa hoc ASP.Net",
                    ListFile = "doc3.docx*doc4.docx"
                }
            };

            wishListProduct = new Dictionary<string, bool>()
            {
                {"1", false},
                {"2", true},
            };

            userLogin = new UserLogin()
            {
                UserID = user.ID,
                FullName = user.Name,
                UserName = user.UserName,
                Password = user.Password,
                WishListIdProduct = wishListProduct,
            };

            _userLoginManager = new Mock<IUserLoginManager>();
            _productDao = new Mock<IProductDao>();
            _productCategoryDao = new Mock<IProductCategoryDao>();
            _managementCourseDao = new Mock<IManagementCourseDao>();
            _courseDocumentDao = new Mock<ICourseDocumentDao>();
            _courseVideoDao = new Mock<ICourseVideoDao>();
            _fileManager = new Mock<IFileManager>();

            ctrl._userLoginManager = _userLoginManager.Object;
            ctrl._productDao = _productDao.Object;
            ctrl._productCategoryDao = _productCategoryDao.Object;
            ctrl._managementCourseDao = _managementCourseDao.Object;
            ctrl._courseDocumentDao = _courseDocumentDao.Object;
            ctrl._courseVideoDao = _courseVideoDao.Object;
            ctrl._fileManager = _fileManager.Object;

            _userLoginManager.Setup(x => x.GetUserLogin()).Returns(userLogin);
            _managementCourseDao.Setup(x => x.GetProductOfUser(1)).Returns(listProducts);
            _productDao.Setup(x => x.GetCreatedByUser(It.IsAny<int>())).Returns(user);
            _productDao.Setup(x => x.GetCountLearner(1)).Returns(5);
            _productDao.Setup(x => x.GetCountComment(1)).Returns(3);

        }


        [TestMethod]
        public void Index()
        {
            // this function checks the return view index

            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void ManagementCourseDetail()
        {
            _courseDocumentDao.Setup(x => x.GetListDocumentInfor(1)).Returns(new List<CourseDocument>());
            _courseVideoDao.Setup(x=>x.GetListVideoInfor(1)).Returns(new List<CourseVideo>());

            ViewResult v = ctrl.ManagementCourseDetail(1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void ViewEditCourse()
        {
            _productCategoryDao.Setup(x=>x.ListAll()).Returns(new List<ProductCategory>());
            _productDao.Setup(x => x.ViewDetail(1)).Returns(new Product());

            ViewResult v = ctrl.ViewEditCourse(1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void ViewEditCourse_NoneProductId()
        {
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new List<ProductCategory>());
            _productDao.Setup(x => x.ViewDetail(1)).Returns(new Product());

            ViewResult v = ctrl.ViewEditCourse(-1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void ViewAddVieoToCourse()
        {
            _productDao.Setup(x => x.ViewDetail(1)).Returns(new Product());

            ViewResult v = ctrl.ViewAddVieoToCourse(1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void AddVieoToCourse()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);
            var file1Mock = new Mock<HttpPostedFileBase>();
            file1Mock.Setup(x => x.FileName).Returns("Video1.mp4");
            _fileManager.Setup(x => x.UploadVideo(file1Mock.Object)).Returns("Video1.mp4");

            _courseVideoDao.Setup(x => x.AddCourseVideo(new CourseVideo())).Returns(true);

            RedirectToRouteResult result = ctrl.AddVieoToCourse(new CourseVideo() { Title = "Video 1"}, file1Mock.Object) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("ManagementCourseDetail"));
        }

        [TestMethod]
        public void AddVieoToCourse_WithNoneVideo()
        {
            _courseVideoDao.Setup(x => x.AddCourseVideo(new CourseVideo())).Returns(true);

            RedirectToRouteResult result = ctrl.AddVieoToCourse(new CourseVideo(), null) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("ManagementCourseDetail"));
        }

        [TestMethod]
        public void ViewAddDocumentToCourse()
        {
            _productDao.Setup(x => x.ViewDetail(1)).Returns(new Product());

            ViewResult v = ctrl.ViewAddDocumentToCourse(1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void AddDocumentToCourse()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);
            var file1Mock = new Mock<HttpPostedFileBase>();
            file1Mock.Setup(x => x.FileName).Returns("Video1.mp4");
            _fileManager.Setup(x => x.UploadVideo(file1Mock.Object)).Returns("Video1.mp4");

            _courseDocumentDao.Setup(x => x.AddCourseDocument(new CourseDocument())).Returns(true);

            RedirectToRouteResult result = ctrl.AddDocumentToCourse(new CourseDocument() { Title = "File 1" }, file1Mock.Object) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("ManagementCourseDetail"));
        }

        [TestMethod]
        public void AddDocumentToCourse_WithNoneDocument()
        {
            _courseDocumentDao.Setup(x => x.AddCourseDocument(new CourseDocument())).Returns(true);

            RedirectToRouteResult result = ctrl.AddDocumentToCourse(new CourseDocument(), null) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("ManagementCourseDetail"));
        }

        [TestMethod]
        public void DeleteDocumentOfCourse()
        {
            _courseDocumentDao.Setup(x=>x.DeleteCourseDocument(1)).Returns(true);

            RedirectToRouteResult result = ctrl.DeleteDocumentOfCourse(1,1) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("ManagementCourseDetail"));
        }

        [TestMethod]
        public void DeleteVideoOfCourse()
        {
            _courseVideoDao.Setup(x => x.DeleteCourseVideo(1)).Returns(true);

            RedirectToRouteResult result = ctrl.DeleteVideoOfCourse(1, 1) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("ManagementCourseDetail"));
        }

        [TestMethod]
        public void AddCourse()
        {
            _productDao.Setup(x => x.Insert(new Product())).Returns(1);

            ViewResult result = ctrl.AddCourse(new Product()) as ViewResult;

            Assert.AreEqual<string>("", result.ViewName);
        }

        [TestMethod]
        public void UpdateCourse()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);
            var file1Mock = new Mock<HttpPostedFileBase>();
            file1Mock.Setup(x => x.FileName).Returns("Background.jpg");
            _fileManager.Setup(x => x.UploadImage(file1Mock.Object)).Returns("Background.jpg");

            _productDao.Setup(x => x.ViewDetail(1)).Returns(new Product());
            _productDao.Setup(x => x.Update(It.IsAny<Product>())).Returns(true);

            RedirectToRouteResult result = ctrl.UpdateCourse(new Product() { ID = 1}, file1Mock.Object) as RedirectToRouteResult;

            Assert.AreEqual<string>("", result.RouteName);
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void UpdateCourse_NoneProductId()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);
            var file1Mock = new Mock<HttpPostedFileBase>();
            //file1Mock.Setup(x => x.FileName).Returns(null);
            _fileManager.Setup(x => x.UploadImage(file1Mock.Object)).Returns("Background.jpg");

            _productDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new Product() { ID = 1});
            _productDao.Setup(x => x.Update(It.IsAny<Product>())).Returns(false);

            JsonResult jsonResult = ctrl.UpdateCourse(new Product() { ID = 0, Image = null }, null) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void DeleteCourse_Sucessfull()
        {
            _productDao.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            JsonResult jsonResult = ctrl.DeleteCourse(It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void DeleteCourse_Failure()
        {
            _productDao.Setup(x => x.Delete(It.IsAny<int>())).Returns(false);

            JsonResult jsonResult = ctrl.DeleteCourse(It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }
    }
}
