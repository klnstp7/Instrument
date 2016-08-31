using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Instrument.Common;
using Instrument.Common.Models;
using Instrument.Business;
using GRGTCommonUtils;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using ToolsLib.FileService;
using Global.Common;
using ToolsLib.Utility;
using System.Text;
using System.Collections;
using System.Data;
using Global.Common.Models;
using System.IO;
using Newtonsoft.Json;

namespace Instrument.WebSite.Controllers
{
    public class DocumentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DocumentList()
        {
            //文件分类
            IList<ParamModel> list = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel paramModel = list.SingleOrDefault(p => p.ParamCode == Constants.SysParamType.DocumentType);
            IList<DocumentModel> documentList = ServiceProvider.DocumentService.GetAll();
            StringBuilder sb = new StringBuilder();
            StringBuilder operate = new StringBuilder();
            sb.Append("[");
            foreach (var document in documentList)
            {
                sb.Append("[");
                if (LoginHelper.LoginUserAuthorize.Contains("/Document/Details".ToLower()))
                    operate.Append("<a href='#' onclick='fnUpdateDocument({0});return false;'>编 辑</a>&nbsp;&nbsp;&nbsp;");
                if (LoginHelper.LoginUserAuthorize.Contains("/Document/Delete".ToLower()))
                    operate.Append("<a href='#' onclick='fnDeleteDocument({0},{1});return false;'>删 除</a>&nbsp;&nbsp;&nbsp;");
                if (LoginHelper.LoginUserAuthorize.Contains("作业指导书-关联仪器".ToLower()))
                    operate.Append("<a href='#' onclick='fnRelateDocument({0});return false;'>关联仪器</a>&nbsp;&nbsp;&nbsp;");
                sb.AppendFormat("\"" + operate + "\"", document.DocumentId, UtilsHelper.Encrypt(document.FileId.ToString()));
                operate.Clear();
                sb.AppendFormat(",\"<a href='#' id='{0}' onclick='fnDownFile(this);return false;'>{1}</a>\"", UtilsHelper.Encrypt(document.FileId.ToString()), document.FileName);
                sb.AppendFormat(",\"{0}\"", paramModel.itemsList.SingleOrDefault(p => p.ParamItemValue == document.DocCategory.ToString()).ParamItemName);
                sb.AppendFormat(",\"{0:d}\"", document.CreateDate);
                sb.AppendFormat(",\"{0}\"", document.Remark);
                sb.Append("],");
            }
            if (documentList.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            ViewBag.documentData = sb.ToString();
            return View(new DocumentModel());
        }


        public JsonResult Details(int documentId)
        {
            DocumentModel documentModel = ServiceProvider.DocumentService.GetById(documentId);
            ParamModel documentTypeModel = Global.Business.ServiceProvider.ParamService.GetAll().SingleOrDefault(S => S.ParamCode == Constants.SysParamType.DocumentType);
            string documentType = documentTypeModel.itemsList.SingleOrDefault(S => Convert.ToInt32(S.ParamItemValue) == documentModel.DocCategory).ParamItemName;
            JsonResult jr = Json(new
            {
                documentModel = documentModel,
                documentType = documentType
            }, JsonRequestBehavior.AllowGet);
            return jr;
        }

        public ActionResult Delete(int documentId)
        {
            ServiceProvider.DocumentService.DeleteById(documentId);//体系文件表
            return Content("OK");
        }

        public ActionResult IsHasInstrument(int documentId, int fileId)
        {
            Hashtable table = ServiceProvider.DocumentService.GetByDocumentId(documentId);
            bool isHave = table == null || table.Count == 0;
            if (isHave)
            {
                return Content("OK");
            }
            else
            {
                return Content("NO");
            }
        }

