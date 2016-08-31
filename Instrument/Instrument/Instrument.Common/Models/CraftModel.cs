using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class CraftModel
    {
        public CraftModel()
		{}
		#region Model
		/// <summary>
		/// 工艺过程标识
		/// </summary>
		public int CraftId { get; set; }
		/// <summary>
		/// 工艺过程代码
		/// </summary>
		public string CraftCode { get; set; }
		/// <summary>
		/// 工艺过程名称
		/// </summary>
		public string CraftName { get; set; }
		/// <summary>
		/// 0：未检；1：内检；2：外检；3：免检
		/// </summary>
		public string InstrumentNo { get; set; }
		/// <summary>
		/// 0：检定；2：校准
		/// </summary>
		public string UsePlace { get; set; }
		/// <summary>
		/// 工艺要求
		/// </summary>
		public string Required { get; set; }
		/// <summary>
		/// 测量范围
		/// </summary>
		public string MeasureRange { get; set; }
		/// <summary>
		/// 精准度
		/// </summary>
		public string Precise { get; set; }
		/// <summary>
		/// 分辨率
		/// </summary>
		public string DPI { get; set; }
		/// <summary>
		/// 工艺允许误差
		/// </summary>
		public string PermissiblError { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }
        /// <summary>
        /// 管理部门
        /// </summary>
        public string ManageDepart { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		public string CreateUser { get; set; }
		#endregion Model
    }
}
