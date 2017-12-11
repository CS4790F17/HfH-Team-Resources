using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using HabitatForHumanity.ViewModels;
using HabitatForHumanity.Models;
using PagedList;

namespace HabitatForHumanity.Models
{
    public class Repository
    {

        #region User functions

        /// <summary>
        /// Returns whether a user waiver is outdated
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ReturnStatus waiverNotSigned(int userId)
        {
            ReturnStatus rs = new ReturnStatus();

            try
            {
                rs = User.waiverNotSigned(userId);
                if (rs.errorCode != 0)
                {
                    rs.errorCode = -1;
                    rs.data = false;
                    return rs;
                }
                return rs;
            }
            catch
            {
                rs.errorCode = 1;
                return rs;
            }
        }

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
            retValue.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;

            try
            {
                userReturn = User.GetUserByEmail(loginVm.email);

                if (userReturn.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    retValue.errorCode = -1;
                    retValue.data = false;
                    return retValue;
                }
                User user = (User)userReturn.data;
                if (user != null && user.Id > 0 && Crypto.VerifyHashedPassword(user.password, loginVm.password))
                {
                    retValue.errorCode = 0;
                    retValue.data = true;
                }
                else
                {
                    retValue.errorCode = 0;
                    retValue.data = false;
                }
                return retValue;
            }
            catch
            {
                retValue.errorCode = 1;
                return retValue;
            }
        }

