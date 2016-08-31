using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using System.Collections;
using Global.Common.Models;
using System.Text;
using Instrument.Business;
using Instrument.Common.Models;
using GRGTCommonUtils;

namespace Instrument.WebSite.Controllers
{
    public class CustKnowledgeController : Controller
    {
      
        /// <summary>
        /// 知识库查询列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.KnowledgeState = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new UtilConstants.KnowledgeState()).ToString();
            ViewBag.KnowledgeType = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new UtilConstants.KnowledgeType()).ToString();
            return View();
        }

        /// <summary>
        /// 获取知识库
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllKnowledgeListJsonData()
        {
            //提取DataTable参数
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? "1=1" : dtm.FieldCondition;
            IList<Hashtable> knowledgeList = ServiceProvider.KnowledgesService.GetAllKnowledgesListForPaging(paging);
            //系统参数
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel pState = paramList.SingleOrDefault(p => p.ParamCode.Equals(UtilConstants.SysParamType.KnowledgeState));
            ParamModel pType = paramList.SingleOrDefault(p => p.ParamCode.Equals(UtilConstants.SysParamType.KnowledgeType));
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();

            StringBuilder operate = new StringBuilder();
            foreach (var item in knowledgeList)
            {
                dtm.aaData.Add(new List<string>());
                string knowledgeId = UtilsHelper.Encrypt(item["KnowledgeId"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<input type='checkbox' name='chk' value='{0}'>", knowledgeId));
                operate.AppendFormat("<div knowledgeId='{0}'>", knowledgeId);
                if (LoginHelper.LoginUserAuthorize.Contains("/CustKnowledge/Edit".ToLower()))
                    operate.AppendFormat("<a href='#' onclick='fnEdit(this)'>编辑</a>&nbsp;|&nbsp;");
                if (LoginHelper.LoginUserAuthorize.Contains("/CustKnowledge/Details".ToLower()))
                    operate.AppendFormat("<a href='#' onclick='fnDetail(this)'>查看</a>");
                operate.Append("</div>");

                dtm.aaData[dtm.aaData.Count - 1].Add(operate.ToString());
                operate.Clear();
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Title"]));
                 ParamItemModel itemModel  = pType.itemsList.SingleOrDefault(p => p.ParamItemValue.Equals(item["KType"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", itemModel == null ? "" : itemModel.ParamItemName));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Abstract"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Creator"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["CreatDate"]));
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
        /// 编辑
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public ActionResult Edit(string knowledgeId)
        {
            KnowledgesModel model = new KnowledgesModel();
            if (knowledgeId != "0")
                model = ServiceProvider.KnowledgesService.GetById(UtilsHelper.Decrypt2Int(knowledgeId));
            //内容类型
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel kType = paramList.SingleOrDefault(m => m.ParamCode.Equals(UtilConstants.SysParamType.KnowledgeType));
            ViewBag.KType = new SelectList(kType.itemsList, "ParamItemValue", "ParamItemName");

            if (model.FileId!=0)
            {
                AttachmentModel attach = Global.Business.ServiceProvider.AttachmentService.GetById(model.FileId);
                ViewBag.FileName = attach.FileName;
            }

            return View(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult Save(KnowledgesModel model, FormCollection collection)
        {
            ServiceProvider.KnowledgesService.Save(model, Request.Files);
            return Json(new { Msg = "OK", KnowledgeId = model.KnowledgeId ,bkId=UtilsHelper.Encrypt(model.KnowledgeId.ToString())});
        }

        /// <summary>
        /// 知识库明细
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public ActionResult Details(string knowledgeId)
        {
            KnowledgesModel model =ServiceProvider.KnowledgesService.GetKnowledgeDetailInfo(UtilsHelper.Decrypt2Int(knowledgeId));
            //内容类型
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel kType = paramList.SingleOrDefault(m => m.ParamCode.Equals(UtilConstants.SysParamType.KnowledgeType));
            ViewBag.KType = kType.itemsList.FirstOrDefault(s=>s.ParamItemValue==String.Format("{0}",model.KType)).ParamItemName;

            //业务附件
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
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
                    sb.AppendFormat(",\"<a href='#' onclick='fnDownLoad(this);' data-id='{0}'>{1}</a>\"", UtilsHelper.Encrypt(String.Format("{0}",item["FileId"])), item["FileName"]);
                    sb.Append("],");
                }
                if (model.businessAttachList.Count > 0) sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            ViewBag.Data = sb.ToString();
            return View(model);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="knowledgeIds"></param>
        /// <returns></returns>
        public ActionResult BatchDeleteByIds(string knowledgeIds)
        {
            IList<int> idLsit = knowledgeIds.Split(',').Select(k=>UtilsHelper.Decrypt2Int(k)).ToList();
            ServiceProvider.KnowledgesService.DeleteByIdList(idLsit);
            return Content("OK");
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="fileId"></param>
        public void DownLoad(string fileId)
        {
            fileId = UtilsHelper.Decrypt(fileId);
            AttachmentModel fileMode= Global.Business.ServiceProvider.AttachmentService.GetById(int.Parse(fileId));
            UtilsHelper.FileDownload(fileMode.FileAccessPrefix, fileMode.FileVirtualPath, fileMode.FileName, (UtilConstants.ServerType)fileMode.FileServerType);
        }

    }
}
