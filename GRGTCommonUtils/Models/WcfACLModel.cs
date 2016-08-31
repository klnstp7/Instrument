using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRGTCommonUtils.Models
{
    /// <summary>
    /// WCF服务类
    /// </summary>
    public class WcfACLModel
    {
        /// <summary>
        /// 忽略验证的IP地址
        /// </summary>
        public List<string> IgnoreIPs { get; set; }

        /// <summary>
        /// 控制接口类
        /// </summary>
        public List<WcfContractModel> ContractList { get; set; }

        /// <summary>
        /// 网络地址访问控制信息
        /// </summary>
        public List<IpAclModel> IpAcls { get; set; }

        /// <summary>
        /// 用户访问控制信息
        /// </summary>
        public List<UserAclModel> UserAcls { get; set; }
    }

    /// <summary>
    /// 网络地址访问控制信息
    /// </summary>
    public class IpAclModel
    {
        /// <summary>
        /// 网络地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 控制接口
        /// </summary>
        public List<WcfContractModel> ContractList { get; set; }
    }

    /// <summary>
    /// 用户访问控制信息
    /// </summary>
    public class UserAclModel
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string ToKen { get; set; }

        /// <summary>
        /// 控制接口
        /// </summary>
        public List<WcfContractModel> ContractList { get; set; }
    }


}
