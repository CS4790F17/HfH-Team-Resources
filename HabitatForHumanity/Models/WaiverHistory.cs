using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HabitatForHumanity.ViewModels;


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
        public DateTime waiverSignDate { get; set; }
        public String emergencyFirstName { get; set; }
        public String emergencyLastName { get; set; }
        public String relation { get; set; }
        public String emergencyHomePhone { get; set; }
        public String emergencyWorkPhone { get; set; }
        public String emergencyStreetAddress { get; set; }
        public String emergencyCity { get; set; }
        public String emergencyZip { get; set; }
        public String signature { get; set; }
    }
}