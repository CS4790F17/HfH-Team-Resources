using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class EventAddRemoveProjectVM
    {
        public int hfhEventId { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }
        public bool isSelected { get; set; }
    }
}