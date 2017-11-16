using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.ViewModels
{
    public class SearchTimeDetailVM
    {
        //public User user { get; set; }
        public int userId { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String emailAddress { get; set; }
        public List<TimeSheet> timeSheets { get; set; }
    }
}