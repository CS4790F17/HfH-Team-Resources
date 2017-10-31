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

        // GET: User
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: User/Details/5
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

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,firstName,lastName,email,password,birthDate,gender,streetAddress,city,zip,homePhone,workPhone")] User user)
        {
            if (ModelState.IsValid)
            {
                if (!Repository.EmailExists(user.email))
                {
                    Repository.CreateUser(user);
                }
                else
                {
                    ViewBag.status = "That email already exists in out system. Click the link below.";
                    return View(user);
                }
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        // GET: User/Edit/5
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

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,firstName,lastName,email,password,birthDate,gender,role,streetAddress,city,zip,homePhone,workPhone")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
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

        /*  LOG IN AND SIGN UP ACTIONS **/

        // GET: Users/Login/5
        public ActionResult Login()
        {
            LoginVM loginVm = new LoginVM();
            return View(loginVm);
        }

        // POST: Users/Login/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                        User user = Repository.GetUser(loginVm.email);
                        Session["role"] = user.role;
                        Session["Username"] = user.email;
                        return RedirectToAction("Index", "Home");
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

        // 
        public ActionResult ForgotPassword()
        {
            LoginVM loginVm = new LoginVM();
            return View(loginVm);
        }



        // POST: Users/forgotPassword/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
