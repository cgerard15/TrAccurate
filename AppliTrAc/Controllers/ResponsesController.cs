using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AppliTrAc.Models;
using AppliTrAc.ViewModels;

namespace AppliTrAc.Controllers
{
    public class ResponsesController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities();

        // GET: Responses
        public ActionResult Index(int? id)
        {
            
            var surveys = db.Surveys
                .Where(u => u.IsActive)
                .Where(u => u.EndDate > DateTime.Now)
                .ToList();
            return View(surveys);
        }


        // GET: Responses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = db.Responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }
        public ActionResult Create(Response model, int SurveyId)
        {

            Survey s = db.Surveys.Find(SurveyId);

                Response r = new Response();


            r.SurveyID = SurveyId;
            r.Surveys.Add(s);


                ViewBag.SurveyID = new SelectList(db.Surveys, "SurveyID", "SurveyID");
                ViewBag.StudentID = new SelectList(db.Students, "StudentID", "StudentID");

            return View(r);

        }
        // POST: Responses/Create
        //Method to save student response to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Response model, string submit, int id)
        {
            if (ModelState.IsValid)
            {
                //save the date of response
                model.Date = DateTime.Now;
                //save student number
                model.StudentID = id;
                //save the value of the button clicked as string
                model.Value = (submit);

                db.Responses.Add(model);

                db.SaveChanges();
                // return user to Survey index and save new response Id
                return RedirectToAction("Index", new {id = model.ResponseID});
                }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserID", model.StudentID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "SurveyID", "SurveyID", model.SurveyID);
            return View(model);
        }

    // GET: Responses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = db.Responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "FirstName", response.StudentID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "SurveyID", "SurveyID", response.SurveyID);
            return View(response);
        }

        // POST: Responses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ResponseID,SurveyID,StudentID,Value")] Response response)
        {
            if (ModelState.IsValid)
            {
                db.Entry(response).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "FirstName", response.StudentID);
            ViewBag.SurveyID = new SelectList(db.Surveys, "SurveyID", "SurveyID", response.SurveyID);
            return View(response);
        }

        // GET: Responses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = db.Responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        // POST: Responses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Response response = db.Responses.Find(id);
            db.Responses.Remove(response);
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
