using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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

        public static TimeSheet GetClockedInUserTimeSheet(int userId)
        {
            //VolunteerDbContext db = new VolunteerDbContext();
            //var punches = db.timeSheets.Where(t => t.user_Id == userId && t.clockInTime.Date == DateTime.Today
            //    && t.clockOutTime.Date > DateTime.Today).ToList();
            //var ordered = punches.OrderByDescending(t => t.clockInTime);
            //return ordered.FirstOrDefault();
            VolunteerDbContext db = new VolunteerDbContext();
            var sheets = from t in db.timeSheets
                    group t by t.user_Id into g
                    select g.OrderByDescending(t => t.clockInTime).FirstOrDefault();
            int id = 0;
            if (sheets.Count() > 0)
            {
                id = sheets.First().Id;
                return db.timeSheets.Find(id);
            }
            return null;

        }

        public static void InsertTimeSheet(TimeSheet ts)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.timeSheets.Add(ts);
            db.SaveChanges();
        }

        public static void UpdateTimeSheet(TimeSheet timeSheet)
        {
            timeSheet.clockInTime = (DateTime)timeSheet.clockInTime;

            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(timeSheet).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}