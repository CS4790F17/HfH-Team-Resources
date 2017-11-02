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

        #region Database Access Methods
        /// <summary>
        /// Gets the record in the timesheet table by it's natural key: user_id+project_id+clockInTime.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="projectId">Id of the project</param>
        /// <param name="clockInTime">MM/DD/YYYY</param>
        /// <returns>Timesheet Object</returns>
        public TimeSheet GetTimeSheetByPrimaryKey(int userId, int projectId, string clockInTime)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            DateTime cit = DateTime.Parse(clockInTime);
            return db.timeSheets.Where(x => x.user_Id == userId && x.project_Id == projectId && x.clockInTime.Equals(cit)).Single();
        }
        #endregion
    }
}