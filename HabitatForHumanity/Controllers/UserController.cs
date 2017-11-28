﻿using System;
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
using System.Net.Mail;


namespace HabitatForHumanity.Controllers
{
    public class UserController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();
        private const string awwSnapMsg = "We're experiencing technical difficulties, try again later";

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
        
        public ActionResult VolunteerPortal()
        {

            ReturnStatus us = Repository.GetUserByEmail(Session["UserName"].ToString());
            if (us.errorCode != 0)
            {
                return RedirectToAction("Login", "User", new { excMsg = "Sorry, the system is temporarily down, please try again later." });
            }
            User user = (User)us.data;

            ReturnStatus rs = Repository.GetPortalVM(user.Id);

            if (rs.errorCode != 0)
            {
                return RedirectToAction("Login", "User", new { excMsg = "Sorry, the system is temporarily down, please try again later." });
            }

            ReturnStatus waiverSigned = Repository.waiverNotSigned(((PortalVM)rs.data).userId);

            if ( waiverSigned.errorCode != 0)
            {
                return RedirectToAction("Login", "User", new { excMsg = "Sorry, the system is temporarily down, please try again later." });
            }

            if ((bool)waiverSigned.data)
            {
                ViewBag.status = "Your Waiver is outdated, please sign below.";
                return RedirectToAction("SignWaiver", "User");
            }
            
            return View((PortalVM)rs.data);
        }


        public ActionResult _PunchOut(int id)
        {
            ReturnStatus rs = Repository.GetClockedInUserTimeSheet(id);

            if (rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, system is temporarily down.";
                return PartialView("_ErrorPunchOut");
            }
            PunchOutVM punchOutVM = new PunchOutVM((TimeSheet)rs.data);
            return PartialView("_PunchOut", punchOutVM);
        }

        public ActionResult _PunchIn(int id)
        {
            ReturnStatus rs = Repository.GetPunchInVM(id);

            if (rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, system is temporarily down.";
                return PartialView("_ErrorPunchIn");
            }
            PunchInVM punchInVM = (PunchInVM)rs.data;
            return PartialView("_PunchIn", punchInVM);
        }

        public ActionResult UserProfile()
        {
            if (Session["UserName"] != null)
            {
                IsEditingVM isEdit = new IsEditingVM();
                isEdit.isEditing = false;
                return View(isEdit);
            }
            else
            {
                return RedirectToAction("Login", "User", new { excMsg = "Something went wrong. Please try logging in again." });
            }
        }

        public ActionResult _ViewProfile()
        {
            if (Session["UserName"] != null)
            {
                UserProfileVM userProfile = new UserProfileVM();
                ReturnStatus rs = Repository.GetUserByEmail(Session["UserName"].ToString());
                if(rs.errorCode != 0)
                {
                    return RedirectToAction("Login", "User", new { excMsg = "Sorry, the system is temporarily down, please try again later." });
                }
                User user = (User)rs.data;
                if (user == null)
                {
                    return RedirectToAction("Login", "User");
                }
                userProfile.firstName = user.firstName;
                userProfile.lastName = user.lastName;
                userProfile.homePhone = user.homePhoneNumber;
                userProfile.workPhone = user.workPhoneNumber;
                userProfile.userEmail = Session["UserName"].ToString();
                userProfile.streetAddress = user.streetAddress;
                userProfile.city = user.city;
                userProfile.zip = user.zip;
                userProfile.newPassword = "";
                userProfile.confirmPassword = "";
                return PartialView(userProfile);
            }
            else
            {
                return RedirectToAction("Login", "User", new { excMsg = "Something went wrong. Please try logging in again." });
            }
        }

