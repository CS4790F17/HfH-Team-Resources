using HabitatForHumanity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.ViewModels
{
    public class DemographicsDropDowns
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
        public class GenericDropDownList
        {
            public int dropdownId { get; set; }
            public string demographic { get; set; }
            public List<SelectListItem> Tiers { get; set; }

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
}