using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class DocumentModel
    {
        public DocumentModel()
		{}
        #region Model
        /// <summary>
        /// 体系文件标识
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// 文件标识
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public int DocCategory { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        #endregion Model
    }
}
