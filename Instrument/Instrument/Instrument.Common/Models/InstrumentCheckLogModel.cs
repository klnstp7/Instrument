using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class InstrumentCheckLogModel
    {
        #region Model
        /// <summary>
        /// 盘点标识
        /// </summary>
        public int CheckLogId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 盘点结果
        /// </summary>
        public string CheckResult { get; set; }
        /// <summary>
        /// 盘点人
        /// </summary>
        public string CheckUser { get; set; }
        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime CheckDate { get; set; }
      
        #endregion Model

    }
}
