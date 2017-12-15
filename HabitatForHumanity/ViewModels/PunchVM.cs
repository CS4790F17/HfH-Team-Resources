using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HabitatForHumanity.ViewModels
{
    public class PunchVM
    {
    }

    public class PunchOutVM
    {
        public int timeSheetNumber { get; set; }
        public int userNumber { get; set; }
        public int projectNumber { get; set; }
        public int orgNumber { get; set; }
        public DateTime inTime { get; set; }

        public PunchOutVM()
        {

        }

        public PunchOutVM(TimeSheet ts)
        {
            timeSheetNumber = ts.Id;
            userNumber = ts.user_Id;
            projectNumber = ts.project_Id;
            orgNumber = ts.org_Id;
            inTime = ts.clockInTime;
        }
    }

    public class PunchInVM
    {
        public int userId { get; set; }
       // [Required, Range(1, int.MaxValue, ErrorMessage = "Please Select an Organization")]
        [DisplayName("Organization")]
        public int orgId { get; set; }

        [DisplayName("Project")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select a Project")]
        public int projectId { get; set; }
        //  public string userName { get; set; }
        public ProjectDropDownList projects = new ProjectDropDownList(true);
        public OrganizationDropDownList orgs = new OrganizationDropDownList(true);

        public PunchInVM()
        {

        }
    }

}