using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    [Serializable]
    public class OrgModel
    {
        public OrgModel()
        {
            ShowOrder = 10;
            IsEnabled = true;
            RecordState = 1;
            OrgId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public int OrgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrgCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ParentOrgId { get; set; }
        /// <summary>
        /// 1：正常，0：禁用
        /// </summary>
        public bool IsEnabled { get; set; }

        public int RecordState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ShowOrder { get; set; }

        //public int OrgType { get; set; }

    }
}
