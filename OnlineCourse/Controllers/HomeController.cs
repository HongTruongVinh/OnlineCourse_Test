using Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Controllers
{
    public class HomeController : Controller
    {
        public IProductCategoryDao _productCategoryDao {  get; set; }
        public IProductDao _productDao { get; set; }
        public IExamDao _examDao { get; set; }
        public IGetInforDao _getInforDao { get; set; }

        public HomeController() 
        {
            _productCategoryDao = new ProductCategoryDao();
            _productDao = new ProductDao();
            _examDao = new ExamDao();
            _getInforDao = new GetInforDao();
        }

        //
        // GET: /Home/Index

        public ActionResult Index()
        {
            ViewBag.CategoryID = _productCategoryDao.ListAll();
            ViewBag.HomeProducts = _productDao.ListAllProduct();
            ViewBag.HomeExams = _examDao.ListAllExam();
            HomeInfor homeInfor = _getInforDao.GetHomeInfor();
            ViewBag.HomeInfor = homeInfor;
            return View();
        }

    }
}
