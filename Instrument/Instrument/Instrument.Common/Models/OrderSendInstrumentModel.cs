using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class OrderSendInstrumentModel
    {
        public OrderSendInstrumentModel()
        { }
        #region Model
        /// <summary>
        /// 自增标识
        /// </summary>
        public int AutoId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 送检单标识
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string InstrumentName { get; set; }
        /// <summary>
        /// 仪器型号
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 证书编号
        /// </summary>
        public string CertificationNumber { get; set; }
        /// <summary>
        /// 出厂编号
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 管理编号
        /// </summary>
        public string ManageNo { get; set; }
        /// <summary>
        /// 检验日期
        /// </summary>
        public DateTime ? InspectDate { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime ? DueEndDate { get; set; }
        /// <summary>
        /// 检验机构
        /// </summary>
        public string InspectOrg { get; set; }
        /// <summary>
        /// 0：未下载；1：已下载
        /// </summary>
        public bool IsDownloadCert { get; set; }
        /// <summary>
        /// 0:未完工；1：已完工
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// 0：未完工；1：已完工
        /// </summary>
        public bool IsCompleteCert { get; set; }
        /// <summary>
        /// 是否送检 0：未送；1：已送
        /// </summary>
        public bool IsSend { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 报价
        /// </summary>
        public decimal PerPrice { get; set; }

        
        #endregion Model
    }
}
