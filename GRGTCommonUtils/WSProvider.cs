using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRGTCommonUtils.WS;
using System.Web;
using System.IO;
using System.Net;
using GRGTCommonUtils.WS.MeasureLab;
using GRGTCommonUtils.WS.Instrument;

namespace GRGTCommonUtils
{
    public static class WSProvider
    {

        #region 账户信息

        public static class HRProvider
        {
            private static AccoutService AccoutClient = null;

            static HRProvider()
            {
                AccoutClient = new AccoutService();
                AccoutClient.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("SSOUrl");
            }

            /// <summary>
            /// 登录验证
            /// </summary>
            /// <param name="loginName"></param>
            /// <param name="loginPwd"></param>
            /// <param name="comeFrom"></param>
            /// <returns></returns>
            public static string Login(string loginName, string loginPwd, string comeFrom)
            {
                return AccoutClient.Login(loginName, loginPwd, comeFrom);
            }

            /// <summary>
            /// 重置密码
            /// </summary>
            /// <param name="loginName"></param>
            /// <param name="loginPwd"></param>
            /// <param name="newLoginPwd"></param>
            /// <returns></returns>
            public static string ResetPassword(string loginName, string loginPwd, string newLoginPwd)
            {
                return AccoutClient.ResetPassword(loginName, loginPwd, newLoginPwd);
            }

            /// <summary>
            /// 获取组织负责人(根据组织编码)
            /// </summary>
            /// <param name="orgCode"></param>
            /// <returns></returns>
            public static string GetOrgLeader(string orgCode)
            {
                return AccoutClient.GetOrgLeader(orgCode);
            }

            /// <summary>
            /// 获取某个用户的区域经理（上级）
            /// </summary>
            /// <param name="orgCode"></param>
            /// <returns></returns>
            public static string GetLeaderRelation(string JobNo)
            {
                return AccoutClient.GetLeaderRelation(JobNo);
            }
        }

        #endregion

        #region 通用信息

        /// <summary>
        /// 通用信息
        /// </summary>
        public static class CommonProvider
        {
            /// <summary>
            /// 通用信息
            /// </summary>
            private static CommonService mCommonClient = null;
            static CommonProvider()
            {
                mCommonClient = new CommonService();
                mCommonClient.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("CommonUrl");
            }

            /// <summary>
            /// 记录接口访问日志
            /// </summary>
            /// <param name="log"></param>
            public static void AddInterfaceLog(LogInterfaceModel log)
            {
                mCommonClient.AddInterfaceLog(log);
            }

            /// <summary>
            /// 同步仪器信息
            /// </summary>
            /// <param name="instrumentDataJson"></param>
            /// <param name="certificationDataJson"></param>
            /// <param name="businessType"></param>
            /// <param name="acessToken"></param>
            /// <returns></returns>
            public static string SendInstrumentData(string instrumentDataJson)
            {
                return mCommonClient.SendInstrumentData(instrumentDataJson);
            }
            /// <summary>
            /// 删除数据同步
            /// </summary>
            /// <param name="itemCode"></param>
            /// <param name="businessType"></param>
            /// <param name="acessToken"></param>
            /// <returns></returns>
            public static string DeleteInstrumentData(string instrumentJsonData)
            {
                return mCommonClient.DeleteInstrumentData(instrumentJsonData);
            }

        }

        #endregion



        #region 客户自助

        /// <summary>
        /// 客户自助
        /// </summary>
        public static class EbusinessProvider
        {
            /// <summary>
            /// 客户自助
            /// </summary>
            private static CustomerPortalService mCustomerPortalClient = null;
            private static KnowledgeService mKnowledgeClient = null;
            static EbusinessProvider()
            {
                mCustomerPortalClient = new CustomerPortalService();
                mCustomerPortalClient.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("EbusinessUrl");
                if (!string.IsNullOrWhiteSpace(ToolsLib.Utility.WebUtils.GetSettingsValue("ProxyServer")))
                    mCustomerPortalClient.Proxy = new WebProxy(ToolsLib.Utility.WebUtils.GetSettingsValue("ProxyServer"), true);


                mKnowledgeClient = new KnowledgeService();
                mKnowledgeClient.Url = string.Format("{0}/KnowledgeService", ToolsLib.Utility.WebUtils.GetSettingsValue("EbusinessUrl"));
                if (!string.IsNullOrWhiteSpace(ToolsLib.Utility.WebUtils.GetSettingsValue("ProxyServer")))
                    mKnowledgeClient.Proxy = new WebProxy(ToolsLib.Utility.WebUtils.GetSettingsValue("ProxyServer"), true);
            }

