using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Instrument.Common.Models
{
    public class InstrumentCertificationModel
    {
        public InstrumentCertificationModel()
        { }
        #region Model
        /// <summary>
        /// 自增标识
        /// </summary>
        public int LogId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 证书文件ID
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// 证书编号
        /// </summary>

        //[Required]
        [StringLength(40, MinimumLength = 5)]
        public string CertificationCode { get; set; }
        /// <summary>
        /// 校准日期
        /// </summary>
        //[Required]
        [DataType(DataType.Date)]
        public DateTime? CheckDate { get; set; }
        /// <summary>
        /// 到期日期
        /// </summary>
        //[Required]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 送出日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? SendInstrumentDate { get; set; }
        /// <summary>
        /// 返还日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? ReturnInstrumentDate { get; set; }
        /// <summary>
        /// 证书取回日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? GetCertificateDate { get; set; }
        /// <summary>
        /// 证书确认日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? CertificateConfirmDate { get; set; }
        /// <summary>
        /// 送检单号
        /// </summary>
        [StringLength(50)]
        public string OrderNo { get; set; }
        /// <summary>
        /// 计量机构
        /// </summary>
        [StringLength(200)]
        public string MeasureOrg { get; set; }
        /// <summary>
        /// 检测结果
        /// </summary>
        [StringLength(500)]
        public string CheckResult { get; set; }
        /// <summary>
        /// 误差
        /// </summary>
        [StringLength(500)]
        public string ErrorValue { get; set; }
        /// <summary>
        /// -1：未检，0：周检中，1：完成周检
        /// </summary>
        public int RecordState { get; set; }
        /// <summary>
        /// -1：未检，0：周检中，1：完成周检
        /// </summary>
        public String RecordStateName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        [StringLength(40)]
        public string ItemCode { get; set; }
        /// <summary>
        /// 是否当前使用证书
        /// </summary>
        public bool IsUseding { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        public decimal CertMoney { get; set; }
        #endregion Model
    }
}
