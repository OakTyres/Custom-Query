using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CustomQuery.Models
{
    public class SQLString
    {
        public string SqlString { get; set; }
        public string QueryName { get; set; }



        public void StoreSql(string sql, string queryName, string username)
        {
            SQLString savedSql = new SQLString();
            savedSql.SqlString = sql.Replace("'","''");
            sql = sql.Replace("'", "''");


            //var queryName = "";

            using (SqlConnection conn = new SqlConnection("Data Source =OAKSQL04; Initial Catalog = customQuery; User Id=customQuery.service; Password=H6g12vxn92; Pooling=False; Connect Timeout = 30"))
            {
                var insertSql = @"INSERT INTO customQuery.dbo.userQueries (userName, sqlString, queryName) VALUES ('" + username + "','" + sql + "','" + queryName + "')";
                SqlCommand comm = new SqlCommand(insertSql, conn);
                comm.Connection.Open();
                comm.ExecuteNonQuery();
                comm.Connection.Close();
            }

            //return "Query saved";
        }

        //public string SaveSqlInDatabase(string sql)
        //{

        //}
    }
}
