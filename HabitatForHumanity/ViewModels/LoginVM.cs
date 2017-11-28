using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class LoginVM
    {
        [Required, StringLength(30), EmailAddress, DisplayName("Email Address")]
        public string email { get; set; }

        [Required, StringLength(15), DisplayName("Password")]
        public string password { get; set; }
      



    }
}