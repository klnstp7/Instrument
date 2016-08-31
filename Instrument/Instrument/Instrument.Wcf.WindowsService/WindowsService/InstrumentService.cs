using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;

namespace GRGT.WCF_Instrument.WinService
{
    public partial class InstrumentService : ServiceBase
    {
        ///// <summary>
        ///// 日志信息
        ///// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InstrumentService));
        private ServiceHost host = null;

        public InstrumentService()
        {
            InitializeComponent();
            host = new ServiceHost(typeof(Instrument.Business.WCF.InstrumentWCFServices));
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            host.Open();
            log.Info("启动WCF-Instrument服务。");
        }

        protected override void OnStop()
        {
            host.Close();
            log.Info("停止WCF-Instrument服务。");
        }
    }
}
