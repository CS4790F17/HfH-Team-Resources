using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using HabitatForHumanity.ViewModels;

namespace HabitatForHumanity.Models
{
    public class Repository
    {
        // user stuff
        public static int CreateUser(User user)
        {
            user.password = Crypto.HashPassword(user.password);
            user.isAdmin = 0;
            user.waiverSignDate = DateTime.Today;
            return User.CreateUser(user);
        }

        public static User GetUser(int id)
        {
            return User.GetUser(id);
        }

        public static bool EmailExists(string email)
        {
            return User.EmailExists(email);
        }

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

        public static User GetUserByEmail(string email)
        {
            return User.GetUserByEmail(email);
        }

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


        public static void EditUser(User user)
        {
            User.EditUser(user);
        }

        // time card stuff

        public static PunchOutVM GetPunchClockVM(int userId)
        {
            PunchOutVM punch = new PunchOutVM();

            User user = GetUser(userId);
            punch.userName = user.firstName + " " + user.lastName;
            punch.projectList = GetProjectListVMs();      
            punch.orgList = Organization.GetOrganizations();
            TimeSheet ts = TimeSheet.GetOpenUserTimeSheet(userId);
            punch.timeSheet = ts;
            return punch;
        }

        public static PunchInVM GetPunchInVM(int userId)
        {
            PunchInVM punch = new PunchInVM();
            User user = GetUser(userId);
            punch.userId = userId;
            punch.userName = user.firstName + " " + user.lastName;
            punch.projectList = GetProjectListVMs();
            punch.orgList = Organization.GetOrganizations();
            return punch;
        }

        public static void PunchIn(PunchInVM punchInVM)
        {
            TimeSheet ts = new TimeSheet();
            ts.user_Id = punchInVM.userId;
            ts.project_Id = punchInVM.projectId;
            ts.clockInTime = DateTime.Now;
            ts.clockOutTime = DateTime.Today.AddDays(1);
            TimeSheet.InsertTimeSheet(ts);

        }
        public static List<ProjectListVM> GetProjectListVMs()
        {
            List<Project> projects = new List<Project>();
            List<ProjectListVM> p = new List<ProjectListVM>();
            projects = Project.GetActiveProjects();
            if (projects.Count() > 0)
            {
                foreach(Project proj in projects)
                {
                    p.Add(new ProjectListVM { Id = proj.Id, projectName = proj.name });
                }
            }
            return p;
        }

    }
}