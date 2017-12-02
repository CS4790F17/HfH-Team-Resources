using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.ViewModels
{
    public class DemographicsVM
    {
        public int volunteerId { get; set; }
        public int incomeId { get; set; }
        public int ethnicityId { get; set; }
        public int collegeStatus { get; set; }
        public int veteranStatus { get; set; }
        public int disabledStatus { get; set; }
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
            "Under $24,999", "$25,000 to $49,999", "$50,000 to $74,999", "$75,000 to $99,999", "Over $100,000" };
        public static string[] ethnicityTiers = new string[] {
                    "Native Amerian or American Indian",
                    "Asian",
                    "Black or African American",
                    "Hispanic or Latino or Spanish Origin",
                    "Native Hawaiian or other Pacific Islander",
                    "White",
                    "Two or more",
                    "Rather not say"};
        public static string[] yesNoTiers = new string[] { "No", "Yes" };
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