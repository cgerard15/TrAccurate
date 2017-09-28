using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppliTrAc.Models;

namespace AppliTrAc.ViewModels
{
    public class CreateSurveyViewModel
    {
        public int ResponseID { get; set; }
        public int SurveyID { get; set; }
        public int ModuleID { get; set; }
        public bool isActive { get; set; }
        public int StudentID { get; set; }
        public string Value { get; set; }

        public virtual Module Module { get; set; }

         public IEnumerable<Student> Students { get; set; }
         public IEnumerable<Module> Modules { get; set; }
         public IEnumerable<Survey> Surveys { get; set; }

    }

}