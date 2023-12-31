﻿using Model.Dao;
using Model.Models;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        public IProductDao _productDao {  get; set; }
        public IProductCategoryDao _productCategoryDao { get; set; }

        public ProductController() 
        {
            _productDao = new ProductDao();
            _productCategoryDao = new ProductCategoryDao();
        }

        public ActionResult Index(string dropdownid,string searchString, int page = 1, int pageSize = 200)
        {
            if(dropdownid == null)
            {
                dropdownid = "-1";
            }
            var model = _productDao.ListAllPaging(Convert.ToInt16(dropdownid), searchString, page, pageSize);

            ViewBag.SearchString = searchString;
            ViewBag.DropDownID = dropdownid;
            SetViewBag();

            
            return View(model);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            _productDao.Delete(id);
            return RedirectToAction("Index");
        }

        public void SetViewBag(long? selectedId = null)
        {
            ViewBag.CategoryID = _productCategoryDao.ListAll();
        }

        [HttpPost]
        public JsonResult AddProductAjax(string name, string code, string metatitle, string description, string image, string categoryid,string detail,
            string listtype,string listfile)
        {
            Product product = new Product();

            product.Name = name;
            product.CreateDate = DateTime.Now;
            product.Code = code;
            product.MetaTitle = metatitle;
            product.Description = description;
            product.Image = image;
            product.CategoryID = Convert.ToInt16(categoryid);
            product.Status = true;
            product.Detail = detail;
            product.ListType = listtype;
            product.ListFile = listfile;

            long id = _productDao.Insert(product);
            if (id > 0)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }

        [HttpPost]
        public JsonResult UpdateProductAjax(long id, string name, string code, string metatitle, string description, string detail, string image, string listtype, string listfile, string categoryid)
        {
            Product product = new Product();

            product = _productDao.ViewDetail(Convert.ToInt16(id));
            product.Name = name;
            product.ModifiDate = DateTime.Now;
            product.Code = code;
            product.MetaTitle = metatitle;
            product.Description = description;
            product.Image = image;

            if (detail.Length > 5)
            {
                product.Detail = detail;
            }

            product.ListType = listtype;
            product.ListFile = listfile;
            product.CategoryID = Convert.ToInt16(categoryid);

            bool editresult = _productDao.Update(product);
            if (editresult == true)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }
    }
}
