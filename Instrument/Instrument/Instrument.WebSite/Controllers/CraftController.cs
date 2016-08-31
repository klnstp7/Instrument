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

namespace Instrument.WebSite.Controllers
{
    public class CraftController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CraftController));

        public ActionResult CraftList()
        {
            return View();
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="craftId"></param>
        /// <returns></returns>
        public ActionResult GetCraftById(string craftId)
        {
            CraftModel model = ServiceProvider.CraftService.GetById(UtilsHelper.Decrypt2Int(craftId));
            if (model == null) model = new CraftModel();
            return Json(new { model = model }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult Save(CraftModel model, FormCollection collection)
        {
            try
            {
                ServiceProvider.CraftService.Save(model);
                return Content("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("保存失败");
            }
        }

        /// <summary>
        /// 仪器查询、仪器维护
        /// </summary>
        /// <param name="type">Search:仪器查询，Maintain：仪器维护</param>
        /// <returns></returns>
        public JsonResult GetAllCraftJsonData(string type)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"CraftId,CraftCode,CraftName,InstrumentNo,UsePlace,CreateUser,CreateDate";
            paging.Where = GetCraftSearchCondition(dtm);
           

            //IList<CraftModel> craftList = ServiceProvider.CraftService.GetAll();
            IList<Hashtable> craftList = ServiceProvider.CraftService.GetAllCraftListForPaging(paging);

            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;

            StringBuilder sbData = new StringBuilder();
            dtm.aaData = new List<List<string>>();
            foreach (var item in craftList)
            {
                dtm.aaData.Add(new List<string>());
                sbData.Clear();
                sbData.AppendFormat("<div craftId='{0}' craftName='{1}'>", item["CraftId"].ToString(), item["CraftName"]);
                //if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentDetail".ToLower()))
                //{
                //    //详细
                //    sbData.Append("<a href='#' onclick='fnInstrumentDetail(this);'>详细</a>&nbsp;&nbsp;");
                //}
                if (type.Equals("CraftList"))
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/Edit".ToLower()))
                    {
                        //修改
                        sbData.Append("<a href='#' onclick='fnEditCraft(" + item["CraftId"] + ");'>修改</a>&nbsp;&nbsp;&nbsp;");
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/Delete".ToLower()))
                    {
                        //删除
                        sbData.Append("<a href='#' onclick='fnDeleteCraft(" + item["CraftId"] + ");'>删除</a>&nbsp;&nbsp;");
                    }
                }

                sbData.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbData.ToString());
                //dtm.aaData[dtm.aaData.Count - 1].Add(item.CraftId.ToString()); 
                dtm.aaData[dtm.aaData.Count - 1].Add(item["CraftCode"] == null ? null : item["CraftCode"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["CraftName"] == null ? null : item["CraftName"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["InstrumentNo"] == null ? null : item["InstrumentNo"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["UsePlace"] == null ? null : item["UsePlace"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["CreateUser"] == null ? null : item["CreateUser"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["CreateDate"]));
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
        private string GetCraftSearchCondition(DataTableUtils.DataTableModel dtm)
        {

            string where = "1=1";
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                where = dtm.FieldCondition;
            }
            return where;
        }

        /// <summary>
        /// 工艺列表
        /// </summary>
        /// <returns></returns>
        public string GetCraftJsonData()
        {
            IList<CraftModel> craftList = ServiceProvider.CraftService.GetAll();
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();
            foreach (var craft in craftList)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                sbOperate.AppendFormat("<a href='#' id='{0}' onclick='fnChooseCraft(this)'>选择</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(craft.CraftId.ToString()));
                sbData.AppendFormat("\"{0}\"", sbOperate);
                sbData.AppendFormat(",\"{0}\"", craft.CraftCode);
                sbData.AppendFormat(",\"{0}\"", craft.CraftName);
                sbData.AppendFormat(",\"{0}\"", craft.InstrumentNo);
                sbData.AppendFormat(",\"{0}\"", craft.UsePlace);
                sbData.AppendFormat(",\"{0}\"", craft.CreateUser);
                sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", craft.CreateDate);
                sbData.Append("],");
            }
            if (craftList.Count > 0)
                sbData.Remove(sbData.Length - 1, 1);
            sbData.Append("]}");
            return sbData.ToString();
        }

        public ActionResult Edit(int craftId)
        {
            //部门...........生成一个下拉框树所需的数据源
            ViewBag.OrgList = Global.Business.ServiceProvider.OrgService.GetAll();
            CraftModel mCraft = ServiceProvider.CraftService.GetById(craftId);
            if (mCraft == null) mCraft = new CraftModel();
            //return View(mCraft);
            return View(mCraft);
        }

        public ActionResult Delete(int craftId)
        {
            ServiceProvider.CraftService.DeleteById(craftId);
            return Content("OK");
        }

        public ActionResult IsHasInstrument(int craftId)
        {
            IList<InstrumentModel> list = ServiceProvider.InstrumentService.GetByCraftIdList(craftId);
            if (list.Count == 0)
            {
                return Content("OK");
            }
            else
            {
                return Content("NO");
            }
        }
    }
}
