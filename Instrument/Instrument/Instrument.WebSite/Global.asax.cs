using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Reflection;
using Spring.Context.Support;
using Spring.Context;
using ToolsLib.Utility;
using GRGTCommonUtils;

namespace Instrument.WebSite
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Global.asax");

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {  
            filters.Add(new ExceptionFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }

        public class ExceptionFilterAttribute : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                //base.OnException(filterContext);
                log.ErrorFormat("（{0}|{1}）{2}{3}", LoginHelper.LoginUser.UserName, LoginHelper.LoginUser.JobNo, System.Environment.NewLine, filterContext.Exception);

                filterContext.HttpContext.Session["ErrorMessage"] = filterContext.Exception.Message;
                HttpContext.Current.Response.Redirect("~/Login/Error", true);
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Login", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //JS文件版本号
            Application["JSVersion"] = ToolsLib.Utility.StrUtils.GetRandomNumb(6);

            //启动Spring
            IApplicationContext ctx = ContextRegistry.GetContext();

            //Log4配置文件加载
            Assembly assembly = Assembly.Load("ToolsLib, Version=2.0.0.5, Culture=neutral, PublicKeyToken=3a4d8e12b8a6b6e6");
            Stream logStream = assembly.GetManifestResourceStream("ToolsLib.log4net.config");
            log4net.Config.XmlConfigurator.Configure(logStream);
            //仪器超期后台服务
            log.Info(string.Format("\r\n============启动更新仪器超期服务============\r\n"));
            UpdateForOverDueService up = new UpdateForOverDueService();
            up.updaService = Instrument.Business.ServiceProvider.InstrumentService.UpdateForOverDue;
            up.StartUpdateForOverDue();

            //在跟目录下创建tempFile临时文件夹
            Directory.CreateDirectory(WebUtils.Resulve("/") + "tempFile");
            //临时文件自动清理服务
            ToolsLib.Utility.CommonUtils.StartFileClear();
        }

        protected void Application_End()
        {
            
        }


        protected void Appliction_Error(Object sender, EventArgs e)
        {

        }
    }
}