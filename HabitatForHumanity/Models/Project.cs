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
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime beginDate { get; set; }
        public char status { get; set; } // i - inactive, a - active
    }
}