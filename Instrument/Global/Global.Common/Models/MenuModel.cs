using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    [Serializable]
    public class MenuModel
    {
        public MenuModel()
        { }
        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int MenuId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ParentMenuId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnabled { get; set; }
        #endregion Model

    }
}
