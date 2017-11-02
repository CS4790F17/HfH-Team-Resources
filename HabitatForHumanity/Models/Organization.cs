using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }


        #region Database Access Methods

        /// <summary>
        /// Get all organizations in the database.
        /// </summary>
        /// <returns>A list of all organizations.</returns>
        public static List<Organization> GetAllOrganizations()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.ToList();
        }


        /// <summary>
        /// Get a single organization by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single organization object with a matching id otherwise null.</returns>
        public static Organization GetOrganizationById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.Find(id);
        }

        /// <summary>
        /// Get a single organization by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A single organization object with a matching name otherwise null.</returns>
        public static Organization GetOrganizationByName(string name)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.Where(x => x.name.Equals(name)).Single();
        }

        /// <summary>
        /// Adds an organization to the database.
        /// </summary>
        /// <param name="org">The organization to be added</param>
        public static void AddOrganization(Organization org)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.organizations.Add(org);
            db.SaveChanges();
        }

        /// <summary>
        /// Edits the organization with new values.
        /// </summary>
        /// <param name="org">The organization object with new values.</param>
        public static void EditOrganization(Organization org)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.Entry(org).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes an organization from the database.
        /// </summary>
        /// <param name="org">The organization object to delete</param>
        public static void DeleteOrganization(Organization org)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            db.organizations.Attach(org);
            db.organizations.Remove(org);
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes ad organization from the database.
        /// </summary>
        /// <param name="id">The id of the organization to delete.</param>
        public static void DeleteOrganizationById(int id)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            Organization org = db.organizations.Find(id);
            if(org != null)
            {
                db.organizations.Remove(org);
                db.SaveChanges();
            }
        }
        #endregion
    }
}