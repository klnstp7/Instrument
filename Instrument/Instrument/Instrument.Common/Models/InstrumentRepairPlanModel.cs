using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class InstrumentRepairPlanModel
    {
        #region Model
        /// <summary>
        /// 计划标识
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 维修人员
        /// </summary>
        public string Repairer { get; set; }
        /// <summary>
        /// 维修公司
        /// </summary>
        public string RepairCompany { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 维修开始时间
        /// </summary>
        public DateTime DueStartDate { get; set; }
        /// <summary>
        /// 维修结束时间
        /// </summary>
        public DateTime DueEndDate { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string Leader { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 报告编号
        /// </summary>
        public string ReportCode { get; set; }
        /// <summary>
        /// 维修金额
        /// </summary>
        public decimal? RepairMoney { get; set; }
        /// <summary>
        /// 故障原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 保修期限
        /// </summary>
        public string TermService { get; set; }
        
        #endregion Model
    }
}
