using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using System.Collections;
using Instrument.Business;
using Instrument.Common.Models;
using System.Text;
using GRGTCommonUtils.WS;
using ToolsLib.Utility;
using ToolsLib.FileService;
using System.IO;
using System.Net;

namespace Instrument.WebSite.Controllers
{
    public class KnowledgeController : Controller
    {
        #region 计量知识库
        //
        // GET: /Knowledge/
        /// <summary>
        /// 用于获取知识类型列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetKnowledgeTypeList()
        {
            string jsonData = WSProvider.MeasureLabProvider.GetKnowledgeTypeList(Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            List<Hashtable> paramItemInfoList = new List<Hashtable>();
            List<Hashtable> countList = new List<Hashtable>();
            if (dic["Msg"].ToString() == "OK")
            {
                paramItemInfoList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(List<Hashtable>)) as List<Hashtable>;
                ViewBag.itemList = paramItemInfoList;
            }
            return View("KnowledgeTypeList");
        }

        public ActionResult BasicInfo(string ParamItemValue,string ParamName)
        {
            //ViewBag.KType = UtilConstants.KnowledgeType.基础知识.GetHashCode();
            //ViewBag.Title = UtilConstants.KnowledgeType.基础知识.ToString();
            ViewBag.KType = ParamItemValue;
            ViewBag.Title = ParamName;
            return View("KnowledgeList");
        }

        public ActionResult MajorCourse()
        {
            ViewBag.KType = UtilConstants.KnowledgeType.专业理论课.GetHashCode();
            ViewBag.Title = UtilConstants.KnowledgeType.专业理论课.ToString();
            return View("KnowledgeList");
        }

        public ActionResult Regulations()
        {
            ViewBag.KType = UtilConstants.KnowledgeType.规程规范.GetHashCode();
            ViewBag.Title = UtilConstants.KnowledgeType.规程规范.ToString();
            return View("KnowledgeList");
        }

        public ActionResult TechnicalInstructions()
        {
            ViewBag.KType = UtilConstants.KnowledgeType.技术说明书.GetHashCode();
            ViewBag.Title = UtilConstants.KnowledgeType.技术说明书.ToString();
            return View("KnowledgeList");
        }

        public ActionResult GetKnowledgesJsonData(int kType)
        {
            //提取DataTable参数
            ToolsLib.Utility.Jquery.DataTableUtils.DataTableModel dtm = ToolsLib.Utility.Jquery.DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.FieldOrder = "KnowledgeId desc";
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            //数据库查询数据
            object ob = new object();
            string[] fieldCondition = new string[4];
            fieldCondition[0] = Request["searchTitle"];
            fieldCondition[1] = Request["searchAbstract"];
            fieldCondition[2] = Request["searchCreatDate1"];
            fieldCondition[3] = Request["searchCreatDate2"];
            string StrResult = WSProvider.EbusinessProvider.GetKnowledgeListForPaging(paging, fieldCondition,kType, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
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
            IList<KnowledgesModel> knowledgeList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(IList<KnowledgesModel>)) as IList<KnowledgesModel>;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = Convert.ToInt32(dic["RecordCount"].ToString());
            if (knowledgeList != null)
            {
                foreach (KnowledgesModel model in knowledgeList)
                {
                    dtm.aaData.Add(new List<string>());
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' onclick=fnDetail(this) id='{0}'>查看</a>", model.KnowledgeId));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", model.Title));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", model.Abstract));
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:d}", model.CreatDate));
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

        /// <summary>
        /// 详细页面
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public ActionResult Details(int knowledgeId)
        {
            KnowledgesModel model = new KnowledgesModel();
            string StrResult = WSProvider.EbusinessProvider.GetKnowledgeDetailInfo(knowledgeId, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(StrResult, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            StringBuilder sb = new StringBuilder(); 
            sb.Append("[");
            if (dic["Msg"].ToString() == "OK")
            {
                model = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(KnowledgesModel)) as KnowledgesModel;
                if (model.swfFileByte != null && model.swfFileByte.Length > 0)
                {
                    string swfFilePath = "/tempFile/" + Guid.NewGuid().ToString() + ".swf";
                    ToolsLib.FileService.NormalFile.SaveInfoToFile(model.swfFileByte, swfFilePath);
                    ViewBag.FilePath = swfFilePath;
                }
                else
                {
                    ViewBag.FilePath = "";
                }
                int i = 1;
                if (model.businessAttachList != null)
                {

                    foreach (Hashtable item in model.businessAttachList)
                    {
                        sb.Append("[");
                        sb.AppendFormat("\"{0}\"", i++);
                        //sb.AppendFormat(",\"<a href='#' onclick='fnDownFile(this);return false;' filePath='{1}{2}' target='_blank'>{0}</a>\"", item["FileName"], item["FileAccessPrefix"], item["FileVirtualPath"]);
                        sb.AppendFormat(",\"<a href='{1}{2}' target='_blank'>{0}</a>\"", item["FileName"], item["FileAccessPrefix"], item["FileVirtualPath"]);
                        sb.Append("],");
                    }
                    if (model.businessAttachList.Count > 0) sb.Remove(sb.Length - 1, 1);
                }
            }
            sb.Append("]");
            ViewBag.Data = sb.ToString();
            return View(model);
        }

        //public void DownLoad(string filePrefix, string fileVirtualPath, string fileName)
        //{
        //    UtilsHelper.FileDownload(filePrefix, fileVirtualPath, fileName, UtilConstants.ServerType.WebFileService);
        //}

        //public ActionResult ReadPDF(string webFileServer, string fileVirtualPath)
        //{
        //    string userId = WebUtils.GetSettingsValue("WebFileServerUser");
        //    string pwd = WebUtils.GetSettingsValue("WebFileServerPwd");
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        userId = "webdevuser";
        //    }
        //    if (string.IsNullOrEmpty(pwd))
        //    {
        //        pwd = "grgfs8875";
        //    }
        //    WebDAVFileServer dav = new WebDAVFileServer(webFileServer, userId, pwd);
        //    Stream fileStream = dav.DownLoadFile(fileVirtualPath.Replace(".pdf", ".swf"));
        //    string swfFilePath = "/tempFile/" + Guid.NewGuid().ToString()+".swf";
        //    ToolsLib.FileService.NormalFile.SaveInfoToFile(fileStream, swfFilePath);
        //    swfFilePath = WebUtils.BaseAppPreSuffix + swfFilePath;
        //    return RedirectToAction("ReadPdf", "SysManage/Common", new { filePath = swfFilePath, CertId = 0, FileType = 0 });
        //}
        #endregion
    }
}
