using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Global.Common.Models
{
    public class InstrumentCheckLogModel
    {
        #region Model

        /// <summary>
        /// 记录标识
        /// </summary>
        [JsonIgnore]
        public int LogId { get; set; }

        /// <summary>
        /// 仪器标识
        /// </summary>
        [JsonIgnore]
        public int InstrumentId { get; set; }
        ///// <summary>
        ///// 访问路径（全路径）
        ///// </summary>
        //public string FileVirtualPath { get; set; }
        /// <summary>
        /// 证书编号
        /// </summary>
        public string CertificationCode { get; set; }      
  
        /// <summary>
        /// 到期日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 周检日期
        /// </summary>
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// 送出日期
        /// </summary>
        public DateTime? SendInstrumentDate { get; set; }

        /// <summary>
        /// 返回日期
        /// </summary>
        public DateTime? ReturnInstrumentDate { get; set; }

        /// <summary>
        /// 证书取回日期
        /// </summary>
        public DateTime? GetCertificateDate { get; set; }

        /// <summary>
        /// 证书确认日期
        /// </summary>
        public DateTime? CertificateConfirmDate { get; set; }

        /// <summary>
        /// 送检单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 计量机构
        /// </summary>
        public string MeasureOrg { get; set; }

        /// <summary>
        /// 记录状态
        /// </summary>
        public int RecordState { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 是否当前使用证书
        /// </summary>
        public bool IsUseding { get; set; }

        

        #endregion Model
    }
}
