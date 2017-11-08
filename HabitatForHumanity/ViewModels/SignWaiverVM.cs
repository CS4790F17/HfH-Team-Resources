using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitatForHumanity.Models;

namespace HabitatForHumanity.ViewModels
{
    public class SignWaiverVM
    {
        public User user { get; set; }
        public string signature { get; set; }
    }
}