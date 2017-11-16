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

            PortalVM portalVM = new PortalVM();


            // user part
            ReturnStatus st = Repository.GetUser((int)id);
            if (st.errorCode != ReturnStatus.ALL_CLEAR)
            {
                return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
            }

            User user = (User)st.data;

            portalVM.fullName = user.firstName + " " + user.lastName;
            portalVM.userId = user.Id;
            portalVM.isPunchedIn = Repository.IsUserClockedIn(user.Id);
            portalVM.cumulativeHours = (double)Repository.getTotalHoursWorkedByVolunteer(user.Id).data;

            return View(portalVM);


            // return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });

        }

        
        public ActionResult _PunchOut(int id)
        {
            ReturnStatus rsTimeSheet = Repository.GetClockedInUserTimeSheet(id);


            if (rsTimeSheet.errorCode == ReturnStatus.ALL_CLEAR)
            {
                PunchOutVM punchOutVM = new PunchOutVM((TimeSheet)rsTimeSheet.data);
                return PartialView("_PunchOut", punchOutVM);
            }
            else
            {
                //return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
                //can't redirect from child action
                return PartialView("_PunchOut", new PunchOutVM());
            }
        }


       
        public ActionResult _PunchIn(int id)
        {
            ReturnStatus rsPunch = Repository.GetPunchInVM(id);

            if (rsPunch.errorCode == ReturnStatus.ALL_CLEAR)
            {
                PunchInVM punchInVM = (PunchInVM)rsPunch.data;
                return PartialView("_PunchIn", punchInVM);
            }
            else
            {
                //return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
                //cant redirect from child action
                return PartialView("_PunchIn", new PunchInVM());
            }
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


        //TODO: test 
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

                ReturnStatus st = Repository.GetUserByEmail(signWaiverVM.userEmail);
                if (st.errorCode != (int)ReturnStatus.ALL_CLEAR)
                {
                    return RedirectToAction("HandleErrors", "User", "The system is temporarily down, please try again.");
                }
                User user = (User)st.data;
                if (user.Id > 0)
                {
                    user.AddWaiverToUser(signWaiverVM);
                    ReturnStatus saveResult = Repository.EditUser(user);
                    if (saveResult.errorCode != (int)ReturnStatus.ALL_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", "The system is temporarily down, please try again.");
                    }
                    return RedirectToAction("VolunteerPortal", new { id = user.Id });
                }
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

                ReturnStatus st = Repository.EmailExists(user.emailAddress);

                if ((bool)st.data == false)
                {
                    user.isAdmin = 0;
                    user.waiverSignDate = DateTime.Now.AddYears(-2);
                    ReturnStatus createResult = Repository.CreateVolunteer(user);
                    if (createResult.errorCode != ReturnStatus.ALL_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", "The system is temporarily down, please try again.");
                    }
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
        #endregion

       

        #region Login
        public ActionResult Login(string excMsg)
        {
            LoginVM loginVm = new LoginVM();
            if (excMsg != null)
            {
                ViewBag.status = excMsg;
            }
            return View(loginVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "email,password")] LoginVM loginVm)
        {
            ReturnStatus emailExistsResult = new ReturnStatus();
            ReturnStatus authResult = new ReturnStatus();

            if (ModelState.IsValid)
            {

                emailExistsResult = Repository.EmailExists(loginVm.email);
                if (emailExistsResult.errorCode != ReturnStatus.ALL_CLEAR)
                {

                    return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
                }

                if ((bool)emailExistsResult.data)
                {
                    authResult = Repository.AuthenticateUser(loginVm);
                    if (authResult.errorCode != ReturnStatus.ALL_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
                    }
                    if ((bool)authResult.data)
                    {
                        User user = (User)Repository.GetUserByEmail(loginVm.email).data;

                        Session["UserName"] = user.emailAddress;

                        if (user.isAdmin == 1)
                        {
                            Session["isAdmin"] = "isAdmin";
                            return RedirectToAction("Dashboard","Admin");
                        }
                       

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
           
            return RedirectToAction("Login", "User", new { excMsg = "The system is temporarily down, please try again." });
        }


        // model was bad

        #endregion

        #region Logout
        public ActionResult Logout(string excMsg)
        {
            Session["UserName"] = null;
            Session["isAdmin"] = null;
            return RedirectToAction("Login", new { excMsg = excMsg });
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
                try
                {
                    ReturnStatus existsResult = new ReturnStatus();
                    existsResult = Repository.EmailExists(forgot.email);
                    if (existsResult.errorCode != (int)ReturnStatus.ALL_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
                    }

                    if ((bool)existsResult.data)
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
                catch
                {
                    ViewBag.status = "System failed to process your request, try again.";
                    return RedirectToAction("Login", "Volunteer");
                }
            }// end modelstate.isvalid
            ViewBag.status = "Please provide a valid email address.";
            return RedirectToAction("Login", "Volunteer");
        }
        #endregion

        public ActionResult HandleErrors(string excMsg)
        {
            return RedirectToAction("Login", "User", new { excMsg = excMsg });
        }

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

        #region Search
        public ActionResult VolunteerSearch()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VolunteerSearch([Bind(Include = "firstName,lastName")] User user)
        {
            //TODO: add error checking
            return View("VolunteerSearchResults", (List<User>)Repository.GetUsersByName(user.firstName, user.lastName).data);
        }

        /// <summary>
        /// This gives all time sheet details for selected user in VolunteerSearchResukts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UserTimeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchTimeDetailVM userTimeDetails = new SearchTimeDetailVM();
            //TODO: add error checking
            User user = (User)Repository.GetUser(id.Value).data;
            userTimeDetails.userId = user.Id;
            userTimeDetails.firstName = user.firstName;
            userTimeDetails.lastName = user.lastName;
            userTimeDetails.emailAddress = user.emailAddress;
            //TODO: add error checking
            userTimeDetails.timeSheets = (List<TimeSheet>)Repository.GetAllTimeSheetsByVolunteer(id.Value).data;
            if (userTimeDetails == null)
            {
                return HttpNotFound();
            }
            return View(userTimeDetails);
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
