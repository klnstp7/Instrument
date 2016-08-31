using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common
{
    public class GlobalConstants
    {
        public struct SysParamType
        {
            /// <summary>
            /// 学历
            /// </summary>
            public static readonly string EducationType = "AF";
            /// <summary>
            /// 计量业务类型
            /// </summary>
            public static readonly string BusinessType = "AD";
            /// <summary>
            /// 员工状态
            /// </summary>
            public static readonly string EmployeeState = "AG";


            /// <summary>
            /// 营销区域编码
            /// </summary>
            public static readonly string SaleAreaCode = "AE";
            /// <summary>
            /// 公司信息
            /// </summary>
            public static readonly string CompanyInfo = "CI";

        }

        /// <summary>
        /// 网站版本号
        /// </summary>
        public static readonly string VesionCode = "V1.0.2-A";

        /// <summary>
        /// 发布日期
        /// </summary>
        public static readonly string PublishDate = "2016-01-10";

        public enum EmployeeState
        {
            在职 = 0,
            离职 = 1,
            虚拟用户 = 2
        }

        /// <summary>
        /// 员工附件类型
        /// </summary>
        public enum EmployeeAttachmentType
        {
            照片 = 0,

            电子签名 = 1
        }

    }
}
