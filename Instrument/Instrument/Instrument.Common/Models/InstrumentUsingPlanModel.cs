using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    /// <summary>
    /// 仪器排期调度
    /// </summary>
    public class InstrumentUsingPlanModel
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
        /// 订单/项目编号
        /// </summary>
        public string Order_ProjectCode { get; set; }

        /// <summary>
        /// 工程师
        /// </summary>
        public string EngineerName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        #endregion Model

    }
}
