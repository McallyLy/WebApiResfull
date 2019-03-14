using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Controller
{
  public  class ImportFileController :BaseApiController
    {

        [HttpPost]
        public object Import() {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                HttpPostedFile file = HttpContext.Current.Request.Files["file"];
                if (file == null)
                    return new { status = "440", msg = "文件为空！" };
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Upload/";
                string filename = Path.GetFileName(file.FileName);
                file.SaveAs(Path.Combine(filePath, filename));
              
                return ImportFileBussinessService.plantImport(Path.Combine(filePath, filename));
            }
            catch (Exception ex)
            {
             
                return new { status = "440", msg = "异常已捕获！请联系研发人员查询问题！" };
            }

        }


    }
}
