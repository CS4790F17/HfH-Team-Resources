using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using HabitatForHumanity.ViewModels;
using System.Data.Entity;
using System.Web.Helpers;
using System.Collections.Generic;

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

        /// <summary>
        /// 0 - volunteer, 1 - admin
        /// </summary>
        public int isAdmin { get; set; }
        public DateTime waiverSignDate { get; set; }
        public string emergencyFirstName { get; set; }
        public string emergencyLastName { get; set; }
        public string relation { get; set; }
        public string emergencyHomePhone { get; set; }
        public string emergencyWorkPhone { get; set; }
        public string emergencyStreetAddress { get; set; }
        public string emergencyCity { get; set; }
        public string emergencyZip { get; set; }



        #region Database Access Methods

        /// <summary>
        /// Checks whether the user entered a bad password for that log in email.
        /// </summary>
        /// <param name="loginVm">The viewmodel containing the users email and password.</param>
        /// <returns>True if user entered a correct password.</returns>
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

        /// <summary>
        /// Gets all the users with matching names. To be used when you know one name, but not the other. 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>List of users</returns>
        public static List<User> GetUsersByName(string firstName, string lastName)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.users.Where(x => x.firstName.Equals(firstName) || x.lastName.Equals(lastName)).ToList();
        }

        /// <summary>
        /// Get a single user out of the database with a matching first and last name.
        /// Only to be used when you know the exact names
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Id of the returned user</returns>
        public static int GetUserByName(string firstName, string lastName)
        {
            VolunteerDbContext db = new VolunteerDbContext();

            var userCount = db.users.Count(x => x.firstName.Equals(firstName) && x.lastName.Equals(lastName));

            //if no users are found or if multiple users are found
            if (userCount != 1)
            {
                return 0;
            }

            var user = db.users.Where(x => x.firstName.Equals(firstName) && x.lastName.Equals(lastName)).Single();

            return user.Id;
        }

        /// <summary>
        /// Finds email if it exists in the database.
        /// </summary>
        /// <param name="email">Email to search for.</param>
        /// <returns>True if email exists</returns>
        public static bool EmailExists(string email)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.users.Any(u => u.emailAddress.Equals(email));
        }

        /// <summary>
        /// Gets the user in the database with the matching email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User with matching email address.</returns>
        public static User GetUserByEmail(string email)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            var users = db.users.Where(u => u.emailAddress.Equals(email));
            return users.FirstOrDefault();
        }

        public static User GetUser(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.users.Find(id);
        }

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>The id of the user or 0 if no user could be added.</returns>
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

        /// <summary>
        /// Updates the users information based on a new model.
        /// </summary>
        /// <param name="user">User object with new information.</param>
        public static void EditUser(User user)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the user from the database.
        /// </summary>
        /// <param name="user">The user object to be deleted.</param>
        public static void DeleteUser(User user)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.users.Attach(user);
            db.users.Remove(user);
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the user in the database with matching id.
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteUserById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            User user = db.users.Find(id);
            if (user != null)
            {
                db.users.Remove(user);
                db.SaveChanges();
            }
        }
        #endregion
    }
}