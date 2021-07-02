using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            BigSchoolContext context = new BigSchoolContext();
            var upcommingCourse = context.Courses.Where(p => p.DataTime > DateTime.Now).OrderBy(p => p.DataTime).ToList();
            foreach (Course i in upcommingCourse)
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(i.LecturerId);
                // quản lý user, khi đăng nhập, lấy ID của đứa nào đăng nhập, system.web là những hàm trong thư viện để gọi id tương ứng, findbyID lấy id dựa vào id của user, dựa vào get identity
                i.Name = user.Name;
              
            }
            return View(upcommingCourse);
        }
    }

      
    
}