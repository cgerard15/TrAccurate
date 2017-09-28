using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppliTrAc.Models;

namespace AppliTrAc.ViewModels
{
    public class StudentIndexData
    {
        public int StudentID { get; set; }
        public int SurveyID { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public bool IsActive { get; set; }


        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<Survey> Surveys { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}