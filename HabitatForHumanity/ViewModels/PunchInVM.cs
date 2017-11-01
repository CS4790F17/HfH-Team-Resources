using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.ViewModels
{
    public class PunchInVM
    {
        public int userId { get; set; }
        public string projectName { get; set; }
        public DateTime timeIn { get; set; }
        public DateTime timeOut { get; set; }
        public List<string> projects { get; set; }
        public PunchInVM()
        {
            this.projects = Repository.GetProjectNames();
        }

    }
}