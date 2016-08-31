using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common
{
    public class Constants
    {

        public struct SysParamType
        {
            /// <summary>
            /// 仪器状态
            /// </summary>
            public static readonly string InstrumentState = "IS";
            /// <summary>
            /// 设备分类
            /// </summary>
            public static readonly string InstrumentCate = "FL";
            /// <summary>
            /// 预警天数
            /// </summary>
            public static readonly string WarnDay = "WD";
            /// <summary>
            /// 资产属性
            /// </summary>
            public static readonly string CalibrationType = "CT";
            /// <summary>
            /// 检定类别
            /// </summary>
            public static readonly string VerificationType = "VT";
            /// <summary>
            /// 配件类别
            /// </summary>
            public static readonly string AccessoriesType = "AI";
            /// <summary>
            /// 设备类别
            /// </summary>
            public static readonly string InstrumentType = "IT";
            /// <summary>
            /// 周检状态
            /// </summary>
            public static readonly string InstrumentCertificationState = "ZJ";
            /// <summary>
            /// 证书受理状态
            /// </summary>
            public static readonly string DownloadCertState = "DC";
            /// <summary>
            /// 体系文件类别
            /// </summary>
            public static readonly string DocumentType = "DT";
            /// <summary>
            /// 公司信息
            /// </summary>
            public static readonly string CompanyInfo = "CI";
            /// <summary>
            /// 管理级别
            /// </summary>
            public static readonly string ManageLevel = "ML";
            /// <summary>
            /// 仪器接收地点
            /// </summary>
            public static readonly string InstrumentReceiveLocale = "YJ";
            /// <summary>
            /// 联络单事项分类
            /// </summary>
            public static readonly string ContactCaseType = "SF";
            /// <summary>
            /// 期间核查结论
            /// </summary>
            public static readonly string PeriodcheckResult = "PR";
            /// <summary>
            /// 内部核查结论
            /// </summary>
            public static readonly string InnerCheckResult = "IR";
            /// <summary>
            /// 计量分公司
            /// </summary>
            public static readonly string BranchCompany = "AE";
            /// <summary>
            ///  盘点结果
            /// </summary>
            //public static readonly string AssetCheckResult = "PJ";
            /// <summary>
            ///  盘点计划类型
            /// </summary>
            public static readonly string AssetCheckPlanType = "PT";
            /// <summary>
            /// 固定资产状态
            /// </summary>
            public static readonly string AssetsState = "AS";
            /// <summary>
            /// 计量机构
            /// </summary>
            public static readonly string TestOrg = "JL";

        }        

        public enum DownloadCertState
        {
            未下载 = 0,

            部分下载 = 1,

            全部下载=2
        }

        public enum InstrumentForm
        {
            仪器 = 0,
            固定资产 = 1
        }

        /// <summary>
        /// 业务附件关系信息:业务类型
        /// </summary>
        public enum AttachmentBusinessType
        {
            周期校准记录 = 0,
            设备档案 = 1,
            联络单 = 2,
            维修单 = 3,
            内部核查 = 4,
            期间核查 = 5,
            仪器照片 = 6,
            本地知识库=7
        }

        public enum InstrumentFlowType
        {
            到达 = 1,
            前往 = 2
        }

        /// <summary>
        /// 周检记录状态(标准器具)
        /// </summary>
        public enum InstrumentCertificationState
        {
            未检 = -1,

            周检中 = 0,

            完成周检 = 1
        }

        /// <summary>
        /// 联络单状态
        /// </summary>
        public enum ContactState
        {
            草稿 = 0,
            已提交 = 1,
            已反馈 = 2,
            已解决 = 3
        }

        /// <summary>
        /// 资产盘点状态
        /// </summary>
        public enum AssetsCheckStatus
        {
            盘亏 = 0,

            已盘点 = 1,

            盘盈 = 2
        }

        /// <summary>
        /// 地点是否一致
        /// </summary>
        public enum AssetsIsRightAddress
        {
            一致 = 0,
            不一致 = 1
        }

    }
}
