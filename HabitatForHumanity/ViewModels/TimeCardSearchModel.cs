using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace HabitatForHumanity.ViewModels
{
    public class TimeCardSearchModel
    {
        public int? Page { get; set; }

        [Display(Name = "Name or email")]
        public string queryString { get; set; }
        public IPagedList<TimeCardVM> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }

    public class TimeCardVM
    {
        public int timeId { get; set; }
        public int userId { get; set; }
        public int projId { get; set; }
        public int orgId { get; set; }

        [Display(Name = "Time In")]
        public DateTime inTime { get; set; }

        [Display(Name = "Time Out")]
        public DateTime outTime { get; set; }

        [Display(Name = "Organization")]
        public string orgName { get; set; }

        [Display(Name = "Project")]
        public string projName { get; set; }

        [Display(Name = "Volunteer")]
        public string volName { get; set; }

        [Display(Name = "Hours")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double elapsedHrs { get; set; }
    }
}