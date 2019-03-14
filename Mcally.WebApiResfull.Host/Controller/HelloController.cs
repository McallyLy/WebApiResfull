using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Controller
{
    public class HelloController : BaseApiController
    {
        [HttpGet]
        public object Test()
        {
            return "Hello World";

        }

        [HttpGet]
        public object Test1()
        {

            return new { msg = "Hello World", list = new List<string>() { "one", "two", "three" } };
        }


        [HttpGet]
        public object Test2()
        {

            return new { msg = "Hello World", Code = "200" };
        }


        [HttpGet]
        public object Test3()
        {
            return BaseJson.toJson(new { msg = "Hello World", Code = 400 });
        }
    }

}
