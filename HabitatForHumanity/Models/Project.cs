using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using HabitatForHumanity.ViewModels;

namespace HabitatForHumanity.Models
{
    [Table("Project")]
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime beginDate { get; set; }
        public int status { get; set; } // 0 - inactive, 1 - active

        public static List<Project> GetActiveProjects()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.Where(p => p.status == 1).ToList();
        }
    }
}