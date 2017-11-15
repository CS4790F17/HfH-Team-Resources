using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;
namespace HabitatForHumanity.ViewModels
{
    public class PortalVM
    {
        public PunchInVM punchInVM { get; set; }
        public PunchOutVM punchOutVM { get; set; }
        public double cumulativeHours { get; set; }
        public string fullName { get; set; }
        public bool isPunchedIn { get; set; }
        public bool isReStore { get; set; }
        public int userId { get; set; }
    }
}