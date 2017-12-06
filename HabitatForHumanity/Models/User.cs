using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using HabitatForHumanity.ViewModels;
using System.Data.Entity;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace HabitatForHumanity.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        [Display(Name = "First Name*")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        [Display(Name = "Last Name*")]
        public string lastName { get; set; }
        [Phone]
        [Required]//(ErrorMessage = "Enter Home Phone")]
        [Display(Name = "Primary Phone*")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string homePhoneNumber { get; set; }
        [Display(Name = "Alternate Phone")]
        [RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string workPhoneNumber { get; set; }

        //[Required, StringLength(40), EmailAddress, DisplayName("Email Address")]
        [Required(ErrorMessage = "Enter Email")]
        [Display(Name = "Email*")]
        [RegularExpression(@"^(([^<>()\[\]\\.,;:\s@']+(\.[^<>()\[\]\\.,;:\s@']+)*)|('.+'))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please Enter a Valid Email Address")]
        public string emailAddress { get; set; }

        [Required(ErrorMessage = "Enter Address")]
        [Display(Name = "Address*")]
        public string streetAddress { get; set; }

        [Required(ErrorMessage = "Enter City")]
        [Display(Name = "City*")]
        public string city { get; set; }

        [Required(ErrorMessage = "Enter Zipcode")]
        [Display(Name = "Zipcode*")]
        [RegularExpression(@"^(^\d{5}$)|(^\d{5}-\d{4}$)$", ErrorMessage = "Please Enter a Valid Zip")]
        public string zip { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [Display(Name = "Password*")]
        public string password { get; set; }

        [Required(ErrorMessage = "Enter Birthdate")]
        [Display(Name = "Birthdate*")]
        [DataType(DataType.Date)]
        public DateTime birthDate { get; set; }

        [Display(Name = "Gender")]
        public string gender { get; set; }

        [Required(ErrorMessage = "Is Admin (0 - Volunteer, 1 - Admin)")]
        [Display(Name = "Admin*")]
        public int isAdmin { get; set; }    // 0 - volunteer, 1 - admin

        [Required(ErrorMessage = "Enter Waiver Sign Date")]
        [Display(Name = "Waiver Sign Date*")]
        public DateTime waiverSignDate { get; set; }

        //[Required(ErrorMessage = "Enter Emergency First Name")]
        [Display(Name = "Contact First Name*")]
        public string emergencyFirstName { get; set; }

        //[Required(ErrorMessage = "Enter Emergency Last Name")]
        [Display(Name = "Contact Last Name*")]
        public string emergencyLastName { get; set; }

        //[Required(ErrorMessage = "Enter Emergency Relation")]
        [Display(Name = "Relation*")]
        public string relation { get; set; }

        //[Required(ErrorMessage = "Enter Emergency Home Phone")]
        [Display(Name = "Contact Primary Phone*")]
        //[RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string emergencyHomePhone { get; set; }

        //[Required(ErrorMessage = "Enter Emergency Work Phone")]
        [Display(Name = "Contact Alternate Phone")]
        //[RegularExpression(@"^\(?(\d{3})\)?[- .]?(\d{3})[- .]?(\d{4})$", ErrorMessage = "Please Enter a Valid Phone Number")]
        public string emergencyWorkPhone { get; set; }

        //[Required(ErrorMessage = "Enter Emergency Address")]
        [Display(Name = "Contact Address*")]
        public string emergencyStreetAddress { get; set; }

        //[Required(ErrorMessage = "Enter Emergency City")]
        [Display(Name = "Contact City*")]
        public string emergencyCity { get; set; }

        //[Required(ErrorMessage = "Enter Emergency Zipcode")]
        [Display(Name = "Contact Zipcode*")]
        public string emergencyZip { get; set; }

        // stuff for demographics forms, see Demographics vm for categories
        public string incomeTier { get; set; }
        public string ethnicity { get; set; }
        public string collegeStatus { get; set; }
        public string veteranStatus { get; set; }
        public string disabledStatus { get; set; }

        public User()
        {
            Id = -1;
            firstName = "";
            lastName = "";
            homePhoneNumber = "";
            workPhoneNumber = "";
            emailAddress = "";
            streetAddress = "";
            city = "";
            zip = "";
            password = "";
            birthDate = DateTime.Now; //the year 0001 is out of range for a datetime object which is the defaul null value
            gender = "";
            isAdmin = 0;
            waiverSignDate = DateTime.Now.AddYears(-2);
            emergencyFirstName = "";
            emergencyLastName = "";
            emergencyStreetAddress = "";
            emergencyWorkPhone = "";
            relation = "";
            emergencyZip = "";
            emergencyCity = "";
            emergencyHomePhone = "";
    }

        public void AddWaiverToUser(SignWaiverVM waiver)
        {
            emergencyCity = waiver.emergencyCity;
            emergencyFirstName = waiver.emergencyFirstName;
            emergencyHomePhone = waiver.emergencyHomePhone;
            emergencyLastName = waiver.emergencyLastName;
            emergencyStreetAddress = waiver.emergencyStreetAddress;
            emergencyWorkPhone = waiver.emergencyWorkPhone;
            emergencyZip = waiver.emergencyZip;
            relation = waiver.relation;
            waiverSignDate = DateTime.Now;
        }

        #region Database Access Methods

        /// <summary>
        /// Returns whether or not a user's waiver is outdated
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ReturnStatus waiverNotSigned(int userId)
        {
            ReturnStatus rs = new ReturnStatus();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                User user = db.users.Find(userId);
                if ((DateTime.Now.AddYears(-1) - user.waiverSignDate).TotalDays < (DateTime.Now - DateTime.Now.AddYears(-1)).TotalDays)
                {
                    rs.data = false;
                }
                else
                {
                    rs.data = true;
                }
                rs.errorCode = 0;
                return rs;
            }
            catch
            {
                rs.errorCode = -1;
                rs.data = new List<User>();
                return rs;
            }
        }

        /// <summary>
        /// Gets all the users that contain any characters in firstName or lastName.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>ReturnStatus object containing a list of users</returns>
        public static ReturnStatus GetUsersByName(string firstName, string lastName)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.data = db.users.Where(x => x.firstName.Contains(firstName) || x.lastName.Contains(lastName)).ToList();
                st.errorCode = 0;
                return st;
            }
            catch
            {
                st.errorCode = -1;
                st.data = new List<User>();
                return st;
            }
        }

        public static ReturnStatus GetAllUsers()
        {
            ReturnStatus rs = new ReturnStatus();
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.data = db.users.ToList();
                st.errorCode = 0;
                return st;
            }
            catch
            {
                st.errorCode = -1;
                st.data = new List<User>();
                return st;
            }
        }

        /// <summary>
        /// Finds email if it exists in the database.
        /// </summary>
        /// <param name="email">Email to search for.</param>
        /// <returns>True if email exists</returns>
        public static ReturnStatus EmailExists(string email)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.data = db.users.Any(u => u.emailAddress.Equals(email));
                return st;
            }
            catch
            {
                st.errorCode = -1;
                return st;
            }
        }


        /// <summary>
        /// Gets the user in the database with the matching email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User with matching email address.</returns>
        public static ReturnStatus GetUserByEmail(string email)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new User();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var users = db.users.Where(u => u.emailAddress.Equals(email));

                st.errorCode = 0;
                st.data = users.FirstOrDefault();

                return st;
            }
            catch
            {
                st.errorCode = -1;
                return st;
            }
        }


        /// <summary>
        /// Gets the user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ReturnStatus GetUser(int id)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new User();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.data = db.users.Find(id);
                st.errorCode = 0;
                return st;
            }
            catch
            {
                st.errorCode = -1;
                return st;
            }
        }



        public static ReturnStatus CreateVolunteer(User user)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                user.homePhoneNumber = user.homePhoneNumber.Replace('(', ' ').Replace(')', ' ').Replace('.', ' ').Replace('-', ' ');
                user.homePhoneNumber = Regex.Replace(user.homePhoneNumber, @"\s", "");
                user.workPhoneNumber = user.workPhoneNumber.Replace('(', ' ').Replace(')', ' ').Replace('.', ' ').Replace('-', ' ');
                user.workPhoneNumber = Regex.Replace(user.workPhoneNumber, @"\s", "");
                db.users.Add(user);
                db.SaveChanges();
                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                return st;
            }
            catch
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                return st;
            }

        }

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>The id of the user or 0 if no user could be added.</returns>
        public static ReturnStatus CreateUser(User user)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                user.password = Crypto.HashPassword(user.password);
                //user.waiverSignDate = DateTime.Today;

                VolunteerDbContext db = new VolunteerDbContext();
                db.users.Add(user);
                db.SaveChanges();

                //entity framework automagically populates a model with all database generated ids
                //so the passed in user object will have an id
                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                st.data = user.Id;



                //var users = db.users.Where(u => u.emailAddress.Equals(user.emailAddress));
                //User newUser = users.FirstOrDefault();
                //if (newUser != null)
                //{
                //    userId = newUser.Id;
                //}
                //return userId;

                return st;

            }
            catch (ArgumentNullException e)
            {
                st.errorCode = (int)ReturnStatus.NULL_ARGUMENT;
                st.errorMessage = e.ToString();
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
        /// Updates the users information based on a new model.
        /// </summary>
        /// <param name="user">User object with new information.</param>
        public static ReturnStatus EditUser(User user)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                user.homePhoneNumber = user.homePhoneNumber.Replace('(', ' ').Replace(')', ' ').Replace('.', ' ').Replace('-', ' ');
                user.homePhoneNumber = Regex.Replace(user.homePhoneNumber, @"\s", "");
                user.workPhoneNumber = user.workPhoneNumber.Replace('(', ' ').Replace(')', ' ').Replace('.', ' ').Replace('-', ' ');
                user.workPhoneNumber = Regex.Replace(user.workPhoneNumber, @"\s", "");
                user.emergencyHomePhone = user.homePhoneNumber.Replace('(', ' ').Replace(')', ' ').Replace('.', ' ').Replace('-', ' ');
                user.emergencyHomePhone = Regex.Replace(user.homePhoneNumber, @"\s", "");
                user.emergencyWorkPhone = user.workPhoneNumber.Replace('(', ' ').Replace(')', ' ').Replace('.', ' ').Replace('-', ' ');
                user.emergencyWorkPhone = Regex.Replace(user.workPhoneNumber, @"\s", "");
                db.Entry(user).State = EntityState.Modified;
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
        /// Deletes the user from the database.
        /// </summary>
        /// <param name="user">The user object to be deleted.</param>
        public static ReturnStatus DeleteUser(User user)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.users.Attach(user);
                db.users.Remove(user);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                return st;

            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Deletes the user in the database with matching id.
        /// </summary>
        /// <param name="id"></param>
        public static ReturnStatus DeleteUserById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                User user = db.users.Find(id);
                if (user != null)
                {
                    db.users.Remove(user);
                    db.SaveChanges();

                }
                else
                {
                    st.errorCode = (int)ReturnStatus.COULD_NOT_UPDATE_DATABASE;
                }
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public class Demog
        {
            public string ageBracket { get; set; }
            public int numPeople { get; set; }
        }

        /*
          public static List<Demog> GetDemographicsForPie(string gender)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            List<Demog> demogs = new List<Demog>();
            List<User> users = new List<User>();
            if (gender.Equals("All"))
            {
                users = db.users.Where(u => u.birthDate != null).ToList();
            }
            else
            {
                users = db.users.Where(u => u.birthDate != null && u.gender.Equals(gender)).ToList();
            }
            Demog dunder18 = new Demog() { ageBracket = "Under 18", numPeople = 0 };
            Demog d18to27 = new Demog() { ageBracket = "18 to 27", numPeople = 0 };
            Demog d27to40 = new Demog() { ageBracket = "27 to 40", numPeople = 0 };
            Demog d40to55 = new Demog() { ageBracket = "40 to 55", numPeople = 0 };
            Demog dover55 = new Demog() { ageBracket = "Over 55", numPeople = 0 };
            foreach (User u in users)
            {
                DateTime present = DateTime.Now;
                if (present.AddYears(-18) < u.birthDate)
                {
                    dunder18.numPeople++;
                }
                else if (present.AddYears(-18) <= u.birthDate && present.AddYears(-27) > u.birthDate)
                {
                    d18to27.numPeople++;
                }
                else if (present.AddYears(-27) <= u.birthDate && present.AddYears(-40) > u.birthDate)
                {
                    d27to40.numPeople++;
                }
                else if (present.AddYears(-40) <= u.birthDate && present.AddYears(-55) > u.birthDate)
                {
                    d40to55.numPeople++;
                }
                else
                {
                    dover55.numPeople++;
                }
            }

            demogs.Add(dunder18);
            demogs.Add(d18to27);
            demogs.Add(d27to40);
            demogs.Add(d40to55);
            demogs.Add(dover55);
            return demogs;

        }
             */
        public static ReturnStatus GetDemographicsForPie(string gender)
        {
            //TODO:   finish up refactor!!!
            ReturnStatus ret = new ReturnStatus();
            ret.data = new List<Demog>();

            VolunteerDbContext db = new VolunteerDbContext();
            List<Demog> demogs = new List<Demog>();
            List<User> users = new List<User>();
            if (gender.Equals("All"))
            {
                users = db.users.Where(u => u.birthDate != null).ToList();
            }
            else
            {
                users = db.users.Where(u => u.birthDate != null && u.gender.Equals(gender)).ToList();
            }
            Demog dunder18 = new Demog() { ageBracket = "Under 18", numPeople = 0 };
            Demog d18to27 = new Demog() { ageBracket = "18 to 26", numPeople = 0 };
            Demog d27to40 = new Demog() { ageBracket = "27 to 39", numPeople = 0 };
            Demog d40to55 = new Demog() { ageBracket = "40 to 55", numPeople = 0 };
            Demog dover55 = new Demog() { ageBracket = "Over 55", numPeople = 0 };
            foreach (User u in users)
            {
                DateTime present = DateTime.Now;
                if (u.birthDate >= present.AddYears(-18))
                {
                    dunder18.numPeople++;
                }
                else if (u.birthDate > present.AddYears(-27))
                {
                    d18to27.numPeople++;
                }
                else if (u.birthDate > present.AddYears(-40))
                {
                    d27to40.numPeople++;

                }
                else if (u.birthDate > present.AddYears(-55))
                {
                    d40to55.numPeople++;
                }
                else
                {
                    dover55.numPeople++;
                }
            }

            demogs.Add(dunder18);
            demogs.Add(d18to27);
            demogs.Add(d27to40);
            demogs.Add(d40to55);
            demogs.Add(dover55);

            ret.errorCode = (demogs.Count > 0) ? ReturnStatus.ALL_CLEAR : ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
            ret.data = demogs;
            return ret;

        }

        #endregion
    }
}