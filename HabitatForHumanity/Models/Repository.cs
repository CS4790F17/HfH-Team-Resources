using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    public class Repository
    {
        // user stuff
        public static int CreateUser(User user)
        {

            return User.CreateUser(user);
        }
    }
}