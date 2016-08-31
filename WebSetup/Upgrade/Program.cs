using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using Upgrade.Common;
using System.IO;

namespace Upgrade
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            StartInit();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// 程序初始化
        /// </summary>
        static void StartInit()
        {
            //log4net配置
            string log4netPath = string.Format("{0}{1}", Application.StartupPath, "\\log4net.xml");
            Stream logStream = new FileStream(log4netPath, FileMode.Open);
            log4net.Config.XmlConfigurator.Configure(logStream);
            logStream.Close();

            //创建目录
            string scriptPath = string.Format("{0}{1}", Application.StartupPath, System.Configuration.ConfigurationManager.AppSettings["ScriptsPath"]);
            string rollBackScriptPath = string.Format("{0}{1}", Application.StartupPath, System.Configuration.ConfigurationManager.AppSettings["RollBackScriptPath"]);
            string updFilesPath = string.Format("{0}{1}", Application.StartupPath, System.Configuration.ConfigurationManager.AppSettings["UpdFilesPath"]);
            FileHelper.CreateDirectory(scriptPath);
            FileHelper.CreateDirectory(rollBackScriptPath);
            FileHelper.CreateDirectory(updFilesPath);
        }
  
    }
}
