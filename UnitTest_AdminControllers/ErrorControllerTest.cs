using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Dao;
using Model.Models;
using Moq;
using OnlineCourse.Common;
using OnlineCourse.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTest_AdminControllers
{
    [TestClass]
    public class ErrorControllerTest
    {
        ErrorController ctrl;

        public ErrorControllerTest() 
        { 

        }

        [TestMethod]
        public void NotAllowedPage()
        {
            ViewResult v = ctrl.Index() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }
    }
}
