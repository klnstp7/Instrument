using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using GRGTCommonUtils;
using Global.Common.Models;
using ToolsLib.FileService;
using System.IO;
using System.Collections;
using Instrument.Common.Models;
using GRGTCommonUtils.WS.MeasureLab;

namespace Instrument.WebSite.Controllers
{
    public class OrderController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OrderController));
           
        public ActionResult OrderList()
        {
            return View();
        }

        public string GetOrderListJsonData()
        {
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel company = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CompanyInfo);
            if (null == company) company = new Global.Common.Models.ParamModel();
            Global.Common.Models.ParamItemModel pCompany = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司编号"));

            StringBuilder sb = new StringBuilder();
            List<MeasureOrderModel> orderBillList = new List<MeasureOrderModel>();
            string endDate = DateTime.Now.ToString("yyyy-MM-dd");
            string startDate = DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd");
            string orderJsonData = WSProvider.MeasureLabProvider.OrderSearch(startDate, endDate, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(orderJsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            if (dic["Msg"].ToString() == "OK")
                orderBillList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(List<MeasureOrderModel>)) as List<MeasureOrderModel>;
            else
                return "{\"data\":[]}";
            sb.Insert(0, "{\"data\":[");
            foreach (var item in orderBillList)
            {
                if (LoginHelper.LoginUserAuthorize.ContainsKey("订单查询-查看仪器"))
                    sb.AppendFormat("[\" <a href='#' OrderNumber='{0}'  onclick='fnCheckInstrument(this)'>查看仪器</a>&nbsp;&nbsp;", item.OrderNumber);
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Order/BatchDownloadCert".ToLower()))
                    sb.AppendFormat("<a href='#' orderNumber='{0}' onclick='fnBatchDownloadCert(this)'>批量下载证书</a>\"", item.OrderNumber);
                sb.AppendFormat(",\"{0}\"", item.OrderNumber);
                sb.AppendFormat(",\"{0}\"", item.CreateDate.ToShortDateString());
                sb.AppendFormat(",\"{0}\"", item.SaleName);
                sb.AppendFormat(",\"{0}\"", item.InstrumentCount);
                sb.AppendFormat(",\"{0}\"", item.IsComplete == true ? "完工" : "未完工");
                sb.AppendFormat(",\"{0}\"", item.IsCompleteCert == true ? "完工" : "未完工");
                //sb.AppendFormat(",\"{0}\"", item.IsInvoice == true ? "已开票" : "未开票");
                //sb.AppendFormat(",\"{0}\"", item.IsPay == true ? "已付款" : "未付款");
                sb.Append("],");
            }
            if (orderBillList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// 下载证书
        /// </summary>
        /// <param name="fileId"></param>
        public ActionResult Download(string certificateNumber)
        {
            byte[] bytes = WSProvider.MeasureLabProvider.DownloadCertification(certificateNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            if (bytes.Length>0)
            {
                MemoryStream stream = new MemoryStream(bytes);
                WebServer.DownLoadFile(stream, certificateNumber+".pdf");
            }
            return Content("参数错误，或无证书可以下载");
        }

        /// <summary>
        /// 批量下载某个订单下的所有证书
        /// </summary>
        /// <param name="orderNumber"></param>
        public ActionResult BatchDownloadCert(string orderNumber)
        {
            byte[] bytes = WSProvider.MeasureLabProvider.BatchDownloadCertificationByOrderNumber(orderNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            if (bytes.Length>0)
            {
                MemoryStream stream = new MemoryStream(bytes);
                WebServer.DownLoadFile(stream, orderNumber + ".zip");
            }
            return Content("无证书可下载");
        }

        /// <summary>
        /// 查看器具
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult ViewInstrumentDetails(string orderNumber)
        {
            string orderDetail = WSProvider.MeasureLabProvider.OrderInstrumentSearch(orderNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(orderDetail, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            List<Hashtable> orderBillDetailList = new List<Hashtable>();
            if (dic["Msg"].ToString() == "OK")
            {
                orderBillDetailList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(List<Hashtable>)) as List<Hashtable>;
            }
            //ViewBag.OrderBillDetailList = orderBillDetailList;
            StringBuilder sb = new StringBuilder();
            sb.Insert(0, "[");
            foreach (var item in orderBillDetailList)
            {
                sb.AppendFormat("[\"{0}\"", item["DownLoadString"]);
                sb.AppendFormat(",\"{0}\"", item["InstrumentName"]);
                sb.AppendFormat(",\"{0}\"", item["Specification"]);
                sb.AppendFormat(",\"{0}\"", item["MadeNumber"]);
                sb.AppendFormat(",\"{0}\"", item["ManageNumber"]);
                sb.AppendFormat(",\"{0}\"", item["CertificateNumber"]);
                sb.AppendFormat(",\"{0}\"", Convert.ToBoolean(item["IsComplete"]) ? "完工" : "未完工");
                sb.AppendFormat(",\"{0}\"", Convert.ToBoolean(item["IsCertComplete"]) ? "完工" : "未完工");
                sb.Append("],");
            }
            if (orderBillDetailList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            ViewBag.Data = sb.ToString();
            return View();
        }

        #region 证书管理

        public ActionResult GetCertList()
        {
            return View();
        }
        public ActionResult GetCertListForJsonData()
        {
            //提取DataTable参数
            ToolsLib.Utility.Jquery.DataTableUtils.DataTableModel dtm = ToolsLib.Utility.Jquery.DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.FieldOrder = "CertificateId desc";
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            //数据库查询数据
            object ob=new object();
            string[] fieldCondition = new string[8];
            fieldCondition[0] = Request["searchOrderNumber"];
            fieldCondition[1] = Request["searchCertificateNumber"];
            fieldCondition[2] = Request["searchMadeNumber"];
            fieldCondition[3] = Request["searchInstrumentName"];
            fieldCondition[4] = Request["searchSpecification"];
            fieldCondition[5] = Request["searchManageNumber"];
            fieldCondition[6] = Request["searchIsComplete"];
            fieldCondition[7] = Request["searchIsCertComplete"];
            string StrResult = WSProvider.MeasureLabProvider.GetCertListForPage(paging, fieldCondition, Global.Business.ServiceProvider.ParamService.GetCompanyCode(Instrument.Common.Constants.SysParamType.CompanyInfo), Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(StrResult, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            dtm.aaData = new List<List<string>>();
            if (dic["Msg"].ToString() != "OK")
            {
                JsonResult jrNull = Json(new
                {
                    sEcho = dtm.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = dtm.iTotalDisplayRecords,
                    aaData = dtm.aaData
                }, JsonRequestBehavior.AllowGet);
                return jrNull;
            }
            IList<Hashtable> orderList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(IList<Hashtable>)) as IList<Hashtable>;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = Convert.ToInt32(dic["RecordCount"].ToString());
            if (orderList != null)
            {
                foreach (Hashtable row in orderList)
                {
                    dtm.aaData.Add(new List<string>());
                    //下载条件：仪器完工，证书完工，仪器有报价，证书文件有上传，客户端允许下载
                    if (Convert.ToBoolean(row["IsDownLoad"]) && LoginHelper.LoginUserAuthorize.ContainsKey("/Order/Download".ToLower()))
                    {
                        dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' onclick=fnDownloadPDF(this) CertificateNumber='{1}' id='{0}'>下载证书</a>", UtilsHelper.Encrypt(row["CertificatePDFFileId"].ToString()), row["CertificateNumber"].ToString()));
                    }
                    else
                        dtm.aaData[dtm.aaData.Count - 1].Add("");
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["OrderNumber"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["CertificateNumber"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["MadeNumber"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["InstrumentName"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Specification"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["ManageNumber"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", Convert.ToBoolean(row["IsComplete"]) ? "已完工" : "未完工"));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", Convert.ToBoolean(row["IsCertComplete"]) ? "已完工" : "未完工"));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["SaleName"]));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:d}", row["CreateDate"]));
                }
            }
            JsonResult jr = Json(new
            {
                sEcho = dtm.sEcho,
                iTotalRecords = dtm.iTotalRecords,
                iTotalDisplayRecords = dtm.iTotalDisplayRecords,
                aaData = dtm.aaData
            }, JsonRequestBehavior.AllowGet);
            return jr;
        }
        #endregion

    }
}
