using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        BigSchoolContext context = new BigSchoolContext();
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

        [HttpGet]
        public ActionResult Edit(int id)
        {

            Course c = context.Courses.SingleOrDefault(p => p.Id == id);
            c.ListCategory = context.Categories.ToList();
            return View(c);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Course c)
        {

            Course edit = context.Courses.SingleOrDefault(p => p.Id == c.Id);
            if (edit != null)
            {
                context.Courses.AddOrUpdate(c);
                context.SaveChanges();

            }
            return RedirectToAction("Mine");
        }
        [Authorize]
        public ActionResult Delete(int id)
        {

            Course delete = context.Courses.SingleOrDefault(p => p.Id == id);
            return View(delete);
        }
        [HttpPost]
        public ActionResult DeleteCourse(int id)
        {

            Course delete = context.Courses.SingleOrDefault(p => p.Id == id);
            if (delete != null)
            {
                context.Courses.Remove(delete);
                context.SaveChanges();

            }
            return RedirectToAction("Mine");
        }


    }
}
