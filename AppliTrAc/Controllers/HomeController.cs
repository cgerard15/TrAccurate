using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AppliTrAc.Models;

namespace AppliTrAc.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles="Lecturer, Admin, Student")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        //User log in and use FormsAuthentication to manage the role
        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            ProjectDBEntities db = new ProjectDBEntities();
            var user = db.Users.Where(x => x.UserID == model
                            .UserID && x.Password == model.Password)
                                .FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserID.ToString(), false);
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "User Id or Password is wrong");
                return View();
            }
        }



private ProjectDBEntities db = new ProjectDBEntities();

        public ActionResult Register()
        {
            ViewBag.Role = new List<SelectListItem> {
                 new SelectListItem { Text = "Student", Value = "Student"},
             };
            return View();
        }

        //Posts new log in information to database
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                ModelState.Clear();
            }
            return RedirectToAction("Login");
        }


        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }


    }
}