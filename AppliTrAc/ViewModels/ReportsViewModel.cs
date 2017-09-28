using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AppliTrAc.Models;

namespace AppliTrAc.ViewModels
{
    public class ReportViewModel
    {
        public int ResponseID { get; set; }
        public int SurveyID { get; set; }
        public string ModuleID { get; set; }
        public bool isActive { get; set; }
        public int StudentID { get; set; }
        public string Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime Date { get; set; }

        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<Survey> Surveys { get; set; }
        public IEnumerable<Response> Responses { get; set; }
    }
}
