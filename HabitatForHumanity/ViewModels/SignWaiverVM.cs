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
        [Display(Name = "I have read and agree to the terms above")]
        public bool signature { get; set; }
        [Display(Name ="Type your name to sign the waiver")]
        [Required(ErrorMessage ="You must sign the waiver to continue.")]
        public String signatureName { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's First Name")]
        [Display(Name = "First Name*")]
        public string emergencyFirstName { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's Last Name")]
        [Display(Name = "Last Name*")]
        public string emergencyLastName { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's Relation")]
        [Display(Name = "Relation to you*")]
        public string relation { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's Primary Phone")]
        [Display(Name = "Primary Phone*")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string emergencyHomePhone { get; set; }
        [Display(Name = "Alternate Phone")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string emergencyWorkPhone { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's Street Address")]
        [Display(Name = "Street Address*")]
        public string emergencyStreetAddress { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's City")]
        [Display(Name = "City*")]
        public string emergencyCity { get; set; }
        [Required(ErrorMessage = "Enter Emergency Contact's Zipcode")]
        [Display(Name = "Zipcode*")]
        [RegularExpression(@"^(^\d{5}$)|(^\d{5}-\d{4}$)$", ErrorMessage = "Please Enter a Valid Zip")]
        public string emergencyZip { get; set; }
    }
}