using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.ViewModels;
using System.Web.Helpers;

namespace HabitatForHumanity.Models
{
    public class Repository
    {
        /**   USER LOG IN / SIGN IN METHODS **/
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

        public static User GetUser(string email)
        {
            return User.GetUserByEmail(email);
        }

        // this not only changes the password, it also hashes it
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



        //  WAIVER BUSINESS ///////////////////////////
        public static SignWaiverVM GetUserWaiver(string email)
        {
            SignWaiverVM sw = new SignWaiverVM();
            User user = User.GetUserByEmail(email);
            sw.userId = user.Id;
            sw.email = user.email;
            sw.firstName = user.firstName;
            sw.lastName = user.lastName;
            sw.streetAddress = user.streetAddress;
            sw.city = user.city;
            sw.zip = user.zip;
            sw.phone = user.homePhone;
            sw.signDate = DateTime.Today;
            return sw;
        }

        public static void AddWaiver(SignWaiverVM svm)
        {
            Waiver w = new Waiver();
            w.userId = svm.userId;
            w.signDate = svm.signDate;
            w.emContFirstName = svm.emContFirstName;
            w.emContLastName = svm.emContLastName;
            w.relation = svm.relation;
            w.emContHomePhone = svm.emContHomePhone;
            w.emContWorkPhone = svm.emContWorkPhone;
            w.consent = svm.consent;
            w.signature = svm.signature;

            Waiver.AddWaiver(w);
        }

        public static void CreateUser(User user)
        {
            user.password = Crypto.HashPassword(user.password);
            user.role = "volunteer";
            User.CreateUser(user);

        }

        /////////////////////     time sheet business ///////////////
        public static PunchInVM GetPunchInVM(int id)
        {
            PunchInVM punch = new PunchInVM();
            punch.userId = id;
            punch.projects = GetProjectNames();
            return punch;
        }

        public static void PunchIn(PunchInVM punchInVM)
        {
            TimeSheet t = new TimeSheet();
            t.userId = punchInVM.userId;
            t.projectId = GetProjectIdByName(punchInVM.projectName);
            t.timeIn = DateTime.Today;
            TimeSheet.PunchIn(t);

        }

        /////////////////////    project business    ////////////////////////
        public static List<string> GetProjectNames()
        {
            List<Project> projects = new List<Project>();
            projects = Project.GetProjects();
            List<string> projectNames = new List<string>();
            foreach(Project p in projects)
            {
                projectNames.Add(p.name);
            }
            return projectNames;
        }

        public static int GetProjectIdByName(string name)
        {
            return Project.GetProjectIdByName(name);
        }

        public static List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();
            return Project.GetProjects();  
        }
    }


}