using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.ViewModels
{
    public class OrganizationVM
    {
        public int _Id { get; set; }
        [Required(ErrorMessage = "Enter organization name")]
        [Display(Name = "Organization Name*")]
        public string _name { get; set; }
        [Display(Name = "Active")]
        public bool _status { get; set; }
        [Display(Name = "Comments")]
        public string comments { get; set; }
    }
}