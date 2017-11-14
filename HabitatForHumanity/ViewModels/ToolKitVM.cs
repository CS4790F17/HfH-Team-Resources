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

        /// <summary>
        /// Takes a list of Projects and separates them into select list items. To be used in conjunction
        /// with @Html.DropDownListFor(x => x.pdd.ProjectId, Model.pdd.Projects)
        /// </summary>
        /// <param name="items">List of Projects</param>
        public void createDropDownList(List<Project> items)
        {
            var SelectList = new List<SelectListItem>();
            SelectList.Add(new SelectListItem
            {
                Value = "-1",
                Text = "Select a Project"
            });
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

        //TODO: Uncomment when Project model has been refactored
        ///// <summary>
        ///// Creates a drop down list for all Projects in the database.
        ///// </summary>
        //public void createDownListFromAll()
        //{
        //    ReturnStatus st = Repository.GetAllProjects();
        //    if(ReturnStatus.tryParseOrganizationList(st, out List<Projects> result))
        //    {
        //        createDropDownList(result);
        //    }

        //}
    }

    public class OrganizationDropDownList
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<SelectListItem> Organizations { get; set; }

        /// <summary>
        /// Takes a list of Organizations and separates them into select list items. To be used in conjunction
        /// with @Html.DropDownListFor(x => x.odd.OrganizationId, Model.odd.Organizations)
        /// </summary>
        /// <param name="items">List of Organizations</param>
        public void createDropDownList(List<Organization> items)
        {
            var SelectList = new List<SelectListItem>();
            SelectList.Add(new SelectListItem
            {
                Value = "-1",
                Text = "Select an Organization"
            });

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


        /// <summary>
        /// Creates a drop down list for all Organizations in the database.
        /// </summary>
        public void createDownListFromAll()
        {
            ReturnStatus st = Repository.GetAllOrganizations();
            if (ReturnStatus.tryParseOrganizationList(st, out List<Organization> result))
            {
                createDropDownList(result);
            }
        }

    }


}