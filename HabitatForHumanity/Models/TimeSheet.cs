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
        public int org_Id { get; set; }
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
        public static ReturnStatus GetTimeSheetByNaturalKey(int userId, int projectId, string clockInTime)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                DateTime cit = DateTime.Parse(clockInTime);

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.timeSheets.Where(x => x.user_Id == userId && x.project_Id == projectId && x.clockInTime.Equals(cit)).Single();
                return st;
            }
            catch (InvalidOperationException e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_FIND_SINGLE_TIMESHEET;
                st.errorMessage = e.ToString();
                st.data = "More than one timesheet found for unique natural key.";
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Get the TimeSheet with the matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A TimeSheet object with matching id otherwise null.</returns>
        public static ReturnStatus GetTimeSheetById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.timeSheets.Find(id);
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Adds the TimeSheet to the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to add.</param>
        public static ReturnStatus InsertTimeSheet(TimeSheet ts)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.timeSheets.Add(ts);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully added TimeSheet.";
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }
        }

        /// <summary>
        /// Updates the timesheet with new information.
        /// </summary>
        /// <param name="ts">TimeSheet object with new values.</param>
        public static ReturnStatus EditTimeSheet(TimeSheet ts)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully updated TimeSheet";
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }
        }

        /// <summary>
        /// Deletes the TimeSheet from the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to be deleted.</param>
        public static ReturnStatus DeleteTimeSheet(TimeSheet ts)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.timeSheets.Attach(ts);
                db.timeSheets.Remove(ts);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully deleted TimeSheet.";
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Deletes the TimeSheet from the database with the matching id.
        /// </summary>
        /// <param name="id"></param>
        public static ReturnStatus DeleteTimeSheetById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                TimeSheet ts = db.timeSheets.Find(id);
                if (ts != null)
                {
                    db.timeSheets.Remove(ts);
                    db.SaveChanges();

                    st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                    st.data = "Successfully deleted TimeSheet.";
                    return st;
                }

                st.errorCode = (int)ReturnStatus.ErrorCodes.ID_CANNOT_BE_NULL;
                st.data = "Id cannot be null.";
                return st;

            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Gets all the timesheets with the supplied project id.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static ReturnStatus GetAllTimeSheetsByProjectId(int projectId)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.timeSheets.Where(x => x.project_Id == projectId).OrderBy(x => x.Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Gets all timesheets where the clock in and out dates are between beginDate and endDate parameters.
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static ReturnStatus GetAllTimeSheetsInDateRange(DateTime beginDate, DateTime endDate)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.timeSheets.Where(x => x.clockInTime >= beginDate && x.clockOutTime <= endDate).OrderBy(x => x.Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }
        }


        /// <summary>
        /// Gets all timesheets with a specified organization id.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static ReturnStatus GetAllTimeSheetsByOrganizationid(int organizationId)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.timeSheets.Where(x => x.org_Id == organizationId).OrderBy(x => x.Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }
        }

        /// <summary>
        /// Attempts to determine if a user is logged in by fetching all the timesheets by user id and 
        /// selecting the most recent one. If the most recent timesheet has a clock out time of midnight
        /// then the user is still clocked in, otherwise they're clocked out.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ReturnStatus GetClockedInUserTimeSheet(int userId)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                // TimeSheet temp = new TimeSheet();
                VolunteerDbContext db = new VolunteerDbContext();

                //var sheets = from t in db.timeSheets
                //             group t by t.user_Id into g
                //             select g.OrderByDescending(t => t.clockInTime).FirstOrDefault();


                var sheet = db.timeSheets.Where(x => x.user_Id == userId).ToList().OrderBy(y => y.clockInTime);
                if (sheet.Count() > 0)
                {
                    if (sheet.Last().clockOutTime == DateTime.Today.AddDays(1))
                    {
                        st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                        st.data = sheet.Last();
                        return st;
                    }

                }

                //if (sheets.Count() > 0)
                //{
                //    temp = sheets.First();
                //    // only if the clockout is midnight today(tomorrow really)
                //    if (temp.clockOutTime == DateTime.Today.AddDays(1))
                //    {
                //        return temp;
                //    }
                //}
                //return new TimeSheet();
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = new TimeSheet();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }
        }



        #endregion




        //public static void UpdateTimeSheet(TimeSheet timeSheet)
        //{
        //    //timeSheet.clockInTime = (DateTime)timeSheet.clockInTime;

        //    VolunteerDbContext db = new VolunteerDbContext();
        //    db.Entry(timeSheet).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        public static List<TimeSheet> GetAllVolunteerTimeSheets(int volunteerId)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.timeSheets.Where(x => x.user_Id == volunteerId).ToList();
        }

        public static List<TimeSheet> GetBadTimeSheets()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var sheets = db.timeSheets.Where(t => t.clockInTime < DateTime.Today && t.clockOutTime.Hour == 0)
                .OrderByDescending(c => c.clockInTime).
                ToList();

            if (sheets.Count() > 0)
            {
            }
            return sheets;
        }



    }
}