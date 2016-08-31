using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    [Serializable]
    public class RoleModel
    {
        public RoleModel()
        { }
        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled { get; set; }
        #endregion Model

        public IList<PermissionModel> permissionList { get; set; }
        public IList<MenuModel> menuList { get; set; }
    }
}
