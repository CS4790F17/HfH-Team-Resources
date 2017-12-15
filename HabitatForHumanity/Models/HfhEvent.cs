using HabitatForHumanity.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("HfhEvent")]
    public class HfhEvent
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Event Name")]
        [Display(Name = "Event Name")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Event Partner")]
        public string eventPartner { get; set; }

        [Required(ErrorMessage = "Start Date is Required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "End Date is Required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime endDate { get; set; }

        #region Database methods
        /// <summary>
        /// Get all events in the database. Ordered with most recent first
        /// </summary>
        /// <returns>A list of all events.</returns>
        public static ReturnStatus GetAllHfhEvents()
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new List<HfhEvent>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.hfhEvents.OrderByDescending(e => e.startDate).ToList();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }


        /// <summary>
        /// Get a single event by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single event object with a matching id otherwise null.</returns>
        public static ReturnStatus GetHfhEventById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new HfhEvent();
            if (id < 1)
            {
                return new ReturnStatus() { errorCode = -1, data = null };
            }
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();    
                st.data = db.hfhEvents.Find(id);
                st.errorCode = ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }


        /// <summary>
        /// Edits the event with new values.
        /// </summary>
        /// <param name="event">The event object with new values.</param>
        public static ReturnStatus EditHfhEvent(HfhEvent hfhEvent)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(hfhEvent).State = EntityState.Modified;
                db.SaveChanges();

                st.errorCode = ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Gets a list of Projects that have a relationship 
        /// to the HfhEvent
        /// </summary>
        /// <param name="id">The Id of the HfhEvent</param>
        /// <returns>List of Project view models</returns>
        public static ReturnStatus GetEventProjectsByEventId(int id)
        {
            ReturnStatus rs = new ReturnStatus();
            string sql = " SELECT " +
                "H.Id AS hfhEventId, " +
                "PE.project_Id AS projectId, " +
                "P.name AS projectName, " +
                "CAST(1 AS bit) AS isSelected " +
                "FROM " +
                "HfhEvent H " +
                "LEFT JOIN ProjectEvent PE " +
                "ON H.Id = PE.event_Id " +
                "LEFT JOIN Project P " +
                "ON PE.project_Id = P.Id " +
                "WHERE " +
                "PE.project_Id IS NOT NULL " +
                "AND " +
                "H.Id = @hfhEventId ";
            var evId = new SqlParameter("@hfhEventId", id);
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var projects = db.Database.SqlQuery<EventAddRemoveProjectVM>(sql, evId).ToList();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
                rs.data = projects;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }

        /// <summary>
        /// Gets a list of Projects that are possible candidates
        /// to add to an HfhEvent. This includes all Projects that
        /// are not currently joined to the HfhEvent.
        /// </summary>
        /// <param name="id">The HfhEvent Id</param>
        /// <returns>List of EventAddRemoveProjectVM view models</returns>
        public static ReturnStatus GetNotHfhEventProjects(int id)
        {
            ReturnStatus rs = new ReturnStatus();
            string sql = " SELECT " +
                " @hfhEventId AS hfhEventId, " +
                " P.Id AS projectId, " +
                " P.name AS projectName, " +
                " CAST(0 AS bit) AS isSelected " +
                " FROM Project P " +
                " WHERE " +
                " P.Id NOT IN( " +
                    " SELECT " +
                    " PE.project_Id " +
                    " FROM " +
                    " ProjectEvent PE " +
                    " WHERE " +
                    " PE.project_Id IS NOT NULL " +
                    " AND PE.event_Id = @hfhEventId )";
            var evId = new SqlParameter("@hfhEventId", id);
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var projects = db.Database.SqlQuery<EventAddRemoveProjectVM>(sql, evId).ToList();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
                rs.data = projects;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }

        /// <summary>
        /// Creates a joining relationship for a Project 
        /// to an HfhEvent
        /// </summary>
        /// <param name="pe">A joining object with the id's of the two objects to join</param>
        /// <returns>Returns an ErrorCode indicating the result of the db insert</returns>
        public static ReturnStatus AddProjectToEvent(ProjectEvent pe)
        {
            ReturnStatus rs = new ReturnStatus();
            VolunteerDbContext db = new VolunteerDbContext();
            try
            {
                db.eventProjects.Add(pe);
                db.SaveChanges();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
            }
       
            return rs;
        }

        /// <summary>
        /// Deletes the relationship between an HfhEvent 
        /// and a Project
        /// </summary>
        /// <param name="vm">An EventAddRemoveProjectVM</param>
        /// <returns>Returns an errorCode describing the result from the database</returns>
        public static ReturnStatus RemoveEventProject(EventAddRemoveProjectVM vm)
        {
            ReturnStatus rs = new ReturnStatus();
            VolunteerDbContext db = new VolunteerDbContext();
            try
            {
                ProjectEvent pe = db.eventProjects.Where(e => e.project_Id == vm.projectId && e.event_Id == vm.hfhEventId).ToList().FirstOrDefault();
                db.eventProjects.Remove(pe);
                db.SaveChanges();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
            }

            return rs;
        }

        public static ReturnStatus CreateEvent(HfhEvent hfhEvent)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.hfhEvents.Add(hfhEvent);
                db.SaveChanges();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
            }
            return rs;
       
        }
        /// <summary>
        /// Deletes an HfhEvent and any relationships
        /// it has to Projects from the database 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ErrorCode describing result of delete action</returns>
        public static ReturnStatus DeleteEvent(int id)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                // get rid of all foreign key relationships first
                var joinsToRemove = db.eventProjects.Where(ep => ep.event_Id == id);
                db.eventProjects.RemoveRange(joinsToRemove);
                db.SaveChanges();

                // remove the event itself
                HfhEvent hfhEvent = db.hfhEvents.Find(id);
                db.hfhEvents.Remove(hfhEvent);
                db.SaveChanges();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
            }
            return rs;
        }

        #endregion
    }


}
