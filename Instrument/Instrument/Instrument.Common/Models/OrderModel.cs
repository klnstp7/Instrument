using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class OrderModel
    {
        public OrderModel()
		{}
        #region Model
        /// <summary>
        /// 送检单标识
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 送检单号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 报价单号
        /// </summary>
        public string QuotationNumber { get; set; }
        /// <summary>
        /// 送检人Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 送检人
        /// </summary>
        public string SendUser { get; set; }
        /// <summary>
        /// 送检日期
        /// </summary>
        public DateTime ? SendDate { get; set; }
        /// <summary>
        /// 仪器数量
        /// </summary>
        public int InstrumentCount { get; set; }
        /// <summary>
        /// 0：未下载；1：部分下载；2：全部下载
        /// </summary>
        public int DownloadCertState { get; set; }
        /// <summary>
        /// 下载时间
        /// </summary>
        public DateTime ? DownloadDate { get; set; }
        /// <summary>
        /// 0：未同步；1：部分同步；2：全部同步
        /// </summary>
        public int SynCertState { get; set; }
        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime? SynCertDate { get; set; }
        /// <summary>
        /// 0：未受理；1：已受理
        /// </summary>
        public bool ReceivedState { get; set; }
        /// <summary>
        /// 受理人
        /// </summary>
        public string ReceivedUser { get; set; }
        /// <summary>
        /// 受理日期
        /// </summary>
        public DateTime ? ReceivedDate { get; set; }
        /// <summary>
        /// 0:未更新；1：已更新
        /// </summary>
        public bool UpdateState { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime ? UpdateDate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        #endregion Model
    }
}
