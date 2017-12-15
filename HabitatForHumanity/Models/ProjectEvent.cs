using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("ProjectEvent")]
    public class ProjectEvent
    {
        [Key]
        public int Id { get; set; }
        public int project_Id { get; set; }
        public int event_Id { get; set; }
    }
}