using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineCourse.Common
{
    public interface IUserLoginManager
    {
        UserLogin GetUserLogin();
        void AddUserLogin(UserLogin userLogin);
        void RemoveUserLogin();
    }

    public class UserLoginManager: IUserLoginManager
    {
        static Controller Controller;

        public UserLoginManager(Controller controller)
        {
            Controller = controller;
        }

        public UserLogin GetUserLogin()
        {
            return (OnlineCourse.Common.UserLogin)Controller.Session[OnlineCourse.Common.CommonConstants.USER_SESSION];
        }

        public void AddUserLogin(UserLogin userLogin)
        {
            Controller.Session.Add(CommonConstants.USER_SESSION, userLogin);
        }

        public void RemoveUserLogin()
        {
            Controller.Session.Remove(CommonConstants.USER_SESSION);
        }
    }
}