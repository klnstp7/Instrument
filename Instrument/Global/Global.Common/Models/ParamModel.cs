using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    [Serializable]
    public class ParamModel
    {
        public ParamModel()
        { }
        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int ParamId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamCode { get; set; }

        public IList<ParamItemModel> itemsList { get; set; }

        #endregion Model

    }
}
