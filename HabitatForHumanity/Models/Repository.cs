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
            ReturnStatus st = new ReturnStatus();

            try
            {
                bool exists = false;

                st = User.GetUserByEmail(loginVm.email);

                //check status to make sure error code and data are correct
                if (ReturnStatus.tryParseUser(st, out User user))
                {
                    if (user != null && Crypto.VerifyHashedPassword(user.password, loginVm.password))
                    {
                        exists = true;
                    }
                }

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = exists;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_AUTHENTICATE_USER;
                st.data = "Failed to authenticate user.";
                st.errorMessage = e.ToString();
                return st;
            }
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
        /// Get a single user out of the database with a matching first and last name.
        /// Only to be used when you know the exact names
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Id of the returned user</returns>
        public static ReturnStatus GetUserByName(string firstName, string lastName)
        {
            //set both names to lowercase to avoid errors
            return User.GetUserByName(firstName.ToLower(), lastName.ToLower());
        }

        /// <summary>
        /// Gets all the users with matching names. To be used when you know one name, but not the other. 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>List of users</returns>
        public static ReturnStatus GetUsersByName(string firstName, string lastName)
        {
            //set both names to lowercase to avoid errors
            return User.GetUsersByName(firstName.ToLower(), lastName.ToLower());
        }


        /// <summary>
        /// Changes the user password and hashes it.
        /// </summary>
        /// <param name="email">Email of current user.</param>
        /// <param name="newPW">New password.</param>
        /// <returns>ReturnStatus object with error code and data</returns>
        public static ReturnStatus ChangePassword(string email, string newPW)
        {
            ReturnStatus st = new ReturnStatus();
            // User user = new User();

            try
            {
                st = User.GetUserByEmail(email);

                if (ReturnStatus.tryParseUser(st, out User user))
                {
                    if (user != null && !String.IsNullOrEmpty(newPW) && !String.IsNullOrWhiteSpace(newPW))
                    {
                        user.password = Crypto.HashPassword(newPW);
                        EditUser(user);
                    }
                }

                st.data = null; //to avoid sending user data up the pipeline
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = -1;
                st.data = "Failed to change password.";
                st.errorMessage = e.ToString();
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

        #endregion

        #region TimeSheet functions


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


        public static ReturnStatus GetClockedInUserTimeSheet(int userId)
        {
            return TimeSheet.GetClockedInUserTimeSheet(userId);
        }

        public static ReturnStatus GetPunchInVM(int userId)
        {
            ReturnStatus projList = GetAllProjects();
            if (projList == null
                || projList.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR
                    || ((List<Project>)projList.data).Count() < 1)
            {
                return new ReturnStatus()
                {
                    errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE,
                    userErrorMsg = "Testing, get punchinvm in repo broke"
                };
            }
            PunchInVM punch = new PunchInVM((List<Project>)projList.data);
            ReturnStatus st = new ReturnStatus();

            // User user = GetUser(userId); 

            try
            {
                st = User.GetUser(userId);

                if (st.errorCode == 0 && st.data != null)
                {
                    User user = (User)st.data;
                    punch.userId = userId;
                    punch.userName = user.firstName + " " + user.lastName;

                    //reset values in st to all good
                    st.errorCode = 0;
                    st.data = punch;

                    return st;
                }
                else
                {
                    //if st was null or had bad error code
                    return st;
                }
            }
            catch (Exception e)
            {
                //TODO: improve error handling
                st.errorCode = -1;
                st.data = e.ToString();
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

        #region OrgUser functions
        /*
        


        /// <summary>
        /// Gets all the OrgUsers in the database.
        /// </summary>
        /// <returns>A list of OrgUser</returns>
        public static List<OrgUser> GetAllOrgUsers()
        {
            return OrgUser.GetAllOrgUsers();
        }

        /// <summary>
        /// Gets all the organization ids for a particular user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <returns></returns>
        public static List<OrgUser> GetOrgUserByUserId(int userId)
        {
            return OrgUser.GetOrgUserByUserId(userId);
        }

        /// <summary>
        /// Gets all the users that belong to a particular organization
        /// </summary>
        /// <param name="orgId">The id of the organization.</param>
        /// <returns></returns>
        public static List<OrgUser> GetOrgUserByOrgId(int orgId)
        {
            return OrgUser.GetOrgUserByOrgId(orgId);
        }

        /// <summary>
        /// Adds an OrgUser object to the database.
        /// </summary>
        /// <param name="orgUser">Object with user_Id and org_Id</param>
        public static void AddOrgUser(OrgUser orgUser)
        {
            OrgUser.AddOrgUser(orgUser);
        }

        /// <summary>
        /// Adds an OrgUser to the database by ids.
        /// </summary>
        /// <param name="orgId">Id of the organization</param>
        /// <param name="userId">Id of the user.</param>
        public static void AddOrgUserByIds(int orgId, int userId)
        {
            OrgUser.AddOrgUserByIds(orgId, userId);
        }

        /// <summary>
        /// Deletes the OrgUser from the database by object.
        /// </summary>
        /// <param name="ou"></param>
        public static void DeleteOrgUser(OrgUser ou)
        {
            OrgUser.DeleteOrgUser(ou);
        }

        /// <summary>
        /// Deletes an OrgUser with the given orgId and userId.
        /// </summary>
        /// <param name="orgId">Id of the organization.</param>
        /// <param name="userId">Id of the user.</param>
        public static void DeleteOrgUserByIds(int orgId, int userId)
        {
            OrgUser.DeleteOrgUserByIds(orgId, userId);
        }


   
    */
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
            ReturnStatus ret = new ReturnStatus();

            DateTime userClockedIn = DateTime.Today.AddDays(1);
            List<TimeSheet> temp = new List<TimeSheet>();
            List<TimeSheet> volunteerTimes = new List<TimeSheet>();
            ReturnStatus st = new ReturnStatus();
            st.data = new List<TimeSheet>();
            try
            {
                st = GetAllTimeSheetsByVolunteer(volunteerId);
                if(st.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                {
                    ret.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                    ret.userErrorMsg = "Your request cannot be processed at this time."; ;
                    return ret;
                }
                temp = (List<TimeSheet>)st.data;
                if(temp!= null && temp.Count() > 0)
                {
                    foreach (TimeSheet ts in temp)
                    {
                        if (ts.clockOutTime != userClockedIn)
                            volunteerTimes.Add(ts);
                    }
                    TimeSpan totalHours = AddTimeSheetHours(volunteerTimes);
                    ret.data =  Math.Round(totalHours.TotalHours, 2, MidpointRounding.AwayFromZero);  
                }     
            }
            catch(Exception e)
            {
                ret.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                ret.data = 0.0;
                ret.userErrorMsg = "No timecards found for that volunteer.";
                ret.errorMessage = e.Message;
            }
            return ret;
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
                if(st.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                {
                    st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                    st.userErrorMsg = "Problem loading demographics data, try again later.";
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
        #endregion

    }
}