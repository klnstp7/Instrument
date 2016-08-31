using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class InnerCheckModel
    {
        public InnerCheckModel()
        {}
        #region Model
        /// <summary>
        /// 内部核查标识
        /// </summary>
        public int InnerCheckId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 检查日期
        /// </summary>
        public DateTime? CheckDate { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime? PeriodDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string Leader { get; set; }
        /// <summary>
        /// 结论
        /// </summary>
        public int Result { get; set; }
        
      #endregion Model
    }
}
