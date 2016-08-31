using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public  class InstrumentFlowModel
    {
        public InstrumentFlowModel()
		{}
		#region Model
		/// <summary>
		/// 流转标识
		/// </summary>
		public int FlowId { get; set; }
		/// <summary>
		/// 仪器主键
		/// </summary>
		public int InstrumentId { get; set; }
		/// <summary>
		/// 1：到达 2：前往
		/// </summary>
		public int Flow_Type { get; set; }
		/// <summary>
		/// 参数配置
		/// </summary>
		public string Place { get; set; }
		/// <summary>
		/// 操作人
		/// </summary>
		public string Creator { get; set; }
		/// <summary>
		/// 操作时间
		/// </summary>
		public DateTime CreateDate { get; set; }
        /// <summary>
        /// 事由
        /// </summary>
        public string Reason { get; set; }
		#endregion Model
    }
}
