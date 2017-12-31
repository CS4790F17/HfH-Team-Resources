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
        [Display(Name = "First Name")]
        public String firstName { get; set; }
        [Display(Name = "Last Name")]
        public String lastName { get; set; }
        [Display(Name = "Phone Number")]
        public String homePhoneNumber { get; set; }
        [Display(Name = "Alternate Phone")]
        public String workPhoneNumber { get; set; }
        [Display(Name = "Email")]
        public String emailAddress { get; set; }
        [Display(Name = "Address")]
        public String streetAddress { get; set; }
        [Display(Name = "City")]
        public String city { get; set; }
        [Display(Name = "Zip")]
        public String zip { get; set; }
        [Display(Name = "Birthdate")]
        public DateTime birthDate { get; set; }
        [Display(Name = "Gender")]
        public String gender { get; set; }
        [Display(Name = "Waiver Sign Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime waiverSignDate { get; set; }
        [Display(Name = "Emergency First Name")]
        public String emergencyFirstName { get; set; }
        [Display(Name = "Emergency Last Name")]
        public String emergencyLastName { get; set; }
        [Display(Name = "Relation")]
        public String relation { get; set; }
        [Display(Name = "Emergency Phone Number")]
        public String emergencyHomePhone { get; set; }
        [Display(Name = "Emergency Alternate Phone")]
        public String emergencyWorkPhone { get; set; }
        [Display(Name = "Emergency Address")]
        public String emergencyStreetAddress { get; set; }
        [Display(Name = "Emergency City")]
        public String emergencyCity { get; set; }
        [Display(Name = "Emergency Zip")]
        public String emergencyZip { get; set; }
        [Display(Name = "Signature")]
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

        public static ReturnStatus GetWaiverHistoryByUserId(int id)
        {
            ReturnStatus rs = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                List<WaiverHistory> waiverHistory = db.waiverHistory.Where(wh => wh.user_Id == id).ToList();
                rs.errorCode = ReturnStatus.ALL_CLEAR;
                rs.data = waiverHistory;
            }
            catch
            {
                rs.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
            }
            return rs;
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