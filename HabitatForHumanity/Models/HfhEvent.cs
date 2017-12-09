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
        [Display(Name = "Event Name*")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Event Partner")]
        public string eventPartner { get; set; }

        [Required(ErrorMessage = "Start Date is Required")]
        [Display(Name = "Start Date*")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "End Date is Required")]
        [Display(Name = "End Date*")]
        [DataType(DataType.Date)]
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
        /// Deletes an event from the database.
        /// </summary>
        /// <param name="hfhEvent">The event object to delete</param>
        public static ReturnStatus DeleteHfhEvent(HfhEvent hfhEvent)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.hfhEvents.Attach(hfhEvent);
                db.hfhEvents.Remove(hfhEvent);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
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
        /// Deletes an organization from the database by id.
        /// </summary>
        /// <param name="id">The id of the organization to delete.</param>
        public static ReturnStatus DeleteHfhEventById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                HfhEvent hfhEvent = db.hfhEvents.Find(id);
                if (hfhEvent != null)
                {
                    db.hfhEvents.Remove(hfhEvent);
                    db.SaveChanges();

                    st.errorCode = ReturnStatus.ALL_CLEAR;
                    return st;
                }
                st.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public static ReturnStatus GetHfhEventByNameSQL(string name)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            ReturnStatus st = new ReturnStatus();
            try
            {
                var hfhName = new SqlParameter("@Name", "%" + name + "%");
                var events = db.hfhEvents.SqlQuery("SELECT * FROM HfhEvent WHERE HfhEvent.name LIKE @Name", hfhName).OrderByDescending(x => x.startDate).ToList();

                st.data = events;
                st.errorCode = ReturnStatus.ALL_CLEAR;
            }
            catch
            {
                st.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
            }
           
            return st;
        }

        #endregion
    }


}
