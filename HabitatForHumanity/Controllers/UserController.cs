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
using System.Net.Mail;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using System.Drawing;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;
using System.Web.UI;

using System.Globalization;




using Point = DotNet.Highcharts.Options.Point;

namespace HabitatForHumanity.Controllers
{
    public class UserController : Controller
    {
        private VolunteerDbContext db = new VolunteerDbContext();
        private const string awwSnapMsg = "Sorry, We're experiencing technical difficulties, please try again later";

        #region Index
        //[AdminFilter]
        //[AuthorizationFilter]
        //public ActionResult Index()
        //{
        //    return View(db.users.ToList());
        //}
        #endregion

        #region Details
        //[AdminFilter]
        //[AuthorizationFilter]
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}
        #endregion

        #region VolunteerPortal
        [AuthorizationFilter]
        public ActionResult VolunteerPortal(int? justPunched, string excMsg)
        {
            ViewBag.status = (!string.IsNullOrEmpty(excMsg)) ? excMsg : null;

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
            ViewBag.justPunched = justPunched;
            return View((PortalVM)rs.data);
        }

        public ActionResult ThankYouModal()
        {
            return View();
        }

        [AuthorizationFilter]
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

        [AuthorizationFilter]
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

        [AuthorizationFilter]
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

        [AuthorizationFilter]
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

        #endregion

