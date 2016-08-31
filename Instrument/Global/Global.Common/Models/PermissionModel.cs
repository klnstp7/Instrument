using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
        [Serializable]
        public class PermissionModel
        {
            public PermissionModel()
            { }
            #region Model
            /// <summary>
            /// 权限点标识
            /// </summary>
            public int PermissionId { get; set; }
            /// <summary>
            /// 权限点名称
            /// </summary>
            public string PermissionName { get; set; }
            /// <summary>
            /// 上级权限点
            /// </summary>
            public int ParentPermissionId { get; set; }
            /// <summary>
            /// 权限资源
            /// </summary>
            public string PermissionResource { get; set; }
            #endregion Model

        }
}
