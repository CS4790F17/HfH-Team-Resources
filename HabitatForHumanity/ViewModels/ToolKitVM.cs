using HabitatForHumanity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.ViewModels
{
    public class ToolKitVM
    {
    }

    public class ProjectDropDownList
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<SelectListItem> Projects { get; set; }

        //extracts items out of 
        public void createDropDownList(List<Project> items)
        {
            var SelectList = new List<SelectListItem>();
            foreach (Project item in items)
            {
                SelectList.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.name
                });
            }
            Projects = SelectList;
        }
    }

    public class OrganizationDropDownList
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<SelectListItem> Organizations { get; set; }

        public void createDropDownList(List<Organization> items)
        {
            var SelectList = new List<SelectListItem>();
            foreach (Organization item in items)
            {
                SelectList.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.name
                });
            }
            Organizations = SelectList;
        }

    }


}