        public static ReturnStatus GetAllVolunteers(int projectId, int orgId)
        {
            ReturnStatus rs = new ReturnStatus();
            try {
                #region if filter by project
                if (projectId > 0 || orgId > 0)
                {
                    ReturnStatus projectUsersReturn = TimeSheet.GetUsersbyTimeSheetFilters(projectId, orgId);
                    if (projectUsersReturn.errorCode == 0)
                    {
                        List<User> users = (List<User>)projectUsersReturn.data;
                        List<UsersVM> volunteers = new List<UsersVM>();
                        foreach (User u in users)
                        {
                            double volHours = 0.0;
                            ReturnStatus hoursRS = getTotalHoursWorkedByVolunteer(u.Id);
                            if (hoursRS.errorCode == ReturnStatus.ALL_CLEAR)
                            {
                                volHours = (double)hoursRS.data;
                            }
                            volunteers.Add(new UsersVM()
                            {
                                userNumber = u.Id,
                                // force alll name to not be null for simple comparison incontroller
                                volunteerName = u.firstName + " " + u.lastName,
                                email = u.emailAddress,
                                hoursToDate = volHours
                            });
                        }
                        rs.data = volunteers;
                        rs.errorCode = ReturnStatus.ALL_CLEAR;
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
                    double volHours = 0.0;
                    ReturnStatus hoursRS = getTotalHoursWorkedByVolunteer(u.Id);
                    if (hoursRS.errorCode == ReturnStatus.ALL_CLEAR)
                    {
                        volHours = (double)hoursRS.data;
                    }
                    volunteers.Add(new UsersVM()
                    {
                        userNumber = u.Id,
                        // force alll name to not be null for simple comparison incontroller
                        volunteerName = u.firstName + " " + u.lastName,
                        email = u.emailAddress,
                        hoursToDate = volHours
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
            catch
            {
                rs.errorCode = -1;
                return rs;
            }
        }


        /// <summary>
        /// Creates a volunteer user
        /// </summary>
        /// <param name="user"></param>
        public static ReturnStatus CreateVolunteer(User user)
        {
            if (user.password != null)
            {
                user.password = Crypto.HashPassword(user.password);
            }
            return User.CreateVolunteer(user);
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
        /// Changes the user password and hashes it.
        /// </summary>
        /// <param name="email">Email of current user.</param>
        /// <param name="newPW">New password.</param>
        /// <returns>ReturnStatus object with error code and data</returns>
        public static ReturnStatus ChangePassword(string email, string newPW)
        {
            ReturnStatus ret = new ReturnStatus();
            ret.data = null;
            try
            {
                ReturnStatus st = new ReturnStatus();
                st.data = new User();
                         
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
                return ret;
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

        public static ReturnStatus GetDemographicsSurveyVM(string email)
        {
            ReturnStatus vmToReturn = new ReturnStatus();
            try
            {
                ReturnStatus userRS = Repository.GetUserByEmail(email);
                if (userRS.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    vmToReturn.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                    return vmToReturn;
                }
                User user = (User)userRS.data;
                DemographicsVM demographicsVM = new DemographicsVM();
                demographicsVM.volunteerId = (int)user.Id;

                vmToReturn.errorCode = ReturnStatus.ALL_CLEAR;
                vmToReturn.data = demographicsVM;
                return vmToReturn;
            }
            catch
            {
                vmToReturn.errorCode = -1;
                return vmToReturn;
            }
        }

        public static void saveWaiverSnapshot(User user, String signatureName)
        {
            //Wasnt sure where to use the ReturnStaus from here
            WaiverHistory snapshot = new WaiverHistory();
            snapshot.user_Id = user.Id;
            snapshot.firstName = user.firstName;
            snapshot.lastName = user.lastName;
            snapshot.homePhoneNumber = user.homePhoneNumber;
            snapshot.workPhoneNumber = user.workPhoneNumber;
            snapshot.emailAddress = user.emailAddress;
            snapshot.streetAddress = user.streetAddress;
            snapshot.city = user.city;
            snapshot.zip = user.zip;
            snapshot.birthDate = user.birthDate;
            snapshot.gender = user.gender;
            snapshot.waiverSignDate = user.waiverSignDate;
            snapshot.emergencyFirstName = user.emergencyFirstName;
            snapshot.emergencyLastName = user.emergencyLastName;
            snapshot.relation = user.relation;
            snapshot.emergencyHomePhone = user.emergencyHomePhone;
            snapshot.workPhoneNumber = user.workPhoneNumber;
            snapshot.emergencyStreetAddress = user.emergencyStreetAddress;
            snapshot.emergencyCity = user.emergencyCity;
            snapshot.emergencyZip = user.emergencyZip;
            snapshot.signatureName = signatureName;

            WaiverHistory.saveWaiverSnapshot(snapshot);
        }


        public static void SaveDemographicsSurvey(DemographicsVM dvm)
        {
            ReturnStatus userRS = GetUser(dvm.volunteerId);
            if(userRS.errorCode == ReturnStatus.ALL_CLEAR)
            {
                User user = (User)userRS.data;
                user.incomeTier = dvm.incomeTier;
                user.ethnicity = dvm.ethnicity;
                user.collegeStatus = dvm.collegeStatus;
                user.veteranStatus = dvm.veteranStatus;
                user.disabledStatus = dvm.disabledStatus;
                ReturnStatus updatedUserRS = EditUser(user);
            }
        }


        #endregion User

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
        public static ReturnStatus EditProject(Project project)
        {
            return Project.EditProject(project);
        }



        /// <summary>
        /// Builds a paginated list of projects to display with the _ProjectList partial view. 
        /// </summary>
        /// <param name="Page">The current page number. Page cannot be null or less than 1.</param>
        /// <param name="statusChoice">The currently selected status choice. 0 - All, 1 - Active, 2 - Inactive</param>
        /// <param name="queryString">The name of the project to search for.</param>
        /// <returns></returns>
        public static StaticPagedList<Project> GetProjectPageWithFilter(int? Page, int statusChoice, string queryString, int categorySelection)
        {
            int RecordsPerPage = 10;
            //page can't be 0 or below
            if (Page == null || Page < 1)
            {
                Page = 1;
            }

            int totalCount = 0;
            ReturnStatus st = new ReturnStatus();
            switch (statusChoice)
            {
                case 0:
                    st = Project.GetProjectPage((Page.Value) - 1, RecordsPerPage, ref totalCount, queryString, categorySelection);
                    break;
                case 1:
                    //search for all active projects
                    st = Project.GetProjectPageWithFilter((Page.Value) - 1, RecordsPerPage, ref totalCount, 1, queryString, categorySelection);
                    break;
                case 2:
                    //search for all inactive projects
                    st = Project.GetProjectPageWithFilter((Page.Value) - 1, RecordsPerPage, ref totalCount, 0, queryString, categorySelection);
                    break;

            }

            //ReturnStatus st = Project.GetProjectPageWithFilter((Page.Value) - 1, RecordsPerPage, ref totalCount, statusChoice, queryString);
            StaticPagedList<Project> SearchResults = new StaticPagedList<Project>(((List<Project>)st.data), Page.Value, RecordsPerPage, totalCount);
            return SearchResults;
        }

        /// <summary>
        /// Builds a paginated list of projects to display with the _ProjectList partial view. 
        /// </summary>
        /// <param name="Page">The current page number. Page cannot be null or less than 1.</param>
        /// <param name="queryString">The name of the project to search for.</param>
        /// <returns></returns>
        public static StaticPagedList<Project> GetProjectPage(int? Page, string queryString, int categorySelection)
        {
            int RecordsPerPage = 10;
            //page can't be 0 or below
            if (Page < 1 || Page == null)
            {
                Page = 1;
            }

            //send in Page - 1 so that the index works correctly
            int totalCount = 0;
            ReturnStatus st = Project.GetProjectPage((Page.Value) - 1, RecordsPerPage, ref totalCount, queryString, categorySelection);

            //supposed to help reduce the load on the database by only getting what's needed
            StaticPagedList<Project> SearchResults = new StaticPagedList<Project>(((List<Project>)st.data), Page.Value, RecordsPerPage, totalCount);
            return SearchResults;
        }


        public static string GetProjectCategoryName(int? id)
        {
            return ProjectCategory.GetProjectCategoryName(id);
        }

        public static int GetProjectVolunteerCount(int? projectId)
        {
            if(projectId == null || projectId < 1) { return 0; }
            ReturnStatus rs = TimeSheet.GetProjectVolunteerCount((int)projectId);
            return (rs.errorCode == ReturnStatus.ALL_CLEAR) ? (int)rs.data : 0;
        }

        public static int GetProjectHours(int? projectId)
        {
            if (projectId == null || projectId < 1) { return 0; }
            ReturnStatus rs = TimeSheet.GetProjectHours((int)projectId);
            return (rs.errorCode == ReturnStatus.ALL_CLEAR) ? (int)rs.data : 0;
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
        /// <summary>
        /// Gets list of timecard vms with the following optional filters
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="orgId"></param>
        /// <param name="projId"></param>
        /// <param name="rangeStart"></param>
        /// <param name="rangeEnd"></param>
        /// <param name="queryString"></param>
        /// <returns>List of timecard viewmodels</returns>
        public static ReturnStatus GetTimeCardPageWithFilter(int? Page, int orgId, int projId, DateTime rangeStart, DateTime rangeEnd, string queryString)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                int RecordsPerPage = 10;


                //page can't be 0 or below
                if (Page == null || Page < 1)
                {
                    Page = 1;
                }
                int totalCount = 0;
                rs = TimeSheet.GetTimeCardPageWithFilter(Page.Value - 1, RecordsPerPage, ref totalCount, orgId, projId, rangeStart, rangeEnd, queryString);
                return rs;
            }
            catch
            {
                rs.errorCode = -1;
                return rs;
            }
        }

        /// <summary>
        /// Gets a unique timecard based on timesheet id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ReturnStatus GetTimeCardVM(int id)
        {
            ReturnStatus cardReturn = new ReturnStatus();
            try
            {
                ReturnStatus timesheetRS = GetTimeSheetById(id);
            
                if (timesheetRS.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    TimeSheet ts = (TimeSheet)timesheetRS.data;
                    TimeCardVM card = new TimeCardVM();
                    card.timeId = ts.Id;
                    card.userId = ts.user_Id;
                    card.projId = ts.project_Id;
                    card.orgId = ts.org_Id;
                    card.inTime = ts.clockInTime;
                    card.outTime = ts.clockOutTime;

                    ReturnStatus orgRS = GetOrganizationById(ts.org_Id);
                    if (orgRS.errorCode == ReturnStatus.ALL_CLEAR)
                    {
                        Organization org = (Organization)orgRS.data;
                        card.orgName = org.name;
                    }

                    ReturnStatus projRS = GetProjectById(ts.project_Id);
                    if (projRS.errorCode == ReturnStatus.ALL_CLEAR)
                    {
                        Project project = (Project)projRS.data;
                        card.projName = project.name;
                    }

                    ReturnStatus userRS = GetUser(ts.user_Id);
                    if (userRS.errorCode == ReturnStatus.ALL_CLEAR)
                    {
                        User user = (User)userRS.data;
                        card.volName = (user.firstName != null) ? user.firstName : user.emailAddress;
                        card.volName += " ";
                        card.volName += (user.lastName != null) ? user.lastName : user.emailAddress;
                    }
                    cardReturn.errorCode = ReturnStatus.ALL_CLEAR;
                    cardReturn.data = card;
                }
                else
                {
                    cardReturn.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                }
                return cardReturn;
            }
            catch
            {
                cardReturn.errorCode = -1;
                return cardReturn;
            }
        }

        #endregion timecard vms

        public static ReturnStatus EditTimeCard(TimeCardVM card)
        {
            ReturnStatus returnable = new ReturnStatus();
            try
            {
                ReturnStatus rs = GetTimeSheetById(card.timeId);
                if (rs.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    rs.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                    return rs;
                }
                TimeSheet ts = (TimeSheet)rs.data;
                ts.clockInTime = card.inTime;
                ts.clockOutTime = card.outTime;

                returnable = EditTimeSheet(ts);
                return returnable;
            }
            catch
            {
                returnable.errorCode = -1;
                return returnable;
            }
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

        /// <summary>
        /// check to see if the user is clocked in
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>       
        public static ReturnStatus IsUserClockedIn(int userId)
        {
            //get user's timesheet
            ReturnStatus rs = new ReturnStatus();
            try
            {
                rs = TimeSheet.GetClockedInUserTimeSheet(userId);
                if (rs.errorCode != 0)
                {
                    return rs;
                }

                TimeSheet userTimeSheet = (TimeSheet)rs.data;

                //if only a default timesheet was found then the user isn't "clocked in"
                if (userTimeSheet.Id < 0)
                {
                    rs.data = false;
                    return rs;
                }
                else
                {
                    rs.data = true;
                    return rs;
                }
            }
            catch
            {
                rs.errorCode = -1;
                return rs;
            }
        }


        public static ReturnStatus GetClockedInUserTimeSheet(int userId)
        {
            return TimeSheet.GetClockedInUserTimeSheet(userId);
        }


        /// <summary>
        /// get information to populate volunteer portal page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ReturnStatus GetPortalVM(int id)
        {
            ReturnStatus returnable = new ReturnStatus();
            try
            {
                PortalVM portalVM = new PortalVM();
                returnable.data = portalVM;
            
                ReturnStatus rs = Repository.GetUser(id);
                if (rs.errorCode != 0)
                {
                    returnable.errorCode = rs.errorCode;
                    return returnable;
                }

                //get users info
                User user = (User)rs.data;
                portalVM.fullName = user.firstName + " " + user.lastName;
                portalVM.userId = user.Id;

                //is user clocked in
                ReturnStatus isPunched = Repository.IsUserClockedIn(user.Id);
                if (isPunched.errorCode != 0)
                {
                    returnable.errorCode = isPunched.errorCode;
                    return returnable;
                }
                portalVM.isPunchedIn = (bool)isPunched.data;

                //get  volunteer's total hours
                ReturnStatus hrs = Repository.getTotalHoursWorkedByVolunteer(user.Id);
                if (hrs.errorCode != 0)
                {
                    returnable.errorCode = hrs.errorCode;
                    return returnable;
                }

                portalVM.cumulativeHours = (double)Repository.getTotalHoursWorkedByVolunteer(user.Id).data;

                return returnable;
            }
            catch
            {
                returnable.errorCode = -1;
                return returnable;
            }
        }


        public static ReturnStatus GetPunchInVM(int userId)
        {
           
            ReturnStatus st = new ReturnStatus();
            try
            {
                PunchInVM punch = new PunchInVM();
                st = User.GetUser(userId);
                if (st.errorCode != 0)
                {
                    return st;
                }

                User user = (User)st.data;
                punch.userId = userId;

                st.errorCode = 0;
                st.data = punch;

                return st;
            }
            catch
            {
                st.errorCode = -1;
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


        /// <summary>
        /// when admin wants to delete time card(on HttpPost)
        /// </summary>
        /// <returns></returns>
        public static ReturnStatus AdminDeleteTimeCard(TimeCardVM Model)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            ReturnStatus rs = new ReturnStatus();
            try
            {               
                TimeSheet timeSheet = db.timeSheets.Find(Model.timeId);
                db.timeSheets.Remove(timeSheet);
                db.SaveChanges();
                rs.errorCode = 0;
                return rs;
            }
            catch
            {
                rs.errorCode = -1;
                return rs;
            }
        }

        #endregion

        #region Report functions

        #region Dashboard Barchart

        public static ReturnStatus GetHoursChartVMByYear()
        {
            ReturnStatus chartReturn = new ReturnStatus();
            try
            {
                ReturnStatus restoreProjectIdListRS = Project.GetProjectIdByCategoryName("ReStore");
                ReturnStatus abwkProjectIdListRS = Project.GetProjectIdByCategoryName("ABWK");
                ReturnStatus homeBuildProjectIdListRS = Project.GetProjectIdByCategoryName("Home");

                List<int> restoreIds = (restoreProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)restoreProjectIdListRS.data : new List<int>();
                List<int> abwkIds = (abwkProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)abwkProjectIdListRS.data : new List<int>();
                List<int> homeBuildIds = (homeBuildProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)homeBuildProjectIdListRS.data : new List<int>();

                ReturnStatus tsArrayRS = TimeSheet.Get3YearsTimeSheetsByCategory(restoreIds, abwkIds, homeBuildIds);
                // this is an array[9] that holds lists of timesheets
                // the first 3 are restore(2yrs ago, last year, now )
                // next 3 are awbk
                // last 3 are homebuilds( whatever isn't restore or awbk)
                if (tsArrayRS.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    List<TimeSheet>[] sheetsArr = (List<TimeSheet>[])tsArrayRS.data;
                    int year = DateTime.Today.Year;
                    string[] cats = new string[] { (year - 2).ToString(), (year - 1).ToString(), year.ToString() };
                    int[] restoreHrs = new int[3];
                    int[] awbkHrs = new int[3];
                    int[] homesHrs = new int[3];
                    int j = 0, k = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        TimeSpan ts = AddTimeSheetHours(sheetsArr[i]);
                        if (i < 3)
                        {
                            restoreHrs[i] = (int)ts.TotalHours;
                        }
                        else if (i < 6)
                        {
                            awbkHrs[j] = (int)ts.TotalHours;
                            j++;
                        }
                        else
                        {
                            homesHrs[k] = (int)ts.TotalHours;
                            k++;
                        }
                    }
                    ChartVM chartVM = new ChartVM("Volunteer Hours by Year", cats, restoreHrs, awbkHrs, homesHrs);
                    chartReturn.errorCode = ReturnStatus.ALL_CLEAR;
                    chartReturn.data = chartVM;
                    return chartReturn;
                }
                else
                {
                    return new ReturnStatus() { errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE, data = null };
                }
            }
            catch
            {
                chartReturn.errorCode = -1;
                return chartReturn;
            }

        }

        public static ReturnStatus GetHoursChartVMByMonth()
        {
            ReturnStatus chartReturn = new ReturnStatus();
            try
            {
                ReturnStatus restoreProjectIdListRS = Project.GetProjectIdByCategoryName("ReStore");
                ReturnStatus abwkProjectIdListRS = Project.GetProjectIdByCategoryName("ABWK");
                ReturnStatus homeBuildProjectIdListRS = Project.GetProjectIdByCategoryName("Home");

                List<int> restoreIds = (restoreProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)restoreProjectIdListRS.data : new List<int>();
                List<int> abwkIds = (abwkProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)abwkProjectIdListRS.data : new List<int>();
                List<int> homeBuildIds = (homeBuildProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)homeBuildProjectIdListRS.data : new List<int>();

                ReturnStatus tsArrayRS = TimeSheet.Get12MonthsTimeSheetsByCategory(restoreIds, abwkIds, homeBuildIds);
                // this is an array[36] that holds lists of timesheets
                // the first 12 are restore(11 months ago, 10... )
                // next 12 are awbk
                // last 12 are homebuilds( whatever isn't restore or awbk)
                if (tsArrayRS.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    List<TimeSheet>[] sheetsArr = (List<TimeSheet>[])tsArrayRS.data;
                    DateTime today = DateTime.Today;
                    string[] cats = new string[12];// { (year - 2).ToString(), (year - 1).ToString(), year.ToString() };
                    int m = 11;
                    for (int i = 0; i < 12; i++)
                    {
                        cats[i] = today.AddMonths(-m).ToString("MMMM");
                        m--;
                    }
                    int[] restoreHrs = new int[12];
                    int[] awbkHrs = new int[12];
                    int[] homesHrs = new int[12];
                    int j = 0, k = 0;
                    for (int i = 0; i < 36; i++)
                    {
                        TimeSpan ts = AddTimeSheetHours(sheetsArr[i]);
                        if (i < 12)
                        {
                            restoreHrs[i] = (int)ts.TotalHours;
                        }
                        else if (i < 24)
                        {
                            awbkHrs[j] = (int)ts.TotalHours;
                            j++;
                        }
                        else
                        {
                            homesHrs[k] = (int)ts.TotalHours;
                            k++;
                        }
                    }
                    ChartVM chartVM = new ChartVM("Volunteer Hours by Month", cats, restoreHrs, awbkHrs, homesHrs);
                    chartReturn.errorCode = ReturnStatus.ALL_CLEAR;
                    chartReturn.data = chartVM;
                    return chartReturn;
                }
                else
                {
                    return new ReturnStatus() { errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE, data = null };
                }
            }
            catch
            {
                chartReturn.errorCode = -1;
                return chartReturn;
            }
        }

        public static ReturnStatus GetHoursChartVMByWeek()
        {
            ReturnStatus chartReturn = new ReturnStatus();
            try
            {
                ReturnStatus restoreProjectIdListRS = Project.GetProjectIdByCategoryName("ReStore");
                ReturnStatus abwkProjectIdListRS = Project.GetProjectIdByCategoryName("ABWK");
                ReturnStatus homeBuildProjectIdListRS = Project.GetProjectIdByCategoryName("Home");

                List<int> restoreIds = (restoreProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)restoreProjectIdListRS.data : new List<int>();
                List<int> abwkIds = (abwkProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)abwkProjectIdListRS.data : new List<int>();
                List<int> homeBuildIds = (homeBuildProjectIdListRS.errorCode == ReturnStatus.ALL_CLEAR) ? (List<int>)homeBuildProjectIdListRS.data : new List<int>();

                ReturnStatus tsArrayRS = TimeSheet.Get12WeeksTimeSheetsByCategory(restoreIds, abwkIds, homeBuildIds);
                // this is an array[36] that holds lists of timesheets
                // the first 12 are restore(11 weeks ago, 10... )
                // next 12 are awbk
                // last 12 are homebuilds( whatever isn't restore or awbk)
                if (tsArrayRS.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    List<TimeSheet>[] sheetsArr = (List<TimeSheet>[])tsArrayRS.data;
                    DateTime today = DateTime.Today;
                    string[] cats = new string[12];
                    int w = 11;
                    for (int i = 0; i < 12; i++)
                    {
                        DateTime myDate = today.AddDays(-w * 7);
                        cats[i] = myDate.Month.ToString() + "/" + myDate.Day.ToString();
                        w--;
                    }
                    int[] restoreHrs = new int[12];
                    int[] awbkHrs = new int[12];
                    int[] homesHrs = new int[12];
                    int j = 0, k = 0;
                    for (int i = 0; i < 36; i++)
                    {
                        TimeSpan ts = AddTimeSheetHours(sheetsArr[i]);
                        if (i < 12)
                        {
                            restoreHrs[i] = (int)ts.TotalHours;
                        }
                        else if (i < 24)
                        {
                            awbkHrs[j] = (int)ts.TotalHours;
                            j++;
                        }
                        else
                        {
                            homesHrs[k] = (int)ts.TotalHours;
                            k++;
                        }
                    }
                    ChartVM chartVM = new ChartVM("Volunteer Hours by Week", cats, restoreHrs, awbkHrs, homesHrs);
                    chartReturn.errorCode = ReturnStatus.ALL_CLEAR;
                    chartReturn.data = chartVM;
                    return chartReturn;
                }
                else
                {
                    return new ReturnStatus() { errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE, data = null };
                }
            }
            catch
            {
                chartReturn.errorCode = -1;
                return chartReturn;
            }

        }
        #endregion Dashboard Barchart

        #region Project Reports
        /// <summary>
        /// Gets a row of volunteer demographics data for each of the 3 project categories
        /// for the number of months defined by @period
        /// </summary>
        /// <param name="period">Number of months to go back for data</param>
        /// <returns></returns>
        public static ReturnStatus GetProjectDemographicsReport(int period)
        {
            ReturnStatus listOfListsRS = new ReturnStatus();
            try
            {
                List<List<ProjDemogReportVM>> reports = new List<List<ProjDemogReportVM>>();
                for (int i = 0; i < period; i++)
                {
                    ReturnStatus aListRS = Project.GetProjectDemographicsReport(i);
                    if (aListRS.errorCode != ReturnStatus.ALL_CLEAR)
                    {
                        listOfListsRS.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                        return listOfListsRS;
                    }
                    reports.Add((List<ProjDemogReportVM>)aListRS.data);
                }
                listOfListsRS.errorCode = ReturnStatus.ALL_CLEAR;
                listOfListsRS.data = reports;
                return listOfListsRS;

                //return Project.GetProjectDemographicsReport(period);
            }
            catch
            {
                listOfListsRS.errorCode = -1;
                return listOfListsRS;
            }
        }

        #endregion Project Reports

        public static ReturnStatus getTotalHoursWorkedByVolunteer(int volunteerId)
        {

            ReturnStatus hoursWorked = new ReturnStatus();
            hoursWorked.data = 0.0;

            try
            {
                ReturnStatus st = new ReturnStatus();
                st.data = new List<TimeSheet>();

                st = GetAllTimeSheetsByVolunteer(volunteerId);
                if (st.errorCode != 0)
                {
                    return st;
                }

                DateTime userClockedIn = DateTime.Today.AddDays(1);
                List<TimeSheet> temp = new List<TimeSheet>();
                List<TimeSheet> volunteerTimes = new List<TimeSheet>();

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

            catch
            {
                hoursWorked.errorCode = -1;
                return hoursWorked;
            }

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
            try
            {
                st.data = new List<User.Demog>();
           
                st = User.GetDemographicsForPie(gender);
                if (st.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                    return st;
                }
                return User.GetDemographicsForPie(gender);
            }
            catch
            {
                st.errorCode = -1;
                return st;
            }           
        }

        public static ReturnStatus GetNumBadPunches()
        {
            return TimeSheet.GetNumBadPunches();
        }



        /// <summary>
        /// return total hours logged into given project
        /// </summary>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
        public static ReturnStatus getTotalHoursLoggedIntoProject(int projectId)
        {
            ReturnStatus hoursLogged = new ReturnStatus();
            try
            {
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
            catch
            {
                hoursLogged.errorCode = -1;
                return hoursLogged;
            }
        }

        #endregion

        #region Admin --> User

        public static ReturnStatus GetAdminViewOfUser(int id)
        {
            ReturnStatus vmToReturn = new ReturnStatus();
            try
            {
                AdminUserVM vm = new AdminUserVM();
                ReturnStatus userRS = GetUser(id);
                if (userRS.errorCode != ReturnStatus.ALL_CLEAR)
                {
                    vmToReturn.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                    return vmToReturn;
                }
                User user = (User)userRS.data;

                // set user info
                vm.userInfo.userInfoId = user.Id;
                vm.userInfo.firstName = user.firstName;
                vm.userInfo.lastName = user.lastName;
                vm.userInfo.homePhone = user.homePhoneNumber;
                vm.userInfo.workPhone = user.workPhoneNumber;
                vm.userInfo.email = user.emailAddress;
                vm.userInfo.streetAddress = user.streetAddress;
                vm.userInfo.city = user.city;
                vm.userInfo.zip = user.zip;
                vm.userInfo.birthDate = user.birthDate;
                vm.userInfo.isAdmin = (user.isAdmin == 1) ? true : false;
                vm.userInfo.waiverSignDate = user.waiverSignDate;
                try
                {
                    vm.userInfo.hoursToDate = (double)getTotalHoursWorkedByVolunteer(user.Id).data;
                }
                catch
                {
                    vm.userInfo.hoursToDate = 0.0;
                }
                vm.userInfo.waiverExpiration = user.waiverSignDate.AddYears(1);
                vm.userInfo.waiverStatus = (vm.userInfo.waiverExpiration > DateTime.Now);

                // set emergency info
                vm.emergInfo.emergencyFirstName = user.emergencyFirstName;
                vm.emergInfo.emergencyLastName = user.emergencyLastName;
                vm.emergInfo.relation = user.relation;
                vm.emergInfo.emergencyHomePhone = user.emergencyHomePhone;
                vm.emergInfo.emergencyWorkPhone = user.emergencyWorkPhone;
                vm.emergInfo.emergencyStreetAddress = user.emergencyStreetAddress;
                vm.emergInfo.emergencyCity = user.emergencyCity;
                vm.emergInfo.emergencyZip = user.emergencyZip;

                // set timecards
                ReturnStatus timeIdsRS = TimeSheet.GetTimeSheetIdsByUserId(id);
                List<TimeCardVM> timecardVMs = new List<TimeCardVM>();
                if (timeIdsRS.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    List<int> timeids = (List<int>)timeIdsRS.data;
                    foreach (int timesheetId in timeids)
                    {
                        ReturnStatus timeVMsRS = GetTimeCardVM(timesheetId);
                        if (timeVMsRS.errorCode == ReturnStatus.ALL_CLEAR)
                        {
                            timecardVMs.Add((TimeCardVM)timeVMsRS.data);
                        }
                    }
                }
                vm.timeCardVMs = timecardVMs;

                vmToReturn.errorCode = ReturnStatus.ALL_CLEAR;
                vmToReturn.data = vm;

                return vmToReturn;
            }
            catch
            {
                vmToReturn.errorCode = -1;
                return vmToReturn;
            }
        }

        public static ReturnStatus AdminEditUser(UserInfo u)
        {
            ReturnStatus result = new ReturnStatus();
            try
            {
                ReturnStatus userRS = GetUser(u.userInfoId);
                if (userRS.errorCode == ReturnStatus.ALL_CLEAR)
                {
                    User user = (User)userRS.data;
                    user.firstName = u.firstName;
                    user.lastName = u.lastName;
                    user.homePhoneNumber = u.homePhone;
                    user.workPhoneNumber = u.workPhone;
                    //if ((bool)EmailExists(u.email).data)
                    //{
                    //    result.errorCode = -1;
                    //    return result;
                    //}
                    user.emailAddress = u.email;
                    user.streetAddress = u.streetAddress;
                    user.city = u.city;
                    user.zip = u.zip;
                    user.birthDate = u.birthDate;
                    user.isAdmin = (u.isAdmin) ? 1 : 0;
                    user.waiverSignDate = u.waiverSignDate;
                    return EditUser(user);
                }
                result.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
                return result;
            }
            catch
            {
                result.errorCode = -1;
                return result;
            }
        }
        #endregion Admin --> User

        #region Project Category

        public static ReturnStatus GetAllCategories()
        {
            return ProjectCategory.GetAllProjectCategories();
        }

        public static StaticPagedList<ProjectCategory> GetAllCategoriesByPageSize(int? page, int recordsPerPage)
        {
            if (page < 1 || page == null)
            {
                page = 1;
            }

            int totalCount = 0;
            ReturnStatus st = ProjectCategory.GetAllCategoriesByPageSize((page.Value) - 1, recordsPerPage, ref totalCount);

            StaticPagedList<ProjectCategory> SearchResults = new StaticPagedList<ProjectCategory>(((List<ProjectCategory>)st.data), page.Value, recordsPerPage, totalCount);

            return SearchResults;
        }

        public static void CreateProjectCategory(ProjectCategory pc)
        {
            ProjectCategory.CreateProjectCategory(pc);
        }


        #endregion

        #region WaiverHistory
            public static WaiverHistoryByUser getWaiverHistoryByUserId(int id)
        {
            WaiverHistoryByUser waiverHistory = new WaiverHistoryByUser();
            waiverHistory.waiverList = WaiverHistory.getWaiverHistoryByUserId(id);

            return waiverHistory;
        }

        public static ReturnStatus GetAWaiverById(int id)
        {
            return WaiverHistory.GetWaiverByID(id);
        }
        #endregion

        #region Event
        public static ReturnStatus GetManageEventVmById(int id)
        {
            ReturnStatus vmToReturn = new ReturnStatus();
            ManageEventVM vm = new ManageEventVM();
            ReturnStatus eventRS = HfhEvent.GetHfhEventById(id);
            if(eventRS.errorCode == ReturnStatus.ALL_CLEAR)
            {
                vm.hfhEvent = (HfhEvent)eventRS.data;
            }
            else
            {
                vmToReturn.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                return vmToReturn;
            }
            //empty project lists should be okay...
            ReturnStatus eventProjectsRS = HfhEvent.GetEventProjectsByEventId(id);
            if (eventProjectsRS.errorCode == ReturnStatus.ALL_CLEAR)
            {
                vm.eventProjects = (List<EventAddRemoveProjectVM>)eventProjectsRS.data;
            }
            else
            {
                vm.eventProjects = new List<EventAddRemoveProjectVM>();
                //vm.eventProjects.Add(new EventAddRemoveProjectVM());
            }
            ReturnStatus addableProjectsRS = HfhEvent.GetNotHfhEventProjects(id);
            if(addableProjectsRS.errorCode == ReturnStatus.ALL_CLEAR)
            {
                vm.addableProjects = (List<EventAddRemoveProjectVM>)addableProjectsRS.data; 
            }
            else
            {
                vm.addableProjects = new List<EventAddRemoveProjectVM>();
            }
           
            vmToReturn.errorCode = ReturnStatus.ALL_CLEAR;
            vmToReturn.data = vm;
            return vmToReturn;
        }
        #endregion Event

    }
}