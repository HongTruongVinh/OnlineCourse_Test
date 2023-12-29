using Model.Dao;
using Model.Models;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Controllers
{
    public class ProductController : BaseController
    {
        public const int ITEMS_PER_PAGE = 120;
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public IUserLoginManager _userLoginManager { get; set; }
        public IProductDao _productDao { get; set; }
        public IProductCategoryDao _productCategoryDao { get; set; }
        public ICommentDao _commentDao { get; set; }
        public ICourseVideoDao _courseVideoDao { get; set; }
        public ICourseDocumentDao _courseDocumentDao { get; set; }

        public ProductController() 
        {
            _userLoginManager = new UserLoginManager(this);
            _productDao = new ProductDao();
            _productCategoryDao = new ProductCategoryDao();
            _commentDao = new CommentDao();
            _courseVideoDao = new CourseVideoDao();
            _courseDocumentDao = new CourseDocumentDao();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Category(string searchString, long cateId)
        {

            countPages = (int)Math.Ceiling((double)_productDao.CountByCategoryID(searchString, cateId) / ITEMS_PER_PAGE);

            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }

            ViewBag.currentPage = currentPage;
            ViewBag.countPages = countPages;

            var category = _productCategoryDao.ViewDetail(cateId);
            ViewBag.Category = category;
            ViewBag.CategoryID = _productCategoryDao.ListAll();
            var model = _productDao.ListByCategoryID(searchString, cateId, currentPage, ITEMS_PER_PAGE);

            return View(model);
        }

        public ActionResult Detaill(long id, long detailid)
        {
            var product = _productDao.ViewDetail(id);
            ViewBag.CategoryID = _productCategoryDao.ListAll();

            var sessionUser = _userLoginManager.GetUserLogin();
            ViewBag.UserID = sessionUser.UserID;
            ViewBag.ListComment = _commentDao.ListCommentViewModel(0, id);

            ViewBag.DetailID = detailid.ToString();

            int createrID = (int)Convert.ToDouble(product.CreateBy);
            ViewBag.CreatedBy = _productDao.GetCreatedByUser(createrID);

            return View(product);
        }

        public ActionResult Detail(int productId, int playingIdVideo)
        {
            var product = _productDao.ViewDetail(productId);
            ViewBag.CategoryID = _productCategoryDao.ListAll();

            var sessionUser = _userLoginManager.GetUserLogin();
            ViewBag.UserID = sessionUser.UserID;
            ViewBag.ListComment = _commentDao.ListCommentViewModel(0, productId);

            ViewBag.isProductOfUserSession = _productDao.IsProductOfUserSession(productId, (int)sessionUser.UserID);

            int createrID = (int)Convert.ToDouble(product.CreateBy);
            ViewBag.CreatedBy = _productDao.GetCreatedByUser(createrID);

            List<CourseVideo> productVideos = _courseVideoDao.GetListVideoInfor(productId);
            ViewBag.productVideos = productVideos;

            ViewBag.productDocuments = _courseDocumentDao.GetListDocumentInfor(productId);

            if (playingIdVideo == -1 && productVideos.Count > 0)
            {
                playingIdVideo = _courseVideoDao.GetListVideoInfor(productId).OrderByDescending(o => o.DateUpdate).ToList().FirstOrDefault().ID;
            }

            ViewBag.playingVideo = _courseVideoDao.GetVideo(playingIdVideo);

            return View(product);
        }

        [ChildActionOnly]
        public ActionResult _ChildComment(long parentid, long productid)
        {
            var data = _commentDao.ListCommentViewModel(parentid, productid);
            var sessionuser = _userLoginManager.GetUserLogin();
            for (int k = 0; k < data.Count; k++)
            {
                data[k].UserID = sessionuser.UserID;
            }
            return PartialView("~/Views/Shared/_ChildComment.cshtml", data);
        }

        [HttpPost]
        public JsonResult AddNewComment(long productid, long userid, long parentid, string commentmsg, string rate)
        {
            Comment comment = new Comment();

            comment.CommentMsg = commentmsg;
            comment.ProductID = productid;
            comment.UserID = userid;
            comment.ParentID = parentid;
            comment.Rate = Convert.ToInt16(rate);
            comment.CommentDate = DateTime.Now;

            bool addcomment = _commentDao.Insert(comment);
            if (addcomment == true)
            {
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
        public ActionResult GetComment(long productid)
        {
            var data = _commentDao.ListCommentViewModel(0, productid);
            return PartialView("~/Views/Shared/_ChildComment.cshtml", data);
        }

        public FileResult LearnerDownloadDocument(string link)
        {
            //var memory = new MemoryStream();
            //using (var stream = new FileStream(link, FileMode.Open))
            //{
            //    stream.CopyTo(memory);
            //}
            //memory.Position = 0;

            string ext = Path.GetExtension(link).ToLowerInvariant();
            return File(link, GetMimeTypes()[ext]);
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
