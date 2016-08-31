using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    [Serializable]
    public class ParamItemModel
    {
        public ParamItemModel()
        { }
        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int ParamItemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ParamId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamItemName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamItemValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }
        #endregion Model

    }
}
