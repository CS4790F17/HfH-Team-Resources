using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("Waiver")]
    public class Waiver
    {
        [Key]
        public int Id { get; set; }
        public int userId { get; set; }
        public DateTime signDate { get; set; }
        public string emContFirstName { get; set; }
        public string emContLastName { get; set; }
        public string relation { get; set; }
        public string emContHomePhone { get; set; }
        public string emContWorkPhone { get; set; }
    }
}