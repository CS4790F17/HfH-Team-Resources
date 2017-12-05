using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HabitatForHumanity.ViewModels;
using System.Data.SqlClient;

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

        public TimeSheet()
        {
            Id = -1;
            user_Id = -1;
            project_Id = -1;
            org_Id = -1;
            clockInTime = DateTime.Today;
            clockOutTime = DateTime.Today.AddDays(1);
        }


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
            st.data = new TimeSheet();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                DateTime cit = DateTime.Parse(clockInTime);

                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Where(x => x.user_Id == userId && x.project_Id == projectId && x.clockInTime.Equals(cit)).Single();
                return st;
            }
            catch (InvalidOperationException e)
            {
                st.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                st.errorMessage = e.ToString();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
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
            st.data = new TimeSheet();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Find(id);
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
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
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.timeSheets.Add(ts);
                db.SaveChanges();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
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
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
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
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.timeSheets.Attach(ts);
                db.timeSheets.Remove(ts);
                db.SaveChanges();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
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
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                TimeSheet ts = db.timeSheets.Find(id);
                if (ts != null)
                {
                    db.timeSheets.Remove(ts);
                    db.SaveChanges();

                    st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                    return st;
                }

                st.errorCode = ReturnStatus.NULL_ARGUMENT;
                return st;

            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
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
            st.data = new List<TimeSheet>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Where(x => x.project_Id == projectId).OrderBy(x => x.Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Gets all the unique user timesheets with the supplied project id.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static ReturnStatus GetUniqueUserTimeSheetsByProjectId(int projectId)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new List<TimeSheet>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Where(x => x.project_Id == projectId).OrderBy(x => x.Id).ToList();
                // st.data = db.timeSheets.Count().Where(x => x.project_Id == projectId).OrderBy(x => x.user_Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
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
            st.data = new List<TimeSheet>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Where(x => x.clockInTime >= beginDate && x.clockOutTime <= endDate).OrderBy(x => x.Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public static ReturnStatus GetTimeCardPageWithFilter(int page, int itemsPerPage, ref int totalTimeCards, int orgId, int projId, DateTime rangeStart, DateTime rangeEnd, string searchTerm)
        {

            ReturnStatus cardsReturn = new ReturnStatus();
            try
            {
                string sql = " SELECT T.Id AS timeId, " +
                        " T.user_Id AS userId, " +
                        " P.Id AS projId, " +
                        " O.Id AS orgId, " +
                        " T.clockInTime AS inTime, " +
                        " T.clockOutTime AS outTime, " +
                        " O.name AS orgName, " +
                        " P.name AS projName, " +
                        " ISNULL(U.firstName,U.emailAddress) + ' ' + ISNULL(U.lastName,U.emailAddress) AS volName " +
                        " FROM dbo.TimeSheet T LEFT JOIN dbo.[User] U ON T.user_Id = U.Id " +
                        " LEFT JOIN Organization O ON T.org_Id = O.Id " +
                        " LEFT JOIN Project P ON P.Id = T.project_Id " +
                        " WHERE 1=1 ";
                sql += (projId > 0) ? " AND P.Id = @projectId " : "";
                sql += (orgId > 0) ? " AND O.Id = @orgId " : "";
                sql += " AND CONVERT(DATE, T.clockInTime) BETWEEN '" + rangeStart.Date.ToString("yyyyMMdd") +
                        "' AND '" + rangeEnd.Date.ToString("yyyyMMdd") + "' ";
                sql += (!string.IsNullOrEmpty(searchTerm)) ?
                          //" AND (U.firstName LIKE '%' + @searchTerm + '%' OR U.lastName LIKE '%' + @searchTerm + '%') " : "";
                          " AND (U.firstName LIKE '%' + @searchTerm + '%' OR U.lastName LIKE '%' + @searchTerm + '%' " +
                          " OR O.name LIKE '%' + @searchTerm + '%' OR P.name LIKE '%' + @searchTerm + '%') " : "";
                sql += " ORDER BY T.clockInTime DESC ";
                VolunteerDbContext db = new VolunteerDbContext();
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                if (projId > 0) { sqlParams.Add(new SqlParameter("@projectId", projId)); }
                if (orgId > 0) { sqlParams.Add(new SqlParameter("@orgId", orgId)); }
                if (!string.IsNullOrEmpty(searchTerm)) { sqlParams.Add(new SqlParameter("@searchTerm", searchTerm)); }

                var cards = db.Database.SqlQuery<TimeCardVM>(sql, sqlParams.ToArray()).ToList();

                cardsReturn.errorCode = 0;
                cardsReturn.data = cards.ToList();
                return cardsReturn;
            }
            catch
            {
                cardsReturn.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                return cardsReturn;
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
            st.data = new List<TimeSheet>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Where(x => x.org_Id == organizationId).OrderBy(x => x.Id).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
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
            st.data = new TimeSheet();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                var sheet = db.timeSheets.Where(x => x.user_Id == userId).ToList().OrderBy(y => y.clockInTime);
                if (sheet.Count() > 0)
                {
                    if (sheet.Last().clockOutTime == DateTime.Today.AddDays(1))
                    {
                        st.errorCode = 0;
                        st.data = sheet.Last();
                        return st;
                    }
                }
                st.errorCode = 0;
                return st;
            }
            catch
            {
                st.errorCode = -1;
                return st;
            }
        }



        #endregion

        /// <summary>
        /// Gets all the supplied volunteers timesheets
        /// </summary>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
        public static ReturnStatus GetAllVolunteerTimeSheets(int volunteerId)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new List<TimeSheet>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.timeSheets.Where(x => x.user_Id == volunteerId).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public static ReturnStatus GetTimeSheetIdsByUserId(int userId)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var ids = (from t in db.timeSheets
                           where t.user_Id == userId
                           select t.Id).ToList();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
                rs.data = ids;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }

        //public static ReturnStatus GetBadTimeSheets()
        //{
        //    ReturnStatus st = new ReturnStatus();
        //    st.data = new List<TimeSheet>();
        //    try
        //    {
        //        VolunteerDbContext db = new VolunteerDbContext();
        //        var sheets = db.timeSheets.Where(t => t.clockInTime < DateTime.Today && t.clockOutTime.Hour == 0)
        //            .OrderByDescending(c => c.clockInTime).
        //            ToList();

        //        st.errorCode = (int)ReturnStatus.ALL_CLEAR;
        //        st.data = sheets;
        //        return st;
        //    }
        //    catch (Exception e)
        //    {
        //        st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
        //        st.errorMessage = e.ToString();
        //        return st;
        //    }
        //}

        /// <summary>
        /// Used in Admin/Volunteers
        /// </summary>
        /// <param name="projectId">ints > 0 are valid ids</param>
        /// <returns>returns a list of users for the given project</returns>
        public static ReturnStatus GetUsersbyTimeSheetFilters(int projectId, int orgId)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                string inFilter = "";

                if (projectId > 0 && orgId > 0)
                {
                    var userIds = (from t in db.timeSheets
                                   where t.project_Id == projectId && t.org_Id == orgId
                                   select t.user_Id).ToArray();
                    inFilter = (userIds.Length > 0) ? " U WHERE U.Id IN (" + string.Join(" , ", userIds) + " ) " : "";
                }
                else if (projectId > 0)
                {
                    var userIds = (from t in db.timeSheets
                                   where t.project_Id == projectId
                                   select t.user_Id).ToArray();
                    inFilter = (userIds.Length > 0) ? " U WHERE U.Id IN (" + string.Join(" , ", userIds) + " ) " : "";
                }
                else if (orgId > 0)
                {
                    var userIds = (from t in db.timeSheets
                                   where t.org_Id == orgId
                                   select t.user_Id).ToArray();
                    inFilter = (userIds.Length > 0) ? " U WHERE U.Id IN (" + string.Join(" , ", userIds) + " ) " : "";
                }

                var users = db.users.SqlQuery("SELECT * FROM dbo.[User]" + inFilter).ToList();
                rs.data = users;
                rs.errorCode = 0;
            }
            catch
            {
                rs.errorCode = -1;
            }
            return rs;
        }

        public static ReturnStatus GetNumBadPunches()
        {
 
            ReturnStatus st = new ReturnStatus();
            try
            {
                DateTime today = DateTime.Today;
                DateTime aMonthAgo = DateTime.Today.AddDays(-30);
                VolunteerDbContext db = new VolunteerDbContext();
                var numsheets = db.timeSheets.Where(
                    t => t.clockInTime < today
                    && t.clockInTime > aMonthAgo
                    && t.clockOutTime.Hour == 0 
                    && t.clockOutTime.Minute == 0).Count();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = numsheets;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        
        }

        /// <summary>
        /// Gets data for Hours volunteered Bar Chart Admin/Dashboard
        /// </summary>
        /// <param name="restoreId"></param>
        /// <param name="awbkId"></param>
        /// <returns>An array of 9 lists of timecards, 3 years worth of timesheets for the 3 categories, restore, awbk, and everything else</returns>
        public static ReturnStatus Get3YearsTimeSheetsByCategory(List<int> restoreIds, List<int> abwkIds, List<int> homeIds)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                int thisYear = DateTime.Today.Year;
                List<TimeSheet>[] timesheetInception = new List<TimeSheet>[9];
                int j = 2, k = 2, l = 2;
                for(int i = 0; i < 9; i++)
                {
                    if (i < 3)
                    {
                        timesheetInception[i] = (
                            from t in db.timeSheets
                            where restoreIds.Contains(t.project_Id) && (t.clockInTime.Year == thisYear - j)
                            select t).ToList();
                j--;
                    }
                    else if (i < 6)
                    {
                        timesheetInception[i] = (
                            from t in db.timeSheets
                            where abwkIds.Contains(t.project_Id) && (t.clockInTime.Year == thisYear - k)
                            select t).ToList();
                        k--;
                    }
                    else
                    {
                        timesheetInception[i] = (
                            from t in db.timeSheets
                            where restoreIds.Contains(t.project_Id) && (t.clockInTime.Year == thisYear - l)
                            select t).ToList();
                        l--;
                    }                  
                }

                rs.data = timesheetInception;
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }

        public static ReturnStatus Get12MonthsTimeSheetsByCategory(List<int> restoreIds, List<int> abwkIds, List<int> homeIds)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                DateTime startRange;
                DateTime endRange;
                DateTime today = DateTime.Today;
                int thisYear = DateTime.Today.Year;
                int thisMonth = DateTime.Today.Month;
                List<TimeSheet>[] timesheetInception = new List<TimeSheet>[36];
                int j = 11, k = 11, l = 11;
                for (int i = 0; i < 36; i++)
                {
                    if (i < 12)
                    {
                        startRange = today.AddMonths(-1 * j);
                        endRange = today.AddMonths(-1 * (j - 1));
                        timesheetInception[i] = (
                            from t in db.timeSheets
                            where restoreIds.Contains(t.project_Id) && (t.clockInTime >= startRange) && (t.clockInTime < endRange)
                            select t).ToList();
                        j--;
                    }
                    else if (i < 24)
                    {
                        startRange = today.AddMonths(-1 * k);
                        endRange = today.AddMonths(-1 * (k - 1));
                        timesheetInception[i] = (
                            from t in db.timeSheets
                            where abwkIds.Contains(t.project_Id) && (t.clockInTime >= startRange) && (t.clockInTime < endRange)
                            select t).ToList();
                        k--;
                    }
                    else
                    {
                        startRange = today.AddMonths(-1 * l);
                        endRange = today.AddMonths(-1 * (l - 1));
                        timesheetInception[i] = (
                            from t in db.timeSheets
                            where abwkIds.Contains(t.project_Id) && (t.clockInTime >= startRange) && (t.clockInTime < endRange)
                            select t).ToList();
                        l--;
                    }
                }

                rs.data = timesheetInception;
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }

        public static ReturnStatus Get12WeeksTimeSheetsByCategory(List<int> restoreIds, List<int> abwkIds, List<int> homeIds)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                DateTime startRange;
                DateTime endRange;
                DateTime today = DateTime.Today;
                int thisYear = DateTime.Today.Year;
                int thisMonth = DateTime.Today.Month;
                List<TimeSheet>[] timesheetInception = new List<TimeSheet>[36];
                int j = 11, k = 11, l = 11;
                for (int i = 0; i < 36; i++)
                {
                    if (i < 12)
                    {
                        startRange = today.AddDays(-7 * j);
                        endRange = today.AddDays(-7 * (j - 1));
                        timesheetInception[i] = (
                             from t in db.timeSheets
                             where restoreIds.Contains(t.project_Id) && (t.clockInTime >= startRange) && (t.clockInTime < endRange)
                             select t).ToList();
                        j--;
                    }
                    else if (i < 24)
                    {
                        startRange = today.AddDays(-7 * k);
                        endRange = today.AddDays(-7 * (k - 1));
                        timesheetInception[i] = (
                             from t in db.timeSheets
                             where abwkIds.Contains(t.project_Id) && (t.clockInTime >= startRange) && (t.clockInTime < endRange)
                             select t).ToList();
                        k--;
                    }
                    else
                    {
                        startRange = today.AddDays(-7 * l);
                        endRange = today.AddDays(-7 * (l - 1));
                        timesheetInception[i] = (
                             from t in db.timeSheets
                             where abwkIds.Contains(t.project_Id) && (t.clockInTime >= startRange) && (t.clockInTime < endRange)
                             select t).ToList();
                        l--;
                    }
                }

                rs.data = timesheetInception;
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }
    }
}