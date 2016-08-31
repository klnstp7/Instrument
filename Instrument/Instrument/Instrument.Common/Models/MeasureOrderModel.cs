using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class MeasureOrderModel
    {
        #region Model
        /// <summary>
        /// 委托单标识
        /// </summary>
        public int OrderId { get; set; }
        ///// <summary>
        ///// 报价单标识
        ///// </summary>
        //public int QuotationId { get; set; }
        /// <summary>
        /// 委托单号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 送检人
        /// </summary>
        public string SendUser { get; set; }
        /// <summary>
        /// 送检时间
        /// </summary>
        public DateTime? SendDate { get; set; }
        /// <summary>
        /// 交通费
        /// </summary>
        public decimal CarFare { get; set; }
        /// <summary>
        /// 加急费
        /// </summary>
        public decimal EmergencyFee { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public decimal OtherFee { get; set; }
        /// <summary>
        /// 报价
        /// </summary>
        public decimal ServiceFee { get; set; }
        /// <summary>
        /// 参考价
        /// </summary>
        public decimal ReferPrice { get; set; }
        /// <summary>
        /// 器具数量
        /// </summary>
        public int InstrumentCount { get; set; }
        /// <summary>
        /// 证书接收单位
        /// </summary>
        public string Authorites { get; set; }
        /// <summary>
        /// 证书接收单位地址
        /// </summary>
        public string AuthoritesAddress { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 客户名称(冗余)
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 业务员标识
        /// </summary>
        public int SaleId { get; set; }
        /// <summary>
        /// 业务员名称(冗余)
        /// </summary>
        public string SaleName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 归属部门
        /// </summary>
        public string BelongDepart { get; set; }
        /// <summary>
        /// 归属部门名称(冗余)
        /// </summary>
        public string BelongDepartName { get; set; }
        /// <summary>
        /// 是否完工
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// 是否证书完工
        /// </summary>
        public bool IsCompleteCert { get; set; }
        /// <summary>
        /// 是否费用核准
        /// </summary>
        public bool IsConfirm { get; set; }
        /// <summary>
        /// 是否开具发票
        /// </summary>
        public bool IsInvoice { get; set; }
        /// <summary>
        /// 款项是否到账
        /// </summary>
        public bool IsPay { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int Creator { get; set; }
        /// <summary>
        /// 创建人名
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 行业类型
        /// </summary>
        public int IndustryType { get; set; }
        /// <summary>
        /// 0：正常，1：申请关联
        /// </summary>
        public int RecordState { get; set; }
        /// <summary>
        /// 完工时间
        /// </summary>
        public DateTime CompleteDate { get; set; }
        /// <summary>
        /// 客户联系人
        /// </summary>
        public string Contactor { get; set; }
        /// <summary>
        /// 客户联系电话(手机)
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 固话
        /// </summary>
        public string TelePhone { get; set; }
        /// <summary>
        /// 客户传真
        /// </summary>
        public string Fax { get; set; }
        #endregion Model
    }
}
