using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using HabitatForHumanity.ViewModels;
using System.Data.Entity;

namespace HabitatForHumanity.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }

        public static bool AuthenticateUser(LoginVM loginVm)
        {
            throw new NotImplementedException();
        }



        public string password { get; set; }
        public DateTime birthDate { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string homePhone { get; set; }
        public string workPhone { get; set; }

        public static bool EmailExists(string email)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.users.Any(u => u.email.Equals(email));
        }
        public static User GetUserByEmail(string email)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var users = db.users.Where(u => u.email.Equals(email));
            return users.FirstOrDefault();
        }

        public static User GetUser(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.users.Find(id);
        }

        public static void CreateUser(User user)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.users.Add(user);
            db.SaveChanges();
        }

        public static void EditUser(User user)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}