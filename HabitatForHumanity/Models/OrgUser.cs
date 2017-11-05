//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace HabitatForHumanity.Models
//{
//    [Table("Org_User")]
//    public class OrgUser
//    {
//        [Key]
//        public int Id { get; set; }
//        public int org_Id { get; set; }
//        public int user_Id { get; set; }


//        #region Database Access Methods

//        /// <summary>
//        /// Gets all the OrgUsers in the database.
//        /// </summary>
//        /// <returns>A list of OrgUser</returns>
//        public static List<OrgUser> GetAllOrgUsers()
//        {
//            VolunteerDbContext db = new VolunteerDbContext();
//            return db.orgUser.ToList();
//        }

//        /// <summary>
//        /// Gets all the organization ids for a particular user.
//        /// </summary>
//        /// <param name="userId">The id of the user.</param>
//        /// <returns></returns>
//        public static List<OrgUser> GetOrgUserByUserId(int userId)
//        {
//            VolunteerDbContext db = new VolunteerDbContext();
//            return db.orgUser.Where(x => x.user_Id == userId).ToList();
//        }

//        /// <summary>
//        /// Gets all the users that belong to a particular organization
//        /// </summary>
//        /// <param name="orgId">The id of the organization.</param>
//        /// <returns></returns>
//        public static List<OrgUser> GetOrgUserByOrgId(int orgId)
//        {
//            VolunteerDbContext db = new VolunteerDbContext();
//            return db.orgUser.Where(x => x.org_Id == orgId).ToList();
//        }

//        /// <summary>
//        /// Adds an OrgUser object to the database.
//        /// </summary>
//        /// <param name="orgUser">Object with user_Id and org_Id</param>
//        public static void AddOrgUser(OrgUser orgUser)
//        {
//            VolunteerDbContext db = new VolunteerDbContext();
//            db.orgUser.Add(orgUser);
//            db.SaveChanges();
//        }

//        /// <summary>
//        /// Adds an OrgUser to the database by ids.
//        /// </summary>
//        /// <param name="orgId">Id of the organization</param>
//        /// <param name="userId">Id of the user.</param>
//        public static void AddOrgUserByIds(int orgId, int userId)
//        {
//            VolunteerDbContext db = new VolunteerDbContext();
//            OrgUser ou = new OrgUser();
//            ou.org_Id = orgId;
//            ou.user_Id = userId;
//        }

//        /// <summary>
//        /// Deletes the OrgUser from the database by object.
//        /// </summary>
//        /// <param name="ou"></param>
//        public static void DeleteOrgUser(OrgUser ou)
//        {
//            VolunteerDbContext db = new VolunteerDbContext();
//            db.orgUser.Attach(ou);
//            db.orgUser.Remove(ou);
//            db.SaveChanges();
//        }

//        /// <summary>
//        /// Deletes an OrgUser with the given orgId and userId.
//        /// </summary>
//        /// <param name="orgId">Id of the organization.</param>
//        /// <param name="userId">Id of the user.</param>
//        public static void DeleteOrgUserByIds(int orgId, int userId)
//        {
//            VolunteerDbContext db = new VolunteerDbContext();

//            //throws an error if it finds duplicates
//            OrgUser ou = db.orgUser.Where(x => x.org_Id == orgId && x.user_Id == userId).SingleOrDefault();
//            if(ou != null)
//            {
//                db.orgUser.Remove(ou);
//                db.SaveChanges();
//            }
//        }

//        #endregion
//    }
//}