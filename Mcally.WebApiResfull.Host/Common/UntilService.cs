using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
