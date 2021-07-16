using BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class AttendencesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Attend (Course attendanceDto)
        {
           
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            if (context.Attendences.Any(p=>p.Attendee == userID && p.CourseId == attendanceDto.Id))
            {
                //return BadRequest("The attendance already exitsts!");
                context.Attendences.Remove(context.Attendences.SingleOrDefault(p =>
p.Attendee == userID && p.CourseId == attendanceDto.Id));
                context.SaveChanges();
                return Ok("cancel");
            }
            var attendance = new Attendence()
            {
                CourseId = attendanceDto.Id,
                Attendee = User.Identity.GetUserId()
            };
            context.Attendences.Add(attendance);
            context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IHttpActionResult Follow(Following follow)
        {
            //user login là ng theo doix, follow.FolloweeId la ng dc theo doix
            var userID = User.Identity.GetUserId();
            if (userID == null)
                return BadRequest("Vui Long dang nhap truoc");
            if (userID == follow.FolloweeId)
                return BadRequest("ok ban da theo gioi bản thân của bạn ((:");
            BigSchoolContext context = new BigSchoolContext();
            // kiem tra xem mã userID đã dc theo doix chưa
            Following find = context.Followings.FirstOrDefault(p => p.FollowerId == userID && p.FolloweeId == follow.FolloweeId);
            if (find != null)
            {
                //return BadRequest("The already following exists!!");
                context.Followings.Remove(context.Followings.SingleOrDefault(p =>
 p.FollowerId == userID && p.FolloweeId == follow.FolloweeId));
                context.SaveChanges();
                return Ok("cancel");
            }
            // set object follow
            follow.FollowerId = userID;
            context.Followings.Add(follow);
            context.SaveChanges();
            return Ok();
        }
    }
}
