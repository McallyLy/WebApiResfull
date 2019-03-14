using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Mcally.WebApiResfull.Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            //config.MapHttpAttributeRoutes();
            //跨域配置
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{Action}/{id}",
                 defaults: new { Action = RouteParameter.Optional, id = RouteParameter.Optional });
            // 使api返回为json 有弊端就是后端里有数值全部转换为字符串
             GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();


        }
    }
}
