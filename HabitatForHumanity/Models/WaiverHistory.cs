using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HabitatForHumanity.ViewModels;
using HabitatForHumanity.Models;


namespace HabitatForHumanity.Models
{
    [Table("WaiverHistory")]
    public class WaiverHistory
    {
        [Key]
        public int Id { get; set; }
        public int user_Id { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String homePhoneNumber { get; set; }
        public String workPhoneNumber { get; set; }
        public String emailAddress { get; set; }
        public String streetAddress { get; set; }
        public String city { get; set; }
        public String zip { get; set; }
        public DateTime birthDate { get; set; }
        public String gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:M/d/yy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime waiverSignDate { get; set; }
        public String emergencyFirstName { get; set; }
        public String emergencyLastName { get; set; }
        public String relation { get; set; }
        public String emergencyHomePhone { get; set; }
        public String emergencyWorkPhone { get; set; }
        public String emergencyStreetAddress { get; set; }
        public String emergencyCity { get; set; }
        public String emergencyZip { get; set; }
        public String signatureName { get; set; }

        /// <summary>
        /// Writes newly created waiver to WaiverHistory table 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static ReturnStatus saveWaiverSnapshot(WaiverHistory snapshot)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.waiverHistory.Add(snapshot);
                db.SaveChanges();
                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch(Exception e)
            {
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public static List<WaiverHistory> getWaiverHistoryByUserId(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            List<WaiverHistory> waiverHistory = db.waiverHistory.Where(wh => wh.user_Id == id).ToList();
            return waiverHistory;
        }

        public static ReturnStatus GetWaiverByID(int id)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var waiver = db.waiverHistory.Find(id);
                rs.errorCode = ReturnStatus.ALL_CLEAR;
                rs.data = waiver;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
        }
    }
}