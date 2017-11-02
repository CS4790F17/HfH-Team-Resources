using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }


        public static List<Organization> GetAllOrganizations()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.ToList();
        }

        public static Organization GetOrganizationById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.Find(id);
        }

        public static Organization GetOrganizationByName(string name)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.Where(x => x.name.Equals(name)).Single();
        }
    }
}