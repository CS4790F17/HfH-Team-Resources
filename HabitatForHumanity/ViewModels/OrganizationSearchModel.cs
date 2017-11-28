using HabitatForHumanity.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.ViewModels
{
    public class OrganizationSearchModel
    {
        public int? Page { get; set; }

        [Display(Name = "Name or email")]
        public string queryString { get; set; } = "";
        public IPagedList<Organization> SearchResults { get; set; }
        // public string SearchButton { get; set; }

        public int statusChoice { get; set; }
        public List<SelectListItem> statusDropDown = new List<SelectListItem>()
        {
            new SelectListItem { Text = "All", Value = "0",  },
            new SelectListItem { Text = "Active", Value = "1" },
            new SelectListItem { Text = "Inactive", Value = "2"}
        };
    }
}