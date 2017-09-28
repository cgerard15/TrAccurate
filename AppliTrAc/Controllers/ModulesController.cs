using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AppliTrAc.Models;
using AppliTrAc.ViewModels;

namespace AppliTrAc.Controllers
{
    public class ModulesController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities();

        // GET: Modules
        public ActionResult Index()
        {
            var modules = db.Modules
                .ToList();
            return View(modules);
        }

        // GET: Modules/Details
        public ActionResult Details(string id)
        {
            var surveys = db.Surveys
                .Where(x=>x.ModuleID==id.ToString())
                .ToList();
            return View(surveys);

        }

        // GET: Modules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModuleID,ModuleName")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(module);
        }

        // GET: Modules/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModuleID,ModuleName")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Modules/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
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

        
        public ActionResult ViewReport(int? id, string searchString)
        {
            {
                var report = db.Responses
                    .Include("Survey")
                    .Include("Students")
                    .Where(x => x.SurveyID == id)
                    .Select(a => new ReportViewModel()
                    {
                        StudentID = a.StudentID,
                        SurveyID = a.SurveyID,
                        ModuleID = a.Survey.ModuleID.ToString(),
                        Value = a.Value,
                        Date = a.Survey.StartDate,
                    })
                    .ToList();

                return View(report);
            }
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

        public ActionResult AllResponses(string id)
        {
            //declare arrays to hold values for the x-axis and y-axis
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var results = (from c in db.Responses select c);

            //LINQ query to select surveys for chosen module and count the instances in the database 
            var axis = results
                .Where(x => x.Survey.ModuleID == id.ToString())
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
                .AddTitle("Module Results")
                                .AddSeries("Default", chartType: "Column", xValue: xValue, yValues: yValue)
                .Write("bmp");

            return null;
        }
    }
}