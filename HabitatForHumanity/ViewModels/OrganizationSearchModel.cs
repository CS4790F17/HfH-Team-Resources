using HabitatForHumanity.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class OrganizationSearchModel
    {
        public int? Page { get; set; }

        [Display(Name = "Name or email")]
        public string queryString { get; set; }
        public IPagedList<Organization> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }
}