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
        public Organization organization { get; set; }
        [Display(Name = "Active")]
        public bool _status { get; set; }
    }
}