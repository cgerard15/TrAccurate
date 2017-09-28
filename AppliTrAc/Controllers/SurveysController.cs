using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppliTrAc.Models;
using System.Data.Entity.Infrastructure;
using System.Web.Helpers;
using AppliTrAc.ViewModels;
using WebGrease.Css.Extensions;

namespace AppliTrAc.Controllers
{
    public class SurveysController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities();

        // GET: Surveys
        public ActionResult Index()
        {
            var surveys = db.Surveys.ToList();
            return View(surveys);
        }

        // GET: Surveys/Details/5
        public ActionResult Details(int? id)
        {
            var details = db.Responses
                .Include("Survey")
                .Include("Student")
                .Where(x => x.SurveyID == id)
                .Select(a => new ReportViewModel()
                {
                    SurveyID = a.SurveyID,
                    StudentID = a.StudentID,
                    Value = a.Value,
                    Date = a.Date,
                })
                .ToList();

            return View(details);
        }

        // GET: Surveys/Create
        public ActionResult Create()
        {
            var survey = new Survey
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            ViewBag.ModuleID = new SelectList(db.Modules, "ModuleId", "ModuleName");
            return View(survey);
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Survey survey, string action)
        {

            if (ModelState.IsValid)
            {
                db.Surveys.Add(survey);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = survey.SurveyID });
            }
            TempData["error"] = "An error occurred while attempting to create this survey.";

            ViewBag.ModuleID = new SelectList(db.Modules, "ModuleId", "ModuleName", survey.ModuleID);
            return View(survey);
        }

        // GET: Surveys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModuleID = new SelectList(db.Modules, "ModuleId", "ModuleName");
            return View(survey);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SurveyID,StartDate,EndDate,isActive,ModuleID,Name")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(survey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ModuleID = new SelectList(db.Modules, "ModuleId", "ModuleName", survey.ModuleID);
            return View(survey);
        }


        // GET: Surveys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Survey survey = db.Surveys.Find(id);
            db.Surveys.Remove(survey);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
        public ActionResult CharterColumn(int? id)
        {
            //declare arrays to hold values for the x-axis and y-axis
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var results = (from c in db.Responses select c);

            //LINQ query to select surveys and count the instances in the database 
            var axis = results
                .Where(x => x.SurveyID == id)
                .GroupBy(r => r.Value.ToString())
                .Select(r => new ChartData()
                {
                    Value = r.Key,
                    Count = r.Count()
                }).ToList();

            //assign values to axis
            foreach (var item in axis)
            {
                xValue.Add(item.Value);
                yValue.Add(item.Count);
            }

            //chart design
            new Chart(width: 600, height: 400, theme: ChartTheme.Blue)
                .SetXAxis("Seat Postion")
                .SetYAxis("Count")
                .AddTitle("Individual Survey Results")
                .AddSeries("Default", chartType: "Column", xValue: xValue, yValues: yValue)
                .Write("bmp");

            return null;
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
