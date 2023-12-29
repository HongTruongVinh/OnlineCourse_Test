using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public interface IGetInforDao
    {
        HomeInfor GetHomeInfor();
    }

    public class GetInforDao: IGetInforDao
    {
        public GetInforDao() { }

        public HomeInfor GetHomeInfor()
        {
            return new HomeInfor() { 
                CountProduct = DataProvider.Ins.DB.Products.Count(), 
                CountStudent = DataProvider.Ins.DB.User_Role.Where(x => x.idRole == 2).Count(), //user are students
                CountTeacher = DataProvider.Ins.DB.User_Role.Where(x => x.idRole == 3).Count(), //user are teachers
                CountCertification = new Random().Next(DataProvider.Ins.DB.Products.Count())
            };
        }
    }

    public class HomeInfor
    {
        public int CountTeacher { get; set; }
        public int CountProduct { get; set; }
        public int CountStudent { get; set; }
        public int CountCertification { get; set; }
        public HomeInfor() { }
    }
}
