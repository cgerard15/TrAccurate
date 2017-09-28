using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AppliTrAc.Models;
using AppliTrAc.ViewModels;
using LumenWorks.Framework.IO.Csv;
using WebGrease.Css.Extensions;

namespace AppliTrAc.Controllers
{
    public class StudentsController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities();


        // GET: Students
        public ViewResult Index(string sortOrder, string currentFilter, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var students = from s in db.Students
                select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s =>
                    s.LastName.ToUpper().Contains(searchString.ToUpper()) ||
                    s.LastName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                default: // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            return View(students.ToList());
        }


        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            var enroll = db. Enrollments
                .Where(x=>x.StudentID==id)
                .ToList();
            return View(enroll);
        }

       
        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "FirstName", student.StudentID);
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "FirstName", student.StudentID);
            ViewBag.StudentID = new SelectList(db.Users, "UserID", "Password", student.StudentID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID,FirstName,LastName,Email")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "FirstName", student.StudentID);
            ViewBag.StudentID = new SelectList(db.Users, "UserID", "Password", student.StudentID);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
        public ActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    //checking for correct file type
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        //declare new stream object
                        Stream stream = upload.InputStream;
                        //declare new DataTable object
                        DataTable csvTable = new DataTable();
                        //open the ".csv" file
                        using (CsvReader csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            //loads the DataTable 
                            csvTable.Load(csvReader);
                        }

                        //establish database connection
                        using (
                            SqlConnection dbConnection =
                                new SqlConnection(
                                    "Data Source=ACER\\SQLEXPRESS;Initial Catalog=ProjectDB;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework")
                        )
                        {
                            dbConnection.Open();
                            //declare SqlBulkCopy object 
                            using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                            {
                                //mapping the data to specific columns 
                                s.ColumnMappings.Add("StudentID", "StudentID");
                                s.ColumnMappings.Add("FirstName", "FirstName");
                                s.ColumnMappings.Add("LastName", "LastName");
                                s.ColumnMappings.Add("Email", "Email");

                                //set the name of the destination table
                                s.DestinationTableName = "Student";

                                //call WriteToSever to import data
                                s.WriteToServer(csvTable);
                            }
                            //close the database connection
                            dbConnection.Close();
                            return View(csvTable);

                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
            
        }

        public ActionResult ModuleSeats(string id, int stId)
        {
            //declare arrays to hold values for the x-axis and y-axis
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var results = (from c in db.Responses select c);

            //LINQ query to select surveys by module and by student count the instances in the database 
            var axis = results
                .Where(x => x.Survey.ModuleID == id.ToString())
                .Where(x => x.StudentID==stId)
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
                .AddTitle("Module Results for Student")
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

           
