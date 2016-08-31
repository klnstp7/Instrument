using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using System.Collections;
using System.Text;
using Instrument.Business;
using Instrument.Common;
using Instrument.Common.Models;
using Global.Common.Models;

namespace Instrument.WebSite.Controllers
{
    public class InstrumentFlowController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InstrumentFlowController));

        #region 流转明细查询
        public ActionResult InstrumentList()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            return View();
        }

        public JsonResult GetAllInstrumentJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InstrumentId,RecordState,InstrumentName,ManageNo,CertificateNo,Specification,InstrumentCate,Manufacturer,SerialNo,DueStartDate,DueEndDate,LeaderName,BelongDepart,StorePalce,CreateDate,CreateUser";
            paging.Where = GetSearchCondition(dtm);
            IList<Hashtable> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListForPaging(paging);
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();
            ParamItemModel mInstrumentCate = null;
            //分类
            ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            IList<ParamItemModel> paramItemList = InstrumentCate.itemsList;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            foreach (var item in instrumentList)
            {
                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<div instrumentId='{0}' instrumentName='{1}'>", UtilsHelper.Encrypt(item["InstrumentId"].ToString()), item["InstrumentName"]);
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/InstrumentFlow/FlowDetails".ToLower()))
                {
                    //详细
                    sbOperate.Append("<a href='#' onclick='fnInstrumentFlowDetail(this);'>流转明细</a>&nbsp;&nbsp;");
                }
             
                sbOperate.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["InstrumentName"]));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));    //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["CertificateNo"]));    //证书编号

                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["InstrumentCate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                //分公司
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", item["BelongDepart"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:d}", item["DueStartDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["LeaderName"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:d}", item["CreateUser"]));
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
        /// 检索条件
        /// </summary>
        /// <returns></returns>
        private string GetSearchCondition(DataTableUtils.DataTableModel dtm)
        {
            string where = "InstrumentForm=" + (int)Constants.InstrumentForm.仪器;
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition)) where = string.Format("{0} and {1}", dtm.FieldCondition, where);
            //所属部门
            string orgName = Request["searchBelongDepart"];
            where = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition(where, "InstrumentFlow-CheckAll", orgName);

            //if (!string.IsNullOrEmpty(orgName))
            //{
            //    IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll().Where(o => o.OrgName == orgName).ToList();
            //    string strFilte = "";
            //    foreach (OrgModel org in orgList)
            //    {
            //        if (string.IsNullOrWhiteSpace(strFilte))
            //            strFilte = string.Format("BelongDepart like '{0}%'", org.OrgCode);
            //        else
            //            strFilte = string.Format("{0} or BelongDepart like '{1}%'", strFilte, org.OrgCode);
            //    }
            //    where = string.Format("{0} and ({1})", where, strFilte);
            //}

            return where;
        }

        ///// <summary>
        ///// 获取用户管理部门的条件语句
        ///// </summary>
        ///// <param name="where"></param>
        ///// <returns></returns>
        //private string GetManageCondition(string where)
        //{
        //    string GetAllAuthorityStr = "InstrumentFlow-CheckAll";
        //    bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

        //    if (!IsGetAllAuthority)
        //    {
        //        //获取当前用户所管辖的所有区域下的仪器SQL语句.
        //        StringBuilder subSqlStr = Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart");
        //        where = string.Format(" {0} and {1}", subSqlStr, where);
        //    }
        //    return where;
        //}

        #endregion

        #region 流转明细
        /// <summary>
        /// 流转明细
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult FlowDetails(string instrumentId)
        {
            ViewBag.InstrumentId = instrumentId;
            return View();
        }

        public string GetFlowDetails(string instrumentId)
        {
            IList<Common.Models.InstrumentFlowModel> flowList = ServiceProvider.InstrumentFlowService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            foreach (var item in flowList)
            {
                sbData.Append("[");
                sbData.AppendFormat("\"{0}\"", string.IsNullOrEmpty(item.Reason) ? "" : item.Reason);
                sbData.AppendFormat(",\"{0}\"", item.Place);
                sbData.AppendFormat(",\"{0}\"", item.Creator);
                sbData.AppendFormat(",\"{0:yyyy-MM-dd HH:mm:ss}\"", item.CreateDate);
                sbData.Append("],");
            }
            if (flowList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }
        #endregion

        #region 仪器扫描
        public ActionResult InstrumentScan()
        {
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel receiveLocale = paramList.SingleOrDefault(m => m.ParamCode.Equals(Constants.SysParamType.InstrumentReceiveLocale));
            ViewBag.InstrumentReceiveLocale = new SelectList(receiveLocale.itemsList, "ParamItemName", "ParamItemName");
            return View();
        }

        public ActionResult Save(string barCode,InstrumentFlowModel model)
        {
            Instrument.Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetByBarCode(barCode);
            if (instrument == null)
                return Json(new { Msg = "Not Found" });
            string InstrumentInfo = "<b>仪器名称：</b>" + instrument.InstrumentName + " | <b>仪器型号：</b>" + instrument.Specification + " | <b>出厂编号：</b>" + instrument.SerialNo + " | <b>管理编号：</b>" + instrument.ManageNo;
            model.InstrumentId = instrument.InstrumentId;
            model.Creator = LoginHelper.LoginUser.UserName;
            model.Place = "";
            ServiceProvider.InstrumentFlowService.Save(model);
            return Json(new { Msg = "OK", InstrumentInfo = InstrumentInfo,InstrumentId=UtilsHelper.Encrypt(instrument.InstrumentId.ToString()) }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
