using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using HabitatForHumanity.ViewModels;
using System.Data.Entity;
using System.Web.Helpers;

namespace HabitatForHumanity.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string homePhoneNumber { get; set; }
        public string workPhoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string password { get; set; }
        public DateTime birthDate { get; set; }
        public string gender { get; set; }
        public int isAdmin { get; set; } // 0 - volunteer, 1 - admin
        public DateTime waiverSignDate { get; set; }
        public string emergencyFirstName { get; set; }
        public string emergencyLastName { get; set; }
        public string relation { get; set; }
        public string emergencyHomePhone { get; set; }
        public string emergencyWorkPhone { get; set; }
        public string emergencyStreetAddress { get; set; }
        public string emergencyCity { get; set; }
        public string emergencyZip { get; set; }


        public static bool AuthenticateUser(LoginVM loginVm)
        {
            bool exists = false;
            User user = User.GetUserByEmail(loginVm.email);
            if (user != null && Crypto.VerifyHashedPassword(user.password, loginVm.password))
            {
                exists = true;
            }
            return exists;
        }
        public static bool EmailExists(string email)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.users.Any(u => u.emailAddress.Equals(email));
        }
        public static User GetUserByEmail(string email)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var users = db.users.Where(u => u.emailAddress.Equals(email));
            return users.FirstOrDefault();
        }

        public static int CreateUser(User user)
        {
            int userId = 0;
            VolunteerDbContext db = new VolunteerDbContext();
            db.users.Add(user);
            db.SaveChanges();
            var users = db.users.Where(u => u.emailAddress.Equals(user.emailAddress));
            User newUser = users.FirstOrDefault();
            if (newUser != null)
            {
                userId = newUser.Id;
            }
            return userId;
        }

        public static void EditUser(User user)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}