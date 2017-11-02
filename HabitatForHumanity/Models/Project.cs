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
        
        public int Id { get; set; }

        [Key, Column(Order = 0)]
        public string name { get; set; }

        public string description { get; set; }

        [Key, Column(Order = 1)]
        public DateTime beginDate { get; set; }

        public int status { get; set; } // 0 - inactive, 1 - active

        public static List<Project> getAllProjects()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.ToList();
        }

        public static Project getProjectById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.Find(id);
        }

        public static List<Project> getActiveProjects()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.Where(x => x.status == 1).ToList();
        }


        /// <summary>
        /// Gets a project by its primary key: name+beginDate. Date must be in the format MM/DD/YYYY.
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="date">MM/DD/YYYY</param>
        /// <returns></returns>
        public static Project getProjectByNameAndDate(string name, string date)
        {
            //parse date into datetime
            //probably not a good method of handling keys
            //seems to work with and without a 24 hour time
            DateTime beginDate = DateTime.Parse(date);
            VolunteerDbContext db = new VolunteerDbContext();

            //find the record with PK_name+beginDate
            return db.projects.Find(name, beginDate);
        }
    }
}