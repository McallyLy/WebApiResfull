using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ImportFileBussinessService
    {
        public static object plantImport(string filename)
        {
       
            if (filename.EndsWith(".xls") || filename.EndsWith(".xlsx"))
            {
                IWorkbook wk = null;
              
                using (FileStream fs = File.OpenRead(@filename))   //打开.xls文件
                {
                    if (filename.EndsWith(".xls"))
                        wk = new HSSFWorkbook(fs); //把xls文件中的数据写入wk中
                    else if (filename.EndsWith(".xlsx"))
                        wk = new XSSFWorkbook(fs);//把xlsx文件中的数据写入wk中

                    ISheet sheet = wk.GetSheetAt(0);   //读取第一表数据
                    for (int j = 2; j <= sheet.LastRowNum; j++)  //LastRowNum 是当前表的总行数
                    {
                        IRow row = sheet.GetRow(j);  //读取当前行数据
                        if (row.GetCell(0) == null) //如果当前行的第一列没有数据，停止读取
                            break;
                        //如果当前行的第一列没有数据，停止读取，这是判断当row.GetCell不能null然后实际是空字符
                        if (string.IsNullOrWhiteSpace(row.GetCell(0).ToString()))
                            break;
                        
                        ICell cell = null;
                        if (row != null)
                        {
                            cell = row.GetCell(0);//序号
                            string s = cell.ToString();

                        }
                      
                    }

                }
            }

          
            return new { status = 200};
        }
    }
}
