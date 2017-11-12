using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HabitatForHumanity.Models;
using HabitatForHumanity.ViewModels;
using System.Web.Helpers;

namespace HabitatForHumanity.Controllers
{
    public class UserController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();

        #region Index
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }
        #endregion

        #region Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        #endregion

        #region VolunteerPortal
        public ActionResult VolunteerPortal(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //User user = Repository.GetUser((int)id);
            try
            {
                ReturnStatus st = Repository.GetUser((int)id);
                if (ReturnStatus.tryParseUser(st, out User user))
                {
                    // User user = (User)st.data;

                    if (user != null)
                    {
                        PortalVM portalVM = new PortalVM();
                        portalVM.punchInVM = new PunchInVM();
                        portalVM.punchOutVM = new PunchOutVM();
                        portalVM.fullName = "";
                        portalVM.cumulativeHours = Repository.getTotalHoursWorkedByVolunteer((int)id);
                        portalVM.isPunchedIn = true;

                        TimeSheet temp = Repository.GetClockedInUserTimeSheet((int)id);

                        if (temp == null || temp.Id < 1)
                        {
                            portalVM.isPunchedIn = false;



                            ReturnStatus st2 = Repository.GetPunchInVM((int)id);

                            if (st2.errorCode == 0 && st2.data != null)
                            {
                                //cast st2.data into a punchInVM
                                portalVM.punchInVM = (PunchInVM)st2.data;
                            }



                            portalVM.punchInVM.projects.createDropDownList(Repository.GetAllProjects());
                            portalVM.punchInVM.orgs.createDropDownList(Repository.GetAllOrganizations());
                        }
                        else
                        {
                            portalVM.punchOutVM.timeSheetNumber = temp.Id;
                            portalVM.punchOutVM.userNumber = temp.user_Id;
                            portalVM.punchOutVM.projectNumber = temp.project_Id;
                            portalVM.punchOutVM.orgNumber = temp.org_Id;
                            portalVM.punchOutVM.inTime = temp.clockInTime;
                        }


                        if (user.firstName != null)
                        {
                            portalVM.fullName += user.firstName + " ";
                        }
                        if (user.lastName != null)
                        {
                            portalVM.fullName += user.lastName;
                        }



                        return View(portalVM);
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: redirect/display error message/log
                return RedirectToAction("Login", "User");
            }


            return RedirectToAction("Login", "User");
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(
            Include = "Id,firstName,gender, isAdmin,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate,waiverSignDate,emergencyFirstName,emergencyLastName,relation,emergencyHomePhone,emergencyWorkPhone,emergencyStreetAddress,emergencyCity,emergencyZip")] User user)
        {
            if (ModelState.IsValid)
            {
                //TODO: potentially rework with a bool.TryParse to ensure no type mismatches
                if ((bool)Repository.EmailExists(user.emailAddress).data == false)
                {
                    // int userId = (int)Repository.CreateUser(user).data;
                    if (int.TryParse(Repository.CreateUser(user).data.ToString(), out int userId))
                    {
                        if (userId > 0)
                        {
                            Session["isAdmin"] = null; // if you're admin, you have to have an admin change isAdmin to 1, then log in
                            Session["UserName"] = user.emailAddress;
                            return RedirectToAction("VolunteerPortal", new { id = userId });
                        }
                        else
                        {
                            ViewBag.status = "An error occurred during account creation please try again.";
                            return View(user);
                        }
                    }
                }
                else
                {
                    // this needs some kind of notification
                    ViewBag.status = "That email already exists in out system. Click the link below.";
                    return RedirectToAction("Login", "User");
                }
            }

            return View(user);
        }
        #endregion

        #region Login get
        public ActionResult Login()
        {
            LoginVM loginVm = new LoginVM();
            return View(loginVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "email,password")] LoginVM loginVm)
        {

            ReturnStatus st = new ReturnStatus();

            try
            {
                if (ModelState.IsValid)
                {
                    if ((bool)Repository.EmailExists(loginVm.email).data)
                    {

                        st = Repository.AuthenticateUser(loginVm);


                        if ((bool)Repository.AuthenticateUser(loginVm).data)
                        {
                            // User user = (User)Repository.GetUserByEmail(loginVm.email).data;
                            ReturnStatus.tryParseUser(Repository.GetUserByEmail(loginVm.email), out User user);

                            if (user.isAdmin == 1)
                            {
                                Session["isAdmin"] = "isAdmin";
                            }

                            Session["UserName"] = user.emailAddress;
                            return RedirectToAction("VolunteerPortal", new { id = user.Id });
                        }
                        else
                        {
                            ViewBag.status = "The password provided is not valid.";
                            return View(loginVm);
                        }
                    }
                    else
                    {
                        ViewBag.status = "The email address provided is not in our system.";
                        return View(loginVm);
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: add error logging/handling
            }
            // model was bad
            return RedirectToAction("Login", "Volunteer");
        }

        #endregion

        #region ForgotPassword
        public ActionResult ForgotPassword()
        {
            LoginVM loginVm = new LoginVM();
            return View(loginVm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword([Bind(Include = "email")] LoginVM forgot)
        {
            if (ModelState.IsValid)
            {
                if ((bool)Repository.EmailExists(forgot.email).data)
                {
                    try
                    {
                        //Configuring webMail class to send emails  
                        //gmail smtp server  
                        WebMail.SmtpServer = "smtp.gmail.com";
                        //gmail port to send emails  
                        WebMail.SmtpPort = 587;
                        WebMail.SmtpUseDefaultCredentials = true;
                        //sending emails with secure protocol  
                        WebMail.EnableSsl = true;
                        //EmailId used to send emails from application  
                        WebMail.UserName = "hfhdwvolunteer@gmail.com";
                        WebMail.Password = "3BlindMice";

                        //Sender email address.  
                        WebMail.From = "hfhdwvolunteer@gmail.com";
                        //Reset code
                        Random rand = new Random();
                        string resetCode = rand.Next(1000, 9999).ToString();
                        string newPW = resetCode + "W1!uk";
                        string pwStr = "New TEMPORARY password is: " + newPW;
                        //Send email  
                        WebMail.Send(to: forgot.email, subject: "Password Reset", body: pwStr, isBodyHtml: false);
                        ViewBag.status = "Email Sent Successfully.";
                        ViewBag.em = forgot.email;
                        Repository.ChangePassword(forgot.email, newPW);
                    }
                    catch (Exception)
                    {
                        ViewBag.Status = "Problem while sending email, Please check details.";
                        return View("Login");
                    }
                    return View("Login");
                }
                ViewBag.status = "No record of provided email address.";
                return RedirectToAction("Login", "Volunteer");
            }
            ViewBag.status = "Please provide a valid email address.";
            return RedirectToAction("Login", "Volunteer");
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,firstName,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate,waiverSignDate,emergencyFirstName,emergencyLastName,relation,emergencyHomePhone,emergencyWorkPhone,emergencyStreetAddress,emergencyCity,emergencyZip")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
