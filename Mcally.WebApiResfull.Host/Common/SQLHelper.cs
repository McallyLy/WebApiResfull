using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;

namespace BaseUtils
{
    public class SQLHelper
    {
       public static  string   connection = ConfigurationManager.ConnectionStrings["Mcally"].ConnectionString;

        public static DataTable ExecuteDataTable(string sql, CommandType commandType=CommandType.Text, List<SqlParameter> pararmeters = null) {
            DataTable data = new DataTable();
            using (SqlConnection conn = new SqlConnection(connection)) {

                using (SqlCommand command = new SqlCommand(sql, conn)) {
                    command.CommandType = commandType;
                    if (pararmeters != null) {

                        command.Parameters.AddRange(pararmeters.ToArray());
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                    command.Parameters.Clear();

                }
            }

            return data;

        }

        public static Object ExecuteScalar(string sql, CommandType type, IList<SqlParameter> parameters)
        {

            Object result = null;
            using (SqlConnection conn = new SqlConnection())
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.CommandType = type;
                    if (parameters.Count > 0)
                    {
                        comm.Parameters.Add(parameters.ToArray());
                    }
                    conn.Open();
                    result = comm.ExecuteScalar();

                }


            }
            return result;

        }
    }
 }