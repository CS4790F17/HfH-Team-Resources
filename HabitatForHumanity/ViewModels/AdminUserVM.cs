using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class AdminUserVM
    {
        public int userNumber { get; set; }
        public UserInfo userInfo { get; set; }
        public EmergInfo emergInfo { get; set; }
        public List<TimeCardVM> timeCardVMs { get; set; }
        public AdminUserVM()
        {
            userInfo = new UserInfo();
            emergInfo = new EmergInfo();
            timeCardVMs = new List<TimeCardVM>();
        }
    }

    public class UserInfo
    {
        [Required, Display(Name = "Name")]
        public string firstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Phone")]
        public string homePhone { get; set; }

        [Display(Name = "Alternate")]
        public string workPhone { get; set; }

        [Required(ErrorMessage = "Enter valid email")]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Address")]
        public string streetAddress { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "Zipcode")]
        public string zip { get; set; }

        [DisplayFormat(DataFormatString = "{0:M/d/yy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Display(Name = "DOB")]
        public DateTime birthDate { get; set; }

        [Display(Name = "Admin?")]
        public bool isAdmin { get; set; }

        [DisplayFormat(DataFormatString = "{0:M/d/yy h:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [Display(Name = "Waiver Date")]
        public DateTime waiverSignDate { get; set; }

        [Display(Name = "Hours")]
        public double hoursToDate { get; set; }

        [Display(Name = "Waiver Active?")]
        public bool waiverStatus { get; set; }

        [Display(Name = "Waiver Expiration")]
        public DateTime waiverExpiration { get; set; }
    }

    public class EmergInfo
    {
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
    }


}