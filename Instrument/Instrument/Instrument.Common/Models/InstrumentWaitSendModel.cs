using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class InstrumentWaitSendModel
    {
        public InstrumentWaitSendModel()
        { }
        #region Model
        /// <summary>
        /// 自增标识
        /// </summary>
        public int AutoId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        #endregion Model
    }
}
