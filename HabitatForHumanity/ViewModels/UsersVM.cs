using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;

namespace HabitatForHumanity.ViewModels
{
    public class UsersVM
    {
        public int userNumber { get; set; }

        [Display(Name ="Name")]
        public string volunteerName { get; set; }


        [Display(Name = "Email Address")]
        public string email { get; set; }


        [Display(Name = "Total Hours To Date")]
        public double hoursToDate { get; set; }
    }
}