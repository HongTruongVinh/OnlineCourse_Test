using Model.Dao;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Areas.Admin.Controllers
{
    public class QuestionController : BaseController
    {
        public IQuestionDao _questionDao {  get; set; }
        public IProductDao _productDao { get; set; }

        public QuestionController() 
        {
            _questionDao = new QuestionDao();
            _productDao = new ProductDao();
        }

        // GET: /Admin/Question/
        public ActionResult Index(string searchString, int page = 1, int pageSize = 200)
        {
            var model = _questionDao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            SetViewBag();
            return View(model);
        }

        public void SetViewBag(long? selectedId = null)
        {
            ViewBag.ProductList = _productDao.ListAllProduct();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            _questionDao.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult AddQuestionAjax(string name, string content, string answer, string productid)
        {
            Question question = new Question();

            question.Name = name;
            question.Content = content;
            question.Answer = answer;
            question.ProductID = Convert.ToInt16(productid);
            question.Type = "1";
            question.Status = true;

            long id = _questionDao.Insert(question);
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
        public JsonResult UpdateQuestionAjax(string id, string name, string content, string answer, string productid)
        {
            Question question = new Question();
            question = _questionDao.ViewDetail(Convert.ToInt16(id));
            question.Name = name;
            question.Content = content;
            question.Answer = answer;
            question.ProductID = Convert.ToInt16(productid);
            question.Status = true;

            bool editquestion = _questionDao.Update(question);
            if (editquestion == true)
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
