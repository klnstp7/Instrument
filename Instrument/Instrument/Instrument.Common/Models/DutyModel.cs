using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
        public class DutyModel
        {
            #region Model
            /// <summary>
            /// 职位标识
            /// </summary>
            public int DutyId { get; set; }
            /// <summary>
            /// 组织编码
            /// </summary>
            public string OrgCode { get; set; }
            /// <summary>
            /// 职位名称
            /// </summary>
            public string Duty { get; set; }
            #endregion Model

    }
}
