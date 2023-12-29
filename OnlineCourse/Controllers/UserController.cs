using Microsoft.AspNetCore.Http;
using Model.Dao;
using Model.Models;
using OnlineCourse.Common;
using OnlineCourse.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Razor.Editor;
using System.Web.Routing;
using System.Web.Security;

namespace OnlineCourse.Controllers
{
    public class UserController : Controller
    {
        public bool isUnitTest = false;
        public IGetInforDao _getInforDao;
        public IUserDao _userDao {  get; set; }
        public IUserLoginManager _userLoginManager { get; set; }
        public IProductDao _productDao { get; set; }

        public UserController()
        {
            _userDao = new UserDao();
            _userLoginManager = new UserLoginManager(this);
            _productDao = new ProductDao();
            _getInforDao = new GetInforDao();
        }

        public ActionResult Login()
        {
            HomeInfor homeInfor = _getInforDao.GetHomeInfor();
            ViewBag.countLearner = homeInfor.CountStudent;
            
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Login(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                string hashPassword = Encryptor.MD5Hash(model.Password);
                var result = _userDao.Login(model.UserName, hashPassword, false);

                if (result == 1)
                {
                    var user = _userDao.GetByUserName(model.UserName);
                    var usersession = SetUserSession(user);
                    usersession.Address = user.Address;
                    usersession.WishListIdProduct = _productDao.GetWishListProduct((int)user.ID);
                    _userLoginManager.AddUserLogin(usersession);
                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tai");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản đang bị khóa");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                }
                else if (result == -3)
                {
                    ModelState.AddModelError("", "Tài khoản không có quyền đăng nhập");
                }


            }
            //return View("model");//code gốc 
            return View(model);
        }

        public ActionResult LogUot()
        {
            _userLoginManager.RemoveUserLogin();

            if(isUnitTest == false)FormsAuthentication.SignOut();

            return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index"}));
        }


        public ActionResult ProfileUser()
        {
            var user = _userLoginManager.GetUserLogin();


            //ViewBag.ListResultExam = new ResultDao().GetListResultExamOfUser(user.UserID);

            //ViewBag.ListOwnProducts = ConvertToProductModels(new OwnProductDao().GetListOwnProduct(user.UserID), true);

            //ViewBag.CartProducts = ConvertToProductModels(new OwnProductDao().GetListCartProduct(user.UserID), false);

            return View(user);
        }

        public UserLogin SetUserSession(User user)
        {
            var usersession = new UserLogin();
            usersession.UserID = user.ID;
            usersession.UserName = user.UserName;
            usersession.FullName = user.Name;
            usersession.Email = user.Email;

            if (user.LinkImage == null)
            {
                usersession.Image = "/assets/client/images/avatar/00.jpg";
            }
            else
            {
                usersession.Image = user.LinkImage;
            }

            usersession.Role = "Học viên";
            usersession.Phone = user.Phone;
            usersession.Address = user.Address;
            usersession.WishListIdProduct = _productDao.GetWishListProduct((int)user.ID);
            return usersession;
        }

    }
}
