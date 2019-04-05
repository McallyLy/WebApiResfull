using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BaseUtils
{
public class UntilService
    {
        public static IList<Hashtable> ToDataList(DataTable data) {
            //IList<Hashtable> hashtables = new List<Hashtable>() { };
            //if (data.Rows.Count>0) {
            //foreach (DataRow row in data.Rows) {
            //        Hashtable h = new Hashtable();
            //        foreach(DataColumn colum in data.Constraints) {
            //            h.Add(colum.ColumnName,row[colum]);
            //        }
            //        hashtables.Add(h);

            //    }
            //}



            IList<Hashtable> tables = new List<Hashtable>() { };
            if (data.Rows.Count > 0)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DataRow row = data.Rows[i];
                    Hashtable hashtable = new Hashtable();

                    for (int k = 0; k < data.Columns.Count; k++)
                    {
                        DataColumn column = data.Columns[k];
                        hashtable.Add(column.ColumnName, row[column]);

                    }
                    tables.Add(hashtable);

                }

            }

            return tables;


        }

        /// <summary>
        /// 反射机制实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList DataTableToIList<T>(DataTable dt)
        {
            IList list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T local = Activator.CreateInstance<T>();
                foreach (PropertyInfo info in local.GetType().GetProperties())
                {
                    string name = info.Name;
                    if (dt.Columns.Contains(name) && info.CanWrite)
                    {
                        object obj2 = row[name];
                        if (obj2 != DBNull.Value)
                        {
                            info.SetValue(local, obj2, null);
                        }
                    }
                }
                list.Add(local);
            }
            return list;
        }

        private static DataRow createRowClone(DataRow sourceRow, DataRow newRow, string[] fieldNames)
        {
            for (int i = 0; i < fieldNames.Length; i++)
            {
                string str = fieldNames[i];
                newRow[str] = sourceRow[str];
            }
            return newRow;
        }

        public static Hashtable DataRowToHashTable(DataRow dr)
        {
            Hashtable hashtable = new Hashtable(dr.ItemArray.Length);
            foreach (DataColumn column in dr.Table.Columns)
            {
                hashtable.Add(column.ColumnName, dr[column.ColumnName]);
            }
            return hashtable;
        }

        public static IList<Hashtable> DataTableToArrayList(DataTable dt)
        {
            if ((dt == null) || (dt.Rows.Count <= 0))
            {
                return new List<Hashtable>();
            }
            IList<Hashtable> list2 = new List<Hashtable>();
            IEnumerator enumerator = dt.Rows.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Hashtable item = DataRowToHashTable((DataRow)enumerator.Current);
                list2.Add(item);
            }

            return list2;
        }

        public static Hashtable DataTableToHashtable(DataTable dt)
        {
            Hashtable hashtable = new Hashtable();
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    hashtable[columnName.ToUpper()] = row[columnName];
                }
            }
            return hashtable;
        }

        public static Hashtable DataTableToHashtableByKeyValue(DataTable dt, string keyField, string valFiled)
        {
            Hashtable hashtable = new Hashtable();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string str = row[keyField].ToString();
                    hashtable[str] = row[valFiled];
                }
            }
            return hashtable;
        }


        public static string DataTableToXML(DataTable dt)
        {
            if (dt != null)
            {
                StringBuilder output = new StringBuilder();
                XmlWriter xmlWriter = XmlWriter.Create(output);
                new XmlSerializer(typeof(DataTable)).Serialize(xmlWriter, dt);
                xmlWriter.Close();
                return output.ToString();
            }
            return string.Empty;
        }

        private static bool fieldValuesAreEqual(object[] lastValues, DataRow currentRow, string[] fieldNames)
        {
            for (int i = 0; i < fieldNames.Length; i++)
            {
                if ((lastValues[i] == null) || !lastValues[i].Equals(currentRow[fieldNames[i]]))
                {
                    return false;
                }
            }
            return true;
        }

        public static DataTable GetNewDataTable(DataTable dt, string condition)
        {
            if (IsExistRows(dt))
            {
                if (condition.Trim() == "")
                {
                    return dt;
                }
                DataTable table2 = new DataTable();
                table2 = dt.Clone();
                DataRow[] rowArray = dt.Select(condition);
                for (int i = 0; i < rowArray.Length; i++)
                {
                    table2.ImportRow(rowArray[i]);
                }
                return table2;
            }
            return null;
        }

        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
            {
                return dt;
            }
            DataTable table2 = dt.Copy();
            table2.Clear();
            int num = (PageIndex - 1) * PageSize;
            int count = PageIndex * PageSize;
            if (num < dt.Rows.Count)
            {
                if (count > dt.Rows.Count)
                {
                    count = dt.Rows.Count;
                }
                for (int i = num; i <= (count - 1); i++)
                {
                    DataRow row = table2.NewRow();
                    DataRow row2 = dt.Rows[i];
                    foreach (DataColumn column in dt.Columns)
                    {
                        row[column.ColumnName] = row2[column.ColumnName];
                    }
                    table2.Rows.Add(row);
                }
            }
            return table2;
        }

        public static bool IsExistRows(DataTable dt)
        {
            return ((dt != null) && (dt.Rows.Count > 0));
        }

        public static DataTable SelectDistinct(DataTable SourceTable, string[] FieldNames)
        {
            if ((FieldNames == null) || (FieldNames.Length == 0))
            {
                throw new ArgumentNullException("FieldNames");
            }
            object[] lastValues = new object[FieldNames.Length];
            DataTable table = new DataTable();
            for (int i = 0; i < FieldNames.Length; i++)
            {
                string columnName = FieldNames[i];
                table.Columns.Add(columnName, SourceTable.Columns[columnName].DataType);
            }
            foreach (DataRow row in SourceTable.Select("", string.Join(",", FieldNames)))
            {
                if (!fieldValuesAreEqual(lastValues, row, FieldNames))
                {
                    table.Rows.Add(createRowClone(row, table.NewRow(), FieldNames));
                    setLastValues(lastValues, row, FieldNames);
                }
            }
            return table;
        }

        private static void setLastValues(object[] lastValues, DataRow sourceRow, string[] fieldNames)
        {
            for (int i = 0; i < fieldNames.Length; i++)
            {
                lastValues[i] = sourceRow[fieldNames[i]];
            }
        }

        public static DataTable SortedTable(DataTable dt, params string[] sorts)
        {
            if (dt.Rows.Count > 0)
            {
                string str = "";
                for (int i = 0; i < sorts.Length; i++)
                {
                    str = str + sorts[i] + ",";
                }
                char[] trimChars = new char[] { ',' };
                dt.DefaultView.Sort = str.TrimEnd(trimChars);
            }
            return dt;
        }




    }
}
