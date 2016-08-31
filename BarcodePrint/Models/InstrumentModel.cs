using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodePrint.Models
{
 public   class InstrumentModel
    {
        #region Model
        
        /// <summary>
        /// 仪器名称
        /// </summary>
     public string InstrumentName { get; set; }
        /// <summary>
     /// 仪器型号
        /// </summary>
     public string Specification { get; set; }
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
     /// 条码
     /// </summary>
     public string BarCode { get; set; }
     /// <summary>
     /// 设备类型
     /// </summary>
     public int InstrumentForm { get; set; }

     /// <summary>
     /// 部门
     /// </summary>
     public string DepartName { get; set; }

     /// <summary>
     /// 保管人
     /// </summary>
     public string LeaderName { get; set; }
        
        #endregion Model
    }
}
