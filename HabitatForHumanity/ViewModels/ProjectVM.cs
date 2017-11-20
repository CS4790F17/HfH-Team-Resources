using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.ViewModels
{
    public class ProjectVM
    {
        public int _Id { get; set; }
        [Required(ErrorMessage = "Enter project name")]
        [Display(Name = "Project Name*")]
        public string _name { get; set; }
        [Required(ErrorMessage = "Enter begin date")]
        [Display(Name = "Begin Date*")]
        public DateTime _beginDate { get; set; }
        [Display(Name = "Description")]
        public string _description { get; set; }
        [Display(Name = "Active")]
        public bool _status { get; set; }
        [Display(Name = "Total Hours Logged ")]
        public double hoursLogged { get; set; }
        [Display(Name = "Total Volunteers")]
        public double numVolunteers { get; set; }

    }
}