        #region Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // GET: User/VolunteerSignup
        public ActionResult VolunteerSignup()
        {
            return View();
        }
        // POST: User/VolunteerSignup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VolunteerSignup([Bind(
            Include = "firstName,gender,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate, confirmPassword")] VolunteerSignupVM volunteerSignupVM)
        {
            if (ModelState.IsValid)
            {

                ReturnStatus st = Repository.EmailExists(volunteerSignupVM.emailAddress);
                if (st.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is currently down. Please try signing up later.";
                    return View(volunteerSignupVM);
                }

                if ((bool)st.data == false)
                {
                    User user = new User();
                    user.birthDate = volunteerSignupVM.birthDate;
                    user.city = volunteerSignupVM.city;
                    user.emailAddress = volunteerSignupVM.emailAddress;
                    user.firstName = volunteerSignupVM.firstName;
                    user.gender = volunteerSignupVM.gender;
                    user.homePhoneNumber = volunteerSignupVM.homePhoneNumber;
                    user.lastName = volunteerSignupVM.lastName;
                    user.password = volunteerSignupVM.password;
                    user.streetAddress = volunteerSignupVM.streetAddress;
                    if (volunteerSignupVM.workPhoneNumber != null) //not sure this is the best solution
                    {
                        user.workPhoneNumber = volunteerSignupVM.workPhoneNumber;
                    }
                    else
                    {
                        user.workPhoneNumber = volunteerSignupVM.homePhoneNumber;
                    }
                    user.zip = volunteerSignupVM.zip;
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

                    ////gmail smtp server  
                    //WebMail.SmtpServer = "smtp.gmail.com";
                    ////gmail port to send emails  
                    //WebMail.SmtpPort = 587;
                    //WebMail.SmtpUseDefaultCredentials = true;
                    ////sending emails with secure protocol  
                    //WebMail.EnableSsl = true;
                    ////EmailId used to send emails from application  
                    //WebMail.UserName = "hfhdwvolunteer@gmail.com";
                    //WebMail.Password = "3BlindMice";
                    ////Sender email address.  
                    //WebMail.From = "hfhdwvolunteer@gmail.com";
                    ////Send email  
                    //string body = "New user created at email: " + user.emailAddress;
                    //try
                    //{
                    //    WebMail.Send(to: "trevororgill@weber.edu", subject: "New Volunteer", body: body, isBodyHtml: false);
                    //}
                    //catch
                    //{
                    //    //not sure what to do here
                    //    //ViewBag.status = "Email not sent.";
                    //}
                    return RedirectToAction("SignWaiver", "User");
                }
                else
                {
                    return RedirectToAction("Login", new { excMsg = "That email already exists in our system. Please login below." });
                }
            }
            return View(volunteerSignupVM);
        }

        // GET: User/SignWaiver
        [AuthorizationFilter]
        public ActionResult SignWaiver()
        {
            if (Session["UserName"] != null)
            {
                SignWaiverVM signWaiver = new SignWaiverVM();
                signWaiver.signature = false;
                signWaiver.signatureName = null;
                signWaiver.emergencyCity = null;
                signWaiver.emergencyFirstName = null;
                signWaiver.emergencyHomePhone = null;
                signWaiver.emergencyLastName = null;
                signWaiver.emergencyStreetAddress = null;
                signWaiver.emergencyWorkPhone = null;
                signWaiver.emergencyZip = null;
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
        [AuthorizationFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignWaiver([Bind(
            Include = "userEmail, emergencyFirstName, emergencyLastName, relation, emergencyHomePhone, emergencyWorkPhone, emergencyStreetAddress, emergencyCity, emergencyZip, signature, signatureName")] SignWaiverVM signWaiverVM)
        {
            if (ModelState.IsValid)
            {

                ReturnStatus rs = Repository.GetUserByEmail(signWaiverVM.userEmail);
                if (rs.errorCode != 0)
                {
                    ViewBag.status = "Sorry, our system is down. Please try again later.";
                    return View(signWaiverVM);
                }
                //Saves the Emergency Contact info to the user profile
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
                 }

                //Saves a snapshot of the waiver infomation at time of signing.
                rs = Repository.GetUserByEmail(signWaiverVM.userEmail);//rs = newly saved user information 
                user = (User)rs.data;//user is assinged to the newly saved data
                if (rs.errorCode != 0)
                {
                    ViewBag.status = "Sorry, our system is down. Please try again later.";
                    return View(signWaiverVM);
                }
                if (user.Id > 0)
                {
                    Repository.saveWaiverSnapshot(user, signWaiverVM.signatureName);
                    using (StringWriter sw = new StringWriter())
                    {
                        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<h2>Waiver</h2><hr/>");
                            sb.Append("<h5>The Volunteer desires to work as a related to being a volunteer. The Volunteer understands volunteer for Habitat and engage in the activities that the activities may include constructing and rehabilitating residential buildings, working in the Habitat offices, and living in housing provided for volunteers of Habitat.</h5>");
                            sb.Append("<h5>The Volunteer does hereby freely, voluntarily, and without duress executes this Release under the following terms:</h5>");
                            sb.Append("<br/><h6>1. Waiver and Release. Volunteer does hereby release and forever discharge and hold harmless Habitat and its successors and assigns from any and all liability, claims and demands of whatever kind or nature, either in law or in equity, which arise or may hereafter arise from Volunteer's work for Habitat.</h6>");
                            sb.Append("<br/><h5>Volunteer understands and acknowledges that this Release discharges Habitat from any liability or claim that the Volunteer may have against Habitat with respect to any bodily injury, personal injury, illness, death, or property damage that may result from Volunteer's work for Habitat, whether caused by the negligence of Habitat or its officers, directors, employees, or agents or otherwise. Volunteer also understands that, except as otherwise agreed to by Habitat in writing, Habitat does not assume any responsibility for or obligation to provide financial assistance or other assistance, including but not limited to medical, health, or disability insurance, in the event of injury or illness.</h5><br/>");
                            sb.Append("<h6>2. Medical Treatment. Except as otherwise agreed to by Habitat in writing, Volunteer does hereby release and forever discharge Habitat from any claim whatsoever which arises or may hereafter arise on account of any first aid, treatment, or service rendered in connection with the Volunteer's work for Habitat.</h6>");
                            sb.Append("<h6>3. Assumption of the Risk. The Volunteer understands that the work for Habitat may include activities that may be hazardous to the Volunteer, including, but not limited to, construction, loading and unloading, and transportation to and from the work sites. In connection thereto, Volunteer recognizes and understands that activities at Habitat may, in some situations, involve inherently dangerous activities.</h6>");
                            sb.Append("<br/><h5>Volunteer hereby expressly and specifically assumes the risk of injury or harm in these activities and releases Habitat from all liability for injury, illness, death, or property damage resulting from the activities of the Volunteer's work for Habitat.</h5><br/>");
                            sb.Append("<h6>4. Insurance. The Volunteer understands that, except as otherwise agreed to by Habitat in writing, Habitat does not carry or maintain health, medical, or disability insurance coverage for any Volunteer.</h6>");
                            sb.Append("<br/><h5>Each Volunteer is expected and encouraged to obtain his or her own medical or health insurance coverage.</h5>");
                            sb.Append("<br/><h6>5. Photographic Release. Volunteer does hereby grant and convey unto Habitat all right, title, and interest in any and all photographic images and video or audio recordings made by Habitat during the Volunteer's work for Habitat, including, but not limited to, any royalties, proceeds, or other benefits derived from such photographs or recordings.</h6>");
                            sb.Append("<h6>6. Other. Volunteer expressly agrees that this Release is intended to be as broad and inclusive as permitted by the laws of the State of Utah, and that this Release shall be governed by and interpreted in accordance with the laws of the State of Utah. Volunteer agrees that in the event that any clause or provision of this Release shall be held to be invalid by any court of competent jurisdiction, the invalidity of such clause or provision shall not otherwise affect the remaining provisions of this Release which shall continue to be enforceable.</h6><br/>");
                            sb.Append("<hr/><h6 style='page-break-after: always'>Signature: " + signWaiverVM.signatureName + "</h6>");
                            sb.Append("<h2>User Information When Signed</h2><hr/>");
                            sb.Append("<div><b>First Name: </b>" + user.firstName + "</div>");
                            sb.Append("<div><b>Last Name: </b>" + user.lastName + "</div>");
                            sb.Append("<div><b>Home Phone: </b>" + user.homePhoneNumber + "</div>");
                            sb.Append("<div><b>Alternate Phone: </b>" + user.workPhoneNumber + "</div>");
                            sb.Append("<div><b>Email: </b>" + user.emailAddress + "</div>");
                            sb.Append("<div><b>Address: </b>" + user.streetAddress + "</div>");
                            sb.Append("<div><b>City: </b>" + user.city + "</div>");
                            sb.Append("<div><b>Zip: </b>" + user.zip + "</div>");
                            sb.Append("<div><b>Birth Date: </b>" + user.birthDate + "</div>");
                            sb.Append("<div><b>Gender: </b>" + user.gender + "</div>");
                            sb.Append("<div><b>Waiver Sign Date: </b>" + user.waiverSignDate + "</div>");
                            sb.Append("<div><b>Emergency First Name: </b>" + user.emergencyFirstName + "</div>");
                            sb.Append("<div><b>Emergency Last Name: </b>" + user.emergencyLastName + "</div>");
                            sb.Append("<div><b>Relation: </b>" + user.relation + "</div>");
                            sb.Append("<div><b>Emergency Home Phone: </b>" + user.emergencyHomePhone + "</div>");
                            sb.Append("<div><b>Emergency Alternate Phone: </b>" + user.emergencyWorkPhone + "</div>");
                            sb.Append("<div><b>Emergency Address: </b>" + user.emergencyStreetAddress + "</div>");
                            sb.Append("<div><b>Emergency City: </b>" + user.emergencyCity + "</div>");
                            sb.Append("<div><b>Emergency Zip: </b>" + user.emergencyZip + "</div>");
                            StringReader sr = new StringReader(sb.ToString());

                            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                                pdfDoc.Open();
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                                pdfDoc.Close();
                                byte[] bytes = memoryStream.ToArray();
                                memoryStream.Close();

                                MailMessage mm = new MailMessage("hfhdwvolunteer@gmail.com", "trevororgill@weber.edu");
                                mm.Subject = "New Waiver Signed";
                                string body = "New waiver signed for email: " + user.emailAddress + ". A copy of their waiver is attached.";
                                mm.Body = body;
                                mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "Waiver.pdf"));
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = "smtp.gmail.com";
                                smtp.EnableSsl = true;
                                NetworkCredential NetworkCred = new NetworkCredential();
                                NetworkCred.UserName = "hfhdwvolunteer@gmail.com";
                                NetworkCred.Password = "3BlindMice";
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = 587;
                                smtp.Send(mm);
                            }
                        }
                    }
                }
                return RedirectToAction("VolunteerPortal");

            }
            ViewBag.status = "An error has occured below.";
            return View(signWaiverVM);
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

        #region Edit

        [AuthorizationFilter]
        [HttpGet]
        public ActionResult _EditProfile()
        {
            if (Session["UserName"] != null)
            {
                UserProfileVM userProfile = new UserProfileVM();
                ReturnStatus rs = Repository.GetUserByEmail(Session["UserName"].ToString());
                if (rs.errorCode != 0)
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

        [AuthorizationFilter]
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
                if (rs.errorCode != 0)
                {
                    ViewBag.status = "Sorry, the system is temporarily down. Please try again later.";
                    return View(userProfile);
                }
                User user = (User)rs.data;
                if (!string.IsNullOrEmpty(userProfile.newPassword))
                {
                    user.password = Crypto.HashPassword(userProfile.newPassword);
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

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,firstName,lastName,homePhoneNumber,workPhoneNumber,emailAddress,streetAddress,city,zip,password,birthDate,waiverSignDate,emergencyFirstName,emergencyLastName,relation,emergencyHomePhone,emergencyWorkPhone,emergencyStreetAddress,emergencyCity,emergencyZip")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(user).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(user);
        //}
        #endregion

        #region Delete
        [AdminFilter]
        [AuthorizationFilter]
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
        [AdminFilter]
        [AuthorizationFilter]
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

        #region Search, Is this being used?
        public ActionResult VolunteerSearch()
        {
            return View();
        }

        [AdminFilter]
        [AuthorizationFilter]
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
        ///         
        [AdminFilter]
        [AuthorizationFilter]
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

        #region Demographics Survey
        [AuthorizationFilter]
        public ActionResult DemographicsSurvey()
        {
            string email = Session["UserName"].ToString();
            ReturnStatus rs = Repository.GetDemographicsSurveyVM(email);
            DemographicsVM demographicsVM = new DemographicsVM();
            if(rs.errorCode == ReturnStatus.ALL_CLEAR)
            {
                demographicsVM = (DemographicsVM)rs.data;
            }
            return View(demographicsVM);
        }

        [AuthorizationFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DemographicsSurvey(DemographicsVM dvm)
        {
            Repository.SaveDemographicsSurvey(dvm);
            return RedirectToAction("VolunteerPortal", new { excMsg = "Thanks for your input!." });
        }


        #endregion Demographics survey
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
