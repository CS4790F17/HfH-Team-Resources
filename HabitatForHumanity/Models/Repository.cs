using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using HabitatForHumanity.ViewModels;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.Models
{
    public class Repository
    {
        #region User functions

        /// <summary>
        /// Checks whether the user entered a bad password for that log in email.
        /// </summary>
        /// <param name="loginVm">The viewmodel containing the users email and password.</param>
        /// <returns>ReturnStatus object that contains true if user entered a correct password.</returns>
        public static ReturnStatus AuthenticateUser(LoginVM loginVm)
        {
            ReturnStatus userReturn = new ReturnStatus();
            userReturn.data = new User();
            ReturnStatus retValue = new ReturnStatus();

            try
            {
                userReturn = User.GetUserByEmail(loginVm.email);

                if (userReturn.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    retValue.errorCode = -1;
                    //retValue.userErrorMsg = "User not found";
                    retValue.data = false;
                    return retValue;
                }
                User user = (User)userReturn.data;
                if (user != null && user.Id > 0 && Crypto.VerifyHashedPassword(user.password, loginVm.password))
                {
                    retValue.errorCode = ReturnStatus.ALL_CLEAR;
                    retValue.data = true;
                }
                return retValue;
            }
            catch (Exception e)
            {
                retValue.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                retValue.errorMessage = e.ToString();
                return retValue;
            }
        }

        public static ReturnStatus GetAllVolunteers(int projectId, int orgId)
        {
            ReturnStatus rs = new ReturnStatus();

            #region if filter by project
            if (projectId > 0 || orgId > 0)
            {
                ReturnStatus projectUsersReturn = TimeSheet.GetUsersbyTimeSheetFilters(projectId,orgId);
                if(projectUsersReturn.errorCode == 0)
                {
                    List<User> users = (List<User>)projectUsersReturn.data;
                    List<UsersVM> volunteers = new List<UsersVM>();
                    foreach (User u in users)
                    {
                        volunteers.Add(new UsersVM()
                        {
                            userNumber = u.Id,
                            // force alll name to not be null for simple comparison incontroller
                            volunteerName = u.firstName ?? "NoName" + " " + u.lastName ?? "NoName",
                            email = u.emailAddress,
                            hoursToDate = 99.9
                        });
                    }
                    rs.data = volunteers;
                    rs.errorCode = 0;
                }
                else
                {
                    rs.errorCode = -2;
                }

                return rs;
            }
            #endregion

            ReturnStatus userResult = User.GetAllUsers();

            if (userResult.errorCode == 0)
            {
                List<User> users = (List<User>)userResult.data;
                List<UsersVM> volunteers = new List<UsersVM>();
                foreach (User u in users)
                {
                    volunteers.Add(new UsersVM()
                    {
                        userNumber = u.Id,
                        // force alll name to not be null for simple comparison incontroller
                        volunteerName = u.firstName ?? "NoName" + " " + u.lastName ?? "NoName",
                        email = u.emailAddress,
                        hoursToDate = 99.9
                    });
                }
                rs.data = volunteers;
                rs.errorCode = 0;
            }
            else
            {
                rs.errorCode = -1;
            }

            return rs;
        }


    /// <summary>
    /// Creates a volunteer user
    /// </summary>
    /// <param name="user"></param>
    public static ReturnStatus CreateVolunteer(User user)
        {
            //if (user.password != null)
            //{
            //    user.password = Crypto.HashPassword(user.password);
            //}
            //User.CreateVolunteer(user);
            return User.CreateUser(user);
        }

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>The id of the user or 0 if no user could be added.</returns>
        public static ReturnStatus CreateUser(User user)
        {
            //user.isAdmin = 0;
            return User.CreateUser(user);
        }

        /// <summary>
        /// Finds email if it exists in the database.
        /// </summary>
        /// <returns>True if email exists</returns>
        /// <param name="email">Email to search for.</param>
        public static ReturnStatus EmailExists(string email)
        {
            return User.EmailExists(email);
        }


        public static ReturnStatus GetUser(int id)
        {
            ReturnStatus st = User.GetUser(id);
            return st;
        }

        //public static User GetUser(int id)
        //{
        //    return User.GetUser(id);
        //}

        /// <summary>
        /// Gets the user in the database with the matching email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User with matching email address.</returns>
        public static ReturnStatus GetUserByEmail(string email)
        {
            return User.GetUserByEmail(email);
        }



        /// <summary>
        /// Gets all the users with matching names. To be used when you know one name, but not the other. 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>List of users</returns>
        public static ReturnStatus GetUsersByName(string firstName, string lastName)
        {
            if (firstName != null)
                firstName = firstName.ToLower();
            if (lastName != null)
                lastName = lastName.ToLower();


            //set both names to lowercase to avoid errors
            return User.GetUsersByName(firstName, lastName);
        }


        /// <summary>
        /// Changes the user password and hashes it.
        /// </summary>
        /// <param name="email">Email of current user.</param>
        /// <param name="newPW">New password.</param>
        /// <returns>ReturnStatus object with error code and data</returns>
        public static ReturnStatus ChangePassword(string email, string newPW)
        {
            ReturnStatus ret = new ReturnStatus();
            ret.data = null;

            ReturnStatus st = new ReturnStatus();
            st.data = new User();

            try
            {
                st = User.GetUserByEmail(email);
                if (st.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    ret.errorCode = -1;
                    return ret;
                }
                User user = (User)st.data;
                if (user != null && !string.IsNullOrEmpty(newPW) && !string.IsNullOrWhiteSpace(newPW))
                {

                    user.password = Crypto.HashPassword(newPW);
                    ret = EditUser(user);
                }

                return ret;
            }
            catch (Exception e)
            {
                ret.errorCode = -1;
                ret.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Updates the users information based on a new model.
        /// </summary>
        /// <param name="user">User object with new information.</param>
        public static ReturnStatus EditUser(User user)
        {
            return User.EditUser(user);
        }


        /// <summary>
        /// Deletes the user from the database.
        /// </summary>
        /// <param name="user">The user object to be deleted.</param>
        public static ReturnStatus DeleteUser(User user)
        {
            return User.DeleteUser(user);
        }

        /// <summary>
        /// Deletes the user in the database with matching id.
        /// </summary>
        /// <param name="id"></param>
        public static ReturnStatus DeleteUserById(int id)
        {
            return User.DeleteUserById(id);
        }



        #endregion

        #region Project functions

        /// <summary>
        /// Get all projects in the database.
        /// </summary>
        /// <returns>A list of all projects.</returns>
        //public static List<Project> GetAllProjects()
        //{
        //    return Project.GetAllProjects();
        //}
        public static ReturnStatus GetAllProjects()
        {
            return Project.GetAllProjects();
        }

        /// <summary>
        /// Get a single project by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single project object with a matching id or null otherwise.</returns>
        //public static Project GetProjectById(int id)
        //{
        //    return Project.GetProjectById(id);
        //}
        public static ReturnStatus GetProjectById(int id)
        {
            return Project.GetProjectById(id);
        }

 

        /// <summary>
        /// Gets all the currently active projects
        /// </summary>
        /// <returns>A list of all projects that are currently active.</returns>
        //public static List<Project> GetActiveProjects()
        //{
        //    return Project.GetActiveProjects();
        //}
        public static ReturnStatus GetActiveProjects()
        {
            return Project.GetActiveProjects();
        }

        /// <summary>
        /// Gets a project by its primary key: name+beginDate. Date must be in the format MM/DD/YYYY.
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="date">MM/DD/YYYY</param>
        /// <returns></returns>
        //public static Project GetProjectByNameAndDate(string name, string date)
        //{
        //    return Project.GetProjectByNameAndDate(name, date);
        //}
        public static ReturnStatus GetProjectByNameAndDate(string name, string date)
        {
            return Project.GetProjectByNameAndDate(name, date);
        }

        /// <summary>
        /// Inserts a project into the database.
        /// </summary>
        /// <param name="project">The new project to be inserted.</param>
        //public static void AddProject(Project project)
        //{
        //    Project.AddProject(project);
        //}
        public static ReturnStatus AddProject(Project project)
        {
            return Project.AddProject(project);
        }

        /// <summary>
        /// Edit the project with new values.
        /// </summary>
        /// <param name="project">Project object where new values are stored.</param>
        //public static void EditProject(Project project)
        //{
        //    Project.EditProject(project);
        //}
        public static ReturnStatus EditProject(Project project)
        {
            return Project.EditProject(project);
        }

        /// <summary>
        /// Deletes a project from the database.
        /// </summary>
        /// <param name="project">The project object to delete.</param>
        //public static void DeleteProject(Project project)
        //{
        //    Project.DeleteProject(project);
        //}

        ///// <summary>
        ///// Deletes a project from the database by id.
        ///// </summary>
        ///// <param name="id">The id of the project to delete</param>
        //public static void DeleteProjectById(int id)
        //{
        //    Project.DeleteProjectById(id);
        //}

        #endregion

        #region Organization functions

        /// <summary>
        /// Get all organizations in the database.
        /// </summary>
        /// <returns>A list of all organizations.</returns>
        public static ReturnStatus GetAllOrganizations()
        {
            return Organization.GetAllOrganizations();
        }

        /// <summary>
        /// Get a single organization by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single organization object with a matching id otherwise null.</returns>
        public static ReturnStatus GetOrganizationById(int id)
        {
            return Organization.GetOrganizationById(id);
        }

        /// <summary>
        /// Get a single organization by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A single organization object with a matching name otherwise null.</returns>
        public static ReturnStatus GetOrganizationByName(string name)
        {
            return Organization.GetOrganizationByName(name);
        }


        /// <summary>
        /// Adds an organization to the database.
        /// </summary>
        /// <param name="org">The organization to be added</param>
        public static ReturnStatus AddOrganization(Organization org)
        {
            return Organization.AddOrganization(org);
        }

        /// <summary>
        /// Edits the organization with new values.
        /// </summary>
        /// <param name="org">The organization object with new values.</param>
        public static ReturnStatus EditOrganization(Organization org)
        {
            return Organization.EditOrganization(org);
        }

        /// <summary>
        /// Deletes an organization from the database.
        /// </summary>
        /// <param name="org">The organization object to delete</param>
        public static ReturnStatus DeleteOrganization(Organization org)
        {
            return Organization.DeleteOrganization(org);
        }

        /// <summary>
        /// Deletes an organization from the database by id.
        /// </summary>
        /// <param name="id">The id of the organization to delete.</param>
        public static ReturnStatus DeleteOrganizationById(int id)
        {
            return Organization.DeleteOrganizationById(id);
        }


        public static ReturnStatus GetOrganizationSQL(string queryFilter, int status)
        {
            return Organization.GetOrganizationSQL(queryFilter, status);
        }

        public static ReturnStatus GetOrganizationByNameSQL(string name)
        {
            return Organization.GetOrganizationByNameSQL(name);
        }


        #endregion

        #region TimeSheet functions

        #region TimeCard VMs by filters
        public static ReturnStatus GetTimeCardsByFilters(int? orgNum, int? projNum, DateTime strt, DateTime end)
        {
            ReturnStatus timeSheetsResult = TimeSheet.GetTimeSheetsByFilters(orgNum, projNum, strt, end);
            ReturnStatus timeCardsReturn = new ReturnStatus();
            List<TimeCardVM> cards = new List<TimeCardVM>();
            List<TimeSheet> sheets = new List<TimeSheet>();
            if(timeSheetsResult.errorCode == 0)
            {
                sheets = (List<TimeSheet>)timeSheetsResult.data;
                foreach(TimeSheet ts in sheets)
                {
                    TimeSpan span = ts.clockOutTime.Subtract(ts.clockInTime);

                    cards.Add(new TimeCardVM()
                    {
                        timeId = ts.Id,
                        userId = ts.user_Id,
                        projId = ts.project_Id,
                        orgId = ts.org_Id,
                        inTime = ts.clockInTime,
                        outTime = ts.clockOutTime,
                        orgName = GetOrgName(ts.org_Id),
                        projName = GetProjName(ts.project_Id),
                        volName = GetVolName(ts.user_Id),
                        elapsedHrs = span.Hours + span.Minutes / 60.0
                    });
                }
                timeCardsReturn.data = cards;
                timeCardsReturn.errorCode = 0;
                return timeCardsReturn;
            }
            else
            {
                timeCardsReturn.errorCode = -1;
                return timeCardsReturn;
            }

        }

        public static string GetOrgName(int orgId)
        {
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                return db.organizations.Where(o => o.Id == orgId).ToList().FirstOrDefault().name;
            }
            catch
            {
                return "none";
            }
      
        }
        public static string GetProjName(int projId)
        {
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                return db.projects.Where(o => o.Id == projId).ToList().FirstOrDefault().name;
            }
            catch
            {
                return "none";
            }
        }
        public static string GetVolName(int userId)
        {
            try
            {
                string volName = "";
                VolunteerDbContext db = new VolunteerDbContext();
                var fname = db.users.Where(o => o.Id == userId).ToList().FirstOrDefault().firstName;
                var lname = db.users.Where(o => o.Id == userId).ToList().FirstOrDefault().lastName;
                var email = db.users.Where(o => o.Id == userId).ToList().FirstOrDefault().emailAddress;
                volName += (fname != null) ? fname : email;
                volName += " ";
                volName += (lname != null) ? lname : email;
                return volName;
            }
            catch
            {
                return "No Name";
            }
        }
        #endregion timecard vms

        public static ReturnStatus EditTimeCard(TimeCardVM card)
        {
            TimeSheet ts = new TimeSheet();
            ts.Id = card.timeId;
            ts.user_Id = card.userId;
            ts.project_Id = card.projId;
            ts.org_Id = card.orgId;
            ts.clockInTime = card.inTime;
            ts.clockOutTime = card.outTime;

            return EditTimeSheet(ts);
        }

        /// <summary>
        /// Gets the record in the timesheet table by it's natural key: user_id+project_id+clockInTime.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="projectId">Id of the project</param>
        /// <param name="clockInTime">MM/DD/YYYY</param>
        /// <returns>Timesheet Object</returns>
        public static ReturnStatus GetTimeSheetByNaturalKey(int userId, int projectId, string clockInTime)
        {
            return TimeSheet.GetTimeSheetByNaturalKey(userId, projectId, clockInTime);
        }

        /// <summary>
        /// Gets all timesheets for a specific project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static ReturnStatus GetAllTimeSheetsByProjectId(int projectId)
        {
            return TimeSheet.GetAllTimeSheetsByProjectId(projectId);
        }

        /// <summary>
        /// Gets all the timesheets for a single volunteer
        /// </summary>
        /// <param name="volunteerId"></param>
        public static ReturnStatus GetAllTimeSheetsByVolunteer(int volunteerId)
        {
            return TimeSheet.GetAllVolunteerTimeSheets(volunteerId);
        }

        /// <summary>
        /// Gets all the timesheets for an organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static ReturnStatus GetAllTimeSheetsByOrganizationId(int organizationId)
        {
            return TimeSheet.GetAllTimeSheetsByOrganizationid(organizationId);
        }


        /// <summary>
        /// Gets all the timesheets within a specified date range.
        /// </summary>
        /// <param name="beginDate">Datetime represntation of the begin date</param>
        /// <param name="endDate">Datetime represntation of the begin date</param>
        /// <returns>ReturnStatus object with errorCode and data</returns>
        public static ReturnStatus GetAllTimeSheetsInDateRange(DateTime beginDate, DateTime endDate)
        {
            return TimeSheet.GetAllTimeSheetsInDateRange(beginDate, endDate);
        }


        /// <summary>
        /// Get the TimeSheet with the matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A TimeSheet object with matching id otherwise null.</returns>
        public static ReturnStatus GetTimeSheetById(int id)
        {
            return TimeSheet.GetTimeSheetById(id);
        }

        /// <summary>
        /// Adds the TimeSheet to the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to add.</param>
        public static ReturnStatus InsertTimeSheet(TimeSheet ts)
        {
            return TimeSheet.InsertTimeSheet(ts);
        }

        /// <summary>
        /// Updates the timesheet with new information.
        /// </summary>
        /// <param name="ts">TimeSheet object with new values.</param>
        public static ReturnStatus EditTimeSheet(TimeSheet ts)
        {
            return TimeSheet.EditTimeSheet(ts);
        }

        /// <summary>
        /// Deletes the TimeSheet from the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to be deleted.</param>
        public static ReturnStatus DeleteTimeSheet(TimeSheet ts)
        {
            return TimeSheet.DeleteTimeSheet(ts);
        }

        /// <summary>
        /// Deletes the TimeSheet from the database with the matching id.
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteTimeSheetById(int id)
        {
            TimeSheet.DeleteTimeSheetById(id);
        }

        public static bool IsUserClockedIn(int userId)
        {
            ReturnStatus rs = TimeSheet.GetClockedInUserTimeSheet(userId);
            TimeSheet userTimeSheet = (TimeSheet)rs.data;

            //if only a default timesheet was found then the user isn't "clocked in"
            if (userTimeSheet.Id < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static ReturnStatus GetClockedInUserTimeSheet(int userId)
        {
            return TimeSheet.GetClockedInUserTimeSheet(userId);
        }

        public static PortalVM GetPortalVM(int id)
        {
            ReturnStatus st = Repository.GetUser(id);

            PortalVM portalVM = new PortalVM();
            if (st.errorCode == ReturnStatus.ALL_CLEAR)
            {
                User user = (User)st.data;

                portalVM.fullName = user.firstName + " " + user.lastName;
                portalVM.userId = user.Id;
                portalVM.isPunchedIn = Repository.IsUserClockedIn(user.Id);
                portalVM.cumulativeHours = (double)Repository.getTotalHoursWorkedByVolunteer(user.Id).data;
            }
            return portalVM;
        }


        public static ReturnStatus GetPunchInVM(int userId)
        {
            //ReturnStatus projList = GetAllProjects();

            //if (projList == null || projList.errorCode != ReturnStatus.ALL_CLEAR || ((List<Project>)projList.data).Count() < 1)
            //{
            //    return new ReturnStatus()
            //    {
            //        errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE,
            //    };
            //}

            //ReturnStatus orgResult = GetAllOrganizations();
            //if (orgResult.errorCode != ReturnStatus.ALL_CLEAR)
            //{
            //    return new ReturnStatus()
            //    {
            //        errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE,
            //    };
            //}

            PunchInVM punch = new PunchInVM();
            ReturnStatus st = new ReturnStatus();


            st = User.GetUser(userId);

            if (st.errorCode == ReturnStatus.ALL_CLEAR && st.data != null)
            {
                User user = (User)st.data;
                punch.userId = userId;
                // punch.userName = user.firstName + " " + user.lastName;

                //reset values in st to all good
                st.errorCode = 0;
                st.data = punch;

                return st;
            }
            else
            {
                //if st was null or had bad error code
                st.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                return st;
            }
        }



        public static ReturnStatus UpdateTimeSheet(TimeSheet timeSheet)
        {
            //TimeSheet.UpdateTimeSheet(timeSheet);
            return TimeSheet.EditTimeSheet(timeSheet);
        }

        public static ReturnStatus PunchIn(TimeSheet ts)
        {
            return TimeSheet.InsertTimeSheet(ts);
        }

        #endregion


        #region Report functions

        //public static double getTotalHoursWorkedByVolunteer(int volunteerId)
        //{
        //    DateTime userClockedIn = DateTime.Today.AddDays(1);
        //    //List<TimeSheet> temp = GetAllTimeSheetsByVolunteer(volunteerId)
        //    List<TimeSheet> volunteerTimes = new List<TimeSheet>();
        //    ReturnStatus st = GetAllTimeSheetsByVolunteer(volunteerId);

        //    if (ReturnStatus.tryParseTimeSheetList(st, out List < TimeSheet > temp))
        //    {
        //        foreach (TimeSheet ts in temp)
        //        {
        //            if (ts.clockOutTime != userClockedIn)
        //                volunteerTimes.Add(ts);
        //        }
        //        TimeSpan totalHours = AddTimeSheetHours(volunteerTimes);
        //        return Math.Round(totalHours.TotalHours, 2, MidpointRounding.AwayFromZero);
        //        //   return 0;
        //    }
        //    return 0; //no timesheets were found
        //}
        public static ReturnStatus getTotalHoursWorkedByVolunteer(int volunteerId)
        {
            ReturnStatus hoursWorked = new ReturnStatus();
            hoursWorked.data = 0.0;


            DateTime userClockedIn = DateTime.Today.AddDays(1);
            List<TimeSheet> temp = new List<TimeSheet>();
            List<TimeSheet> volunteerTimes = new List<TimeSheet>();
            ReturnStatus st = new ReturnStatus();
            st.data = new List<TimeSheet>();

            st = GetAllTimeSheetsByVolunteer(volunteerId);
            if (st.errorCode != ReturnStatus.ALL_CLEAR)
            {
                //ret.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = 0.0;
                return st;
            }
            temp = (List<TimeSheet>)st.data;
            if (temp != null && temp.Count() > 0)
            {
                foreach (TimeSheet ts in temp)
                {
                    if (ts.clockOutTime != userClockedIn)
                        volunteerTimes.Add(ts);
                }
                TimeSpan totalHours = AddTimeSheetHours(volunteerTimes);
                hoursWorked.data = Math.Round(totalHours.TotalHours, 2, MidpointRounding.AwayFromZero);
            }

            return hoursWorked;
        }




        /// <summary>
        /// Takes a refence to a list and adds all the worked hours up into a total.
        /// </summary>
        /// <param name="ts">List of timesheets to calculate hours on.</param>
        /// <returns>A timespan object with the total time worked.</returns>
        public static TimeSpan AddTimeSheetHours(List<TimeSheet> ts)
        {
            TimeSpan hoursWorked = TimeSpan.Zero;
            foreach (TimeSheet sheet in ts)
            {
                hoursWorked += sheet.clockOutTime - sheet.clockInTime;
            }
            return hoursWorked;
        }

        //public static List<User.Demog> GetDemographicsForPie(string gender)
        //{
        //    return User.GetDemographicsForPie(gender);
        //}
        public static ReturnStatus GetDemographicsForPie(string gender)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new List<User.Demog>();
            try
            {
                st = User.GetDemographicsForPie(gender);
                if (st.errorCode != (int)ReturnStatus.ALL_CLEAR)
                {
                    st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                    return st;
                }
            }
            catch
            {
                //log something
            }
            return User.GetDemographicsForPie(gender);
        }

        public static ReturnStatus GetBadTimeSheets()
        {
            return TimeSheet.GetBadTimeSheets();
        }

        /// <summary>
        /// return total hours logged into given project
        /// </summary>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
        public static ReturnStatus getTotalHoursLoggedIntoProject(int projectId)
        {
            ReturnStatus hoursLogged = new ReturnStatus();
            hoursLogged.data = 0.0;


            DateTime userClockedIn = DateTime.Today.AddDays(1);
            List<TimeSheet> temp = new List<TimeSheet>();
            List<TimeSheet> volunteerHours = new List<TimeSheet>();
            ReturnStatus st = new ReturnStatus();
            st.data = new List<TimeSheet>();

            st = GetAllTimeSheetsByProjectId(projectId);
            if (st.errorCode != ReturnStatus.ALL_CLEAR)
            {
                st.data = 0.0;
                return st;
            }
            temp = (List<TimeSheet>)st.data;
            if (temp != null && temp.Count() > 0)
            {
                foreach (TimeSheet ts in temp)
                {
                    if (ts.clockOutTime != userClockedIn)
                        volunteerHours.Add(ts);
                }
                TimeSpan totalHours = AddTimeSheetHours(volunteerHours);
                hoursLogged.data = Math.Round(totalHours.TotalHours, 2, MidpointRounding.AwayFromZero);
            }

            return hoursLogged;
        }

        #endregion
    }
}