using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace HabitatForHumanity.ViewModels
{
    public class VolunteerSearchModel
    {
        public int? Page { get; set; }

        [Display(Name ="Name or email")]
        public string queryString { get; set; }
        public ProjectDropDownList projects { get; set; }
        public int projectId { get; set; }

        public OrganizationDropDownList orgs { get; set; }
        public int orgId { get; set; }
        public IPagedList<UsersVM> SearchResults { get; set; }
        public string SearchButton { get; set; }

        public VolunteerSearchModel()
        {
            projects = new ProjectDropDownList();
            orgs = new OrganizationDropDownList();
        }
    }
}