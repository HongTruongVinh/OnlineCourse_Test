using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Common;
using OnlineCourse.Controllers;
using OnlineCourse.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UnitTestProject1
{
    [TestClass]
    public class ProfileControllerTest
    {
        ProfileController ctrl;

        User user;
        UserLogin userLogin;
        Dictionary<string, bool> wishListProduct;
        List<Product> listProducts;

        Mock<IUserDao> _userDao;
        Mock<IUserLoginManager> _userLoginManager;
        Mock<IProductDao> _productDao;
        Mock<IExamDao> _examDao;
        Mock<IResultDao> _resultDao;
        Mock<IWishProductDao> _wishProductDao;
        Mock<IFileManager> _fileManager;

        public ProfileControllerTest() 
        {
            ctrl = new ProfileController();

            user = new User() 
            { 
                ID = 1, 
                UserName = "vinh", 
                Password = "1", 
                Name = "vinh", 
                LinkImage = "UserAvatar.png",

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

            listProducts = new List<Product>()
            {
                new Product()
                {
                    ID = 1,
                    CategoryID = 1,
                    Name = "Khoa hoc Toeic",
                },
                new Product()
                {
                    ID = 2,
                    CategoryID = 2,
                    Name = "Khoa hoc ASP.Net",
                }
            };


            _userLoginManager = new Mock<IUserLoginManager>();
            _userLoginManager.Setup(x => x.GetUserLogin()).Returns(userLogin);

            _productDao = new Mock<IProductDao>();
            _examDao = new Mock<IExamDao>();
            _resultDao = new Mock<IResultDao>();
            _userDao = new Mock<IUserDao>();
            _wishProductDao = new Mock<IWishProductDao>();
            _fileManager = new Mock<IFileManager>();

            ctrl._productDao = _productDao.Object;
            ctrl._userLoginManager = _userLoginManager.Object;
            ctrl._examDao = _examDao.Object;
            ctrl._resultDao = _resultDao.Object;
            ctrl._userDao = _userDao.Object;
            ctrl._wishProductDao = _wishProductDao.Object;
            ctrl._fileManager = _fileManager.Object;

            _productDao.Setup(x => x.GetCreatedByUser(It.IsAny<int>())).Returns(user);
            _productDao.Setup(x=>x.GetCountComment(It.IsAny<long>())).Returns(1);

        }



        [TestMethod]
        public void Index()
        {
            // this function checks the return view index

            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void AcademicAchievement()
        {
            var listExams = new Dictionary<string, string>()
            {
                {"1", ""},
                {"2", ""},
            };

            _resultDao.Setup(x => x.GetListResultExamOfUser(It.IsAny<long>())).Returns(listExams);
            ctrl._resultDao = _resultDao.Object;

            ViewResult v = ctrl.AcademicAchievement() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Exam()
        {
            _productDao.Setup(x => x.GetWishListProduct(It.IsAny<int>())).Returns(wishListProduct);

            _examDao.Setup(x => x.ListExamOfUser(It.IsAny<int>())).Returns(new List<Exam>());

            _wishProductDao.Setup(x => x.GetListWishProduct(It.IsAny<long>())).Returns(listProducts);

            ViewResult v = ctrl.Exam() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);

            var actual = (List<ProductModel>)v.ViewData["ListOwnProducts"];

            Assert.IsNotNull((List<ProductModel>)v.ViewData["ListOwnProducts"]);

            Assert.IsTrue(ctrl.isBoughtProduct);
        }

        [TestMethod]
        public void CourseBought()
        {
            _wishProductDao.Setup(x => x.GetListWishProduct(It.IsAny<long>())).Returns(listProducts);

            ViewResult v = ctrl.CourseBought() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
            Assert.IsTrue(ctrl.isBoughtProduct);
        }

        [TestMethod]
        public void Cart()
        {
            _wishProductDao.Setup(x => x.GetListCartProduct(It.IsAny<long>())).Returns(listProducts);

            ViewResult v = ctrl.Cart() as ViewResult;
            
            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void MyCourse()
        {
            RedirectToRouteResult redirectToRouteResult = ctrl.MyCourse() as RedirectToRouteResult;

            var values = (RouteValueDictionary)redirectToRouteResult.RouteValues;

            Assert.IsTrue(values.ContainsValue("ManagementCourse"));
            Assert.IsTrue(values.ContainsValue("Index"));
        }

        [TestMethod]
        public void UpdateProfile_Failure()
        {
            // arrange
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            var file1Mock = new Mock<HttpPostedFileBase>();
            file1Mock.Setup(x => x.FileName).Returns("file1.pdf");
            var file2Mock = new Mock<HttpPostedFileBase>();
            file2Mock.Setup(x => x.FileName).Returns("file2.doc");
            var files = new[] { file1Mock.Object, file2Mock.Object };

            user.LinkImage = null;
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(user);
            _userDao.Setup(x => x.Update(user)).Returns(false);
            _fileManager.Setup(x => x.UploadImage(files[0])).Returns("-1");

            JsonResult jsonResult = ctrl.UpdateProfile(userLogin, files[0]) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void UpdateProfile_NoneAvatarUser()
        {
            // arrange
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            var file1Mock = new Mock<HttpPostedFileBase>();
            file1Mock.Setup(x => x.FileName).Returns("file1.pdf");
            var file2Mock = new Mock<HttpPostedFileBase>();
            file2Mock.Setup(x => x.FileName).Returns("file2.doc");
            var files = new[] { file1Mock.Object, file2Mock.Object };

            user.LinkImage = null;
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(user);
            _userDao.Setup(x => x.Update(user)).Returns(false);
            _fileManager.Setup(x => x.UploadImage(files[0])).Returns("-1");

            userLogin.Image = null;

            JsonResult jsonResult = ctrl.UpdateProfile(userLogin, files[0]) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }


        [TestMethod]
        public void UpdateProfile_Successfull()
        {
            // arrange
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath("~/app_data")).Returns(@"c:\work\app_data");
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            var file1Mock = new Mock<HttpPostedFileBase>();
            file1Mock.Setup(x => x.FileName).Returns("file1.pdf");
            var file2Mock = new Mock<HttpPostedFileBase>();
            file2Mock.Setup(x => x.FileName).Returns("file2.doc");
            var files = new[] { file1Mock.Object, file2Mock.Object };


            _fileManager.Setup(x => x.UploadImage(files[0])).Returns("UserAvatar.png");
            _userDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(user);
            _userDao.Setup(x => x.Update(user)).Returns(true);

            ActionResult actionResult = ctrl.UpdateProfile(userLogin, files[0]) as ActionResult;

            var values = ((RedirectToRouteResult)actionResult).RouteValues;

            Assert.IsTrue(values.ContainsValue("Index"));
        }

        [TestMethod]
        public void BuyProduct_Successfull()
        {
            _productDao.Setup(x => x.BuyProduct(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            ActionResult actionResult = ctrl.BuyProduct(It.IsAny<int>(), It.IsAny<int>()) as ActionResult;

            var values = ((RedirectToRouteResult)actionResult).RouteValues;

            Assert.IsTrue(values.ContainsValue("Cart"));
        }

        [TestMethod]
        public void BuyProduct_Failure()
        {
            _productDao.Setup(x => x.BuyProduct(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            JsonResult jsonResult = ctrl.BuyProduct(It.IsAny<int>(), It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void AddProductToCart_Successfull()
        {
            _productDao.Setup(x => x.AddProductToCart(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            JsonResult jsonResult = ctrl.AddProductToCart(It.IsAny<int>(), It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddProductToCart_Failure()
        {
            _productDao.Setup(x => x.AddProductToCart(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            JsonResult jsonResult = ctrl.AddProductToCart(It.IsAny<int>(), It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void DeleteProduct_Successfull()
        {
            _productDao.Setup(x => x.DeleteProductFromCart(It.IsAny<int>(), 1)).Returns(true);

            ActionResult actionResult = ctrl.DeleteProduct(It.IsAny<int>(), 1) as ActionResult;

            var values = ((RedirectToRouteResult)actionResult).RouteValues;

            Assert.IsTrue(values.ContainsValue("Cart"));
        }

        [TestMethod]
        public void DeleteProduct_Failure()
        {
            _productDao.Setup(x => x.DeleteProductFromCart(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            JsonResult jsonResult = ctrl.DeleteProduct(It.IsAny<int>(), It.IsAny<int>()) as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void Constructer_Test()
        {
            ctrl = new ProfileController();
            Assert.IsNotNull(ctrl._productDao);
            Assert.IsNotNull(ctrl._userDao);
            Assert.IsNotNull(ctrl._userLoginManager);
            Assert.IsNotNull(ctrl._examDao);
            Assert.IsNotNull(ctrl._resultDao);
            Assert.IsNotNull(ctrl._wishProductDao);
            Assert.IsNotNull(ctrl._fileManager);

        }


    }
}
