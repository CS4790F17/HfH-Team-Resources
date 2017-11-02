using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class ToolKitExampleVM
    {
        public ProjectDropDownList pdd = new ProjectDropDownList();
        public OrganizationDropDownList odd = new OrganizationDropDownList();
        public string projectTest { get; set; }
        public string projectTest2 { get; set; }
    }
}