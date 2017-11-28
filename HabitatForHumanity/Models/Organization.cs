using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HabitatForHumanity.ViewModels;
using System.Data.SqlClient;

namespace HabitatForHumanity.Models
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public int status { get; set; }

        public Organization()
        {
            Id = -1;
            name = "";
        }


        #region Database Access Methods

        /// <summary>
        /// Get all organizations in the database.
        /// </summary>
        /// <returns>A list of all organizations.</returns>
        public static ReturnStatus GetAllOrganizations()
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new List<Organization>();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.organizations.ToList();
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
        /// Get a single organization by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single organization object with a matching id otherwise null.</returns>
        public static ReturnStatus GetOrganizationById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new Organization();
            if (id < 1)
            {
                return new ReturnStatus() { errorCode = -1, data = null };
            }
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.organizations.Find(id);
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
        /// Get a single organization by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A single organization object with a matching name otherwise null.</returns>
        public static ReturnStatus GetOrganizationByName(string name)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = new Organization();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = ReturnStatus.ALL_CLEAR;
                st.data = db.organizations.Where(x => x.name.Equals(name)).Single();
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
        /// Gets all the currently active organizations
        /// </summary>
        /// <returns>A list of all organizatinos that are currently active.</returns>
        public static List<Organization> GetActiveOrganizations()
        {
            VolunteerDbContext db = new VolunteerDbContext();
            return db.organizations.Where(x => x.status == 1).ToList();
        }

        /// <summary>
        /// Adds an organization to the database.
        /// </summary>
        /// <param name="org">The organization to be added</param>
        public static ReturnStatus AddOrganization(Organization org)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.organizations.Add(org);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
                st.data = "Successfully added organization.";
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
        /// Edits the organization with new values.
        /// </summary>
        /// <param name="org">The organization object with new values.</param>
        public static ReturnStatus EditOrganization(Organization org)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(org).State = EntityState.Modified;
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
        /// Deletes an organization from the database.
        /// </summary>
        /// <param name="org">The organization object to delete</param>
        public static ReturnStatus DeleteOrganization(Organization org)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.organizations.Attach(org);
                db.organizations.Remove(org);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ALL_CLEAR;
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
        /// Deletes an organization from the database by id.
        /// </summary>
        /// <param name="id">The id of the organization to delete.</param>
        public static ReturnStatus DeleteOrganizationById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            st.data = null;
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                Organization org = db.organizations.Find(id);
                if (org != null)
                {
                    db.organizations.Remove(org);
                    db.SaveChanges();

                    st.errorCode = ReturnStatus.ALL_CLEAR;
                    return st;
                }
                st.errorCode = ReturnStatus.COULD_NOT_UPDATE_DATABASE;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = ReturnStatus.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public static ReturnStatus GetOrganizationByNameSQL(string name)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            ReturnStatus st = new ReturnStatus();

            var orgName = new SqlParameter("@Name", "%" + name + "%");

            var orgs = db.organizations.SqlQuery("SELECT * FROM Organization WHERE Organization.name LIKE @Name", orgName).OrderByDescending(x => x.status).ToList<Organization>();


            if(orgs.Count < 1)
            {
                List<Organization> orgList = new List<Organization>();
                st.data = orgList;
                st.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                return st;
            }

            st.errorCode = ReturnStatus.ALL_CLEAR;
            st.data = orgs;
            return st;
        }

        public static ReturnStatus GetOrganizationSQL(string queryFilter, int status)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            ReturnStatus st = new ReturnStatus();

            
            var orgStatus = new SqlParameter("@Status", status);
            var orgName = new SqlParameter("@Name", "%" + queryFilter + "%");
            

            var orgs = db.organizations.SqlQuery(
                "SELECT * FROM Organization " +
                "WHERE Organization.status = @Status " +
                "AND Organization.name in " +
                "(SELECT Organization.name FROM Organization WHERE Organization.name LIKE @Name)", orgStatus, orgName).OrderByDescending(x => x.status).ToList<Organization>();


            if(orgs.Count < 1)
            {
                List<Organization> orgList = new List<Organization>();
                st.data = orgList;
                st.errorCode = ReturnStatus.ERROR_WHILE_ACCESSING_DATA;
                return st;
            }

            st.errorCode = ReturnStatus.ALL_CLEAR;
            st.data = orgs;
            return st;
        }
        #endregion
    }
}