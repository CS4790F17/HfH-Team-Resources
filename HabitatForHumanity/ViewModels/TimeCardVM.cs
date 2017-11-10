using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;
namespace HabitatForHumanity.ViewModels
{
    public class TimeCardVM
    {
        public string orgName { get; set; }
        public string projName { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public TimeSheet timeSheet{ get; set; }

        public TimeCardVM(TimeSheet ts)
        {
            this.timeSheet = ts;

            User user = Repository.GetUser(ts.user_Id);
    
            this.orgName = Repository.GetOrganizationById(ts.org_Id).name;
            this.projName = Repository.GetProjectById(ts.project_Id).name;
            this.userName = GetUserName(user);
            this.userEmail = user.emailAddress;
        }

        private string GetUserName (User user)
        {
            string volName = "";
            if (string.IsNullOrEmpty(user.firstName) && string.IsNullOrEmpty(user.lastName))
            {
                volName = user.emailAddress;
            }
            else if (string.IsNullOrEmpty(user.firstName))
            {
                volName += user.emailAddress + " ";
            }
            else if (string.IsNullOrEmpty(user.lastName))
            {
                volName += user.emailAddress;
            }
            else
            {
                volName += user.firstName + " " + user.lastName;
            }
            return volName;
        }
    }
}