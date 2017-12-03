using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.ViewModels
{
    public class DemographicsVM
    {
        public int volunteerId { get; set; }

        [Display(Name = "HouseHold Income")]
        public int incomeId { get; set; }


        [Display(Name = "Ethnic Background")]
        public int ethnicityId { get; set; }


        [Display(Name = "College Student?")]
        public int collegeStatus { get; set; }


        [Display(Name = "Veteran of the Armed Forces?")]
        public int veteranStatus { get; set; }


        [Display(Name = "Disabled?")]
        public int disabledStatus { get; set; }
        public List<SelectListItem> incomeTiers { get; set; }
        public GenericDropDownList incomeDD { get; set; }
        public GenericDropDownList ethnicityDD { get; set; }
        public GenericDropDownList collegeDD { get; set; }
        public GenericDropDownList veteranDD { get; set; }
        public GenericDropDownList disabledDD { get; set; }
        public DemographicsVM()
        {
            this.incomeDD = new GenericDropDownList(DemographicsData.incomeTiers);
            this.ethnicityDD = new GenericDropDownList(DemographicsData.ethnicityTiers);
            this.collegeDD = new GenericDropDownList(DemographicsData.yesNoTiers);
            this.veteranDD = new GenericDropDownList(DemographicsData.yesNoTiers);
            this.disabledDD = new GenericDropDownList(DemographicsData.yesNoTiers);
        }
    }

    public class DemographicsData
    {
        public static string[] incomeTiers = new string[] {
            "Prefer not to answer",
            "Under $24,999",
            "$25,000 to $49,999",
            "$50,000 to $74,999",
            "$75,000 to $99,999",
            "Over $100,000" };
        public static string[] ethnicityTiers = new string[] {
            "Prefer not to answer",
            "Native Amerian or American Indian",
            "Asian",
            "Black or African American",
            "Hispanic or Latino or Spanish Origin",
            "Native Hawaiian or other Pacific Islander",
            "White",
            "Two or more"};
        public static string[] yesNoTiers = new string[] { "Prefer not to answer", "No", "Yes" };
    }

    public class GenericDropDownList
    {
        public int dropdownId { get; set; }
        public string demographic { get; set; }
        public List<SelectListItem> Tiers { get; set; }

        // note that indexes (ids) start at 1 to support moving to data tables later
        public GenericDropDownList(string[] tiers)
        {
            var SelectList = new List<SelectListItem>();

            for (int i = 1; i <= tiers.Length; i++)
            {
                SelectList.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = tiers[i - 1]
                });
            }
            Tiers = SelectList;
        }

    }
}