            ///// <summary>
            ///// 同步客户关系管理账号到客户自助平台
            ///// </summary>
            ///// <param name="mAccount"></param>
            ///// <returns></returns>
            //public static void SyncAccountToCustomerPortal(string JobNo, string loginPwd, string companyCode, string userState)
            //{
            //    mEbusinessClient.SyncAccountToCustomerPortal(JobNo, loginPwd, companyCode, userState);
            //}

            ///// <summary>
            ///// 同步同步客户关系管理账号到客户自助平台
            ///// </summary>
            ///// <param name="JobNo"></param>
            //public static void DeleteAccountInCustomerPortal(string JobNo)
            //{
            //    mEbusinessClient.DeleteAccountInCustomerPortal(JobNo);
            //}

            /// <summary>
            /// 仪器管理系统-仪器送检
            /// </summary>
            /// <param name="sendOrderJson"></param>
            /// <param name="sendOrderProjectJson"></param>
            public static string SendOrder(string sendOrderJson, string sendOrderProjectJson, string companyCode)
            {
                return mCustomerPortalClient.SendOrder(sendOrderJson, sendOrderProjectJson, companyCode);
            }

            /// <summary>
            /// 仪器管理系统-更新仪器受理状态接口
            /// </summary>
            /// <param name="orderNumber"></param>
            /// <returns></returns>
            public static string UpdateOrderState(string orderNumber, string companyCode)
            {
                return mCustomerPortalClient.UpdateOrderState(orderNumber, companyCode);
            }

            /// <summary>
            /// 仪器管理系统-撤销送检清单
            /// </summary>
            /// <param name="orderNumber"></param>
            /// <returns></returns>
            public static string RemoveOrder(string orderNumber,string companyCode)
            {
                return mCustomerPortalClient.RemoveOrder(orderNumber, companyCode);
            }

            /// <summary>
            /// 同步联络单和附件
            /// </summary>
            /// <param name="contactInfo">联络单</param>
            /// <param name="fileInputSting">附件</param>
            /// <param name="sourceFileName">附件名字（含后缀）</param>
            /// <returns></returns>
            public static string SynInsertContact(string contactInfo, byte[] fileData, string sourceFileName, string accessToken)
            {
                return mCustomerPortalClient.SendContact(contactInfo, fileData, sourceFileName, accessToken);
            }

            public static string GetContactByItemCode(string itemCode, string accessToken)
            {
                return mCustomerPortalClient.GetContactFeedbackInfo(itemCode, accessToken);
            }

            /// <summary>
            /// 版本检测接口
            /// </summary>
            /// <param name="currentVersion"></param>
            /// <param name="accessToken"></param>
            /// <returns></returns>
            public static string CheckInstrumentVersion(string currentVersion, string accessToken)
            {
                return mCustomerPortalClient.CheckInstrumentVersion(currentVersion, accessToken);
            }

            /// <summary>
            /// 获取计量知识库列表-分页显示
            /// </summary>
            /// <param name="paging"></param>
            /// <param name="fieldCondition"></param>
            /// <param name="type"></param>
            /// <param name="accessToken"></param>
            /// <returns></returns>
            public static string GetKnowledgeListForPaging(GRGTCommonUtils.WS.PagingModel paging, string[] fieldCondition, int type, string accessToken)
            {
                return mKnowledgeClient.GetKnowledgeListForPaging(paging, fieldCondition, type, true, accessToken);
            }

            public static string GetKnowledgeDetailInfo(int knowledgeId, string accessToken)
            {
                return mKnowledgeClient.GetKnowledgeDetailInfo(knowledgeId, true, accessToken);
            }

        }

        #endregion


        #region 计量业务

        /// <summary>
        /// 计量业务
        /// </summary>
        public static class MeasureLabProvider
        {
            /// <summary>
            /// 计量业务
            /// </summary>
            //private static MeasureLabService mMeasureLabClient = null;
            private static MeasureExternalService mMeasureLabExternalClient = null;
            static MeasureLabProvider()
            {
                //mMeasureLabClient = new MeasureLabService();
                //mMeasureLabClient.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("MeasureLabUrl");

                mMeasureLabExternalClient = new MeasureExternalService();
                mMeasureLabExternalClient.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("MeasureLabUrl");
            }

