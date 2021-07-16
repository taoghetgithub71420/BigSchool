using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BigSchool.Models;
namespace BigSchool.Models
{
    public class Cheked
    {
        BigSchoolContext db = new BigSchoolContext();
        public bool check(String userId, int courseId)
        {
            var result = db.Attendences.Where(m => m.CourseId == courseId && m.Attendee == userId).FirstOrDefault();
            if(result != null)
                 return true;
            return false;
        }
    }
}