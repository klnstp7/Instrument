using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    /// <summary>
    /// 盘点明细
    /// </summary>
    public class AssetCheckPlanDetailModel
    {
        public AssetCheckPlanDetailModel()
        { }
        #region Model
        /// <summary>
        /// 盘点计划明细标识
        /// </summary>
        public int PlanDetailId { get; set; }
        /// <summary>
        /// 盘点计划标识
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        /// <summary>
        /// 0:盘输 1：已盘点 3: 盘盈
        /// </summary>
        public int Statuse { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string BelongDepart { get; set; }
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string InstrumentName { get; set; }
        /// <summary>
        /// 仪器型号
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 出厂编号
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 管理编号
        /// </summary>
        public string ManageNo { get; set; }
        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetsNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 盘点人
        /// </summary>
        public string Checkor { get; set; }
        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime CheckDate { get; set; }
        /// <summary>
        /// 地点是否一致 0：一致 1：不一致
        /// </summary>
        public int IsRightAddress { get; set; }
        #endregion Model


    }
}
