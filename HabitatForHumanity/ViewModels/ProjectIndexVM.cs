using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.ViewModels
{
    public class ProjectIndexVM
    {
        public int _Id { get; set; }
        [Display(Name = "Project Name")]
        public string _name { get; set; }
        [Display(Name = "Begin Date")]
        public DateTime _beginDate { get; set; }   
        [Display(Name = "Total Hours Logged ")]
        public double _hoursLogged { get; set; }
        [Display(Name = "Number of Volunteers")]
        public int _numVolunteers { get; set; }

    }
}