            /// <summary>
            /// 获取计量知识库菜单和数量
            /// </summary>
            /// <param name="accessToken"></param>
            /// <returns></returns>

            public static string GetKnowledgeTypeList(string accessToken)
            {
                return mMeasureLabExternalClient.GetKnowledgeTypeList(accessToken);
            }


            ///// <summary>
            ///// 根据CertificatePDFFileId获取附件对象
            ///// </summary>
            ///// <returns>附件对象(Json)</returns>
            //public static string GetAttachmentById(int CertificatePDFFileId, string accessToken)
            //{
            //    return mMeasureLabClient.GetAttachmentById(CertificatePDFFileId, true, accessToken);
            //}

            /// <summary>
            /// 根据certificateNumber获取附件流
            /// </summary>
            /// <param name="certificateNumber"></param>
            /// <param name="accessToken"></param>
            /// <returns></returns>
            public static byte[] DownloadCertification(string certificateNumber, string accessToken)
            {
                return mMeasureLabExternalClient.DownloadCertification(certificateNumber, accessToken);
            }



            ///// <summary>
            ///// 获取委托单下所有已完工的证书 ，返回证书编号，和附件的访问前缀和虚拟地址的列表,文件的访问前缀和虚拟地址在最后
            ///// </summary>
            ///// <param name="OrderNumber"></param>
            ///// <returns></returns>
            //public static string GetCompleteCertificateList(string OrderNumber)
            //{
            //    return mMeasureLabClient.GetCompleteCertificateList(OrderNumber);
            //}

            ///// <summary>
            ///// 证书核查
            ///// </summary>
            ///// <param name="CertificateNumber">证书编码</param>
            ///// <returns>
            /////  证书不存在(未审核通过+未完工) 返回0，代表证书不存在
            /////  证书通过审核但未完工 证书已完工但未通过审核 返回1,代表证书未完工
            /////  证书通过审核且已完工 返回证书路径
            ///// </returns>
            //public static string CertVerification(string CertificateNumber)
            //{
            //    return mMeasureLabClient.CertVerification(CertificateNumber);
            //}

            /// <summary>
            /// 获取委托单列表信息
            /// </summary> 
            /// <param name="StartDate">开始日期(格式如：2014-06-20)</param>
            /// <param name="EndDate">结束日期(格式如：2014-06-20)</param>
            /// <param name="accessToken">客户令牌，编号和密码对称加密的字符串</param>
            /// <returns></returns>
            public static string OrderSearch(string StartDate, string EndDate, string accessToken)
            {
                return mMeasureLabExternalClient.OrderSearch(StartDate, EndDate, accessToken);
            }

            /// <summary>
            /// 获取某个委托单的器具清单信息(根据委托单号) 
            /// </summary>
            /// <param name="orderNumber"></param>
            /// <param name="companyCode"></param>
            /// <returns></returns>
            public static string OrderInstrumentSearch(string orderNumber, string accessToken)
            {
                return mMeasureLabExternalClient.OrderInstrumentSearch(orderNumber, accessToken);
            }

            ///// <summary>
            ///// 获取器具(根据委托单号)
            ///// </summary>
            ///// <param name="orderNumber">委托单号</param>
            ///// <returns></returns>
            //public static string GetBillProjectListByOrderNumber(string orderNumber)
            //{
            //    return mMeasureLabClient.GetBillProjectListByOrderNumber(orderNumber);
            //}

            ///// <summary>
            /////  获取任务
            ///// </summary>
            ///// <param name="topN">任务个数</param>
            ///// <param name="taskType">任务类型</param>
            ///// <returns></returns>
            //public static string GetTopNByType(int topN, int taskType)
            //{
            //    return mMeasureLabClient.GetTopNByType(topN, true, taskType, true);
            //}

            /// <summary>
            /// 同步周期校准记录-根据报价单号获取委托单下服务项目对应的证书信息
            /// </summary>
            /// <param name="MD5Codes"></param>
            /// <returns></returns>
            public static string GetCertInfoByQuotationNumber(string quotationNumber, string accessToken)
            {
                return mMeasureLabExternalClient.GetCertInfoByQuotationNumber(quotationNumber, accessToken);
            }

            /// <summary>
            /// 同步周期校准记录-根据证书编号获取委托单下服务项目对应的证书信息
            /// </summary>
            /// <param name="MD5Codes"></param>
            /// <returns></returns>
            public static string GetCertInfoByCertificateNumber(string certificateNumber, string accessToken)
            {
                return mMeasureLabExternalClient.GetCertInfoByCertificateNumber(certificateNumber, accessToken);
            }