        public ActionResult Save(DocumentModel documentModel, FormCollection collection)
        {
            ParamModel documentTypeModel = Global.Business.ServiceProvider.ParamService.GetAll().SingleOrDefault(S => S.ParamCode == Constants.SysParamType.DocumentType);
            if (Request.Files.Count > 0)//有文件上传（新增或修改）
            {
                HttpPostedFileBase fileData = Request.Files[0];
                if (null != fileData && 0 != fileData.ContentLength)
                {
                    string url = WebUtils.GetSettingsValue("InstrumentDocumentFilePath") + @"/" + StrUtils.GetUniqueFileName(null) + System.IO.Path.GetExtension(Request.Files[0].FileName);
                    var AttachmentModel = UtilsHelper.FileUpload(Request.Files[0], url, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
                    Global.Business.ServiceProvider.AttachmentService.Save(AttachmentModel);
                    documentModel.FileId = AttachmentModel.FileId;
                    documentModel.FileName = fileData.FileName;
                    documentModel.DocCategory = int.Parse(documentTypeModel.itemsList.SingleOrDefault(S => S.ParamItemName == collection["DocCategory"]).ParamItemValue);
                    documentModel.Remark = collection["Remark"];
                    documentModel.CreateDate = System.DateTime.Now;
                    documentModel.Creator = LoginHelper.LoginUser.UserName;
                    ServiceProvider.DocumentService.Save(documentModel);
                    return Content("OK");
                }
                else
                {
                    return Content("体系文件无效！");
                }

            }
            else if (documentModel.DocumentId != 0)//修改
            {
                documentModel.DocCategory = int.Parse(documentTypeModel.itemsList.SingleOrDefault(S => S.ParamItemName == collection["DocCategory"]).ParamItemValue);
                documentModel.Remark = collection["Remark"];
                documentModel.CreateDate = System.DateTime.Now;
                documentModel.Creator = LoginHelper.LoginUser.UserName;
                ServiceProvider.DocumentService.Save(documentModel);
                return Content("OK");
            }//新增
            else
            {
                return Content("体系文件无效！");
            }

        }

        public ActionResult DocumentOwnInstrumentList(int documentId)
        {
            IList<ParamModel> list = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel paramModel = null;
            string InstrumentType = null;
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel org = new OrgModel();
            string where = "InstrumentId in (Select InstrumentId From DocumentOwnInstrument Where SysDocumentId='" + documentId + "')";
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(where);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var instrument in instrumentList)
            {
                paramModel = list.SingleOrDefault(p => p.ParamCode == Constants.SysParamType.InstrumentType);
                InstrumentType = paramModel.itemsList.SingleOrDefault(p => p.ParamItemValue == instrument.InstrumentType.ToString()).ParamItemName;
                org = orgList.SingleOrDefault(o => o.OrgCode == instrument.BelongDepart);
                sb.AppendFormat("[\"<a href='#' onclick='fnDelete({0},{1});return false;'>删 除</a>&nbsp;&nbsp;&nbsp;"
                    + "\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\""
                    + "],", documentId, instrument.InstrumentId, instrument.InstrumentName, instrument.ManageNo, instrument.Specification, InstrumentType, instrument.Manufacturer,
                    instrument.SerialNo, instrument.CreateDate, instrument.DueStartDate, instrument.DueEndDate, org == null ? null : org.OrgName);
            }
            if (instrumentList.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            ViewBag.instrumentData = sb.ToString();
            ViewBag.DocumentId = documentId.ToString();
            return View();
        }

        public ActionResult InstrumentList(int documentId)
        {
            ViewBag.DocumentId = documentId.ToString();
            return View();
        }
        /// <summary>
        /// 批量关联仪器
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult ChooseMoreInstrument(int documentId, string instrumentIds)
        {
            string dic = ServiceProvider.DocumentService.AddMoreDocument(documentId, instrumentIds);
            string[] result = dic.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return Json(new
            {
                Msg = result[0],
                success = result[1],
                failure = result[2]
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 关联仪器
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult ChooseInstrument(int documentId, int instrumentId)
        {
            return Content(ServiceProvider.DocumentService.AddOwnDocument(documentId, instrumentId));
        }

        /// <summary>
        /// 解除关联仪器
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult DeleteDocumentOwnInstrument(int documentId, int instrumentId)
        {
            try
            {
                ServiceProvider.DocumentService.DeleteInstrumentOwnDocument(documentId, instrumentId);
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content("删除失败");
            }
        }

        public JsonResult GetAllInstrumentJsonData(string documentId)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InstrumentId,InstrumentName,ManageNo,Specification,InstrumentType,Manufacturer,SerialNo,CreateDate,DueStartDate,DueEndDate,BelongDepart";
            paging.Where = GetInstrumentSearchCondition();
            IList<ParamModel> list = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel paramModel = null;
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel org = new OrgModel();
            //IList<CraftModel> craftList = ServiceProvider.CraftService.GetAll();
            IList<Hashtable> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListForPaging(paging);
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            //StringBuilder sbData = new StringBuilder();
            dtm.aaData = new List<List<string>>();
            foreach (var item in instrumentList)
            {
                paramModel = list.SingleOrDefault(p => p.ParamCode == Constants.SysParamType.InstrumentType);
                org = orgList.SingleOrDefault(o => o.OrgCode == (item["BelongDepart"] == null ? "" : item["BelongDepart"].ToString()));
                dtm.aaData.Add(new List<string>());
                //sbData.Clear();
                //sbData.AppendFormat("<div documentId='{0}' instrumentId='{1}'>", documentId, item["InstrumentId"].ToString());
                //if (LoginHelper.LoginUserAuthorize.ContainsKey("/Document/InstrumentList".ToLower()))
                //{
                //    //修改
                //    sbData.Append("<a href='#' onclick='fnChoose(" + documentId + "," + item["InstrumentId"] + ");'>关 联</a>&nbsp;&nbsp;&nbsp;");
                //}
                //sbData.Append("</div>");
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DeleteInstruments".ToLower()))
                {
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<input type='checkbox' name='chk' value={0} />", item["InstrumentId"]));
                }
                //dtm.aaData[dtm.aaData.Count - 1].Add(sbData.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["InstrumentName"] == null ? "" : item["InstrumentName"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["ManageNo"] == null ? "" : item["ManageNo"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["Specification"] == null ? "" : item["Specification"].ToString());
                //设备分类
                dtm.aaData[dtm.aaData.Count - 1].Add(paramModel.itemsList.SingleOrDefault(p => p.ParamItemValue == item["InstrumentType"].ToString()).ParamItemName);
                dtm.aaData[dtm.aaData.Count - 1].Add(item["Manufacturer"] == null ? "" : item["Manufacturer"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["SerialNo"] == null ? "" : item["SerialNo"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["CreateDate"] == null ? "" : item["CreateDate"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["DueStartDate"] == null ? "" : item["DueStartDate"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["DueEndDate"] == null ? "" : item["DueEndDate"].ToString()));
                //所属部门
                dtm.aaData[dtm.aaData.Count - 1].Add(org == null ? null : org.OrgName);
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
        /// 检索条件
        /// </summary>
        /// <returns></returns>
        private string GetInstrumentSearchCondition()
        {
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = "1=1";
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                where = dtm.FieldCondition;
            }
            return where;
        }

    }
}
