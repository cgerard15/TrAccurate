using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LumenWorks.Framework.IO.Csv;
using AppliTrAc.Models;

namespace AppliTrAc.Controllers
{
    public class UsersController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Lecturer).Include(u => u.Student);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            ViewBag.Role = new List<SelectListItem> {                  
                 new SelectListItem { Text = "Admin", Value = "Admin"},                   
                 new SelectListItem { Text = "Lecturer", Value = "Lecturer"}
             };
            ViewBag.UserID = new SelectList(db.Lecturers, "LecturerID", "FirstName");
            ViewBag.UserID = new SelectList(db.Students, "StudentID", "FirstName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Lecturers, "LecturerID", "FirstName", user.UserID);
            ViewBag.UserID = new SelectList(db.Students, "StudentID", "FirstName", user.UserID);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Role = new List<SelectListItem> {
                 new SelectListItem { Text = "Admin", Value = "Admin"},
                 new SelectListItem { Text = "Lecturer", Value = "Lecturer"}
             };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Lecturers, "LecturerID", "FirstName", user.UserID);
            ViewBag.UserID = new SelectList(db.Students, "StudentID", "FirstName", user.UserID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Lecturers, "LecturerID", "FirstName", user.UserID);
            ViewBag.UserID = new SelectList(db.Students, "StudentID", "FirstName", user.UserID);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
