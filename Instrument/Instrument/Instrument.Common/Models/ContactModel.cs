using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    /// <summary>
    /// 联络单
    /// </summary>
    public class ContactModel
    {
        public ContactModel()
        { }
        #region Model
        /// <summary>
        /// 联络单标识
        /// </summary>
        public int ContactId { get; set; }
        /// <summary>
        /// 客户名称(冗余)
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 事项分类
        /// </summary>
        public int CaseType { get; set; }
        /// <summary>
        /// 事项分类（冗余）只为接口传输使用  数据库不存在该字段
        /// </summary>
        public string CaseTypeStr { get; set; }
        /// <summary>
        /// 事项摘要
        /// </summary>
        public string Abstract { get; set; }
        /// <summary>
        /// 1：草稿 2：已提交 3：已反馈 4：已解决
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 沟通描述
        /// </summary>
        public string ContactContent { get; set; }
        /// <summary>
        /// 反馈信息
        /// </summary>
        public string FeedbackContent { get; set; }
        /// <summary>
        /// 反馈日期
        /// </summary>
        public DateTime ? FeedbackDate { get; set; }

        public int CreatId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatDate { get; set; }
        /// <summary>
        /// 项目编号
        /// </summary>
        public string ItemCode { get; set; }
        
        #endregion Model

    }
}
