using Castle.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Model.ViewModel;
using Moq;
using OnlineCourse.Common;
using OnlineCourse.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace UnitTestProject1
{
    [TestClass]
    public class ProductControllerTest
    {
        ProductController ctrl;

        User user;
        UserLogin userLogin;
        Dictionary<string, bool> wishListProduct;
        List<Product> listProducts;


        Mock<IUserLoginManager> _userLoginManager;
        Mock<IProductDao> _productDao;
        Mock<IProductCategoryDao> _productCategoryDao;
        Mock<ICommentDao> _commentDao;
        Mock<ICourseVideoDao> _courseVideoDao;
        Mock<ICourseDocumentDao> _courseDocumentDao;

        
        public ProductControllerTest()
        {
            ctrl = new ProductController();

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

            _userLoginManager = new Mock<IUserLoginManager>();
            _productDao = new Mock<IProductDao>();
            _productCategoryDao = new Mock<IProductCategoryDao>();
            _commentDao = new Mock<ICommentDao>();
            _courseDocumentDao = new Mock<ICourseDocumentDao>();
            _courseVideoDao = new Mock<ICourseVideoDao>();

            _userLoginManager = new Mock<IUserLoginManager>();
            _userLoginManager.Setup(x => x.GetUserLogin()).Returns(userLogin);

            ctrl._userLoginManager = _userLoginManager.Object;
            ctrl._productDao = _productDao.Object;
            ctrl._productCategoryDao = _productCategoryDao.Object;
            ctrl._commentDao = _commentDao.Object;
            ctrl._courseDocumentDao = _courseDocumentDao.Object;
            ctrl._courseVideoDao = _courseVideoDao.Object;

        }

        [TestMethod]
        public void Index()
        {
            // this function checks the return view index

            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Category()
        {
            ctrl.currentPage = 0;
            _productDao.Setup(x=>x.CountByCategoryID(It.IsAny<string>(), It.IsAny<long>())).Returns(1);
            _productCategoryDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new ProductCategory());
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new List<ProductCategory>());
            _productDao.Setup(x => x.ListByCategoryID(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Product>());

            ViewResult v = ctrl.Category(It.IsAny<string>(), It.IsAny<long>()) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Category_OutOfPage()
        {
            ctrl.currentPage = 2;
            _productDao.Setup(x => x.CountByCategoryID(It.IsAny<string>(), It.IsAny<long>())).Returns(1);
            _productCategoryDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new ProductCategory());
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new List<ProductCategory>());
            _productDao.Setup(x => x.ListByCategoryID(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Product>());

            ViewResult v = ctrl.Category(It.IsAny<string>(), It.IsAny<long>()) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Detaill()
        {
            _productDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new Product() { ID = 1, CategoryID = 1, Name = "Toeiv", CreateBy = "1234" });
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new List<ProductCategory>());
            _commentDao.Setup(x => x.ListCommentViewModel(It.IsAny<long>(), It.IsAny<long>())).Returns(new List<Model.ViewModel.CommentViewModel>());
            _productDao.Setup(x => x.GetCreatedByUser(It.IsAny<int>())).Returns(user);


            ViewResult v = ctrl.Detaill(It.IsAny<long>(), It.IsAny<long>()) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Detail()
        {
            var listVideos = new List<CourseVideo>()
            { new CourseVideo() { ID = 1, Name = "Bai 1",  productID = 1 },
              new CourseVideo() { ID = 2, Name = "Bai 2",  productID = 1 }
            };

            _productDao.Setup(x => x.ViewDetail(It.IsAny<long>())).Returns(new Product() { ID = 1, CategoryID = 1, Name = "Toeiv", CreateBy = "1234" });
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new List<ProductCategory>());
            _commentDao.Setup(x => x.ListCommentViewModel(It.IsAny<long>(), It.IsAny<long>())).Returns(new List<Model.ViewModel.CommentViewModel>());
            _productDao.Setup(x => x.GetCreatedByUser(It.IsAny<int>())).Returns(user);
            _productDao.Setup(x=>x.IsProductOfUserSession(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            _courseVideoDao.Setup(x => x.GetListVideoInfor(It.IsAny<int>())).Returns(listVideos);
            _courseVideoDao.Setup(x => x.GetVideo(It.IsAny<int>())).Returns(new CourseVideo());

            ViewResult v = ctrl.Detail(It.IsAny<int>(), -1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void _ChildComment()
        {
            List<CommentViewModel> comments = new List<CommentViewModel>()
            {
                new CommentViewModel {ID = 1, ParentID = 1, ProductID = 1, UserID = 1},
                new CommentViewModel {ID = 2, ParentID = 1, ProductID = 1, UserID = 2}
            };

            _commentDao.Setup(x => x.ListCommentViewModel(It.IsAny<long>(), It.IsAny<long>())).Returns(comments);

            PartialViewResult v = ctrl._ChildComment(It.IsAny <long>(), It.IsAny<long>()) as PartialViewResult;

            Assert.AreEqual<string>("~/Views/Shared/_ChildComment.cshtml", v.ViewName);
        }

        [TestMethod]
        public void AddNewComment_Successfull()
        {
            _commentDao.Setup(x => x.Insert(It.IsAny<Comment>())).Returns(true);

            JsonResult jsonResult = ctrl.AddNewComment(1, 1, 1, "ABC", "3") as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddNewComment_Failure()
        {
            _commentDao.Setup(x => x.Insert(new Comment())).Returns(false);

            JsonResult jsonResult = ctrl.AddNewComment(1, 1, 1, "ABC", "3") as JsonResult;

            var respone = jsonResult.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void GetComment()
        {
            List<CommentViewModel> comments = new List<CommentViewModel>()
            {
                new CommentViewModel {ID = 1, ParentID = 1, ProductID = 1, UserID = 1},
                new CommentViewModel {ID = 2, ParentID = 1, ProductID = 1, UserID = 2}
            };

            _commentDao.Setup(x => x.ListCommentViewModel(1, 1)).Returns(comments);

            PartialViewResult v = ctrl.GetComment(1) as PartialViewResult;

            Assert.AreEqual<string>("~/Views/Shared/_ChildComment.cshtml", v.ViewName);
        }

        [TestMethod]
        public void LearnerDownloadDocument()
        {
            FilePathResult result = ctrl.LearnerDownloadDocument("Bai1.docx") as FilePathResult;

            Assert.AreEqual<string>("Bai1.docx", result.FileName);
        }

    }
}
