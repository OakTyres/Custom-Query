using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomQuery.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static CustomQuery.Controllers.ReportController;
using Microsoft.EntityFrameworkCore.Update;
using System.IO.Pipelines;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Dynamic;
using static CustomQuery.Models.CameoSQLDataAccess;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CustomQuery.Controllers
{
    public class HomeController : Controller
    {

        public string username = "";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        

    

        public IActionResult Index()
        {
            username = HttpContext.User.Identity.Name;
            var dataCollector = new TableDataCollector();
            dataCollector.getTableInformation("TRTRAN");
            System.Diagnostics.Debug.WriteLine(username);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult QueryBuilder(string data)
        {
            var dataCollector = new TableDataCollector();
            var myQueryObject = dataCollector.getTableInformation(data);
            return View(myQueryObject);
        }

        [HttpPost]
        public IActionResult AddQueryParameters(QueryObject data)
        {
            var reportCont = new ReportController();

            return PartialView("~/Views/Home/partialViews/_AddQueryOptions.cshtml", data);
        }

        [HttpPost]
        public ReportDictionary RunFinalQuery(UserCreatedQueryObject FinalQuery, string username)
        {
            // create static values to construct the SQL query
            var arrayCount = FinalQuery.ColumnNames.Count;
            var sqlSelect = @"SELECT ";
            var sqlFrom = @" FROM cameo_DWH.dbo." + FinalQuery.TableName.ToString();
            var queryName = FinalQuery.QueryName;
            // create arrays to store the selected columns, where and group clauses (if any)
            List<string> columnArray = new List<string>();
            List<string> columnArrayForDictionary = new List<string>();
            List<string> groupArray = new List<string>();
            List<string> whereArray = new List<string>();

            // iterate through the list of columns and add each value to the correct array
            for (var i = 0; i < arrayCount; i++)
            {

                if (FinalQuery.AdditionalActions[i].columnsToAggregate == "None")
                {

                    if (FinalQuery.IncludeInViews[i].columnsToInclude == "Yes")
                    {
                        columnArray.Add("ISNULL(" + FinalQuery.ColumnNames[i].ColumnName.ToString() + ",'') AS " + FinalQuery.ColumnNames[i].ColumnName.ToString());
                        groupArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString());
                        columnArrayForDictionary.Add(FinalQuery.ColumnNames[i].ColumnName.ToString());
                    }
                }
                else
                {
                    //columnArray.Add(FinalQuery.AdditionalActions[i].columnsToAggregate.ToString() + "(" + FinalQuery.ColumnNames[i].ColumnName.ToString() + ") AS " + FinalQuery.ColumnNames[i].ColumnName.ToString());
                    columnArrayForDictionary.Add(FinalQuery.ColumnNames[i].ColumnName.ToString());
                    columnArray.Add(FinalQuery.AdditionalActions[i].columnsToAggregate.ToString() + "(" + FinalQuery.ColumnNames[i].ColumnName.ToString() + ") AS " + FinalQuery.ColumnNames[i].ColumnName.ToString());
                    //groupArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString());
                    //if (FinalQuery.IncludeInViews[i].columnsToInclude == "Yes")
                    //{
                    //groupArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString());
                    //}
                }

                if (FinalQuery.FilterOperators[i].FilterOperator != "None")
                {

                    if (FinalQuery.FilterOperators[i].FilterOperator == "=")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + FinalQuery.FilterOperators[i].FilterOperator.ToString() + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "'");
                        }
                        else
                        {
                            
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + FinalQuery.FilterOperators[i].FilterOperator.ToString() + " "
                            + FinalQuery.FilterValues[i].FilterValue.ToString());
                        }
                    }

                    else if (FinalQuery.FilterOperators[i].FilterOperator == "not=")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "<>" + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "'");
                        }
                        else
                        {
                            whereArray.Add(" WHERE " + FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                                + "<>" + " "
                                + FinalQuery.FilterValues[i].FilterValue.ToString());
                        }
                    }

                    else if (FinalQuery.FilterOperators[i].FilterOperator == "greaterThan")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + ">" + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "'");
                        }
                        else
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + ">" + " "
                            + FinalQuery.FilterValues[i].FilterValue.ToString());
                        }
                    }

                    else if (FinalQuery.FilterOperators[i].FilterOperator == "lessThan")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "<" + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "'");
                        }
                        else
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "<" + " "
                            + FinalQuery.FilterValues[i].FilterValue.ToString());
                        }
                    }

                    else if (FinalQuery.FilterOperators[i].FilterOperator == "greaterEqual")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + ">=" + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "'");
                        }
                        else
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + ">=" + " "
                            + FinalQuery.FilterValues[i].FilterValue.ToString());
                        }
                    }

                    else if (FinalQuery.FilterOperators[i].FilterOperator == "lessEqual")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "<=" + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "'");
                        }
                        else
                        {
                            whereArray.Add(" WHERE " + FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "<=" + " "
                            + FinalQuery.FilterValues[i].FilterValue.ToString());
                        }
                    }

                    else if (FinalQuery.FilterOperators[i].FilterOperator == "between")
                    {
                        if (FinalQuery.ColumnTypeSelection[i].columnTypes == "STRING" || FinalQuery.ColumnTypeSelection[i].columnTypes == "DATE")
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "BETWEEN" + " "
                            + "'" + FinalQuery.FilterValues[i].FilterValue.ToString() + "' AND '" + FinalQuery.SecondFilterValue[i].SecondFilterValue.ToString() + "'");
                        }
                        else
                        {
                            whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                            + "BETWEEN" + " "
                            + FinalQuery.FilterValues[i].FilterValue.ToString() + " AND " + FinalQuery.SecondFilterValue[i].SecondFilterValue.ToString());
                        }
                    }
                    else if (FinalQuery.FilterOperators[i].FilterOperator == "contains")
                    {
                        whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                         + "LIKE" + " "
                         + "'%" + FinalQuery.FilterValues[i].FilterValue.ToString() + "%'");
                    }
                    else if (FinalQuery.FilterOperators[i].FilterOperator == "isOneOf")
                    {
                        whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                         + "IN" + " ('" + FinalQuery.FilterValues[i].FilterValue.ToString().Replace(",","','") + "')");
                    }
                    else if (FinalQuery.FilterOperators[i].FilterOperator == "isNotOneOf")
                    {
                        whereArray.Add(FinalQuery.ColumnNames[i].ColumnName.ToString() + " "
                         + "NOT IN" + " ('" + FinalQuery.FilterValues[i].FilterValue.ToString().Replace(",", "','") + "')");
                    }

                }
            }



            // Add comma seperators to the select section
            foreach (var j in columnArray)
            {
                sqlSelect = sqlSelect + j + ",";
            }

            //System.Diagnostics.Debug.WriteLine(sqlSelect);

            // add the FROM section of the SQL query
            sqlSelect = sqlSelect.Remove(sqlSelect.Length - 1, 1) + sqlFrom;

            // add WHERE clause values if any exist and remove the additional AND from the end of the string
            if (whereArray.Count > 0)
            {
                sqlSelect += " WHERE ";
                foreach (var g in whereArray)
                {
                    sqlSelect = sqlSelect + g + " AND ";
                }
                sqlSelect = sqlSelect.Remove(sqlSelect.Length - 4, 3);
            }


            // add an optional GROUP BY clause
            if (groupArray.Count > 0)
            {
                sqlSelect = sqlSelect + " GROUP BY ";
                foreach (var h in groupArray)
                {
                    sqlSelect = sqlSelect + h + ",";
                }
            }

            // remove extra commas from the end of the string
            sqlSelect = sqlSelect.Remove(sqlSelect.Length - 1, 1);

            //System.Diagnostics.Debug.WriteLine(sqlSelect);

            // create a multidimensional dictionary object 
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

            // add keys to the dictionary (column headers)
            if (columnArrayForDictionary.Count > 0)
            {
                for (var colArray = 0; colArray < columnArrayForDictionary.Count; colArray++)
                {
                    //List<string> list = new List<string>();
                    List<string> arraystring = new List<string>();
                    results.Add(columnArrayForDictionary[colArray], arraystring);
                }
            }

            // open a localised connection to SQL Server and pass in the generated query. Push each row returned to the dictionary values
            using (SqlConnection conn = new SqlConnection("Data Source =OAKSQL03; Initial Catalog = Cameo_DWH; User Id=camarchive; Password=camarchive; Pooling=False; Connect Timeout = 30"))
            {
                SqlCommand comm = new SqlCommand(sqlSelect, conn);
                comm.Connection.Open();
                comm.ExecuteNonQuery();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, List<string>> clm = new Dictionary<string, List<string>>();
                    for (int h = 0; h < columnArrayForDictionary.Count; h++)
                    {
                        dynamic fieldMetaData = new ExpandoObject();
                        var sqlType = reader.GetDataTypeName(h);
                        //var sqlColName = reader.GetName(h).First();

                        //System.Diagnostics.Debug.WriteLine(sqlColName);

                        if (sqlType == "varchar" || sqlType == "nvarchar")
                        {
                            results[reader.GetName(h)].Add(reader.GetString(h));
                        }
                        else if (sqlType == "bigint")
                        {
                            results[reader.GetName(h)].Add(reader.GetInt64(h).ToString());
                        }
                        else if (sqlType == "float")
                        {
                            results[reader.GetName(h)].Add(reader.GetFloat(h).ToString());
                        }
                        else if (sqlType == "int")
                        {
                            results[reader.GetName(h)].Add(reader.GetInt32(h).ToString());
                        }
                        else if (sqlType == "decimal")
                        {
                            results[reader.GetName(h)].Add(reader.GetDecimal(h).ToString());
                        }
                        else if (sqlType == "numeric")
                        {
                            results[reader.GetName(h)].Add(reader.GetDecimal(h).ToString());
                        }
                        else if (sqlType == "datetime")
                        {
                            if (reader.GetDateTime(h).ToString() == null)
                            {
                                results[reader.GetName(h)].Add("");
                            }
                            else
                            {
                                //results[reader.GetName(h)].Add(reader.GetDateTime(h).ToShortDateString().ToString());
                                results[reader.GetName(h)].Add(reader.GetDateTime(h).ToString());
                            }
                        }
                    }
                }

                comm.Connection.Close();
            }


            // pass the final dictionary object to the global dictionary
            ReportDictionary report = new ReportDictionary();
            report.ReportResult = results;

            SQLString queryToSave = new SQLString();
            queryToSave.StoreSql(sqlSelect, queryName, username);

            // return a partial view to the JQuery request and pass in the dictionary class
            return report;

        }

        [HttpPost]
        public ActionResult DisplayReport(UserCreatedQueryObject report, string username)
        {
            ReportDictionary query = RunFinalQuery(report, username);

            return PartialView("~/Views/Home/partialViews/_CompileReport.cshtml", query);
        }

        //[HttpPost]
        //public ActionResult ExportToExcel(UserCreatedQueryObject report, string us)
        //{
        //    ReportDictionary query = RunFinalQuery(report);
        //    return PartialView("~/Views/Home/partialViews/_CompileReport.cshtml", query);
        //}

        //[HttpPost]
        //public FileResult SaveQuery()
        //{

        //}
       

        public static List<SavedQuery> LoadQueryList(string username)
        {
            
            //string username = HttpContext.User.Identity.Name;
            var loadQueries = @"SELECT [Id]
                                      ,[userName]
                                      ,[sqlString]
                                      ,[queryName]
                                  FROM [customQuery].[dbo].[userQueries]
                                  WHERE [userName] = '" + username + "'";
            return SQL04DataAccess.LoadData<SavedQuery>(loadQueries);
        }

        [HttpGet]
        public ActionResult SavedQueries(string username)
        {
            var savedQueryList = LoadQueryList(username);
            List<SavedQuery> savedQueries = new List<SavedQuery>();

            foreach (var item in savedQueryList)
            {
                savedQueries.Add(new SavedQuery
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    SqlString = item.SqlString,
                    QueryName = item.QueryName
                });
            }

            return View(savedQueries);
        }


        //public string LoadQuery(int Id)
        //{

        //    System.Diagnostics.Debug.WriteLine(Id);
        //    var loadQuery = @"SELECT sqlString FROM customQuery.dbo.userQueries WHERE Id = '" + Id + "'";
        //    return SQL04DataAccess.LoadData<ReportDictionary>(loadQuery);
        //}

        [HttpPost]
        public ReportDictionary LoadExecutedQuery(int Id)
        {

            var SQL04 = "Data Source =OAKSQL04; Initial Catalog = customQuery; User Id=customQuery.service; Password=H6g12vxn92; Pooling=False; Connect Timeout = 30";
            var SQL03 = "Data Source = OAKSQL03; Initial Catalog = Cameo_DWH; User Id = camarchive; Password = camarchive; Pooling = False; Connect Timeout = 30";
            var loadQuery = @"SELECT sqlString FROM customQuery.dbo.userQueries WHERE Id = '" + Id + "'";
            List<string> columns = new List<string>();
            List<string> newColumns = new List<string>();
            var loadedQuery = "";

            List<ReportDictionary> f = new List<ReportDictionary>();

            using (SqlConnection conn = new SqlConnection(SQL04))
            {
                SqlCommand comm = new SqlCommand(loadQuery, conn);
                comm.Connection.Open();
                comm.ExecuteNonQuery();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    loadedQuery = reader.GetString(0);
                }
                comm.Connection.Close();
            }

            Dictionary<string, List<string>> loadedResults = new Dictionary<string, List<string>>();

            var col = 0;

            // open a localised connection to SQL Server and pass in the generated query. Push each row returned to the dictionary values
            using (SqlConnection conn = new SqlConnection(SQL03))
            {
                SqlCommand comm = new SqlCommand(loadedQuery, conn);
                comm.Connection.Open();
                comm.ExecuteNonQuery();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    col += reader.VisibleFieldCount;
                    for (var colArray = 0; colArray < reader.FieldCount; colArray++)
                    {
                        columns.Add(reader.GetName(colArray).ToString());
                    }
                }

                foreach (var s in columns)
                {
                    if (!newColumns.Contains(s))
                    {
                        newColumns.Add(s);
                    }

                }

                foreach (var s in newColumns)
                {
                    System.Diagnostics.Debug.WriteLine(s);
                }

                // add keys to the dictionary (column headers)

                for (var colArray = 0; colArray < newColumns.Count(); colArray++)
                {
                    //List<string> list = new List<string>();
                    List<string> arraystring = new List<string>();
                    loadedResults.Add(newColumns[colArray], arraystring);
                }
                
                comm.Connection.Close();
            }



            using (SqlConnection conn = new SqlConnection(SQL03))
            {
                SqlCommand comm = new SqlCommand(loadedQuery, conn);
                comm.Connection.Open();
                comm.ExecuteNonQuery();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, List<string>> clm = new Dictionary<string, List<string>>();
                    for (int h = 0; h < newColumns.Count(); h++)
                    {


                        dynamic fieldMetaData = new ExpandoObject();
                        var sqlType = reader.GetDataTypeName(h);
                        //var sqlColName = reader.GetName(h).First();

                        //System.Diagnostics.Debug.WriteLine(sqlColName);

                        if (sqlType == "varchar" || sqlType == "nvarchar")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetString(h));
                        }
                        else if (sqlType == "bigint")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetInt64(h).ToString());
                        }
                        else if (sqlType == "float")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetFloat(h).ToString());
                        }
                        else if (sqlType == "int")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetInt32(h).ToString());
                        }
                        else if (sqlType == "decimal")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetDecimal(h).ToString());
                        }
                        else if (sqlType == "numeric")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetDecimal(h).ToString());
                        }
                        else if (sqlType == "datetime")
                        {
                            loadedResults[reader.GetName(h).ToString()].Add(reader.GetDateTime(h).ToString());
                        }

                    }

                }
                  

                
                comm.Connection.Close();
                // pass the final dictionary object to the global dictionary
                

                //SQLString queryToSave = new SQLString();
                //queryToSave.StoreSql(sqlSelect, queryName);

                // return a partial view to the JQuery request and pass in the dictionary class
                //return report;
                //foreach (var item in loadedResults)
                //{
                //    System.Diagnostics.Debug.WriteLine("this " + item.Value.Last().ToString());
                //}


                

            }
        ReportDictionary report = new ReportDictionary();
        report.ReportResult = loadedResults;
            return report;

        }

        [HttpPost]
        public ActionResult DisplayLoadedReport(int Id)
        {
            ReportDictionary loadedQuery = LoadExecutedQuery(Id);
            
            return PartialView("~/Views/Home/partialViews/_CompileSavedReport.cshtml", loadedQuery);
        }

        public FileResult DownloadReport(string data)
        {
            //var report = LoadExecutedQuery(Id);
            ReportDictionary report = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportDictionary>(data);



            var sb = new StringBuilder();
            var headers = "";
            foreach (var item in report.ReportResult.Keys)
            {
                headers += item + ",";
            }
            sb.AppendLine(headers);
            //foreach (var item in report)
            //{
            //    sb.AppendLine(item.EmployeeId + "," + item.EmployeeName + "," + item.MonthsOfEmployment + ", " + item.EquipmentDeduction + "," + item.UniformDeduction + "," + item.TotalDeductions);
            //}
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");

        }


        public ActionResult DeleteReport(int Id, string username)
        {
            var deleteSql = @"DELETE FROM customQuery.dbo.userQueries WHERE Id = " + Id;
            var SQL04 = "Data Source =OAKSQL04; Initial Catalog = customQuery; User Id=customQuery.service; Password=H6g12vxn92; Pooling=False; Connect Timeout = 30";
            using (SqlConnection conn = new SqlConnection(SQL04))
            {
                SqlCommand comm = new SqlCommand(deleteSql, conn);
                comm.Connection.Open();
                comm.ExecuteNonQuery();
                comm.Connection.Close();

            }

            return Redirect("/Home/SavedQueries?username=" + username);
        }
    }

    }

