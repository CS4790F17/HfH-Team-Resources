using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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
        [Display(Name = "Description")]
        public string description { get; set; }
        public DateTime beginDate { get; set; }
        public int status { get; set; }




        #region Database Access Methods
        /// <summary>
        /// Get all projects in the database.
        /// </summary>
        /// <returns>A list of all projects.</returns>
        public static List<Project> GetAllProjects()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.ToList();
        }

        /// <summary>
        /// Get a single project by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single project object with a matching id or null otherwise.</returns>
        public static Project GetProjectById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.Find(id);
        }

        /// <summary>
        /// Gets all the currently active projects
        /// </summary>
        /// <returns>A list of all projects that are currently active.</returns>
        public static List<Project> GetActiveProjects()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.projects.Where(x => x.status == 1).ToList();
        }


        /// <summary>
        /// Gets a project by its primary key: name+beginDate. Date must be in the format MM/DD/YYYY.
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="date">MM/DD/YYYY</param>
        /// <returns>Project object</returns>
        public static Project GetProjectByNameAndDate(string name, string date)
        {
            //parse date into datetime
            //probably not a good method of handling keys
            //seems to work with and without a 24 hour time
            DateTime beginDate = DateTime.Parse(date);
            VolunteerDbContext db = new VolunteerDbContext();

            //find the record with PK_name+beginDate
            //doesn't work with auto incrementing id field
            //return db.projects.Find(name, beginDate);

            //work around that lets the db save
            return db.projects.Where(x => x.name.Equals(name) && x.beginDate.Equals(beginDate)).Single();

        }

        /// <summary>
        /// Inserts a project into the database.
        /// </summary>
        /// <param name="project">The new project to be inserted.</param>
        public static void AddProject(Project project)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.projects.Add(project);
            db.SaveChanges();
        }

        /// <summary>
        /// Edit the project with new values.
        /// </summary>
        /// <param name="project">Project object where new values are stored.</param>
        public static void EditProject(Project project)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();
        }

        #endregion
    }

}