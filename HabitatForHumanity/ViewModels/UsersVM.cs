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

        [DisplayFormat(DataFormatString = "{0:M/d/yy h:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Waiver Signed")]
        public DateTime waiverSignDate { get; set; }

        [Display(Name = "Waiver Active?")]
        public bool waiverStatus { get; set; }

        [Display(Name = "Waiver Expiration")]
        public DateTime waiverExpiration { get; set; }

        [Display(Name = "Admin?")]
        public bool isAdmin { get; set; }

        [Display(Name = "Emergency Contact First Name")]
        public string emergencyFirstName { get; set; }
        
        [Display(Name = "Emergency Contact Last Name")]
        public string emergencyLastName { get; set; }
        
        [Display(Name = "Relation")]
        public string relation { get; set; }
        
        [Display(Name = "Emergency Contact Primary Phone")]
        public string emergencyHomePhone { get; set; }
        
        [Display(Name = "Emergency Contact Alternate Phone")]
        public string emergencyWorkPhone { get; set; }
        
        [Display(Name = "Emergency Contact Address")]
        public string emergencyStreetAddress { get; set; }
        
        [Display(Name = "Emergency Contact City")]
        public string emergencyCity { get; set; }
        
        [Display(Name = "Emergency Contact Zipcode")]
        public string emergencyZip { get; set; }

        public List<TimeCardVM> timeCardVM { get; set; }
    }
}