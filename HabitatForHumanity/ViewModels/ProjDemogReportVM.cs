using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class ProjDemogReportVM
    {
        public DateTime monthName { get; set; }
        public string category { get; set; }

        [Display(Name = "Volunteers")]
        public int numVolunteers { get; set; }

        [Display(Name = "Hours")]
        public int numHours { get; set; }

        [Display(Name = "Student")]
        public int numStudents { get; set; }

        [Display(Name = "Veteran")]
        public int numVeterans { get; set; }

        [Display(Name = "Disabled")]
        public int numDisabled { get; set; }

        [Display(Name = "Income <$25k")]
        public int numUnder25k { get; set; }

        [Display(Name = "Native American")]
        public int numNative { get; set; }

        [Display(Name = "Asian")]
        public int numAsian { get; set; }

        [Display(Name = "Black/ Afro-Amer")]
        public int numBlack { get; set; }

        [Display(Name = "Hispanic")]
        public int numHispanic { get; set; }

        [Display(Name = "Hawaiian/ Pac.Isl")]
        public int numHawaiian { get; set; }

        [Display(Name = "White")]
        public int numWhite { get; set; }

        [Display(Name = "Two Ethnicities")]
        public int numTwoEthnic { get; set; }

        [Display(Name = "Male")]
        public int male { get; set; }

        [Display(Name = "Female")]
        public int female { get; set; }


    }

    public class ProjDemogSearchResults
    {

    }
}