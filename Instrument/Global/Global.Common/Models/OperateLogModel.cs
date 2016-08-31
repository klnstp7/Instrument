using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    public class OperateLogModel
    {
        public OperateLogModel()
		{}
		#region Model
		/// <summary>
		/// 日志标识
		/// </summary>
		public int LogId { get; set; }
		/// <summary>
		/// 操作类别，0：其它，1：新增，2：修改，3：删除, 4:登录
		/// </summary>
		public int OperateType { get; set; }
		/// <summary>
		/// 操作人
		/// </summary>
		public string Operator { get; set; }
		/// <summary>
		/// 操作时间
		/// </summary>
		public DateTime OperateDate { get; set; }
		/// <summary>
		/// 操作者IP
		/// </summary>
		public string OperateIP { get; set; }
		/// <summary>
		/// 操作内容描述
		/// </summary>
		public string OperateContent { get; set; }
		/// <summary>
		/// 操作对象主键
		/// </summary>
		public string TargetPK { get; set; }
		/// <summary>
		/// 操作对象类别
		/// </summary>
		public int TargetType { get; set; }
		#endregion Model
    }
}
