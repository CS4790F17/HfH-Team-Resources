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
        // user stuff
        public static int CreateUser(User user)
        {
            user.password = Crypto.HashPassword(user.password);
            user.isAdmin = 0;
            user.waiverSignDate = DateTime.Today;
            return User.CreateUser(user);
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
        #endregion

        #region Project functions
        public static List<Project> getAllProjects()
        {
            return Project.getAllProjects();
        }

        public static Project getProjectById(int id)
        {
            return Project.getProjectById(id);
        }

        public static List<Project> getActiveProjects()
        {
            return Project.getActiveProjects();
        }

        /// <summary>
        /// Gets a project by its primary key: name+beginDate. Date must be in the format MM/DD/YYYY.
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="date">MM/DD/YYYY</param>
        /// <returns></returns>
        public static Project getProjectByNameAndDate(string name, string date)
        {
            return Project.getProjectByNameAndDate(name, date);
        }
        #endregion

        #region Organization functions
        public static List<Organization> getAllOrganizations()
        {
            return Organization.GetAllOrganizations();
        }

        public static Organization GetOrganizationById(int id)
        {
            return Organization.GetOrganizationById(id);
        }

        public static Organization GetOrganizationByName(string name)
        {
            return Organization.GetOrganizationByName(name);
        }
        #endregion
    }
}