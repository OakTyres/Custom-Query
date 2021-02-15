using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomQuery.Models
{
    public class QueryObject
    {
        public string tableName { get; set; }
        public List<Columns> ColumnNames { get;set; }
        public List<Types> ColumnTypes { get; set; }
    }

    public class Columns
    {
        public string ColumnName { get; set; }
    }

    public class Types
    {
        public string ColumnType { get; set; }
    }
}
