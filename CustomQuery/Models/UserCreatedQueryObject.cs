using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomQuery.Models
{
    public class UserCreatedQueryObject
    {
        public string TableName { get; set; }
        public string QueryName { get; set; }
        public List<ReportCriteria> ColumnNames { get; set; }
        public List<ColumnTypesToInclude> ColumnTypeSelection { get; set; }
        public List<ReportCriteria> FilterOperators { get; set; }
        public List<ReportCriteria> FilterValues { get; set; }
        public List<ReportCriteria> SecondFilterValue { get; set; }
        public List<ColumnsToInclude> IncludeInViews { get; set; }
        public List<ColumnsToAggregate> AdditionalActions { get; set; }
    }

    public class ReportCriteria
    {
        public string FilterOperator { get; set; }
        public string ColumnName { get; set; }
        public string FilterValue { get; set; }
        public string SecondFilterValue { get; set; }
        public string IncludeInView { get; set; }
        public string AdditionalAction { get; set; }
    }

    public class ColumnsToAggregate
    {
        public string columnsToAggregate { get; set; }
     
    }

    public class ColumnsToInclude
    {
        public string columnsToInclude { get; set; }
    }

    public class ColumnTypesToInclude
    {
        public string columnTypes { get; set; }
    }
}
//[{"columnsToAggregate":"None"},{ "columnsToAggregate":"Sum"}]}