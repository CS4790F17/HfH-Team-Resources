using HabitatForHumanity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabitatForHumanity.ViewModels
{
    public static class ToolKitVM
    {
        public static IHtmlString Modal(string title)
        {
            string htmlString = String.Format("<!-- Modal --><div class=\"modal fade\" id=\"adminModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"adminModalLabel\"><div class=\"modal-dialog\" role=\"document\">" +
                "<div class=\"modal-content\"><div class=\"modal-header\">" +
                "<button type = \"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                "<h4 class=\"modal-title\" id=\"adminModallLabel\">{0}</h4></div>" +
                " <div class=\"modal-body\">" +
                "<!--The Div to insert returned partial into--><div id = \"partialTarget\"></div></div></div></div></div>", title);
            return new HtmlString(htmlString);
        }

        public static IHtmlString ModalButton(int id)
        {
            string htmlString = "<button targetId={0} type=\"button\" class=\"btn editButton\" data-toggle=\"modal\" data-target=\"#adminModal\"><span class=\"glyphicon glyphicon-pencil\" aria- hidden=\"true\" title=\"Edit\"></span></button>";
            return new HtmlString(htmlString);
        }
    }

    public class ProjectDropDownList
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<SelectListItem> Projects { get; set; }

        public ProjectDropDownList()
        {
            ReturnStatus st = Repository.GetAllProjects();
            createDropDownList((List<Project>)st.data);
        }
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

    }

    public class OrganizationDropDownList
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<SelectListItem> Organizations { get; set; }

        public OrganizationDropDownList()
        {
            ReturnStatus st = Repository.GetAllOrganizations();
            createDropDownList((List<Organization>)st.data);
        }

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
    }
    
}