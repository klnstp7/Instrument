using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class BusinessAttachmentModel
    {
        public BusinessAttachmentModel()
        { }
        #region Model
		/// <summary>
		/// 自增标识
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// 0:联络单
		/// </summary>
		public int BusinessType { get; set; }
		/// <summary>
		/// 业务主键标识
		/// </summary>
		public int BusinessKeyId { get; set; }
		/// <summary>
		/// 文件标识
		/// </summary>
		public int FileId { get; set; }
		/// <summary>
		/// 文件名称
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// 上传人姓名
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 上传时间
		/// </summary>
		public DateTime CreateDate { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }
		#endregion Model

    }
}
