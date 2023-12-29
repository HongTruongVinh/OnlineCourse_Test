using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Common
{
    public interface IFileManager
    {
        string UploadImage(HttpPostedFileBase file);
        string UploadDocument(HttpPostedFileBase file);
        string UploadVideo(HttpPostedFileBase file);
    }

    public class FileManager: IFileManager
    {
        static Controller Controller;

        public FileManager(Controller controller) 
        {
            Controller = controller;
        }

        public string UploadImage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".img") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Controller.Server.MapPath("~/assets/client/images/avatar"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "/assets/client/images/avatar/" + random + Path.GetFileName(file.FileName);

                    }
                    catch
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Controller.Response.Write("<script>alert('Only jpg, png or img formats are acceptable....'); </script>");
                }
            }
            else
            {
                Controller.Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }

            return path;
        }

        public string UploadDocument(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".txt") || extension.ToLower().Equals(".pdf") || extension.ToLower().Equals(".docx") || extension.ToLower().Equals(".zip"))
                {
                    try
                    {
                        path = Path.Combine(Controller.Server.MapPath("~/assets/client/documents"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "/assets/client/documents/" + random + Path.GetFileName(file.FileName);

                    }
                    catch
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Controller.Response.Write("<script>alert('Only pdf, txt, doc or zip formats are acceptable....'); </script>");
                }
            }
            else
            {
                Controller.Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }

            return path;
        }

        public string UploadVideo(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".mp4"))
                {
                    try
                    {
                        path = Path.Combine(Controller.Server.MapPath("~/assets/client/videos"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "/assets/client/videos/" + random + Path.GetFileName(file.FileName);

                    }
                    catch
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Controller.Response.Write("<script>alert('Only mp4 formats are acceptable....'); </script>");
                }
            }
            else
            {
                Controller.Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }

            return path;
        }
    }
}