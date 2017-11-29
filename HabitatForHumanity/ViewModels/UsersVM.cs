using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PagedList;

namespace HabitatForHumanity.ViewModels
{
    public class UsersVM
    {
        public int userNumber { get; set; }

        [Required(ErrorMessage = "Enter Volunteer Name"), Display(Name = "Name")]
        public string volunteerName { get; set; }

        [Required(ErrorMessage = "Enter valid email")]
        [Display(Name = "Email Address")]
        public string email { get; set; }
        
        [Display(Name = "Total Hours To Date")]
        public double hoursToDate { get; set; }

        [Display(Name = "Waiver Active?")]
        public bool waiverStatus { get; set; }

        [Display(Name = "Waiver Expiration")]
        public DateTime waiverExpiration { get; set; }

        [Display(Name = "Admin?")]
        public bool isAdmin { get; set; }

        public List<TimeCardVM> timeCardVM { get; set; }
    }
}