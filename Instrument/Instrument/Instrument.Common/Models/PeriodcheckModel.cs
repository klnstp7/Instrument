using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class PeriodcheckModel
    {
      public PeriodcheckModel()
		{}
      #region Model
      /// <summary>
      /// 期间核查标识
      /// </summary>
      public int PeriodcheckId { get; set; }
      /// <summary>
      /// 仪器标识
      /// </summary>
      public int InstrumentId { get; set; }
      /// <summary>
      /// 频次
      /// </summary>
      public string Frequency { get; set; }
      /// <summary>
      /// 计划日期
      /// </summary>
      public DateTime? PlanDate { get; set; }
      /// <summary>
      /// 完成日期
      /// </summary>
      public DateTime? CompleteDate { get; set; }
      /// <summary>
      /// 负责人
      /// </summary>
      public string Leader { get; set; }
      /// <summary>
      /// 参数配置
      /// </summary>
      public int Result { get; set; }
      /// <summary>
      /// 备注
      /// </summary>
      public string Remark { get; set; }
      #endregion Model
    }
}
