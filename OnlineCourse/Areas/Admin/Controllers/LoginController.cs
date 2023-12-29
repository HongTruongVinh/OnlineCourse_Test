using Model.Dao;
using OnlineCourse.App_Start;
using OnlineCourse.Areas.Admin.Models;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        public IUserLoginManager _userLoginManager {  get; set; }
        public IUserDao _userDao { get; set; }


        public LoginController() 
        {
            _userLoginManager = new UserLoginManager(this);
            _userDao = new UserDao();
        }

        // GET: /Admin/Login/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var result = _userDao.Login(model.UserName, Encryptor.MD5Hash(model.Password), true);
                if(result == 1)
                {
                    var user = _userDao.GetByUserName(model.UserName);
                    if (_userDao.isAdminRole((int)user.ID))
                    {
                        var usersession = new UserLogin();
                        usersession.UserName = user.UserName;
                        usersession.UserID = user.ID;
                        //Session.Add(CommonConstants.USER_SESSION, usersession);
                        _userLoginManager.AddUserLogin(usersession);
                        
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản không có quyền đăng nhập");
                    }
                }
                else if(result == 0)
                    {
                        ModelState.AddModelError("","Tài khoản không tồn tại");
                    }
                else if(result ==-1)
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
            return View("Index");
        }



    }
}
