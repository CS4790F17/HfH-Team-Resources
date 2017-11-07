using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class BadPunchVM
    {
        public string name { get; set; }
        public string strPunchDate { get; set; }

        public static List<BadPunchVM> GetDummyBadPunches()
        {
            List<BadPunchVM> punches = new List<BadPunchVM>();
            punches.Add(new BadPunchVM() { name = "Fred Flinstone", strPunchDate = "11/12/17" });
            punches.Add(new BadPunchVM() { name = "Wilma Flinstone", strPunchDate = "11/12/17" });
            punches.Add(new BadPunchVM() { name = "Barney Rubble", strPunchDate = "11/11/17" });
            punches.Add(new BadPunchVM() { name = "Betty Rubble", strPunchDate = "11/11/17" });
            return punches;
        }
    }


}