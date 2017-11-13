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
            User user = Repository.GetUser((int)id);
            if(user != null)
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
                    portalVM.punchInVM = Repository.GetPunchInVM((int)id);
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
  
    
            return RedirectToAction("Login", "User");
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }
        
        // GET: User/VolunteerSignup
        public ActionResult VolunteerSignup()
        {
            return View();
        }

        // GET: User/SignWaiver
        public ActionResult SignWaiver()
        {
            if (Session["UserName"] != null)
            {
                SignWaiverVM signWaiver = new SignWaiverVM();
                signWaiver.signature = "";
                signWaiver.userEmail = Session["UserName"].ToString();
                return View(signWaiver);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        // POST: User/SignWaiver
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignWaiver([Bind(
            Include = "userEmail, emergencyFirstName, emergencyLastName, relation, emergencyHomePhone, emergencyWorkPhone, emergencyStreetAddress, emergencyCity, emergencyZip, signature")] SignWaiverVM signWaiverVM)
        {
            if (ModelState.IsValid)
            {
                if (signWaiverVM.signature == null)
                {
                    return View(signWaiverVM);
                }
                User user = Repository.GetUserByEmail(signWaiverVM.userEmail);
                user.emergencyCity = signWaiverVM.emergencyCity;
                user.emergencyFirstName = signWaiverVM.emergencyFirstName;
                user.emergencyHomePhone = signWaiverVM.emergencyHomePhone;
                user.emergencyLastName = signWaiverVM.emergencyLastName;
                user.emergencyStreetAddress = signWaiverVM.emergencyStreetAddress;
                user.emergencyWorkPhone = signWaiverVM.emergencyWorkPhone;
                user.emergencyZip = signWaiverVM.emergencyZip;
                user.relation = signWaiverVM.relation;
                user.waiverSignDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("VolunteerPortal", new { id = user.Id });
            }

            return View(signWaiverVM);
        }

        // POST: User/VolunteerSignup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VolunteerSignup([Bind(
            Include = "Id,firstName,gender, isAdmin,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate")] User user)
        {
            if (ModelState.IsValid)
            {
                if (Repository.EmailExists(user.emailAddress) == false)
                {
                    user.isAdmin = 0;
                    user.waiverSignDate = DateTime.Now.AddYears(-2);
                    Repository.CreateVolunteer(user);
                    Session["isAdmin"] = user.isAdmin;
                    Session["UserName"] = user.emailAddress;
                    return RedirectToAction("SignWaiver", "User");
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

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(
            Include = "Id,firstName,gender, isAdmin,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate,waiverSignDate,emergencyFirstName,emergencyLastName,relation,emergencyHomePhone,emergencyWorkPhone,emergencyStreetAddress,emergencyCity,emergencyZip")] User user)
        {
            if (ModelState.IsValid)
            {
                if(Repository.EmailExists(user.emailAddress) == false)
                {
                    int userId = Repository.CreateUser(user);
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
            if (ModelState.IsValid)
            {
                if (Repository.EmailExists(loginVm.email))
                {
                    if (Repository.AuthenticateUser(loginVm))
                    {
                        User user = Repository.GetUserByEmail(loginVm.email);
                        if(user.isAdmin == 1)
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
                if (Repository.EmailExists(forgot.email))
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
