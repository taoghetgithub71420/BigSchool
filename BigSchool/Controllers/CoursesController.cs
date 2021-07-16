using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
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
            var listAttendance = context.Attendences.Where(p => p.Attendee == currenUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendence temp in listAttendance)
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
            var course = context.Courses.Where(c => c.LecturerId == currenUser.Id && c.DateTime > DateTime.Now).ToList();
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

            Course e = context.Courses.SingleOrDefault(p => p.Id == c.Id);
            if (e != null)
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

           
            try

            {

                Course delete = context.Courses.SingleOrDefault(p => p.Id == id);
                if (delete != null)
                {
                    context.Courses.Remove(delete);
                    context.SaveChanges();

                }

            }

            catch (DbEntityValidationException ex)

            {

                // Retrieve the error messages as a list of strings.

                var errorMessages = ex.EntityValidationErrors

                .SelectMany(x => x.ValidationErrors)

                .Select(x => x.ErrorMessage);

                // Join the list to a single string.

                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.

                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }
            return RedirectToAction("Mine");
        }
        //[HttpPost]
        //public JsonResult RegisterCourse(int id)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var attending = context.Attendences.Where(m => m.CourseId == id && m.Attendee == userId).FirstOrDefault();
        //    if(attending != null)
        //    {
        //        // có dữ liệu thì nó đăng ký rồi => xóa 
        //        context.Attendences.Remove(attending);
        //        context.SaveChanges();
        //        return Json("true", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        // ngược lại chưa có thì thêm .
        //        Attendence item = new Attendence();
        //        item.Attendee = userId;
        //        item.CourseId = id;
        //        context.Attendences.Add(item);
        //        context.SaveChanges();
        //        return Json("false", JsonRequestBehavior.AllowGet);
        //    }   
        //}
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
           System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.Followings.Where(p => p.FollowerId ==
            currentUser.Id).ToList();
            //danh sách các khóa học mà người dùng đã đăng ký
            var listAttendances = context.Attendences.Where(p => p.Attendee ==
            currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                       System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }

            }
            return View(courses);
        }
    }
}
