using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using ToolsLib.Utility;

namespace GRGTCommonUtils
{
    public class UpdateForOverDueService
    {
         /// <summary>
        /// 日志
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UpdateForOverDueService));

        private System.Timers.Timer updateForOverDueTimer;
    
        public delegate void DelegateUpDateService();
        public DelegateUpDateService updaService = null;
        /// <summary>
        /// 停止
        /// </summary>
        public void StopUpdateForOverDue()
        {
            updateForOverDueTimer.Stop();
            log.Info("\r\n-----------停止仪器超期服务-----------执行时间：" + DateTime.Now);
        }

        public UpdateForOverDueService()
        {
            updateForOverDueTimer = new System.Timers.Timer();
            updateForOverDueTimer.Elapsed += new ElapsedEventHandler(UpdateForOverDue);
            updateForOverDueTimer.AutoReset = true;
        }


        public void StartUpdateForOverDue()
        {
            log.Info(string.Format("\r\n============开始更新仪器超期服务============\r\n"));
            updateForOverDueTimer.Interval = 6 * 60 * 60 * 1000;
            updateForOverDueTimer.Start();
            updaService();
        }


        public void UpdateForOverDue(object sender, ElapsedEventArgs e)
        {
            updaService();
            log.Info(string.Format("\r\n============已完成更新仪器超期============\r\n"));
        }
    }
}
