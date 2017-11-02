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

        #region Database Access Methods

        /// <summary>
        /// Gets the record in the timesheet table by it's natural key: user_id+project_id+clockInTime.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="projectId">Id of the project</param>
        /// <param name="clockInTime">MM/DD/YYYY</param>
        /// <returns>Timesheet Object</returns>
        public static TimeSheet GetTimeSheetByNaturalKey(int userId, int projectId, string clockInTime)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            DateTime cit = DateTime.Parse(clockInTime);
            return db.timeSheets.Where(x => x.user_Id == userId && x.project_Id == projectId && x.clockInTime.Equals(cit)).Single();
        }

        /// <summary>
        /// Get the TimeSheet with the matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A TimeSheet object with matching id otherwise null.</returns>
        public static TimeSheet GetTimeSheetById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.timeSheets.Find(id);
        }

        /// <summary>
        /// Adds the TimeSheet to the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to add.</param>
        public static void AddTimeSheet(TimeSheet ts)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.timeSheets.Add(ts);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the timesheet with new information.
        /// </summary>
        /// <param name="ts">TimeSheet object with new values.</param>
        public static void EditTimeSheet(TimeSheet ts)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(ts).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the TimeSheet from the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to be deleted.</param>
        public static void DeleteTimeSheet(TimeSheet ts)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.timeSheets.Attach(ts);
            db.timeSheets.Remove(ts);
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the TimeSheet from the database with the matching id.
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteTimeSheetById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            TimeSheet ts = db.timeSheets.Find(id);
            if(ts != null)
            {
                db.timeSheets.Remove(ts);
                db.SaveChanges();
            }
        }
        #endregion
    }
}