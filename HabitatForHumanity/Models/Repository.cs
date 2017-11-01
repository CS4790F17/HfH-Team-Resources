using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

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
    }
}