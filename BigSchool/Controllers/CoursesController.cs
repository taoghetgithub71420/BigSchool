using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            //get list category
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();

            return View(objCourse);
           
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext context = new BigSchoolContext();

            //(Kiểm tra dữ liệu nhập phía Server, trước khi lưu vào co sở dữ liệu, nếu nhập sai yêu cầu lỗi sẽ được gửi lại trang đang thao tác)
            // Không xét valid LectureId vì bằng user đăng nhập
            // Không xét valid LectureId vì bằng user đăng nhập
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }
            // lay login user ID
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            // add vao csdl
            context.Courses.Add(objCourse);
            context.SaveChanges();
            // trở về home
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currenUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendance = context.Attendances.Where(p => p.Attendee == currenUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendance)
            {
                Course objCourse = temp.Course;

                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }



        public ActionResult Mine()
        {
            ApplicationUser currenUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var course = context.Courses.Where(c => c.LecturerId == currenUser.Id && c.DataTime > DateTime.Now).ToList();
            foreach (Course i in course)
            {
                i.LectureName = currenUser.Name; // name la cot da them vao aspnet user
            }
            return View(course);
         }

        


    }     
    }
