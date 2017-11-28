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
        [Display(Name = "Project Name")]
        public string name { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Begin Date")]
        public DateTime beginDate { get; set; }
        public int status { get; set; }
        //[Display(Name = "Total Hours Logged ")]
        //public double hoursLogged { get; set; }
        //[Display(Name = "Total Volunteers")]
        //public double numVolunteers { get; set; }

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

        public static ReturnStatus GetProjectIdByName(string name)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var pId = db.projects.Where(p => p.name.Contains(name)).FirstOrDefault().Id;
                rs.errorCode = ReturnStatus.ALL_CLEAR;
                rs.data = pId;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }   
            return rs;
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
            catch (Exception e)
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


        public static ReturnStatus GetProjectPage(int page, int itemsPerPage, ref int totalProjects, string queryString)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            List<Project> proj = new List<Project>();

            proj = (from p in db.projects
                    where p.name.Contains(queryString)
                    orderby p.Id ascending
                    select p)
                    .Skip(itemsPerPage * page)
                    .Take(itemsPerPage).ToList();
            totalProjects = db.projects.Count();
            return new ReturnStatus { errorCode = ReturnStatus.ALL_CLEAR, data = proj };
        }

        public static ReturnStatus GetProjectPageWithFilter(int page, int itemsPerPage, ref int totalProjects, int statusChoice, string queryString)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            List<Project> proj = new List<Project>();

            proj = (from p in db.projects
                    where p.status.Equals(statusChoice) && p.name.Contains(queryString)
                    orderby p.Id ascending
                    select p)
                    .Skip(itemsPerPage * page)
                    .Take(itemsPerPage).ToList();
            totalProjects = db.projects.Count(x =>x.status.Equals(statusChoice) && x.name.Contains(queryString));
            return new ReturnStatus { errorCode = ReturnStatus.ALL_CLEAR, data = proj };
        }

        #endregion
    }

}