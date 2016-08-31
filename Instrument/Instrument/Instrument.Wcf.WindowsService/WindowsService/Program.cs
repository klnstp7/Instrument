using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.IO;
using Spring.Context;
using System.Diagnostics;
using Spring.Context.Support;

namespace GRGT.WCF_Instrument.WinService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //Log4配置文件加载
            Assembly assembly = Assembly.Load("ToolsLib, Version=2.0.0.5, Culture=neutral, PublicKeyToken=3a4d8e12b8a6b6e6");
            Stream logStream = assembly.GetManifestResourceStream("ToolsLib.log4net.config");
            log4net.Config.XmlConfigurator.Configure(logStream);

            try
            {
                //启动Spring
                IApplicationContext ctx = ContextRegistry.GetContext();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("GRGT-WCF-Instrument", e.Message, EventLogEntryType.Error);
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new InstrumentService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
