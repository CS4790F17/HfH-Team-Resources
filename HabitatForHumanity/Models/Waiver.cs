using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("Waiver")]
    public class Waiver
    {
        [Key]
        public int Id { get; set; }
        public int userId { get; set; }
        public DateTime signDate { get; set; }
        public string emContFirstName { get; set; }
        public string emContLastName { get; set; }
        public string relation { get; set; }
        public string emContHomePhone { get; set; }
        public string emContWorkPhone { get; set; }
        public string signature { get; set; } // change me later to an image
        public bool consent { get; set; }

        public static void AddWaiver(Waiver w)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.waivers.Add(w);
            db.SaveChanges();
        }

        public static bool HasWaiver(int userId)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var waivers = db.waivers.Where(w => w.userId == userId);
            bool isValid = false;
            if(waivers.Count() > 0)
            {
                foreach (Waiver w in waivers)
                {
                    if(DateTime.Today.AddDays(-365) < w.signDate && w.consent)
                    {
                        isValid = true;
                    }
                }
            }
          
            return isValid;
        }
    }
}