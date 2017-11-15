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

            try
            {
                // user part
                ReturnStatus st = Repository.GetUser((int)id);
                if(st.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                {
                    return RedirectToAction("HandleErrors", "User", new { excMsg = (string)st.userErrorMsg });
                }
                User user = (User)st.data;
                if (user.firstName != null)
                {
                    portalVM.fullName += user.firstName + " ";
                }
                if (user.lastName != null)
                {
                    portalVM.fullName += user.lastName;
                }

                // punch in vm
                ReturnStatus pivm = Repository.GetPunchInVM((int)id);
                if (pivm.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                {
                    return RedirectToAction("HandleErrors", "User", new { excMsg = (string)pivm.userErrorMsg });
                }

                PunchInVM punchInVM = (PunchInVM)pivm.data;

                // punch out stuff
                PunchOutVM punchOutVM = new PunchOutVM();

                ReturnStatus timeSheetResult = new ReturnStatus();
                timeSheetResult.data = new TimeSheet();
                timeSheetResult.data = Repository.GetClockedInUserTimeSheet((int)id);
                if (timeSheetResult.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                {
                    return RedirectToAction("HandleErrors", "User", new { excMsg = (string)timeSheetResult.userErrorMsg });
                }
                TimeSheet temp = (TimeSheet)timeSheetResult.data;
                if (temp.Id < 1)
                {
                    portalVM.isPunchedIn = false;
                }
                else
                {
                    portalVM.punchOutVM.timeSheetNumber = temp.Id;
                    portalVM.punchOutVM.userNumber = temp.user_Id;
                    portalVM.punchOutVM.projectNumber = temp.project_Id;
                    portalVM.punchOutVM.orgNumber = temp.org_Id;
                    portalVM.punchOutVM.inTime = temp.clockInTime;
                }

                return View(portalVM);
                
            }
            catch (Exception e)
            {
                //TODO: redirect/display error message/log
                //return RedirectToAction("Login", "User");
                return RedirectToAction("HandleErrors", "User", new { excMsg = "The system is temporarily down, please try again." });
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
                if(st.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                {
                    return RedirectToAction("HandleErrors", "User", new { excMsg = st.userErrorMsg });
                }
                User user = (User)st.data;
                if (user.Id > 0)
                {
                    user.emergencyCity = signWaiverVM.emergencyCity;
                    user.emergencyFirstName = signWaiverVM.emergencyFirstName;
                    user.emergencyHomePhone = signWaiverVM.emergencyHomePhone;
                    user.emergencyLastName = signWaiverVM.emergencyLastName;
                    user.emergencyStreetAddress = signWaiverVM.emergencyStreetAddress;
                    user.emergencyWorkPhone = signWaiverVM.emergencyWorkPhone;
                    user.emergencyZip = signWaiverVM.emergencyZip;
                    user.relation = signWaiverVM.relation;
                    user.waiverSignDate = DateTime.Now;
                    ReturnStatus saveResult = Repository.EditUser(user);
                    if (saveResult.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = saveResult.userErrorMsg });
                    }
                    //db.Entry(user).State = EntityState.Modified;
                    //db.SaveChanges();
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
                    if (createResult.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = createResult.userErrorMsg });
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

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(
        //    Include = "Id,firstName,gender, isAdmin,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate,waiverSignDate,emergencyFirstName,emergencyLastName,relation,emergencyHomePhone,emergencyWorkPhone,emergencyStreetAddress,emergencyCity,emergencyZip")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //TODO: potentially rework with a bool.TryParse to ensure no type mismatches
        //        if ((bool)Repository.EmailExists(user.emailAddress).data == false)
        //        {
        //            // int userId = (int)Repository.CreateUser(user).data;
        //            if (int.TryParse(Repository.CreateUser(user).data.ToString(), out int userId))
        //            {
        //                if (userId > 0)
        //                {
        //                    Session["isAdmin"] = null; // if you're admin, you have to have an admin change isAdmin to 1, then log in
        //                    Session["UserName"] = user.emailAddress;
        //                    return RedirectToAction("VolunteerPortal", new { id = userId });
        //                }
        //                else
        //                {
        //                    ViewBag.status = "An error occurred during account creation please try again.";
        //                    return View(user);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // this needs some kind of notification
        //            ViewBag.status = "That email already exists in out system. Click the link below.";
        //            return RedirectToAction("Login", "User");
        //        }
        //    }

        //    return View(user);
        //}
        #endregion

        #region Login
        //public ActionResult Login()
        //{
        //    LoginVM loginVm = new LoginVM();
        //    return View(loginVm);
        //}
        public ActionResult Login(string excMsg)
        {
            LoginVM loginVm = new LoginVM();
            ViewBag.status = excMsg;
            return View(loginVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "email,password")] LoginVM loginVm)
        {

            ReturnStatus emailExistsResult = new ReturnStatus();
            ReturnStatus authResult = new ReturnStatus();
            try
            {
                if (ModelState.IsValid)
                {
                    emailExistsResult = Repository.EmailExists(loginVm.email);
                    if (emailExistsResult.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = (string)emailExistsResult.userErrorMsg });
                    }
                    
                    if ((bool)emailExistsResult.data)
                    {
                        authResult = Repository.AuthenticateUser(loginVm);
                        if (authResult.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                        {
                            return RedirectToAction("HandleErrors", "User", new { excMsg = (string)authResult.userErrorMsg });
                        }

                        if ((bool)authResult.data)
                        {
                            User user = (User)Repository.GetUserByEmail(loginVm.email).data;
                            ////TODO: add to if
                            //ReturnStatus.tryParseUser(Repository.GetUserByEmail(loginVm.email), out User user);
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
                    if (existsResult.errorCode != (int)ReturnStatus.ErrorCodes.All_CLEAR)
                    {
                        return RedirectToAction("HandleErrors", "User", new { excMsg = (string)existsResult.userErrorMsg });
                    }

                    //TODO: replace with bool.Tryparse to ensure no type mismatch
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
