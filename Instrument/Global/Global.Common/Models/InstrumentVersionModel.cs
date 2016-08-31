using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    public class InstrumentVersionModel
    {
        public InstrumentVersionModel()
		{}
		#region Model
		/// <summary>
		/// 标识
		/// </summary>
		public int VersonId { get; set; }
		/// <summary>
		/// 版本编号
		/// </summary>
		public string VersonCode { get; set; }
		/// <summary>
		/// 版本类别：SqlServer、Sqlite、MySql、Access
		/// </summary>
		public string VersonType { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		public bool IsUse { get; set; }
		/// <summary>
		/// 升级版本号
		/// </summary>
		public string NextVersonCode { get; set; }
		/// <summary>
		/// 升级地址
		/// </summary>
		public string DownloadUrl { get; set; }
		/// <summary>
		/// 创建日期
		/// </summary>
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// SVN版本号
		/// </summary>
		public string SVNLable { get; set; }
		#endregion Model
    }
}
