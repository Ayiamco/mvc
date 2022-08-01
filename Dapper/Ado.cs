using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MvcPractice.EF;

namespace MvcPractice.Dapper
{
    public class Ado
    {
        private static string connectionString =
            @"data source=.; database = MvcPractice; integrated security=true";

        public static  List<Lga> GetLgas()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new("SP_GetAllLgas",connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                var reader=cmd.ExecuteReader();

                List<Lga> lgas = new();

                while (reader.Read())
                {
                    lgas.Add(new Lga()
                    {
                        Id=(int)reader["Id"],
                        Name = (string)reader["Name"],
                        StateId = (int)reader["StateId"]
                    });
                }

                return lgas;
            }
        }
    }
}
