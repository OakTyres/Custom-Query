using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomQuery.Models;
using System.Data.SqlClient;
using System.Configuration;



namespace CustomQuery.Models
{
    public class TableDataCollector
    {

        //[HttpGet]
        public QueryObject getTableInformation(string data)
        {

            string sql = @"SELECT 
	                        COLUMN_NAME as columnName, 
	                        CASE WHEN DATA_TYPE = 'bit' THEN 'BOOL' ELSE 
		                        (CASE WHEN NUMERIC_SCALE IS NOT NULL THEN (CASE WHEN NUMERIC_SCALE = 0 THEN 'INT' ELSE 'NUMERIC' END) ELSE 
		                        (CASE WHEN NUMERIC_SCALE IS NULL AND DATETIME_PRECISION IS NULL THEN 'STRING' ELSE 
		                        (CASE WHEN DATETIME_PRECISION IS NOT NULL THEN 'DATE' END)END) END) END AS columnType
                           FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + data + "'";


            var tableCulmns = CameoSQLDataAccess.LoadCamData<Columns>(sql);
            var columnTypes = CameoSQLDataAccess.LoadCamData<Types>(sql);


            var TableForCustomQuery = new QueryObject();
            TableForCustomQuery.tableName = data;
            TableForCustomQuery.ColumnNames = tableCulmns;
            //TableForCustomQuery.ColumnTypes = tableCulmns;
            TableForCustomQuery.ColumnTypes = columnTypes;
            return TableForCustomQuery;

        }

        public DisplayReportModel getQueryResult(string sql)
        {

            var tableColumns = CameoSQLDataAccess.LoadCamData<ListOfColumnNames>(sql);
            var sqlResult = new DisplayReportModel();

            sqlResult.ColumnNames = tableColumns;
            //foreach(var row in tableColumns)
            //{
            //    sqlResult.ColumnNames.Add(new ListOfColumnNames
            //    {
            //        ReportColumnName = row.ReportColumnName
            //    });

            //    System.Diagnostics.Debug.WriteLine(row.ReportColumnName);
            //}

            
            //sqlResult.ColumnNames = tableColumns;
            return sqlResult;
        }

    }
}
