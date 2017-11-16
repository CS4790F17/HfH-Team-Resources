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
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = ReturnStatus.ALL_CLEAR;
            st.errorMessage = "";
            st.data = projects;
            return st;
        }


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
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = ReturnStatus.ALL_CLEAR;
            st.errorMessage = "";
            st.data = project;
            return st;
        }


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
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = (int)ReturnStatus.ALL_CLEAR;
            st.errorMessage = "";
            st.data = projects;
            return st;
        }

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
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database. Try again later.";
                return st;
            }
            st.errorCode = (int)ReturnStatus.ALL_CLEAR;
            st.errorMessage = "";
            st.data = project;
            return st;

        }


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
                st.errorCode = ReturnStatus.FAIL_ON_INSERT;
                st.errorMessage = e.Message; // project data here?
                //log some stuff
                return st;
            }
            st.errorCode = ReturnStatus.ALL_CLEAR;
            st.errorMessage = "";
            return st;

        }

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
                st.errorCode = (int)ReturnStatus.FAIL_ON_INSERT;
                st.errorMessage = e.Message; // project data here?
                //log some stuff
                return st;
            }
            st.errorCode = (int)ReturnStatus.ALL_CLEAR;
            st.errorMessage = "";
            return st;     
        }

 

        #endregion
    }

}