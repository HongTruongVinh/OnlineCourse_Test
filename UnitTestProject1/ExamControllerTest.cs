using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Common;
using OnlineCourse.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestProject1
{
    [TestClass]
    public class ExamControllerTest
    {
        ExamController ctrl;

        User user;
        UserLogin userLogin;


        Mock<IUserLoginManager> _userLoginManager;
        Mock<IExamDao> _examDao;
        Mock<IProductCategoryDao> _productCategoryDao;
        Mock<IQuestionDao> _questionDao;
        Mock<IResultDao> _resultDao;

        public ExamControllerTest() 
        {
            ctrl = new ExamController();
            _userLoginManager = new Mock<IUserLoginManager>();
            _productCategoryDao = new Mock<IProductCategoryDao>();
            _questionDao = new Mock<IQuestionDao>();
            _resultDao = new Mock<IResultDao>();
            _examDao = new Mock<IExamDao>();

            ctrl._userLoginManager = _userLoginManager.Object;
            ctrl._examDao = _examDao.Object;
            ctrl._questionDao = _questionDao.Object;
            ctrl._resultDao = _resultDao.Object;
            ctrl._productCategoryDao = _productCategoryDao.Object;

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
                WishListIdProduct = new Dictionary<string, bool>(),
            };


            _userLoginManager.Setup(x => x.GetUserLogin()).Returns(userLogin);


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
            _productCategoryDao.Setup(x => x.ListAll()).Returns(new List<ProductCategory>());
            _examDao.Setup(x => x.ListByType(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<Exam>());


            ViewResult v = ctrl.Category("Abc", "English") as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void Detail()
        {
            _examDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Exam());
            _questionDao.Setup(x=>x.ListExamQuestion("ABC")).Returns(new List<Question>());
            _resultDao.Setup(x=>x.GetByUserExamID(It.IsAny<long>(), It.IsAny<long>())).Returns(new Result());

            ViewResult v = ctrl.Detail(1) as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void AddResult_Successfull()
        {
            _examDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Exam() { QuestionList = "1*2*3"});
            _resultDao.Setup(x => x.Insert(It.IsAny<Result>())).Returns(true);

            JsonResult result = ctrl.AddResult(It.IsAny<long>(), It.IsAny<long>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void AddResult_Failure()
        {
            _examDao.Setup(x => x.ViewDetail(It.IsAny<int>())).Returns(new Exam() { QuestionList = "1*2*3" });
            _resultDao.Setup(x => x.Insert(It.IsAny<Result>())).Returns(false);

            JsonResult result = ctrl.AddResult(It.IsAny<long>(), It.IsAny<long>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }

        [TestMethod]
        public void UpdateResult_Successfull()
        {
            _resultDao.Setup(x => x.Update(It.IsAny<Result>())).Returns(true);

            JsonResult result = ctrl.UpdateResult(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = True }", respone);
        }

        [TestMethod]
        public void UpdateResult_Failure()
        {
            _resultDao.Setup(x => x.Update(It.IsAny<Result>())).Returns(false);

            JsonResult result = ctrl.UpdateResult(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()) as JsonResult;

            var respone = result.Data.ToString();

            Assert.AreEqual<string>("{ status = False }", respone);
        }
    }
}
