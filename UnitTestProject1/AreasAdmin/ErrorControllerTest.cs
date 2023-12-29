using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineCourse.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;

namespace UnitTestProject1.AreasAdmin
{
    [TestClass]
    public class ErrorControllerTest
    {
        ErrorController ctrl;

        public ErrorControllerTest() 
        {
            ctrl = new ErrorController();
        }

        [TestMethod]
        public void NotAllowedPage()
        {
            ViewResult v = ctrl.NotAllowedPage() as ViewResult;

            Assert.AreEqual<string>("", v.ViewName);
        }

        [TestMethod]
        public void NotAllowed()
        {
            PartialViewResult v = ctrl.NotAllowed() as PartialViewResult;

            Assert.AreEqual<string>("_NoPermission", v.ViewName);
        }
    }
}
