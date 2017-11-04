using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;
using System.ComponentModel.DataAnnotations;

namespace HabitatForHumanity.ViewModels
{
    public class PunchOutVM
    {
        public TimeSheet timeSheet { get; set; }
        public int orgId { get; set; }
        public string project { get; set; }
        public string userName { get; set; }


    }
    public class PunchInVM
    {
        public int userId { get; set; }
        public int orgId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please Select a Project")]
        public int projectId { get; set; }
        public string userName { get; set; }
        // public List<ProjectListVM> projectList { get; set; }
        // public List<Organization> orgList { get; set; }
        public ProjectDropDownList projects = new ProjectDropDownList();
        public OrganizationDropDownList orgs = new OrganizationDropDownList();

    }
    public class ProjectListVM
    {
        public int Id { get; set; }
        public string projectName { get; set; }
    }

}