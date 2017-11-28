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
    public class SignWaiverVM
    {
        public string userEmail { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessage = "Must agree to terms and conditions to continue.")]
        [Display(Name = "I have read and agree to the terms above.")]
        public bool signature { get; set; }
        [Required(ErrorMessage = "Enter Contact's First Name")]
        [Display(Name = "Contact's First Name*")]
        public string emergencyFirstName { get; set; }
        [Required(ErrorMessage = "Enter Contact's Last Name")]
        [Display(Name = "Contact's Last Name*")]
        public string emergencyLastName { get; set; }
        [Required(ErrorMessage = "Enter Contact's Relation")]
        [Display(Name = "Contact's Relation*")]
        public string relation { get; set; }
        [Required(ErrorMessage = "Enter Contact's Home Phone")]
        [Display(Name = "Contact's Home Phone*")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string emergencyHomePhone { get; set; }
        [Required(ErrorMessage = "Enter Contact's Work Phone")]
        [Display(Name = "Contact's Work Phone*")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string emergencyWorkPhone { get; set; }
        [Required(ErrorMessage = "Enter Contact's Street Address")]
        [Display(Name = "Contact's Street Address*")]
        public string emergencyStreetAddress { get; set; }
        [Required(ErrorMessage = "Enter Contact's City")]
        [Display(Name = "Contact's City*")]
        public string emergencyCity { get; set; }
        [Required(ErrorMessage = "Enter Contact's Zipcode")]
        [Display(Name = "Contact's Zipcode*")]
        [RegularExpression(@"^(^\d{5}$)|(^\d{5}-\d{4}$)$", ErrorMessage = "Please Enter a Valid Zip")]
        public string emergencyZip { get; set; }
    }
}