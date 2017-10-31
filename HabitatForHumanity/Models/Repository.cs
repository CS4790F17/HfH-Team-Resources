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
            User user = User.GetUser(loginVm.email);
            if (user != null && Crypto.VerifyHashedPassword(user.password, loginVm.password))
            {
                exists = true;
            }
            return exists;
        }

        public static User GetUser(string email)
        {
            return User.GetUser(email);
        }

        // this not only changes the password, it also hashes it
        public static void ChangePassword(string email, string newPW)
        {
            User user = new User();
            user = User.GetUser(email);
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

        public static void CreateUser(User user)
        {
            user.password = Crypto.HashPassword(user.password);
            user.role = "volunteer";
            User.CreateUser(user);

        }
    }


}