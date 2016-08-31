using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AssetCheckPlanModel
    {
        public AssetCheckPlanModel()
        { }
        #region Model
        /// <summary>
        /// 计划标识
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 参数配置
        /// </summary>
        public int PlanType { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 计划开始
        /// </summary>
        public DateTime ? StartDate { get; set; }
        /// <summary>
        /// 计划结束
        /// </summary>
        public DateTime ? EndDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion Model


    }
}
