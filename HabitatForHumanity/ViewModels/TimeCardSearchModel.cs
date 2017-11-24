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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "From")]
        public DateTime rangeStart { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "To")]
        public DateTime rangeEnd { get; set; }

        public TimeCardSearchModel()
        {
            rangeStart = DateTime.Now.AddMonths(-1);
            rangeEnd = DateTime.Now;
        }
    }

    public class TimeCardVM
    {
        public int timeId { get; set; }
        public int userId { get; set; }
        public int projId { get; set; }
        public int orgId { get; set; }

        [Display(Name = "Time In")]
        [DataType(DataType.DateTime)]
        public DateTime inTime { get; set; }
        [DataType(DataType.DateTime)]
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