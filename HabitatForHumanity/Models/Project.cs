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
        public string description { get; set; }
        public DateTime beginDate { get; set; }

        /// <summary>
        /// Current open status of the project. 0 - inactive, 1 - active
        /// </summary>
        public int status { get; set; }


        public Project()
        {
            Id = -1;
            name = "";
            description = "";
            beginDate = DateTime.Today;
        }




        #region Database Access Methods
        /// <summary>
        /// Get all projects in the database.
        /// </summary>
        /// <returns>A list of all projects.</returns>
        //public static List<Project> GetAllProjects()
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    return db.projects.ToList();
        //}
        public static ReturnStatus GetAllProjects()
        {
            ReturnStatus st = new ReturnStatus();
            List<Project> projects = new List<Project>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                projects = db.projects.ToList();
                
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
            st.errorMessage = "";
            st.userErrorMsg = "";
            st.data = projects;
            return st;
        }

        /// <summary>
        /// Get a single project by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single project object with a matching id or null otherwise.</returns>
        //public static Project GetProjectById(int id)
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    return db.projects.Find(id);
        //}
        public static ReturnStatus GetProjectById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            Project project = new Project();
            
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                project = db.projects.Find(id);
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
            st.errorMessage = "";
            st.userErrorMsg = "";
            st.data = project;
            return st;
        }

        /// <summary>
        /// Gets all the currently active projects
        /// </summary>
        /// <returns>A list of all projects that are currently active.</returns>
        //public static List<Project> GetActiveProjects()
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    return db.projects.Where(x => x.status == 1).ToList();
        //}
        public static ReturnStatus GetActiveProjects()
        {
            ReturnStatus st = new ReturnStatus();
            List<Project> projects = new List<Project>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                projects = db.projects.Where(x => x.status == 1).ToList();

            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
            st.errorMessage = "";
            st.userErrorMsg = "";
            st.data = projects;
            return st;
        }


        /// <summary>
        /// Gets a project by its primary key: name+beginDate. Date must be in the format MM/DD/YYYY.
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="date">MM/DD/YYYY</param>
        /// <returns>Project object</returns>
        //public static Project GetProjectByNameAndDate(string name, string date)
        //{
        //    //parse date into datetime
        //    //probably not a good method of handling keys
        //    //seems to work with and without a 24 hour time
        //    DateTime beginDate = DateTime.Parse(date);
        //    VolunteerDbContext db = new VolunteerDbContext();

        //    //find the record with PK_name+beginDate
        //    //doesn't work with auto incrementing id field
        //    //return db.projects.Find(name, beginDate);

        //    //work around that lets the db save
        //    return db.projects.Where(x => x.name.Equals(name) && x.beginDate.Equals(beginDate)).Single();

        //}
        public static ReturnStatus GetProjectByNameAndDate(string name, string date)
        {
            //parse date into datetime
            //probably not a good method of handling keys
            //seems to work with and without a 24 hour time
            DateTime beginDate = DateTime.Parse(date);
          

            //find the record with PK_name+beginDate
            //doesn't work with auto incrementing id field
            //return db.projects.Find(name, beginDate);

            //work around that lets the db save
            //return db.projects.Where(x => x.name.Equals(name) && x.beginDate.Equals(beginDate)).Single();

            ReturnStatus st = new ReturnStatus();
            Project project = new Project();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                project = db.projects.Where(x => x.name.Equals(name) && x.beginDate.Equals(beginDate)).Single();
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
            st.errorMessage = "";
            st.userErrorMsg = "";
            st.data = project;
            return st;

        }


        /// <summary>
        /// Inserts a project into the database.
        /// </summary>
        /// <param name="project">The new project to be inserted.</param>
        //public static void AddProject(Project project)
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    db.projects.Add(project);
        //    db.SaveChanges();
        //}
        public static ReturnStatus AddProject(Project project)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.projects.Add(project);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.FAIL_ON_INSERT;
                st.userErrorMsg = "Project failed to save. Try again.";
                st.errorMessage = e.Message; // project data here?
                //log some stuff
                return st;
            }
            st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
            st.userErrorMsg = "";
            st.errorMessage = "";
            return st;

        }

        /// <summary>
        /// Edit the project with new values.
        /// </summary>
        /// <param name="project">Project object where new values are stored.</param>
        //public static void EditProject(Project project)
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    db.Entry(project).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        public static ReturnStatus EditProject(Project project)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.FAIL_ON_INSERT;
                st.userErrorMsg = "Project failed to save. Try again.";
                st.errorMessage = e.Message; // project data here?
                //log some stuff
                return st;
            }
            st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
            st.userErrorMsg = "";
            st.errorMessage = "";
            return st;     
        }

        ///// <summary>
        ///// Deletes a project from the database.
        ///// </summary>
        ///// <param name="project">The project object to delete.</param>
        //public static void DeleteProject(Project project)
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    db.projects.Attach(project);
        //    db.projects.Remove(project);
        //    db.SaveChanges();
        //}

        ///// <summary>
        ///// Deletes a project from the database by id.
        ///// </summary>
        ///// <param name="id">The id of the project to delete</param>
        //public static void DeleteProjectById(int id)
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    Project proj = db.projects.Find(id);
        //    if(proj != null)
        //    {
        //        db.projects.Remove(proj);
        //        db.SaveChanges();
        //    }
        //}
        #endregion
    }

}