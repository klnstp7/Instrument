using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    public class InstrumentVersionLogModel
    {
        /// <summary>
        /// key
        /// </summary>
        public int VersonLogId { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
