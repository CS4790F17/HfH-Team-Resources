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
        public IPagedList<UsersVM> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }
}