using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    public class OrgAppendModel
    {
        public OrgAppendModel()
        {
            OrgId = 0;
            OrgLeader = 0;
            BusinessType = 0;
            OrgType = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int OrgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int OrgLeader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string OfficeTel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string OfficeFax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string OfficeAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int BusinessType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int OrgType { get; set; }
    }
}
