using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class DataLogerService
    {
        static Mutex write = new Mutex();

        /// <summary>
        /// 写日志，从View到controller
        /// Mcally 2015年8月16日21:16:42
        /// </summary>
        /// <param name="Data">参数</param>
        public static void writeURL(String Token, String Data)
        {
            String url = HttpContext.Current.Request.RawUrl;
            StringBuilder sb = new StringBuilder();
            sb.Append("URL:" + url + "\r\n");
            sb.Append("参数:" + Data);
            writeLog(Token, sb.ToString());
        }

        /// <summary>
        /// 写日志，记录SQL字符串
        /// Mcally 2015年8月16日21:16:37
        /// </summary>
        /// <param name="SQL">SQL字符串</param>
        public static void writeSQL(String SQL)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SQL字符串:" + SQL);
            writeLog(null, sb.ToString());
        }

        /// <summary>
        /// 写日志，记录从controller到View的数据（Hashtable）
        /// Mcally 2015年8月16日21:16:34
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="ITable"></param>
        public static void writeData(String Token, IList<Hashtable> ITable)
        {
            String url = HttpContext.Current.Request.RawUrl;
            StringBuilder sb = new StringBuilder();
            sb.Append("URL:" + url + "\r\n");
            sb.Append("输出数据:\r\n");
            foreach (Hashtable table in ITable)
            {
                foreach (DictionaryEntry de in table)
                {
                    sb.Append(de.Key + ":" + de.Value + "  ");
                }
                sb.Append("\r\n");
            }
            writeLog(Token, sb.ToString());
        }

        /// <summary>
        /// 写日志，记录从controller到View的数据（Jobect）
        /// Mcally 2015年8月16日21:16:34
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="ITable"></param>
        public static void writeData(String Token, String Jobect)
        {
            String url = HttpContext.Current.Request.RawUrl;
            StringBuilder sb = new StringBuilder();
            sb.Append("URL:" + url + "\r\n");
            sb.Append("输出数据:\r\n");
            sb.Append(Jobect);
            writeLog(Token, sb.ToString());
        }

        /// <summary>
        /// 写日志，记录从DB查询出来的DataTable
        /// Mcally 2015年8月16日21:16:29
        /// </summary>
        /// <param name="data">DataTable</param>
        public static void writeSQLDataTable(DataTable data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SQL输出数据:\r\n");
            if (data.Rows.Count > 0)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    DataRow row = data.Rows[i];
                    for (int x = 0; x < data.Columns.Count; x++)
                    {
                        sb.Append(data.Columns[x].ColumnName + ":" + row[data.Columns[x].ColumnName].ToString() + "  ");
                    }
                    sb.Append("\r\n");
                }
            }
            writeLog(null, sb.ToString());
        }

        /// <summary>
        /// 写日志，写入SQL执行结果
        /// Mcally 2015年8月16日23:58:15
        /// </summary>
        /// <param name="data"></param>
        public static void writeSQLData(String data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SQL查询结果:" + data);
            writeLog(null, sb.ToString());
        }

        /// <summary>
        /// 写日志
        /// Mcally 2015年8月16日21:16:19
        /// </summary>
        /// <param name="Log">日志</param>
        public static void writeLog(String Token, String Log)
        {
            //实现
            write.WaitOne();

            String strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //表示换行

            String strBody = DateTime.Now.ToString() + strCrLf + Log + strCrLf;

            //String strLogFolderPath = HttpContext.Current.Server.MapPath("/MES/logs");
            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Logs";

            DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String MFCUserID = null;
            if (String.IsNullOrWhiteSpace(Token))
                MFCUserID = "test";
            else
            {
                Token = Token.Replace(" ", "+");
               // String ken = Utils.EncryptHelper.DESDecrypt(Token);//解析Token
               // MFCUserID = ken.Split('-')[0];
            }

            String strLogName = DateTime.Now.ToString("yyyyMMdd_") + MFCUserID + ".txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(strBody);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }

            write.ReleaseMutex();
        }

        /// <summary>
        /// 功能:构造错误日志
        ///Mcally 2016年11月3日18:43:52
        /// </summary>
        /// <param name="sb">存储错误日志</param>
        /// <param name="exception">错误信息</param>
        private static void writeerrorlog(StringBuilder sb, Exception exception)
        {
            sb.Append("错误发生时间:" + DateTime.Now + "\r\n");
            sb.Append("当前登录用户:" + HttpContext.Current.User.Identity.Name + "\r\n");
            sb.Append("错误类型:" + exception.GetType().Name + "\r\n");
            sb.Append("错误信息:" + exception.Message + "\r\n");

            if (!string.IsNullOrEmpty(exception.StackTrace))
                sb.Append("StackTrace:\r\n" + exception.StackTrace + "\r\n");

            if (exception.InnerException != null)
            {
                sb.Append("InnerException:\r\n");
                writeerrorlog(sb, exception.InnerException);
            }
        }

        /// <summary>
        /// 功能:写底层错误信息
        /// Mcally 2016年11月3日18:43:52
        /// </summary>
        /// <param name="lastException"></param>
        public static void writeerrlog(Exception lastException)
        {
            if (lastException == null) return;
            StringBuilder sb = new StringBuilder();
            writeerrorlog(sb, lastException);
            writelog(sb.ToString());
        }

        static Mutex m_writemutex = new Mutex();

        /// <summary>
        /// 功能:写数据库操作日志，先用空方法代替
        /// Mcally 2016年11月3日18:43:52
        /// </summary>
        /// <param name="err">错误原因</param>
        public static void writelog(string err)
        {

            //错误日志大体这样实现
            //在服务的虚拟目录新键一个错误日志的文件夹
            //以月份建立错误日志文件 例如:2004-05.txt
            //里面存储数据错操作失败的所有原因
            //系统定期检查一下 通过自己检查 把错误日志通过Internet提交到开发方 以便开发商完善版本
            //最新的记录在文件的最下面。格式如下

            //实现
            m_writemutex.WaitOne();

            String strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString();

            String strBody = "" +
                "==================================================================================" + strCrLf +
                "发生时间：" + DateTime.Now.ToString() + strCrLf +
                 err + strCrLf;

            //String strLogFolderPath = System.Web.HttpContext.Current.Server.MapPath("/MES/ErrLog");
            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "ErrLog";
            System.IO.DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String strLogName = DateTime.Now.ToString("yyyy_MM_dd") + ".txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(strBody);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }

            m_writemutex.ReleaseMutex();
        }

        /// <summary>
        /// 功能:写数据库操作日志，先用空方法代替
        /// Mcally 2016年11月3日18:43:52
        /// </summary>
        /// <param name="err">错误原因</param>
        public static void writeLanguageLog(string err)
        {

            //错误日志大体这样实现
            //在服务的虚拟目录新键一个错误日志的文件夹
            //以月份建立错误日志文件 例如:2004-05.txt
            //里面存储数据错操作失败的所有原因
            //系统定期检查一下 通过自己检查 把错误日志通过Internet提交到开发方 以便开发商完善版本
            //最新的记录在文件的最下面。格式如下

            //实现
            m_writemutex.WaitOne();

            String strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString();

            String strBody = "" +
                "==================================================================================" + strCrLf +
                "发生时间：" + DateTime.Now.ToString() + strCrLf +
                 err + strCrLf;

            //String strLogFolderPath = System.Web.HttpContext.Current.Server.MapPath("/MES/ErrLog");
            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Language";
            System.IO.DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String strLogName = DateTime.Now.ToString("yyyy_MM_dd") + ".txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(strBody);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }

            m_writemutex.ReleaseMutex();
        }

        /// <summary>
        /// Mcally专用测试写日志方法
        /// Mcally 2015年12月18日16:36:25
        /// </summary>
        /// <param name="Log"></param>
        public static void McallywriteLog(string Log)
        {
            string url = HttpContext.Current.Request.RawUrl;
            StringBuilder sb = new StringBuilder();
            sb.Append("URL:" + url + "\r\n");
            sb.Append("信息:" + Log);
            //实现
            write.WaitOne();

            string strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //表示换行

            string strBody = DateTime.Now.ToString() + strCrLf + Log + strCrLf;

            //string strLogFolderPath = HttpContext.Current.Server.MapPath("/MES/Mcallylogs");
            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Mcallylogs";
            System.IO.DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String strLogName = DateTime.Now.ToString("yyyyMMdd") + ".txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(strBody);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }

            write.ReleaseMutex();
        }

        /// <summary>
        /// Mcally专用测试写日志方法
        /// Mcally 2015年12月18日16:36:25
        /// </summary>
        /// <param name="Log"></param>
        public static void Inf00010ListWriteLog(string Log)
        {
            string url = HttpContext.Current.Request.RawUrl;
            StringBuilder sb = new StringBuilder();
            sb.Append("URL:" + url + "\r\n");
            sb.Append("信息:" + Log);
            //实现
            write.WaitOne();

            string strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //表示换行

            string strBody = DateTime.Now.ToString() + strCrLf + Log + strCrLf;

            //string strLogFolderPath = HttpContext.Current.Server.MapPath("/MES/Mcallylogs");
            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Inf00010ListLogs";
            System.IO.DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String strLogName = DateTime.Now.ToString("yyyyMMdd") + ".txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(strBody);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }

            write.ReleaseMutex();
        }


        /// <summary>
        /// 生成导入错误的txt文件报告
        /// Mcally 2017年10月17日11:12:56
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Log"></param>
        public static void ImportWriteErrLog(DateTime StartTime, DateTime EndTime, string Hour, int total, int success, int nosuccess, string Name, string Log)
        {
            StringBuilder sb = new StringBuilder();
            string strBody = "==============================================================";
            sb.Append(strBody + "\r\n");
            sb.Append("导入开始时间：" + StartTime + "\r\n");
            sb.Append("导入结果：总数：" + total + "笔,成功笔数：" + success + "笔,失败笔数：" + nosuccess + "笔。共耗时：" + Hour + "\r\n");
            sb.Append(Log);
            sb.Append("导入结束时间：" + EndTime + "\r\n");
            sb.Append(strBody + "\r\n");

            string strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //表示换行

            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "ImportWriteErrLog";

            DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String strLogName = Name + ".txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }
            else
            {
                File.Delete(strLogPath);
                var o = File.Create(strLogPath);
                o.Close();
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }
        }

        /// <summary>
        /// 制品制程专属的导入错误报告生成函数
        /// Mcally 2017年10月26日15:45:49
        /// </summary>
        /// <param name="Bookmark">页签明</param>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="Hour">耗时</param>
        /// <param name="total">总数</param>
        /// <param name="success">成功数</param>
        /// <param name="nosuccess">失败数</param>
        /// <param name="Log">日志</param>
        /// <param name="New">是否需要新的Log</param>
        public static void Sfc01ImportErrLog(string Bookmark, DateTime StartTime, DateTime EndTime, string Hour, int total, int success, int nosuccess, string Log, bool New)
        {
            StringBuilder sb = new StringBuilder();
            string strBody = "==============================================================";
            sb.Append(strBody + "\r\n");
            sb.Append("页签：" + Bookmark + "\r\n");
            sb.Append("导入开始时间：" + StartTime + "\r\n");
            sb.Append("导入结果：总数：" + total + "笔,成功笔数：" + success + "笔,失败笔数：" + nosuccess + "笔。共耗时：" + Hour + "\r\n");
            sb.Append(Log);
            sb.Append("导入结束时间：" + EndTime + "\r\n");
            sb.Append(strBody + "\r\n");
            sb.Append("\r\n");

            string strCrLf = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //表示换行

            string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "ImportWriteErrLog";

            DirectoryInfo di = new DirectoryInfo(strLogFolderPath);

            if (!di.Exists)//创建目录
                di.Create();

            String strLogName = "Sfc00001ImportErrorLog.txt";

            string strLogPath = Path.Combine(strLogFolderPath, strLogName);
            if (!File.Exists(strLogPath))
            {
                var o = File.Create(strLogPath);
                o.Close();
            }
            else
            {
                if (New)//如果需要从此生成文件的话
                {
                    File.Delete(strLogPath);
                    var o = File.Create(strLogPath);
                    o.Close();
                }
            }

            try
            {
                FileInfo fi = new FileInfo(strLogPath);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception em)
            {
                Console.WriteLine(em.Message.ToString());
            }
        }
    }
}
