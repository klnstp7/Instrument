using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GRGTCommonUtils
{
    public class UtilConstants
    {
        public struct SysParamType
        {
            /// <summary>
            /// 科室
            /// </summary>
            public static readonly string MeasureLab = "AD";

            /// <summary>
            /// 子公司
            /// </summary>
            public static readonly string BranchCompany = "AE";
            /// <summary>
            /// 标准器具状态
            /// </summary>
            public static readonly string InstrumentRecordState = "IS";

            /// <summary>
            /// 标准器具类别
            /// </summary>
            public static readonly string InstrumentType = "IT";
            /// <summary>
            /// 操作类型(系统日志)
            /// </summary>
            public static readonly string OperateType = "OP";

            /// <summary>
            /// 计量知识状态
            /// </summary>
            public static readonly string KnowledgeState = "KS";

            /// <summary>
            /// 计量知识类型
            /// </summary>
            public static readonly string KnowledgeType = "KT";
            
        }

        /// <summary>
        /// 存储批量上传的文件
        /// </summary>
        public static Hashtable UploadSession { get; set; }

        /// <summary>
        /// 组织根目录
        /// </summary>
        public static readonly string RootOrgCode = "GRGT";

        public enum EmployeeState
        {
            在职 = 0,
            离职 = 1,
            虚拟用户 = 2
        }

        /// <summary>
        /// 文件存储的服务器类型
        /// </summary>
        public enum ServerType
        { 
            WebService = 1,
            WebFileService,
            FTPService
        }

        /// <summary>
        /// 组织类型
        /// </summary>
        public enum OrgType
        {
            分公司 = 1,
            市场部 = 2,
            实验室 = 3
        }

    

      
        /// <summary>
        /// 系统
        /// </summary>
        public enum SysType
        {
            HR = 0,

            CRM = 1,

            MeasureLab = 2,

            Finance = 3,

            Environment = 4,

            EMC = 5,

            CustomerPortal = 9,

            Instrument = 10
        }

        /// <summary>
        /// 信息类别
        /// </summary>
        public enum MsgType
        {
            短信 = 0,

            彩信 = 1,

            邮件 = 2,

            微信 = 3
        }

        /// <summary>
        /// 消息发送结果
        /// </summary>
        public enum SendMsgResult
        {
            未发送 = 0,

            发送成功 = 1,

            发送失败 = 2
        }

        /// <summary>
        /// 消息发送方式
        /// </summary>
        public enum SendMsgType
        {
            立即发送 = 0,

            按计划时间发送 = 1
        }

        /// <summary>
        /// 上传文件类型
        /// </summary>
        public enum AttachmentType
        {
            未知 = 0,
            证书模板 = 1,
            技术依据 = 2,
            客户回签单 = 3,
            原始记录 = 4,
            证书及报告 = 5,
            证书及报告历史记录 = 6,
            标准器具 = 7,
            日志记录 = 8,
            仪器周检证书 = 9,
            联络单 = 10
        }


        /// <summary>
        /// 操作日志类别，0：其它，1：新增，2：修改，3：删除
        /// </summary>
        public enum OperateType
        {
            其它 = 0,
            新增 = 1,
            修改 = 2,
            删除 = 3,
            登录 = 4          
        }

        /// <summary>
        /// 系统操作日志--操作对象类别
        /// </summary>
        public enum TargetType
        {
            委托单操作日志 = 1,
            系统登录日志 = 2
        }

        /// <summary>
        /// 标准器具状态
        /// </summary>
        public enum InstrumentState
        {
            //0：合格，1：删除，2：封存，3：过期禁用，4：限用，5：修理，6：不合格,7:停用,8:报废
            合格 = 0,

            //删除 = 1,

            封存 = 2,

            过期禁用 = 3,

            限用 = 4,

            修理 = 5,

            不合格 = 6,

            停用 = 7,

            报废=8
        }

        /// <summary>
        /// 附件类型(标准器具)
        /// </summary>
        public enum InstrumentAttachmentType
        {
            标准证书 = 0,

            说明书 = 1
        }

        /// <summary>
        /// 周检记录状态(标准器具)
        /// </summary>
        public enum InstrumentCheckLogState
        {
            未完成周检 = 0,

            完成周检 = 1
        
        }

        /// <summary>
        /// 计量知识库状态
        /// </summary>
        public enum KnowledgeState
        {
            未分享 = 0,
            已分享 = 1
        }

        /// <summary>
        /// 计量知识库类型
        /// </summary>
        public enum KnowledgeType
        {
            基础知识 = 0,
            专业理论课 = 1,
            规程规范 = 2,
            技术说明书 = 3
        }
    }
}
