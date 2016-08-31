using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Instrument.Common.Models;
using Instrument.Business;
using GRGTCommonUtils;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using Global.Common;
using ToolsLib.Utility;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using Instrument.Common;
using Global.Common.Models;
using System.Net;

namespace Instrument.WebSite.Controllers
{
    public class CertificationController : Controller
    {
        //
        // GET: /Certification/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CertificationList()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            return View();
        }

        public JsonResult GetAllCertificationJsonData()
        {

            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //string where = GetCertificationSearchCondition(dtm);
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            paging.FieldShow = @"LogId,InstrumentId,CertificationCode,FileId,CheckDate,EndDate,SendInstrumentDate,CheckResult,ErrorValue,RecordState,IsUseding,CertMoney,CreateDate,CreateUser";
            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? dtm.FieldCondition = "1=1" : dtm.FieldCondition;
            //是否超期
            string overTime = Request["searchIsOverTime"];
            if (!string.IsNullOrEmpty(overTime))
            {
                if (overTime == "0")
                    paging.Where = string.Format("{0} and {1}", paging.Where, "GetDate()<=EndDate");//未超期
                else
                    paging.Where = string.Format("{0} and {1}", paging.Where, "GetDate()>EndDate");//已超期
            }
            //是否上传报告
            string UpdateReport = Request["searchIsUpdateReport"];
            if (!string.IsNullOrEmpty(UpdateReport))
                paging.Where = string.Format("{0} and {1}", paging.Where, UpdateReport == "0" ? "(FileId IS null or FileId=0)" : "FileId>0");

            //添加委托单查询条件.
            string orderParam = Request["InstrumentParam"];
            if (!string.IsNullOrWhiteSpace(orderParam))
            {
                orderParam = string.Format(" and {0}", orderParam);
            }
            paging.Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {2}{1}))", paging.Where, orderParam, GetManageCondition("InstrumentForm=0 "));

            DateTime dueEndDate;
            bool isOverTime = false;
            IList<Hashtable> instrumentCertificationList = ServiceProvider.InstrumentCertificationService.GetInstrumentCertificationListForPaging(paging);
            IList<int> instrumentIds = instrumentCertificationList.Select(s => Convert.ToInt32(s["InstrumentId"])).Distinct().ToList();
            IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;

            ////系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.OrgModel belongDeptModel = new Global.Common.Models.OrgModel();
            Global.Common.Models.ParamItemModel mInstrumentCate = null;
            IList<Global.Common.Models.ParamItemModel> paramItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate).itemsList;
            ///周检状态
            IList<ParamItemModel> CertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();

            StringBuilder sbData = new StringBuilder();
            dtm.aaData = new List<List<string>>();
            foreach (var item in instrumentCertificationList)
            {
                InstrumentModel instrumentModel = instrumentList.SingleOrDefault(p => p.InstrumentId == Convert.ToInt32(item["InstrumentId"]));
                if (instrumentModel == null)
                {
                    instrumentModel = new InstrumentModel();
                }
                if (item["EndDate"] == null)
                {
                    isOverTime = true;
                }
                else
                {
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item["EndDate"]));
                    //是否过期
                    isOverTime = DateTime.Now.CompareTo(dueEndDate) > 0 ? true : false;
                }
                dtm.aaData.Add(new List<string>());
                sbData.Clear();
                sbData.AppendFormat("<a href='#' onclick='fnDetails({0})'>详细</a>&nbsp;&nbsp;", item["LogId"]);
                if (item["FileId"] != null)
                {
                    sbData.Append("<a href='#' onclick='fnDownFile(\"" + UtilsHelper.Encrypt(item["FileId"].ToString()) + "\");'>下 载</a>&nbsp;&nbsp;");
                    //sbData.Append(string.Format("<a href='/Certification/ReadCert?Id={1}' target='_blank' >{0}</a>","浏 览", UtilsHelper.Encrypt(item["FileId"].ToString())));
                }
                dtm.aaData[dtm.aaData.Count - 1].Add(sbData.ToString());
                mParamItem = CertificationState.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item["RecordState"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mParamItem == null ? "" : mParamItem.ParamItemName);    //状态
                dtm.aaData[dtm.aaData.Count - 1].Add(isOverTime ? "已超期" : "未超期");    //证书超期
                dtm.aaData[dtm.aaData.Count - 1].Add(item["FileId"] == null || item["FileId"].ToString() == "" ? "否" : "是");    //是否上传证书
                dtm.aaData[dtm.aaData.Count - 1].Add(item["EndDate"] == null ? null : string.Format("{0:yyyy-MM-dd}", item["EndDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(item["CertificationCode"] == null ? "" : item["CertificationCode"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:F2}", item["CertMoney"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(instrumentModel.InstrumentName);
                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrumentModel.InstrumentCate));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                dtm.aaData[dtm.aaData.Count - 1].Add(instrumentModel.ManageNo);
                dtm.aaData[dtm.aaData.Count - 1].Add(instrumentModel.Specification);
                dtm.aaData[dtm.aaData.Count - 1].Add(instrumentModel.SerialNo);
                dtm.aaData[dtm.aaData.Count - 1].Add(item["CheckDate"] == null ? null : string.Format("{0:yyyy-MM-dd}", item["CheckDate"]));

                dtm.aaData[dtm.aaData.Count - 1].Add(item["CheckResult"] == null ? null : item["CheckResult"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["ErrorValue"] == null ? null : item["ErrorValue"].ToString());

                dtm.aaData[dtm.aaData.Count - 1].Add(item["CreateDate"] == null ? null : string.Format("{0:yyyy-MM-dd}", item["CreateDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(item["CreateUser"] == null ? null : item["CreateUser"].ToString());
            }
            JsonResult jr = Json(new
            {
                sEcho = dtm.sEcho,
                iTotalRecords = dtm.iTotalRecords,
                iTotalDisplayRecords = dtm.iTotalDisplayRecords,
                aaData = dtm.aaData,
            }, JsonRequestBehavior.AllowGet);
            return jr;
        }

        /// <summary>
        /// 获取用户管理部门的条件语句
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        private string GetManageCondition(string where)
        {
            string GetAllAuthorityStr = "Instrument-CheckAll";
            bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

            if (!IsGetAllAuthority)
            {
                //获取当前用户所管辖的所有区域下的仪器SQL语句.
                StringBuilder subSqlStr = Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart");
                where = string.Format(" {0} and {1}", subSqlStr, where);
            }
            return where;
        }

        /// <summary>
        /// 检索条件
        /// </summary>
        /// <returns></returns>
        private string GetCertificationSearchCondition(DataTableUtils.DataTableModel dtm)
        {

            string where = "1=1";
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                where = dtm.FieldCondition;
            }
            string instrumentParam = Request["InstrumentParam"];
            if (!string.IsNullOrEmpty(instrumentParam))
            {
                where = string.Format("{0} and {1}", where, instrumentParam);
            }
            return where;
        }


        public ActionResult Export()
        {
            DataTable dtData = new DataTable();
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = GetCertificationSearchCondition(dtm);
            string Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? dtm.FieldCondition = "1=1" : dtm.FieldCondition;
            //是否超期
            string overTime = Request["searchIsOverTime"];
            if (!string.IsNullOrEmpty(overTime))
            {
                if (overTime == "0")
                    Where = string.Format("{0} and {1}", Where, "GetDate()<=EndDate");//未超期
                else
                    Where = string.Format("{0} and {1}", Where, "GetDate()>EndDate");//已超期
            }
            //添加委托单查询条件.
            string orderParam = Request["InstrumentParam"];
            if (!string.IsNullOrWhiteSpace(orderParam))
            {
                orderParam = string.Format(" and {0}", orderParam);
            }
            if (!string.IsNullOrWhiteSpace(orderParam))
                Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {2}{1}))", Where, orderParam, GetManageCondition("InstrumentForm=0 "));

            IList<InstrumentCertificationModel> instrumentCertificationList = ServiceProvider.InstrumentCertificationService.GetByWhere(Where);
            IList<int> instrumentIds = instrumentCertificationList.Select(s => Convert.ToInt32(s.InstrumentId)).Distinct().ToList();
            IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);
            if (0 == instrumentCertificationList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            dtData.Columns.Add("RecordState", typeof(string));    //周检状态
            dtData.Columns.Add("IsOverTime", typeof(string));    //是否过期
            dtData.Columns.Add("CertificateNo", typeof(string));    //证书编号
            dtData.Columns.Add("CertMoney", typeof(string));    //费用
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
            dtData.Columns.Add("Specification", typeof(string));    //型号规格
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
            dtData.Columns.Add("CheckDate", typeof(string));    //检验日期
            dtData.Columns.Add("EndDate", typeof(string));    //有效日期
            dtData.Columns.Add("CheckResult", typeof(string));    //检测结果
            dtData.Columns.Add("ErrorValue", typeof(string));    //误 差
            dtData.Columns.Add("CreateDate", typeof(string));    //创建日期
            dtData.Columns.Add("CreateUser", typeof(string));    //创建人


            DateTime dueEndDate;
            bool isOverTime = false;

            ////系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ///周检状态
            IList<ParamItemModel> CertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();

            foreach (var item in instrumentCertificationList)
            {
                DataRow drData = dtData.NewRow();
                mParamItem = CertificationState.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item.RecordState));
                drData["RecordState"] = mParamItem == null ? "" : mParamItem.ParamItemName;   //周检状态
                InstrumentModel instrumentModel = instrumentList.SingleOrDefault(p => p.InstrumentId == Convert.ToInt32(item.InstrumentId));
                if (instrumentModel == null)
                {
                    instrumentModel = new InstrumentModel();
                }
                if (instrumentModel.DueEndDate == null) dueEndDate = DateTime.MinValue;
                else dueEndDate = Convert.ToDateTime(string.Format("{0:d}", instrumentModel.DueEndDate));
                if (item.EndDate == null)
                {
                    isOverTime = true;
                }
                else
                {
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item.EndDate));
                    //是否过期
                    isOverTime = DateTime.Now.CompareTo(dueEndDate) > 0 ? true : false;
                }
                //isOverTime = DateTime.Now.CompareTo(dueEndDate) > 0 ? true : false;
                drData["IsOverTime"] = isOverTime ? "已超期" : "未超期";   //是否过期
                drData["CertificateNo"] = item.CertificationCode == null ? null : item.CertificationCode.ToString();  //证书编号
                drData["CertMoney"] = string.Format("{0:F2}", item.CertMoney);
                drData["InstrumentName"] = instrumentModel.InstrumentName;   //仪器名称
                drData["ManageNo"] = instrumentModel.ManageNo;   //管理编号
                drData["Specification"] = instrumentModel.Specification;   //型号规格
                drData["SerialNo"] = instrumentModel.SerialNo;   //出厂编号
                drData["CheckDate"] = item.CheckDate == null ? null : string.Format("{0:yyyy-MM-dd}", item.CheckDate);   //检验日期
                drData["EndDate"] = item.EndDate == null ? null : string.Format("{0:yyyy-MM-dd}", item.EndDate);   //有效日期
                drData["CheckResult"] = item.CheckResult;   //检测结果
                drData["ErrorValue"] = item.ErrorValue;   //误 差
                drData["CreateDate"] = item.CreateDate == null ? null : string.Format("{0:yyyy-MM-dd}", item.CreateDate);  //创建日期
                drData["CreateUser"] = item.CreateUser;   //创建人
                dtData.Rows.Add(drData);
            }

            //导出
            List<string> headerList = new List<string>(new string[] { 
                "周检状态","是否过期","证书编号","费用","仪器名称", "管理编号", "仪器型号", "出厂编号", "检验日期", "有效日期","检测结果","误 差","创建日期","创建人"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "证书", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}证书{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        /// <summary>
        /// 获取周检明细
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult GetInstrumentCertificationDetail(int logId)
        {
            InstrumentCertificationModel model = null;
            model = ServiceProvider.InstrumentCertificationService.GetById(logId);
            ViewBag.RecordState = (Constants.InstrumentCertificationState)model.RecordState;
            ViewBag.AttachmentBusinessType = Constants.AttachmentBusinessType.周期校准记录.GetHashCode();
            ViewBag.BusinessId = UtilsHelper.Encrypt(model.LogId.ToString());
            return View("InstrumentCertificationDetail", model);
        }

        #region 浏览证书
        public ActionResult ReadCert(string Id)
        {
            int certId = UtilsHelper.Decrypt2Int(Id);
            Global.Common.Models.AttachmentModel model = Global.Business.ServiceProvider.AttachmentService.GetById(certId);
            string tempPath = "/tempFile/";
            string tempFileName = string.Format("{0}.swf", Guid.NewGuid().ToString());
            string webFileServer = WebUtils.GetSettingsValue("WebFileServer");
            string userId = WebUtils.GetSettingsValue("WebFileServerUser");
            string pwd = WebUtils.GetSettingsValue("WebFileServerPwd");
            string oldFileSwf = model.FileVirtualPath.Replace(".pdf", ".swf");

            try
            {
                if (!UtilsHelper.IsExistInFSServer(webFileServer, oldFileSwf, userId, pwd))
                {
                    if (UtilsHelper.IsExistInFSServer(webFileServer, model.FileVirtualPath, userId, pwd))
                    {
                        Stream swfStream = UtilsHelper.FileDownload(model.FileAccessPrefix, model.FileVirtualPath, UtilConstants.ServerType.WebFileService);
                        string pdfFilestr = CommonUtils.GetPhysicsPath(string.Format("{0}{1}.pdf", tempPath, Guid.NewGuid()));
                        ToolsLib.FileService.NormalFile.SaveInfoToFile(swfStream, pdfFilestr);
                        string swfSourceFile = UtilsHelper.PdfToSwf(pdfFilestr, tempFileName);
                        model.FileVirtualPath = string.Format("{0}{1}", tempPath, Path.GetFileName(swfSourceFile));
                        UtilsHelper.FileUpload(model.FileAccessPrefix, swfSourceFile, oldFileSwf, UtilConstants.ServerType.WebFileService);
                    }
                }
                else
                {
                    Stream swfStream = UtilsHelper.FileDownload(model.FileAccessPrefix, oldFileSwf, UtilConstants.ServerType.WebFileService);
                    string swfFilestr = CommonUtils.GetPhysicsPath(string.Format("{0}{1}", tempPath, tempFileName));
                    ToolsLib.FileService.NormalFile.SaveInfoToFile(swfStream, swfFilestr);
                    model.FileVirtualPath = string.Format("{0}{1}",tempPath,tempFileName); 
                }
            }
            catch (Exception e)
            {
                return Content("浏览的文件已经顺坏或者加密");
            }
            return RedirectToAction("ReadPdf", "SysManage/Common", new { filePath = model.FileVirtualPath });
        }
        #endregion

    }
}
