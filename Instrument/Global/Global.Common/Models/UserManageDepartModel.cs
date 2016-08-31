using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    [Serializable]
    public class UserManageDepartModel
    {
        public UserManageDepartModel()
        { }
        #region Model
        /// <summary>
        /// 管理部门标识
        /// </summary>
        public int OrgId { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        public int UserId { get; set; }
        ///// <summary>
        ///// 员工工号
        ///// </summary>
        //public string JobNo { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string OrgCode { get; set; }
        #endregion Model

    }
}
