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
    }
}