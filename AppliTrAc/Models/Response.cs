//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppliTrAc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Response
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Response()
        {
            this.Surveys = new HashSet<Survey>();
        }

        public int ResponseID { get; set; }
        public int SurveyID { get; set; }
        public int StudentID { get; set; }
        public string Value { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Student Student { get; set; }
        public virtual Survey Survey { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Survey> Surveys { get; set; }
    }
}
