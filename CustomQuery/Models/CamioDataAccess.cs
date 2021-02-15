using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


// Model for loading data from Cameo_dwh only. Users should not be allowed to insert or update data in this db.

namespace CustomQuery.Models
{
    public class CameoSQLDataAccess
    {
        public static string GetConnectionString(string connectionName = "Data Source =OAKSQL03; Initial Catalog = Cameo_DWH; User Id=camarchive; Password=camarchive; Pooling=False; Connect Timeout = 30")
        {
            return connectionName;
            //ConfigurationManager.AppSettings[connectionName];
        }

        public static List<T> LoadCamData<T>(string sql)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {

                return cnn.Query<T>(sql).ToList();
            }
        }

        public class SQL04DataAccess
        {
            public static string GetConnectionString(string connectionName = "Data Source =OAKSQL04; Initial Catalog = customQuery; User Id=customQuery.service; Password=H6g12vxn92; Pooling=False; Connect Timeout = 30")
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
        }

    }
}