            /// <summary>
            /// 根据报价单号批量下载电子证书
            /// </summary>
            /// <param name="quotationNumber"></param>
            /// <returns></returns>
            public static byte[] BatchDownloadCertificationByQuotationNumber(string quotationNumber, string accessToken)
            {
                return mMeasureLabExternalClient.BatchDownloadCertification(quotationNumber, accessToken);
            }

            /// <summary>
            /// 根据委托单号批量下载电子证书
            /// </summary>
            /// <param name="orderNumber"></param>
            /// <param name="accessToken"></param>
            /// <returns></returns>
            public static byte[] BatchDownloadCertificationByOrderNumber(string orderNumber, string accessToken)
            {
                return mMeasureLabExternalClient.BatchDownloadCertification2(orderNumber, accessToken);
            }

            /// <summary>
            /// 仪器管理系统使用-根据报价单号获取业务系统中的仪器信息和仪器下的证书信息
            /// </summary>
            /// <param name="quotationNumber"></param>
            /// <returns></returns>
            public static string QuotationInstrumentSearch(string quotationNumber, string accessToken)
            {
                return mMeasureLabExternalClient.QuotationInstrumentSearch(quotationNumber, accessToken);
            }

            /// <summary>
            /// 仪器管理系统使用-仪器综合查询
            /// </summary>
            /// <param name="quotationNumber"></param>
            /// <returns></returns>
            public static string GetCertListForPage(GRGTCommonUtils.WS.MeasureLab.PagingModel page, string[] fieldCondition, string companyCode, string accessToken)
            {
                return mMeasureLabExternalClient.GetCertListForPage(page, fieldCondition, companyCode, accessToken);
            }


        }

        #endregion


        #region 计量仪器管理系统
        public static class InstrumentProvider
        {
            private static InstrumentWCFServices Instrument = null;
            static InstrumentProvider()
            {
                Instrument = new InstrumentWCFServices();
                Instrument.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("InstrumentUrl");
            }

            public static string GetBaseInfoByBarCode(string barCode, string accessToKen)
            {
                if (Instrument == null)
                {
                    Instrument = new InstrumentWCFServices();
                    Instrument.Url = ToolsLib.Utility.WebUtils.GetSettingsValue("InstrumentUrl");
                }
                return Instrument.GetBaseInfoByBarCode(barCode, accessToKen);
            }

            public static string GetInstrumentDetailByBarCode(string barCode, string accessToKen)
            {
                return Instrument.GetInstrumentDetailByBarCode(barCode, accessToKen);
            }

            public static string GetInstrumentList(string instrumentName, string specification, string serialNo, string accessToKen)
            {
                return Instrument.GetInstrumentList(instrumentName, specification, serialNo, accessToKen);
            }

            public static string InstrumentCheck(string barCode, string operatorJobNo, string accessToKen)
            {
                return Instrument.InstrumentCheck(barCode, operatorJobNo, accessToKen);
            }

            public static string AddInstrument(string instrumentName, string sOperator, string specification, string manufacturer, string serialNo, int planId, string accessToKen)
            {
                return Instrument.AddInstrument(instrumentName, sOperator, specification, manufacturer, serialNo, planId, false, accessToKen);
            }

            public static string GetCirculationRecordList(string barCode, string accessToKen)
            {
                return Instrument.GetCirculationRecordList(barCode, accessToKen);
            }

            public static string AddCirculationRecord(string barCode, string sOperator, string currentPosition, string reason, string accessToKen)
            {
                return Instrument.AddCirculationRecord(barCode, sOperator, currentPosition, reason, accessToKen);
            }

            public static string GetCheckingPlanBaseInfoList(string jobNo,string accessToKen)
            {
                return Instrument.GetCheckingPlanBaseInfoList(jobNo,accessToKen);
            }
            public static string AppLogin(string JobNo, string loginPwd, string accessToKen)
            {
                return Instrument.AppLogin(JobNo, loginPwd, accessToKen);
            }
            public static string SendOrder(string instrumentIds, string orderJson,string BelongDepart, string accessToKen)
            {
                return Instrument.SendOrder(instrumentIds, orderJson, BelongDepart,"",0,true, accessToKen);
            }

        }
        #endregion

    }
}
