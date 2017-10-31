using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("Project")]
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string category { get; set; }

        public static List<Project> GetProjects()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.ToList();
        }

        public static int GetProjectIdByName(string name)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var ps = db.projects.Where(p => p.name.Equals(name));
            return ps.FirstOrDefault().Id;
        }
    }
}