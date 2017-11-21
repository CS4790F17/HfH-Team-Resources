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
        /// <summary>
        /// Creates a Modal with the given properties
        /// </summary>
        /// <param name="title">The title that the modal is to display.</param>
        /// <param name="divId">The id of the Modal div.</param>
        /// <param name="partialTarget">The innermost div that the partial page needs to target.</param>
        /// <returns></returns>
        public static IHtmlString Modal(string title, string divId, string partialTarget)
        {
            string htmlString = String.Format("<!-- Modal --><div class=\"modal fade\" id=\"{1}\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"{1}Label\"><div class=\"modal-dialog\" role=\"document\">" +
                "<div class=\"modal-content\"><div class=\"modal-header\">" +
                "<button type = \"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                "<h4 class=\"modal-title\" id=\"{1}lLabel\">{0}</h4></div>" +
                " <div class=\"modal-body\">" +
                "<!--The Div to insert returned partial into--><div id = \"{2}\"></div></div></div></div></div>", title, divId, partialTarget);
            return new HtmlString(htmlString);
        }

        /// <summary>
        /// Creates a Button that can activate a modal.
        /// </summary>
        /// <param name="elementClass">The class of the button. Used for ajax targeting: $("[elementClass]").attr("dataId")</param>
        /// <param name="targetModal">The modal that opens when the button is clicked</param>
        /// <param name="innerHtml">Html inside of the button itself. Useful for using spans with glyphicons in them.</param>
        /// <param name="dataId">The id of the data type used by the button. If my VolunteerId is 1, my dataId would be 1.</param>
        /// <returns></returns>
        public static IHtmlString ModalButton(string elementClass, string targetModal, string innerHtml, string dataId)
        {
            string htmlString = String.Format("<button  dataId={3} type=\"button\" class=\"btn {0}\" data-toggle=\"modal\" data-target=\"#{1}\">{2}</button>", elementClass, targetModal, innerHtml, dataId);
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