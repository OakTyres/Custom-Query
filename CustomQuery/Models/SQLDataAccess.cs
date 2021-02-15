using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace CustomQuery.Models
{
    public static class SQLDataAccess
    {
        public static string GetConnectionString(string connectionName = "Data Source =oaksql04; Initial Catalog = customQuery; User Id=customQuery.service; Password=3Gy5pW96MtaF; Pooling=False;Connect Timeout = 30")
        {
            return connectionName;
            //ConfigurationManager.AppSettings[connectionName];
        }

        public static List<T> LoadData<T>(string sql)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                return cnn.Query<T>(sql).ToList();
            }
        }


        // when we pass a list of objects to this function we can decide if its groupd or not and then build the query depend on this 
        public static int SaveData<T>(string sql, T data)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                return cnn.Execute(sql, data);
            }
        }

    }
}
