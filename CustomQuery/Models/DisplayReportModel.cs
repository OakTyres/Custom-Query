using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CustomQuery.Models
{
    public class DisplayReportModel
    {
        public List<ListOfColumnNames> ColumnNames { get; set; }
        //public List<ListOfColumnTypes> listOfColumnTypes { get; set; }
    }

    public class ListOfColumnNames
    {
        public string ReportColumnName { get; set; }
    }

    public class ListOfColumnTypes
    {
        public string ReportColumnType { get; set; }
    }

    public class ReportDictionary
    {
        public Dictionary<string, List<string>> ReportResult { get; set; }
    }
}
