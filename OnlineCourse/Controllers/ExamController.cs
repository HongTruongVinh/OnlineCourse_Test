using Model.Dao;
using Model.Models;
using OnlineCourse.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Controllers
{
    public class ExamController : BaseController
    {
        public IUserLoginManager _userLoginManager;
        public IExamDao _examDao {  get; set; }
        public IProductCategoryDao _productCategoryDao { get; set; }
        public IQuestionDao _questionDao { get; set; }
        public IResultDao _resultDao { get; set; }

        public ExamController()
        {
            _userLoginManager = new UserLoginManager(this);
            _examDao = new ExamDao();
            _questionDao = new QuestionDao();
            _resultDao = new ResultDao();
            _productCategoryDao = new ProductCategoryDao();
        }

        // GET: /Exam/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Category(string searchString, string Type)
        {
            ViewBag.Category = Type;
            ViewBag.CategoryID = _productCategoryDao.ListAll();
            var model = _examDao.ListByType(searchString, Type);
            return View(model);
        }

        public ActionResult Detail(long id)
        {
            var exam = _examDao.ViewDetail(Convert.ToInt16(id));
            ViewBag.ExamQuestion = _questionDao.ListExamQuestion(exam.QuestionList);
            //var session = (UserLogin)Session[CommonConstants.USER_SESSION];
            var session = _userLoginManager.GetUserLogin();
            ViewBag.Result = _resultDao.GetByUserExamID(session.UserID, exam.ID);

            ViewBag.Msnv = session.UserName;
            ViewBag.UserID = session.UserID;

            //if(!dao.UserList.Contains("*" +session.UserID.ToString() + "*"))
            //{
            //    return Redirect("/trang-chu");
            //}
            return View(exam);
        }
        [HttpPost]
        public JsonResult AddResult(long examid,long userid)
        {
            Result result = new Result();

            result.ExamID = examid;
            result.UserID = userid;
            result.Status = false;
            result.ResultQuiz = "";
            result.ResultEssay = "";
            result.StartDateQuiz = DateTime.Now.ToShortDateString();
            result.StartTimeQuiz = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute;


            Exam exam = _examDao.ViewDetail((int)examid);
            var x = exam.QuestionList.Split('*');
            int totalQuestion = 0;
            foreach (var item in x)
            {
                if (item != "")
                {
                    totalQuestion += 1;
                }
            }
            Random random = new Random();
            int score = (100 / totalQuestion) * random.Next(1, totalQuestion);
            result.Score = score.ToString();


            bool addresult = _resultDao.Insert(result);
            if (addresult == true)
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

        [HttpPost]
        public JsonResult UpdateResult(long examid, long userid, string resultessay, string resultquiz)
        {
            Result result = new Result();

            result.ExamID = examid;
            result.UserID = userid;
            result.Status = true;
            result.ResultQuiz = resultquiz;
            result.ResultEssay = resultessay;
            result.FinishTimeEssay = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute;
            result.FinishTimeQuiz = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute;

            bool addresult = _resultDao.Update(result);
            if (addresult == true)
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



    
    }
}
