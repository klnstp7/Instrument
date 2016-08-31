using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Diagnostics;


namespace Instrument.Wcf.WindowsService
{
    [RunInstaller(true)]
    public partial class GRGTInstaller : System.Configuration.Install.Installer
    {
        public GRGTInstaller()
        {
            InitializeComponent();

            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;

            //轻量级流程
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //轻量级流程服务
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "GRGT.WCF-Instrument.WinService";
            serviceInstaller.DisplayName = "GRGT WCF-Instrument服务";
            serviceInstaller.Description = "GRGT WCF-Instrument服务。";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

        // 实现安装后自动启动
        public override void Commit(IDictionary savedState)
        {
            ServiceController controller = new ServiceController();

            controller = new ServiceController("GRGT.WCF-Instrument.WinService");
            controller.Start();
            EventLog.WriteEntry("GRGT服务安装程序", "正常启动WCF-Instrument服务", EventLogEntryType.Information);

            base.Commit(savedState);
        }
    }
}
