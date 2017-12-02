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
        public class IncomeDropDownList
        {
            public int incomeTierId { get; set; }
            public string tierName { get; set; }
            public List<SelectListItem> Tiers { get; set; }

            public IncomeDropDownList()
            {
                var SelectList = new List<SelectListItem>();
                string[] tiers = new string[] { "Under $24,999", "$25,000 to $49,999", "$50,000 to $74,999", "$75,000 to $99,999", "Over $100,000" };
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

        }// end household income
    }
}