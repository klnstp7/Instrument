using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{

    public class Sys_BusinessLogModel
    {
        public Sys_BusinessLogModel()
        { }
        #region Model
        /// <summary>
        /// 日志标识
        /// </summary>
        public int LogId { get; set; }
        /// <summary>
        /// 外键标识
        /// </summary>
        public int FKValue { get; set; }
        /// <summary>
        /// 外键类别：0：未知，1
        /// </summary>
        public int FKType { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 创建人标识
        /// </summary>
        public int Creator { get; set; }
        /// <summary>
        /// 日志描述
        /// </summary>
        public string LogConent { get; set; }
        /// <summary>
        /// 文件标识
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion Model
    }
}
