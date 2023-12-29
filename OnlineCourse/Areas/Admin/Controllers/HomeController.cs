using OnlineCourse.App_Start;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace OnlineCourse.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public bool isUnitTest = false;

        public IUserLoginManager _userLoginManager { get; set; }

        public HomeController()
        {
            _userLoginManager = new UserLoginManager(this);
        }

        // GET: /Admin/Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogUot()
        {
            //Session.Remove(CommonConstants.USER_SESSION);
            _userLoginManager.RemoveUserLogin();

            if (isUnitTest == false)FormsAuthentication.SignOut();

            return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index", Area = "Admin" }));
        }
    }
}
