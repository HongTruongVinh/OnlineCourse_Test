using Microsoft.AspNetCore.Mvc.ModelBinding;
using Model.Dao;
using Model.Models;
using OnlineCourse.Common;
using OnlineCourse.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineCourse.Controllers
{
    public class ProfileController : Controller
    {
        public IUserLoginManager _userLoginManager { get; set; }
        public IResultDao _resultDao { get; set; }
        public IWishProductDao _wishProductDao { get; set; }
        public IExamDao _examDao { get; set; }

        public IProductDao _productDao { get; set; }

        public IUserDao _userDao { get; set; }

        public IFileManager _fileManager { get; set; }

        public ProfileController() 
        {
            _userLoginManager = new UserLoginManager(this);
            _resultDao = new ResultDao();
            _wishProductDao = new WishProductDao();
            _examDao = new ExamDao();
            _productDao = new ProductDao();
            _userDao = new UserDao();
            _fileManager = new FileManager(this);
        }

        // GET: Profile
        public ActionResult Index()
        {
            //var user = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
            //if (user == null)
            //{
            //    return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            //}

            var user = _userLoginManager.GetUserLogin();

            return View(user);
        }

        public ActionResult AcademicAchievement()
        {
            //var user = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
            //if (user == null)
            //{
            //    return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            //}

            var user = _userLoginManager.GetUserLogin();

            ViewBag.ListResultExam = _resultDao.GetListResultExamOfUser(user.UserID);

            return View(user);
        }

        public bool isBoughtProduct {  get; set; }
        public ActionResult Exam()
        {
            //var user = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
            //if (user == null)
            //{
            //    return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            //}

            var user = _userLoginManager.GetUserLogin();
            isBoughtProduct = true;
            ViewBag.ListOwnProducts = ConvertToProductModels(_wishProductDao.GetListWishProduct(user.UserID), isBoughtProduct);

            ViewBag.Exams = (List<Model.Models.Exam>)_examDao.ListExamOfUser((int)user.UserID);

            return View(user);
        }

        public ActionResult CourseBought()
        {
            //var user = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
            //if (user == null)
            //{
            //    return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            //}

            var user = _userLoginManager.GetUserLogin();

            isBoughtProduct = true;

            ViewBag.ListOwnProducts = ConvertToProductModels(_wishProductDao.GetListWishProduct(user.UserID), isBoughtProduct);

            return View(user);
        }

        public ActionResult Cart()
        {
            //var user = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
            //if (user == null)
            //{
            //    return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            //}

            var user = _userLoginManager.GetUserLogin(); 

            isBoughtProduct = false;

            ViewBag.CartProducts = ConvertToProductModels(_wishProductDao.GetListCartProduct(user.UserID), isBoughtProduct);

            return View(user);
        }

        public ActionResult MyCourse()
        {
            //var user = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
            //if (user == null)
            //{
            //    return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            //}

            var user = _userLoginManager.GetUserLogin();

            return RedirectToAction("Index", "ManagementCourse");
        }

        [System.Web.Http.HttpPost]
        public ActionResult UpdateProfile(UserLogin _user, HttpPostedFileBase imageFile)
        {
            User user = new User();

            user = _userDao.ViewDetail(Convert.ToInt16(_user.UserID));

            user.Name = _user.FullName;
            user.Address = _user.Address;
            user.Email = _user.Email;
            user.Phone = _user.Phone;

            string path = _fileManager.UploadImage(imageFile);
            if (!path.Equals("-1"))
            {
                user.LinkImage = path;
            }

            if (user.LinkImage == null)
            {
                user.LinkImage = "/assets/client/images/avatar/00.jpg";
            }

            bool editresult = _userDao.Update(user);

            if (editresult == true)
            {
                user = _userDao.ViewDetail(Convert.ToInt16(_user.UserID));
                var usersession = SetUserSession(user);
                //Session.Remove(CommonConstants.USER_SESSION);
                //Session.Add(CommonConstants.USER_SESSION, usersession);
                _userLoginManager.RemoveUserLogin();
                _userLoginManager.AddUserLogin(usersession);
                return RedirectToAction("Index");
            }
            else
            {
                return Json(new { status = false });
            }
        }


        UserLogin SetUserSession(User user)
        {
            var usersession = new UserLogin();
            usersession.UserID = user.ID;
            usersession.UserName = user.UserName;
            usersession.FullName = user.Name;
            usersession.Email = user.Email;

            //if (user.LinkImage == null)
            //{
            //    usersession.Image = "/assets/client/images/avatar/00.jpg";
            //}
            //else
            //{
            //    usersession.Image = user.LinkImage;
            //}
            usersession.Image = user.LinkImage;

            usersession.Role = "Học viên";
            usersession.Phone = user.Phone;
            usersession.Address = user.Address;
            usersession.WishListIdProduct = _productDao.GetWishListProduct((int)user.ID);
            return usersession;
        }

        List<ProductModel> ConvertToProductModels(List<Product> products, bool isBought = false)
        {
            List<ProductModel> productModels = new List<ProductModel>();

            foreach (Product product in products)
            {
                ProductModel model = new ProductModel();
                model.ID = product.ID;
                model.Name = product.Name;
                model.Description = product.Description;
                model.ModifiDate = product.ModifiDate;
                model.Detail = product.Detail;
                model.Image = product.Image;
                model.IsBought = isBought;
                model.Price = product.Price;
                model.MetaTitle = product.MetaTitle;

                int createrID = (int)Convert.ToDouble(product.CreateBy);
                model.CreateBy = _productDao.GetCreatedByUser(createrID).Name;

                model.CountComment = _productDao.GetCountComment(product.ID);

                productModels.Add(model);
            }

            return productModels;
        }

        [System.Web.Http.HttpGet]
        public ActionResult BuyProduct(int userId, int productId)
        {
            bool status = _productDao.BuyProduct(userId, productId);

            if (status == true)
            {
                return RedirectToAction("Cart");
            }
            else
            {
                return Json(new { status = false });
            }
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult AddProductToCart(int userId, int productId)
        {
            bool status = _productDao.AddProductToCart(userId, productId);

            if (status == true)
            {
                //var usersession = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
                //Session.Remove(CommonConstants.USER_SESSION);
                //usersession.WishListIdProduct.Add(productId.ToString(), false);
                //Session.Add(CommonConstants.USER_SESSION, usersession);

                var usersession = _userLoginManager.GetUserLogin();
                _userLoginManager.RemoveUserLogin();
                usersession.WishListIdProduct.Add(productId.ToString(), false);
                _userLoginManager.AddUserLogin(usersession);

                return Json(new { status = true }); ;
            }
            else
            {
                return Json(new { status = false });
            }
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteProduct(int userId, int productId)
        {
            bool status = _productDao.DeleteProductFromCart(userId, productId);

            if (status == true)
            {
                //var usersession = (OnlineCourse.Common.UserLogin)Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
                //Session.Remove(CommonConstants.USER_SESSION);
                //usersession.WishListIdProduct.Remove(usersession.WishListIdProduct.Where(x => x.Key == productId.ToString()).SingleOrDefault().Key);
                //Session.Add(CommonConstants.USER_SESSION, usersession);

                var usersession = _userLoginManager.GetUserLogin();
                _userLoginManager.RemoveUserLogin();
                usersession.WishListIdProduct.Remove(usersession.WishListIdProduct.Where(x => x.Key == productId.ToString()).SingleOrDefault().Key);
                _userLoginManager.AddUserLogin(usersession);

                return RedirectToAction("Cart");
            }
            else
            {
                return Json(new { status = false });
            }
        }
    }
}