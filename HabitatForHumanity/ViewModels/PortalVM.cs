using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;
namespace HabitatForHumanity.ViewModels
{
    public class PortalVM
    {
        public double cumulativeHours { get; set; }
        public string fullName { get; set; }
        public bool isPunchedIn { get; set; }
        public bool isReStore { get; set; }
        public int userId { get; set; }
    }
}