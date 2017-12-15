using HabitatForHumanity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class ManageEventVM
    {
        public HfhEvent hfhEvent { get; set; }
        public List<EventAddRemoveProjectVM> eventProjects { get; set; }
        public List<EventAddRemoveProjectVM> addableProjects { get; set; }

    }
}