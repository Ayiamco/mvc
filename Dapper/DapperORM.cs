using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;



namespace DapperMVCCRUD.Models
{
    public static class DapperOrm
    {
        private static string connectionString =
            @"data source=.; database = MvcPractice; integrated security=true";


        public static void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new (connectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }

        }

        //DapperORM.ExecuteReturnScalar<int>(_,_);
        public static T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new (connectionString))
            {
                sqlCon.Open();
                return (T)Convert.ChangeType(
                    sqlCon.ExecuteScalar(procedureName, param, commandType: CommandType.StoredProcedure), typeof(T));
            }

        }

        //DapperORM.ReturnList<EmployeeModel> <=  IEnumerable<EmployeeModel>
        public static IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new (connectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }

        }
    }
}