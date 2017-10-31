using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class SignWaiverVM
    {
        // join stuff
        public int waiverId { get; set; }
        public int userId { get; set; }

        //user stuff
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string phone { get; set; }

        // waiver stuff
        public DateTime signDate { get; set; }
        public string emContFirstName { get; set; }
        public string emContLastName { get; set; }
        public string relation { get; set; }
        public string emContHomePhone { get; set; }
        public string emContWorkPhone { get; set; }
        public string signature { get; set; } // change me later to an image
        public bool consent { get; set; }

    }
}