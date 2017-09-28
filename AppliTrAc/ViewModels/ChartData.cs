using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppliTrAc.Models;

namespace AppliTrAc.ViewModels
{
    public class ChartData
    {
        public IEnumerable<Response> Responses { get; set; }

        public string Value { get; set; }
        public int Count { get; set; }
    }

}