using BaseUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model
{


    public class SuperModel<T> where T : new()
    {
        // Fields
        protected static string InsertSql;
        protected static string selectSql;
        protected static string updateSql;

        // Methods
        static SuperModel()
        {
            SuperModel<T>.updateSql = "update {0} set {1} where {2}={3}";
            SuperModel<T>.InsertSql = "insert into {0}({1}) values({2});select @@identity";
            SuperModel<T>.selectSql = "select top 1 * from {0} where {1}='{2}' and {3}";
        }

        protected static string _f(string k)
        {
            return ("[" + k + "]");
        }

        protected static T ToEntity(DataRow dr)
        {
            T local = Activator.CreateInstance<T>();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                if (dr.Table.Columns.Contains(info.Name))
                {
                    Type propertyType = info.PropertyType;
                    if (info.PropertyType.IsGenericType && (info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        propertyType = info.PropertyType.GetGenericArguments()[0];
                    }
                    if (!dr[info.Name].Equals(DBNull.Value))
                    {
                        info.SetValue(local, Convert.ChangeType(dr[info.Name], propertyType), null);
                    }
                }
            }
            return local;
        }

        protected static IList<System.Collections.Hashtable> ToHashtableList(DataTable dt)
        {
            return UntilService.DataTableToArrayList(dt);
        }

        protected static List<T> ToList(DataTable dt)
        {
            List<T> list = new List<T>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(SuperModel<T>.ToEntity(row));
            }
            return list;
        }
    }
}