        [HttpGet]
        public ActionResult _EditProfile()
        {
            if (Session["UserName"] != null)
            {
                UserProfileVM userProfile = new UserProfileVM();
                ReturnStatus rs = Repository.GetUserByEmail(Session["UserName"].ToString());
                if(rs.errorCode != 0)
                {
                    return RedirectToAction("Login", "User", new { excMsg = "Sorry, the system is temporarily down, please try again later." });
                }
                User user = (User)rs.data;
                if (user == null)
                {
                    return RedirectToAction("Login", "User");
                }
                userProfile.firstName = user.firstName;
                userProfile.lastName = user.lastName;
                userProfile.homePhone = user.homePhoneNumber;
                userProfile.workPhone = user.workPhoneNumber;
                userProfile.userEmail = Session["UserName"].ToString();
                userProfile.streetAddress = user.streetAddress;
                userProfile.city = user.city;
                userProfile.zip = user.zip;
                userProfile.newPassword = "";
                userProfile.confirmPassword = "";
                return PartialView(userProfile);
            }
            else
            {
                return RedirectToAction("Login", "User", new { excMsg = "Something went wrong. Please try logging in again." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _EditProfile([Bind(Include = "firstName,lastName,homePhone,workPhone,userEmail,streetAddress,city,zip,newPassword,confirmPassword")] UserProfileVM userProfile)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            if (ModelState.IsValid)
            {
                ReturnStatus rs = Repository.GetUserByEmail(Session["UserName"].ToString());
                if(rs.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is temporarily down. Please try again later.";
                    return View(userProfile);
                }
                User user = (User)rs.data;
                if (userProfile.newPassword != null)
                {
                    user.password = Crypto.HashPassword(user.password);
                }
                user.firstName = userProfile.firstName;
                user.lastName = userProfile.lastName;
                user.homePhoneNumber = userProfile.homePhone;
                user.workPhoneNumber = userProfile.workPhone;
                user.streetAddress = userProfile.streetAddress;
                user.city = userProfile.city;
                user.zip = userProfile.zip;
                ReturnStatus us = new ReturnStatus();
                us = Repository.EditUser(user);
                if (us.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is temporarily down. Please try again later.";
                    return View(userProfile);
                }
                return RedirectToAction("UserProfile", "User");
            }
            ViewBag.status = "An Error Has Occured";
            return View(userProfile);
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

                ReturnStatus rs = Repository.GetUserByEmail(signWaiverVM.userEmail);
                if (rs.errorCode != 0)
                {
                    ViewBag.status = "Sorry, our system is down. Please try again later.";
                    return View(signWaiverVM);
                }
             
                User user = (User)rs.data;
                if (user.Id > 0)
                {
                    user.AddWaiverToUser(signWaiverVM);
                    ReturnStatus saveResult = Repository.EditUser(user);
                    if (saveResult.errorCode != 0)
                    {
                        ViewBag.status = "Sorry, our system is down. Please try again later.";
                        return View(signWaiverVM);
                    }
                    return RedirectToAction("VolunteerPortal");
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
                if(st.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is currently down. Please try signing up later.";
                    return View(user);
                }

                if ((bool)st.data == false)
                {
                    user.isAdmin = 0;
                    user.waiverSignDate = DateTime.Now.AddYears(-2);
                    ReturnStatus createResult = Repository.CreateVolunteer(user);
                    if (createResult.errorCode != 0)
                    {
                        ViewBag.status = "Sorry, the system is currently down. Please try signing up later.";
                        return View(user);
                    }
                    Session["isAdmin"] = user.isAdmin;
                    Session["UserName"] = user.emailAddress;

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
                    //Send email  
                    string body = "New user created at email: " + user.emailAddress;
                    WebMail.Send(to: "trevororgill@weber.edu", subject: "New Volunteer", body: body, isBodyHtml: false);

                    return RedirectToAction("SignWaiver", "User");
                }
                else
                {
                    return RedirectToAction("Login", new {excMsg = "That email already exists in our system. Please login below." });
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
                if (emailExistsResult.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is temporarily down, please try again later.";
                    return View(loginVm);
                    //return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
                }

                if ((bool)emailExistsResult.data)
                {
                    authResult = Repository.AuthenticateUser(loginVm);
                    if (authResult.errorCode != ReturnStatus.ALL_CLEAR)
                    {
                        ViewBag.status = awwSnapMsg;
                        return View(loginVm);
                    }
                    if ((bool)authResult.data)
                    {
                        ReturnStatus rsUser = Repository.GetUserByEmail(loginVm.email);
                        
                        if (rsUser.errorCode != 0)
                        {
                            ViewBag.status = awwSnapMsg;
                            return View(loginVm);
                        }

                        User user = (User)rsUser.data;
                        Session["UserName"] = user.emailAddress;

                        if (user.isAdmin == 1)
                        {
                            Session["isAdmin"] = "isAdmin";
                            return RedirectToAction("Dashboard","Admin");
                        }
                                               
                        return RedirectToAction("VolunteerPortal");
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
            ViewBag.status = awwSnapMsg;
            return View(loginVm);          
        }


        

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
            loginVm.password = "blah";
            return View(loginVm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword([Bind(Include = "email, password")] LoginVM forgot)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ReturnStatus existsResult = new ReturnStatus();
                    existsResult = Repository.EmailExists(forgot.email);
                    if (existsResult.errorCode != ReturnStatus.ALL_CLEAR)
                    {
                        ViewBag.status = "Sorry, the system is temporarily down, please try again later.";
                        return View("Login");                      
                    }

                    if ((bool)existsResult.data == true)
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
                            return View(forgot);
                        }
                        return RedirectToAction("Login", new { excMsg = "New password has been sent to " + forgot.email });
                    }
                    else
                    {
                        ViewBag.status = "No record of provided email address.";
                        return View(forgot);
                    }
           
                }
                catch
                {
                    ViewBag.status = "System failed to process your request, try again.";
                    return View(forgot);
                }
            }// end modelstate.isvalid
            ViewBag.status = "Please provide a valid email address.";
            return View(forgot);
        }
        #endregion

        //public ActionResult HandleErrors(string excMsg)
        //{
        //    return RedirectToAction("Login", "User", new { excMsg = excMsg });
        //}

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
            ReturnStatus rs = Repository.GetUsersByName(user.firstName, user.lastName);
            if(rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, system is temporarily down. Please try again later";
                return View(user);
            }
            List<User> users = (List<User>)rs.data;
            return View("VolunteerSearchResults", users);
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
            ReturnStatus rs = Repository.GetUser(id.Value);
            if(rs.errorCode != 0)
            {
                ViewBag.status = "Sorry, system is temporarily down. Please try again later";
                return View();
            }
            User user = (User)rs.data;
            userTimeDetails.userId = user.Id;
            userTimeDetails.firstName = user.firstName;
            userTimeDetails.lastName = user.lastName;
            userTimeDetails.emailAddress = user.emailAddress;
            //TODO: add error checking
            ReturnStatus st = Repository.GetAllTimeSheetsByVolunteer(id.Value);
            if(st.errorCode != 0)
            {
                ViewBag.status = "Sorry, system is temporarily down. Please try again later";
                return View();
            }
            userTimeDetails.timeSheets = (List<TimeSheet>)st.data;
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
