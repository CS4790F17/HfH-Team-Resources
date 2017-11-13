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
        [Required(ErrorMessage = "Enter your signature")]
        [Display(Name = "Signature*")]
        public string signature { get; set; }
        [Required(ErrorMessage = "Enter Emergency First Name")]
        [Display(Name = "Emergency First Name*")]
        public string emergencyFirstName { get; set; }
        [Required(ErrorMessage = "Enter Emergency Last Name")]
        [Display(Name = "Emergency Last Name*")]
        public string emergencyLastName { get; set; }
        [Required(ErrorMessage = "Enter Emergency Relation")]
        [Display(Name = "Emergency Relation*")]
        public string relation { get; set; }
        [Required(ErrorMessage = "Enter Emergency Home Phone")]
        [Display(Name = "Emergency Home Phone*")]
        public string emergencyHomePhone { get; set; }
        [Required(ErrorMessage = "Enter Emergency Work Phone")]
        [Display(Name = "Emergency Work Phone*")]
        public string emergencyWorkPhone { get; set; }
        [Required(ErrorMessage = "Enter Emergency Street Address")]
        [Display(Name = "Emergency Street Address*")]
        public string emergencyStreetAddress { get; set; }
        [Required(ErrorMessage = "Enter Emergency City")]
        [Display(Name = "Emergency City*")]
        public string emergencyCity { get; set; }
        [Required(ErrorMessage = "Enter Emergency Zipcode")]
        [Display(Name = "Emergency Zipcode*")]
        public string emergencyZip { get; set; }
    }
}