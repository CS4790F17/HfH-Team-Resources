using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HabitatForHumanity.ViewModels;

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

        public static ReturnStatus GetTimeCardPageWithFilter(
            int page, 
            int itemsPerPage, 
            ref int totalTimeCards, 
            int orgId, 
            int projId,
            DateTime rangeStart,
            DateTime rangeEnd, 
            string queryString)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            string userIdInList = "";
            if (!string.IsNullOrEmpty(queryString))
            {
                //List<int> userIds = new List<int>();
                List<string> userTerms = queryString.Split(' ').ToList();
                var userIds = (from u in db.users
                               where userTerms.Any(term => u.firstName.Contains(term)) || userTerms.Any(term => u.lastName.Contains(term))
                               select u.Id).ToArray();
                userIdInList = (userIds.Length > 0) ? " AND U.Id IN (" + string.Join(" , ", userIds) + " ) " : "";
            }
      
            ReturnStatus cardsReturn = new ReturnStatus();
            string whereClause = " WHERE 1 = 1 ";
            //whereClause += (userId > 0) ? " AND U.Id = " + userId.ToString() + " " : ""; // make whole other function for know userid
            whereClause += userIdInList;
            whereClause += (projId > 0) ? " AND P.Id = " + projId.ToString() + " " : "";
            whereClause += (orgId > 0) ? " AND O.Id = " + orgId.ToString() + " " : "";
            whereClause += " AND CONVERT(DATE, T.clockInTime) >= CONVERT(DATE, '" + rangeStart.Date.ToString("d") + "' ) ";
            whereClause += " AND CONVERT(DATE, T.clockInTime) <= CONVERT(DATE, '" + rangeEnd.Date.ToString("d") + "' ) ";
            //whereClause += " AND T.clockInTime < CONVERT(DATETIME, " + rangeEnd.ToString() + ") ";

            var cards = db.Database.SqlQuery<TimeCardVM>(
                " SELECT T.Id AS timeId, " +
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
                    whereClause +
                        " ORDER BY T.clockInTime DESC ").ToList();
         
            cardsReturn.errorCode = 0;
            cardsReturn.data = cards.ToList();// Skip(itemsPerPage * page).Take(itemsPerPage).ToList();
            return cardsReturn;
            
            //VolunteerDbContext db = new VolunteerDbContext();
            ////List<TimeCardVM> cards = new List<TimeCardVM>();
            //List<string> userTerms = new List<string>();
            //userTerms.Add("Steve");
            ////if (!string.IsNullOrEmpty(queryString))
            ////{
            ////    userTerms = queryString.Split(' ').ToList();
            ////}
            //userId = null;
            //orgId = 4;
            //projId = 4;
            //var cards = (
            //            from t in db.timeSheets
            //            join o in db.organizations on t.org_Id equals o.Id
            //            join p in db.projects on t.project_Id equals p.Id
            //            join u in db.users on t.user_Id equals u.Id
            //            where (t.user_Id == userId) 
            //                && (t.org_Id == orgId)
            //                && (t.project_Id == projId) 
            //                && (t.clockInTime > rangeStart)
            //                && (t.clockInTime < rangeEnd)      //&& (userTerms.Contains(u.firstName))    
            //            orderby t.clockInTime descending
            //            select new TimeCardVM()
            //            {
            //                timeId = t.Id,
            //                userId = t.user_Id,
            //                projId = t.project_Id,
            //                orgId = t.org_Id,
            //                inTime = t.clockInTime,
            //                outTime = t.clockOutTime,
            //                orgName = o.name,
            //                projName = p.name,
            //                volName = u.emailAddress,//u.firstName + " " + u.lastName,
            //                elapsedHrs = 0.0
            //            })    
            //        .Skip(itemsPerPage * page)
            //        .Take(itemsPerPage).ToList();
            //totalTimeCards = cards.Count;
            //return new ReturnStatus { errorCode = ReturnStatus.ALL_CLEAR, data = cards };
        }

        // deprecated

        //public static ReturnStatus GetTimeSheetsByFilters(int orgNum, int projNum, DateTime strt, DateTime end)
        //{
        //    ReturnStatus st = new ReturnStatus();
        //    st.data = new List<TimeSheet>();

        //    try
        //    {
        //        VolunteerDbContext db = new VolunteerDbContext();
        //        int? org = null;
        //        int? proj = null;
        //        var query = db.timeSheets.AsQueryable();
        //        if (org != null)
        //        {
        //            query = query.Where(t => t.org_Id == org);
        //        }
        //        if (proj != null)
        //        {
        //            query = query.Where(t => t.project_Id == proj);
        //        }
        //        st.data = query.OrderByDescending(x => x.clockInTime).ToList();
        //        //if (orgNum > 0 && projNum > 0)
        //        //{
        //        //    st.data = db.timeSheets.Where(
        //        //        x => x.org_Id == orgNum
        //        //        && x.project_Id == projNum
        //        //        && x.clockInTime >= strt
        //        //        && x.clockOutTime <= end).OrderByDescending(x => x.clockInTime).ToList();
        //        //}
        //        //else if (orgNum > 0)
        //        //{
        //        //    st.data = db.timeSheets.Where(
        //        //       x => x.org_Id == orgNum
        //        //       && x.clockInTime >= strt
        //        //       && x.clockOutTime <= end).OrderByDescending(x => x.clockInTime).ToList();
        //        //}
        //        //else if (projNum > 0)
        //        //{
        //        //    st.data = db.timeSheets.Where(
        //        //       x => x.project_Id == projNum
        //        //       && x.clockInTime >= strt
        //        //       && x.clockOutTime <= end).OrderByDescending(x => x.clockInTime).ToList();
        //        //}
        //        //else
        //        //{
        //        //    st.data = db.timeSheets.Where(
        //        //      x => x.clockInTime >= strt
        //        //      && x.clockOutTime <= end).OrderByDescending(x => x.clockInTime).ToList();
        //        //}
        //        st.errorCode = 0;
        //        return st;
        //    }
        //    catch (Exception e)
        //    {
        //        st.errorCode = -1;
        //        st.errorMessage = e.ToString();
        //        return st;
        //    }
        //}


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

        public static ReturnStatus GetBadTimeSheets()
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new List<TimeSheet>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var sheets = db.timeSheets.Where(t => t.clockInTime < DateTime.Today && t.clockOutTime.Hour == 0)
                    .OrderByDescending(c => c.clockInTime).
                    ToList();

                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                st.data = sheets;
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


    }
}