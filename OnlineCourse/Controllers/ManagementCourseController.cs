using Microsoft.Ajax.Utilities;
using Model.Dao;
using Model.Models;
using OnlineCourse.Common;
using OnlineCourse.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Editor;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace OnlineCourse.Controllers
{
    public class ManagementCourseController : Controller
    {
        public IUserLoginManager _userLoginManager { get; set; }
        public IProductDao _productDao { get; set; }
        public ICourseVideoDao _courseVideoDao { get; set; }
        public ICourseDocumentDao _courseDocumentDao { get; set; }
        public IProductCategoryDao _productCategoryDao { get; set; }
        public IManagementCourseDao _managementCourseDao { get; set; }
        public IFileManager _fileManager { get; set; }

        public ManagementCourseController()
        {
            _userLoginManager = new UserLoginManager(this);
            _productDao = new ProductDao();
            _productCategoryDao = new ProductCategoryDao();
            _managementCourseDao = new ManagementCourseDao();
            _courseVideoDao = new CourseVideoDao();
            _courseDocumentDao = new CourseDocumentDao();
            _fileManager = new FileManager(this);

            MyProducts = new List<Product>();
        }

        // GET: ManagementCourse

        List<Product> MyProducts;

        public ActionResult Index()
        {
            var user = _userLoginManager.GetUserLogin();

            GetListProductOfUser((int)user.UserID);
            ViewBag.MyProducts = ConvertToProductModels(MyProducts);

            return View();
        }

        public ActionResult ManagementCourseDetail(long productId)
        {
            ViewBag.ListDocument = _courseDocumentDao.GetListDocumentInfor((int)productId);
            ViewBag.ListVideo = _courseVideoDao.GetListVideoInfor((int)productId);

            var user = _userLoginManager.GetUserLogin();

            GetListProductOfUser((int)user.UserID);

            Product product = MyProducts.Where(x => x.ID == productId).FirstOrDefault();
            
            ProductModel model = new ProductModel() { ID = product.ID, Name = product.Name, Description = product.Description };

            return View(model);
        }

        public ActionResult ViewEditCourse(long productId)
        {
            var user = _userLoginManager.GetUserLogin();

            ViewBag.ListCategories = _productCategoryDao.ListAll();

            if (productId == -1)
            {
                return View(new Product());
            }

            var model = _productDao.ViewDetail(productId);

            return View(model);
        }

        public ActionResult ViewAddVieoToCourse(long productId)
        {
            var user = _userLoginManager.GetUserLogin();

            var product = _productDao.ViewDetail(productId);

            var model = new CourseVideo();

            ViewBag.product = new ProductModel() { CategoryID = productId, Name = product.Name };

            return View(model);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddVieoToCourse(CourseVideo video, HttpPostedFileBase file)
        {
            var user = _userLoginManager.GetUserLogin();

            if (file != null && video.Title != null)
            {
                video.Name = file.FileName;
                video.DateUpdate = DateTime.Now;
                video.Link = _fileManager.UploadVideo(file);

                var result = _courseVideoDao.AddCourseVideo(video);
            }

            return RedirectToAction("ManagementCourseDetail", new { productId = video.productID });
        }


        public ActionResult ViewAddDocumentToCourse(long productId)
        {
            var user = _userLoginManager.GetUserLogin();

            var product = _productDao.ViewDetail(productId);

            var model = new CourseDocument();

            ViewBag.product = new ProductModel() { CategoryID = productId, Name = product.Name };

            return View(model);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddDocumentToCourse(CourseDocument document, HttpPostedFileBase file)
        {
            var user = _userLoginManager.GetUserLogin();

            if (document.Title != null && file != null)
            {
                document.Name = file.FileName;
                document.DateUpdate = DateTime.Now;
                document.Link = _fileManager.UploadDocument(file);
                var result = _courseDocumentDao.AddCourseDocument(document);
            }

            return RedirectToAction("ManagementCourseDetail", new { productId = document.productID });
        }

        public ActionResult DeleteDocumentOfCourse(int documentId, int productId)
        {
            var user = _userLoginManager.GetUserLogin();

            var result = _courseDocumentDao.DeleteCourseDocument(documentId);

            return RedirectToAction("ManagementCourseDetail", new { productId = productId });
        }

        public ActionResult DeleteVideoOfCourse(int videoId, int productId)
        {
            var user = _userLoginManager.GetUserLogin();

            var result = _courseVideoDao.DeleteCourseVideo(videoId);

            return RedirectToAction("ManagementCourseDetail", new { productId = productId });
        }
        public ActionResult AddCourse(Product product)
        {
            var user = _userLoginManager.GetUserLogin();

            long id = _productDao.Insert(product);

            return View(id);
        }

        [System.Web.Http.HttpPost]
        public ActionResult UpdateCourse(Product product, HttpPostedFileBase imageFile)
        {
            var user = _userLoginManager.GetUserLogin();

            product.ModifiDate = DateTime.Now;


            if(imageFile != null)
            {
                string path = _fileManager.UploadImage(imageFile);

                if (!path.Equals("-1"))
                {
                    product.Image = path;
                }
            }

            bool result = false;

            if (product.ID != 0)
            {
                Product _product = _productDao.ViewDetail(product.ID);
                _product.ModifiDate = DateTime.Now;
                _product.Name = product.Name;
                _product.Description = product.Description;
                _product.Detail = product.Detail;
                _product.Price = product.Price;

                if (product.Image != null)
                {
                    _product.Image = product.Image;
                }

                result = _productDao.Update(_product);
            }
            else
            {
                product.CreateBy = user.UserID.ToString();
                product.Status = true;
                product.ListFile = " Bài 1: Giới thiệu khóa học " + product.Name + "* Bài 2: bai 2* Bài 3: bai 3* Bài 4: bai 4*Bài 5: bai 5";
                product.ListType = "0,0,0,0,0";
                product.CreateDate = DateTime.Now;

                if (product.Image == null)
                {
                    product.Image = "/assets/client/images/course/4by3/01.jpg";
                }


                long _id = _productDao.Insert(product);

                var _product = _productDao.ViewDetail(_id);
                _product.MetaTitle = _product.ID.ToString();

                result = _productDao.Update(_product);

            }

            if (result == true)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Json(new { status = false });
            }

        }

        public ActionResult DeleteCourse(int productId)
        {
            var user = _userLoginManager.GetUserLogin();

            bool result = _productDao.Delete(productId);

            if (result == true)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }

        void GetListProductOfUser(int userId)
        {
            MyProducts = _managementCourseDao.GetProductOfUser(userId);
        }

        List<ProductModel> ConvertToProductModels(List<Product> products)
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
                
                model.Price = product.Price;
                model.MetaTitle = product.MetaTitle;

                int createrID = (int)Convert.ToDouble(product.CreateBy);
                model.CreateBy = _productDao.GetCreatedByUser(createrID).Name;

                model.CountVideo = product.ListFile.Split('*').Length;

                model.CountComment = _productDao.GetCountComment(product.ID);

                model.CountLearner = _productDao.GetCountLearner(product.ID);

                productModels.Add(model);
            }

            return productModels;
        }

    }
}