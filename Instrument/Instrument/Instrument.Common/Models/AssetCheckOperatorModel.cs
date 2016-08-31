using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AssetCheckOperatorModel
    {
        public AssetCheckOperatorModel()
        { }
        #region Model
        /// <summary>
        /// 自增标识
        /// </summary>
        public int AutoId { get; set; }
        /// <summary>
        /// 盘点计划标识
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatDate { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string UserName { get; set; }
        
        #endregion Model
    }
}
