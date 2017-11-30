using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HabitatForHumanity.ViewModels;
using System.Web.Helpers;

namespace HabitatForHumanity.ViewModels
{
    public class UserProfileVM
    {
        [Required(ErrorMessage = "Enter Email")]
        [Display(Name = "Email")]
        [RegularExpression(@"^(([^<>()\[\]\\.,;:\s@']+(\.[^<>()\[\]\\.,;:\s@']+)*)|('.+'))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please Enter a Valid Email Address")]
        public string userEmail { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }
        [Required(ErrorMessage = "Enter Primary Phone")]
        [Display(Name = "Primary Phone")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string homePhone { get; set; }
        [Display(Name = "Alternate Phone")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string workPhone { get; set; }
        [Required(ErrorMessage = "Enter Street Address")]
        [Display(Name = "Street Address")]
        public string streetAddress { get; set; }
        [Required(ErrorMessage = "Enter City")]
        [Display(Name = "City")]
        public string city { get; set; }
        [Required(ErrorMessage = "Enter Zipcode")]
        [Display(Name = "Zipcode")]
        [RegularExpression(@"^(^\d{5}$)|(^\d{5}-\d{4}$)$", ErrorMessage = "Please Enter a Valid Zip")]
        public string zip { get; set; }
        [Display(Name = "New Password")]
        public string newPassword { get; set; }
        [Display(Name = "Confirm Password")]
        [Compare("newPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string confirmPassword { get; set; }
    }
}