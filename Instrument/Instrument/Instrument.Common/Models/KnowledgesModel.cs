using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Instrument.Common.Models
{
    public class KnowledgesModel
    {
        #region Model
        /// <summary>
        /// 标识
        /// </summary>
        public int KnowledgeId { get; set; }
        /// <summary>
        /// 内容摘要
        /// </summary>
        public string Abstract { get; set; }
        /// <summary>
        /// 0：未分享 2：已分享 
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 内容PDF
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatDate { get; set; }
        /// <summary>
        /// 0：基础知识、1：专业理论:2：规程规范、3：技术说明书
        /// </summary>
        public int KType { get; set; }
        /// <summary>
        /// 内容标题
        /// </summary>
        public string Title { get; set; }
        #endregion Model
        /// <summary>
        /// 业务附件列表
        /// </summary>
        public IList<Hashtable> businessAttachList { get; set; }
        /// <summary>
        /// PDF文件字节流
        /// </summary>
        public byte[] swfFileByte { get; set; }
    }
}
