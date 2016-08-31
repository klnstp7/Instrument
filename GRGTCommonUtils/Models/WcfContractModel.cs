using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GRGTCommonUtils.Models
{
    /// <summary>
    /// WCF控制接口类
    /// </summary>
    public class WcfContractModel
    {

        /// <summary>
        /// 类名
        /// </summary>
        public string AclClass { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        public string AclContract { get; set; }

        /// <summary>
        /// 是否启动IP地址验证
        /// </summary>
        public int IpAccess { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public List<string> IPList { get; set; }

        /// <summary>
        /// 是否启动ToKen验证
        /// </summary>
        public int UserAccess { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public List<UserModel> UsersList { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public class UserModel
        {
            /// <summary>
            /// 令牌
            /// </summary>
            public string ToKen { get; set; }

            /// <summary>
            /// 过期时间("yyyy-MM-dd HH:mm:ss")
            /// </summary>
            public DateTime? Overdue { get; set; }
        }

    }
}
