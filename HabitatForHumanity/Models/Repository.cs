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
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>The id of the user or 0 if no user could be added.</returns>
        public static int CreateUser(User user)
        {
            user.password = Crypto.HashPassword(user.password);
            user.isAdmin = 0;
            user.waiverSignDate = DateTime.Today;
            return User.CreateUser(user);
        }

        /// <summary>
        /// Finds email if it exists in the database.
        /// </summary>
        /// <returns>True if email exists</returns>
        /// <param name="email">Email to search for.</param>
        public static bool EmailExists(string email)
        {
            return User.EmailExists(email);
        }


        /// <summary>
        /// Checks whether the user entered a bad password for that log in email.
        /// </summary>
        /// <param name="loginVm">The viewmodel containing the users email and password.</param>
        /// <returns>True if user entered a correct password.</returns>
        public static bool AuthenticateUser(LoginVM loginVm)
        {
            bool exists = false;
            User user = User.GetUserByEmail(loginVm.email);
            if (user != null && Crypto.VerifyHashedPassword(user.password, loginVm.password))
            {
                exists = true;
            }
            return exists;
        }

        public static User GetUser(int id)
        {
            return User.GetUser(id);
        }

        /// <summary>
        /// Gets the user in the database with the matching email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User with matching email address.</returns>
        public static User GetUserByEmail(string email)
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
        public static int GetUserByName(string firstName, string lastName)
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
        public static List<User> GetUsersByName(string firstName, string lastName)
        {
            //set both names to lowercase to avoid errors
            return User.GetUsersByName(firstName.ToLower(), lastName.ToLower());
        }


        /// <summary>
        /// Changes the user password and hashes it.
        /// </summary>
        /// <param name="email">Email of current user.</param>
        /// <param name="newPW">New password.</param>
        public static void ChangePassword(string email, string newPW)
        {
            User user = new User();
            user = User.GetUserByEmail(email);
            if (user != null && !String.IsNullOrEmpty(newPW) && !String.IsNullOrWhiteSpace(newPW))
            {
                user.password = Crypto.HashPassword(newPW);
                EditUser(user);
            }
        }

        /// <summary>
        /// Updates the users information based on a new model.
        /// </summary>
        /// <param name="user">User object with new information.</param>
        public static void EditUser(User user)
        {
            User.EditUser(user);
        }


        /// <summary>
        /// Deletes the user from the database.
        /// </summary>
        /// <param name="user">The user object to be deleted.</param>
        public static void DeleteUser(User user)
        {
            User.DeleteUser(user);
        }

        /// <summary>
        /// Deletes the user in the database with matching id.
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteUserById(int id)
        {
            User.DeleteUserById(id);
        }



        #endregion

        #region Project functions

        /// <summary>
        /// Get all projects in the database.
        /// </summary>
        /// <returns>A list of all projects.</returns>
        public static List<Project> GetAllProjects()
        {
            return Project.GetAllProjects();
        }

        /// <summary>
        /// Get a single project by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single project object with a matching id or null otherwise.</returns>
        public static Project GetProjectById(int id)
        {
            return Project.GetProjectById(id);
        }

        /// <summary>
        /// Gets all the currently active projects
        /// </summary>
        /// <returns>A list of all projects that are currently active.</returns>
        public static List<Project> GetActiveProjects()
        {
            return Project.GetActiveProjects();
        }

        /// <summary>
        /// Gets a project by its primary key: name+beginDate. Date must be in the format MM/DD/YYYY.
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="date">MM/DD/YYYY</param>
        /// <returns></returns>
        public static Project GetProjectByNameAndDate(string name, string date)
        {
            return Project.GetProjectByNameAndDate(name, date);
        }

        /// <summary>
        /// Inserts a project into the database.
        /// </summary>
        /// <param name="project">The new project to be inserted.</param>
        public static void AddProject(Project project)
        {
            Project.AddProject(project);
        }

        /// <summary>
        /// Edit the project with new values.
        /// </summary>
        /// <param name="project">Project object where new values are stored.</param>
        public static void EditProject(Project project)
        {
            Project.EditProject(project);
        }

        /// <summary>
        /// Deletes a project from the database.
        /// </summary>
        /// <param name="project">The project object to delete.</param>
        public static void DeleteProject(Project project)
        {
            Project.DeleteProject(project);
        }

        /// <summary>
        /// Deletes a project from the database by id.
        /// </summary>
        /// <param name="id">The id of the project to delete</param>
        public static void DeleteProjectById(int id)
        {
            Project.DeleteProjectById(id);
        }

        #endregion

        #region Organization functions

        /// <summary>
        /// Get all organizations in the database.
        /// </summary>
        /// <returns>A list of all organizations.</returns>
        public static List<Organization> GetAllOrganizations()
        {
            return Organization.GetAllOrganizations();
        }

        /// <summary>
        /// Get a single organization by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single organization object with a matching id otherwise null.</returns>
        public static Organization GetOrganizationById(int id)
        {
            return Organization.GetOrganizationById(id);
        }

        /// <summary>
        /// Get a single organization by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A single organization object with a matching name otherwise null.</returns>
        public static Organization GetOrganizationByName(string name)
        {
            return Organization.GetOrganizationByName(name);
        }


        /// <summary>
        /// Adds an organization to the database.
        /// </summary>
        /// <param name="org">The organization to be added</param>
        public static void AddOrganization(Organization org)
        {
            Organization.AddOrganization(org);
        }

        /// <summary>
        /// Edits the organization with new values.
        /// </summary>
        /// <param name="org">The organization object with new values.</param>
        public static void EditOrganization(Organization org)
        {
            Organization.EditOrganization(org);
        }

        /// <summary>
        /// Deletes an organization from the database.
        /// </summary>
        /// <param name="org">The organization object to delete</param>
        public static void DeleteOrganization(Organization org)
        {
            Organization.DeleteOrganization(org);
        }

        /// <summary>
        /// Deletes an organization from the database by id.
        /// </summary>
        /// <param name="id">The id of the organization to delete.</param>
        public static void DeleteOrganizationById(int id)
        {
            Organization.DeleteOrganizationById(id);
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
        public static TimeSheet GetTimeSheetByNaturalKey(int userId, int projectId, string clockInTime)
        {
            return TimeSheet.GetTimeSheetByNaturalKey(userId, projectId, clockInTime);
        }

        /// <summary>
        /// Gets all timesheets for a specific project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static List<TimeSheet> GetAllTimeSheetsByProjectId(int projectId)
        {
            return TimeSheet.GetAllTimeSheetsByProjectId(projectId);
        }

        /// <summary>
        /// Gets all the timesheets for a single volunteer
        /// </summary>
        /// <param name="volunteerId"></param>
        public static List<TimeSheet> GetAllTimeSheetsByVolunteer(int volunteerId)
        {
            return TimeSheet.GetAllVolunteerTimeSheets(volunteerId);
        }

        /// <summary>
        /// Gets all the timesheets for an organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static List<TimeSheet> GetAllTimeSheetsByOrganizationId(int organizationId)
        {
            return TimeSheet.GetAllTimeSheetsByOrganizationid(organizationId);
        }


        /// <summary>
        /// Gets all the timesheets within a specified date range.
        /// </summary>
        /// <param name="beginDate">Datetime represntation of the begin date</param>
        /// <param name="endDate">Datetime represntation of the begin date</param>
        /// <returns></returns>
        public static List<TimeSheet> GetAllTimeSheetsInDateRange(DateTime beginDate, DateTime endDate)
        {
            return TimeSheet.GetAllTimeSheetsInDateRange(beginDate, endDate);
        }


        /// <summary>
        /// Get the TimeSheet with the matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A TimeSheet object with matching id otherwise null.</returns>
        public static TimeSheet GetTimeSheetById(int id)
        {
            return TimeSheet.GetTimeSheetById(id);
        }

        /// <summary>
        /// Adds the TimeSheet to the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to add.</param>
        public static void AddTimeSheet(TimeSheet ts)
        {
            TimeSheet.AddTimeSheet(ts);
        }

        /// <summary>
        /// Updates the timesheet with new information.
        /// </summary>
        /// <param name="ts">TimeSheet object with new values.</param>
        public static void EditTimeSheet(TimeSheet ts)
        {
            TimeSheet.EditTimeSheet(ts);
        }

        /// <summary>
        /// Deletes the TimeSheet from the database.
        /// </summary>
        /// <param name="ts">TimeSheet object to be deleted.</param>
        public static void DeleteTimeSheet(TimeSheet ts)
        {
            TimeSheet.DeleteTimeSheet(ts);
        }

        /// <summary>
        /// Deletes the TimeSheet from the database with the matching id.
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteTimeSheetById(int id)
        {
            TimeSheet.DeleteTimeSheetById(id);
        }


        public static TimeSheet GetClockedInUserTimeSheet(int userId)
        {
            return TimeSheet.GetClockedInUserTimeSheet(userId);
        }
        //public static PunchOutVM GetPunchClockVM(int userId)
        //{
        //    PunchOutVM punch = new PunchOutVM();

        //    User user = GetUser(userId);
        //    punch.userName = user.firstName + " " + user.lastName;
        //    punch.projectList = GetProjectListVMs();      
        //    punch.orgList = Organization.GetOrganizations();
        //    TimeSheet ts = TimeSheet.GetClockedInUserTimeSheet(userId);
        //    punch.timeSheet = ts;
        //    return punch;
        //}

        public static PunchInVM GetPunchInVM(int userId)
        {
            PunchInVM punch = new PunchInVM();
            User user = GetUser(userId);
            punch.userId = userId;
            punch.userName = user.firstName + " " + user.lastName;
           // punch.projectList = GetProjectListVMs();
           // punch.orgList = Organization.GetAllOrganizations();
            return punch;
        }

        public static void UpdateTimeSheet(TimeSheet timeSheet)
        {
            TimeSheet.UpdateTimeSheet(timeSheet);
        }

        public static void PunchIn(TimeSheet ts)
        {
            TimeSheet.InsertTimeSheet(ts);
        }
        public static List<ProjectListVM> GetProjectListVMs()
        {
            List<Project> projects = new List<Project>();
            List<ProjectListVM> p = new List<ProjectListVM>();
            projects = Project.GetActiveProjects();
            if (projects.Count() > 0)
            {
                foreach (Project proj in projects)
                {
                    p.Add(new ProjectListVM { Id = proj.Id, projectName = proj.name });
                }
            }
            return p;
        }

        #endregion

        /*
        #region OrgUser functions


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


        #endregion
    */


        #region Report functions


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


        #endregion




    }
}