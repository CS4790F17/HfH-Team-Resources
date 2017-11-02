using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("TimeSheet")]
    public class TimeSheet
    {
        [Key]
        public int Id { get; set; }
        public int user_Id { get; set; }
        public int project_Id { get; set; }
        public DateTime clockInTime { get; set; }
        public DateTime clockOutTime { get; set; }

        public static TimeSheet GetOpenUserTimeSheet(int userId)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var punches = db.timeSheets.Where(t => t.user_Id == userId && t.clockInTime.Date == DateTime.Today
                && t.clockOutTime.Date > DateTime.Today);
            TimeSheet ts = new TimeSheet();
            ts = punches.OrderByDescending(t => t.clockInTime).FirstOrDefault();
            return ts;
        }

        public static void InsertTimeSheet(TimeSheet ts)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.timeSheets.Add(ts);
            db.SaveChanges();
        }
    }
}