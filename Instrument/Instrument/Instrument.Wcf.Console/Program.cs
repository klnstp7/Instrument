using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.ServiceModel;
using Spring.Context.Support;
using GRGTCommonUtils;

namespace InstrumentConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //启动Spring
            ContextRegistry.GetContext();

            //Log4配置文件加载
            Assembly assembly = Assembly.Load("ToolsLib, Version=2.0.0.5, Culture=neutral, PublicKeyToken=3a4d8e12b8a6b6e6");
            Stream logStream = assembly.GetManifestResourceStream("ToolsLib.log4net.config");
            log4net.Config.XmlConfigurator.Configure(logStream);

            ServiceHost sh = new ServiceHost(typeof(Instrument.Business.WCF.InstrumentWCFServices));
            sh.Open();
            System.Console.WriteLine("仪器系统接口，启动成功。");

            //WSProvider.InstrumentProvider.AppLogin("GDJL01001", "123456", "");



            System.Console.ReadLine();
            sh.Close();
        }
    }
}
