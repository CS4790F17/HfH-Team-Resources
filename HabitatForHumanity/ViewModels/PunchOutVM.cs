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
        public int timeSheetNumber { get; set; }
        public int userNumber { get; set; }
        public int projectNumber { get; set; }
        public int orgNumber { get; set; }
        public DateTime inTime { get; set; }

    }
    public class PunchInVM
    {
        public int userId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Please Select an Organization")]
        public int orgId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Please Select a Project")]
        public int projectId { get; set; }
        public string userName { get; set; }
        public ProjectDropDownList projects;
        public OrganizationDropDownList orgs = new OrganizationDropDownList();
        public PunchInVM(List<Project> ps)
        {
            projects = new ProjectDropDownList(ps);
        }

    }
    public class ProjectListVM
    {
        public int Id { get; set; }
        public string projectName { get; set; }
    }

}