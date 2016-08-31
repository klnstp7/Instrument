using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Global.Common.Models
{
    [Serializable]
    public class UserModel
    {
        #region Model
        /// <summary>
        /// 员工标识
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string JobNo { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPwd { get; set; }
        /// <summary>
        /// 归属部门
        /// </summary>
        public string BelongDepart { get; set; }
        /// <summary>
        /// 工作职务
        /// </summary>
        public string Duty { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile1 { get; set; }
        /// <summary>
        /// 电子邮件-公司
        /// </summary>
        public string Email1 { get; set; }       
        /// <summary>
        /// 员工状态
        /// </summary>
        public int EmployeeState { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName { get; set; }

        #endregion Model

        /// <summary>
        /// 用户拥有的角色
        /// </summary>
        public IList<RoleModel> roleList { get; set; }

        public IList<UserManageDepartModel> manageDepartList { get; set; }
    }
}
