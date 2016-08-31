using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using System.Text;

using System.Collections;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using Instrument.Business;
using Instrument.Common.Models;
using Instrument.Common;
using Newtonsoft.Json.Linq;
using ToolsLib.Utility;
using ToolsLib.FileService;
using System.IO;
using System.Data;
using Global.Common.Models;

namespace Instrument.WebSite.Controllers
{
    public class SendInstrumentController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SendInstrumentController));
        #region 送检仪器选择
        public ActionResult InstrumentChoose()
        {
            ////todo:所属部门...........生成一个下拉框树所需的数据源
            //ViewBag.OrgList =Global.Business.ServiceProvider.OrgService.GetAll();
            ViewBag.JsonWarnDay = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.WarnDay).ToString();
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            ViewBag.JsonManageLevel = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.ManageLevel).ToString();
            ViewBag.JsonInstrumentRecordState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.InstrumentState).ToString();
            //ViewBag.JsonBranchCompany = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(UtilConstants.SysParamType.BranchCompany).ToString();
            return View();
        }

        /// <summary>
        /// 仪器列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllInstrumentJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InstrumentId,RecordState,InstrumentName,ManageNo,CertificateNo,ManageLevel,Specification,InstrumentCate,Manufacturer,SerialNo,DueStartDate,DueEndDate,LeaderName,BelongDepart,CreateDate,CreateUser";
            paging.Where = GetSearchCondition();
            IList<Hashtable> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListForPaging(paging);
            //系统参数
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //ParamModel branchCompany = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.BranchCompany);    //分公司
            //if (null == branchCompany) branchCompany = new ParamModel();
            //ParamItemModel mBranchCompany = null;
            ///设备状态
            IList<ParamItemModel> RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();

            Global.Common.Models.ParamItemModel mInstrumentCate = null;
            Global.Common.Models.ParamItemModel mManageLevel = null;

            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            Global.Common.Models.OrgModel belongDeptModel = new Global.Common.Models.OrgModel();

            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            DateTime dueEndDate;
            bool isOverTime = false;
            int warnDay = 0;
            foreach (var item in instrumentList)
            {
                if (!string.IsNullOrWhiteSpace(string.Format("{0}", item["DueEndDate"])))
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item["DueEndDate"]));
                else
                    dueEndDate = DateTime.MinValue;
                //预警天数
                isOverTime = dueEndDate < Convert.ToDateTime(string.Format("{0:d}", DateTime.Now));
                warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                warnDay = (warnDay < 0) ? 0 : warnDay;
                //超期无预警天数
                if (isOverTime) warnDay = 0;

                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<input type='checkbox'  name='chk' value='{0}' />", UtilsHelper.Encrypt(item["InstrumentId"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["DueEndDate"] == null ? "" : isOverTime ? "已超期" : "未超期");    //证书超期
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", (warnDay == 0) ? "无预警" : warnDay.ToString() + "天"));//预警天数
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["DueEndDate"]));    //到期日期
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item["RecordState"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mParamItem == null ? "" : mParamItem.ParamItemName);    //状态
                //管理级别	
                Global.Common.Models.ParamModel ManageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);    //设备分类
                mManageLevel = ManageLevel.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["ManageLevel"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mManageLevel == null ? "" : mManageLevel.ParamItemName);

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["InstrumentName"]));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));    //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["CertificateNo"]));    //证书编号
                //分类
                Global.Common.Models.ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
                mInstrumentCate = InstrumentCate.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["InstrumentCate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                ////分公司
                //mBranchCompany = branchCompany.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["BelongSubCompany"]));
                //dtm.aaData[dtm.aaData.Count - 1].Add(mBranchCompany == null ? "" : mBranchCompany.ParamItemName);
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Manufacturer"]));    //生产厂家
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == (item["BelongDepart"] == null ? "" : item["BelongDepart"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);//所属部门
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["DueStartDate"]));    //校准日期

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["LeaderName"]));    //保管人
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["StorePalce"]));    //存放地址
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["CreateUser"]));    //创建人
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
        /// 添加到待送仪器清单
        /// </summary>
        /// <param name="instrumentIds"></param>
        /// <returns></returns>
        public ActionResult AddToPreSendList(string instrumentIds)
        {
            var list = instrumentIds.Split(',');
            var instrumentIdList = list.Select(l => UtilsHelper.Decrypt2Int(l)).ToList();
            //当前用户下已加入清单但未送检的仪器
            var preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            var preInstrumentIdList = preSendList.Select(o => o.InstrumentId).ToList();
            StringBuilder where = new StringBuilder();
            where.Append("0,");
            foreach (var item in instrumentIdList)
            {
                where.Append(item + ",");
            }
            where.Remove(where.Length - 1, 1);
            //待送检的仪器列表
            var instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere("InstrumentId in (" + where + ")");
            foreach (var item in instrumentList)
            {
                if (preInstrumentIdList.Contains(item.InstrumentId)) continue;//已经送检的不再添加
                InstrumentWaitSendModel model = new InstrumentWaitSendModel();
                model.InstrumentId = item.InstrumentId;
                model.UserId = LoginHelper.LoginUser.UserId;
                ServiceProvider.InstrumentWaitSendService.Save(model);
            }
            return Content("OK");
        }


        #endregion

        #region 检索条件（公用）
        /// <summary>
        /// 检索条件
        /// </summary>
        /// <returns></returns>
        private string GetSearchCondition()
        {
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = "InstrumentForm=" + (int)Constants.InstrumentForm.仪器;
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition)) where = string.Format("{0} and {1}", dtm.FieldCondition, where);
            //所属部门
            string orgName = Request["searchBelongDepart"];
            where = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition(where, "InstrumentChoose-CheckAll", orgName);

            //预警天数
            string isWarnDay = Request["searchIsWarnDay"];
            if (!string.IsNullOrEmpty(isWarnDay))
            {
                int iWarnDay = Convert.ToInt32(isWarnDay);
                where = string.Format("{0} and DueEndDate<='{1:yyyy-MM-dd}' and DueEndDate>='{2:yyyy-MM-dd}'", where, DateTime.Now.AddDays(iWarnDay), DateTime.Now);
                //where = string.Format("{0} and DateDiff(dd,getdate(),DueEndDate)<={1} and DateDiff(dd,getdate(),DueEndDate)>=0", where, iWarnDay);
            }

            //是否超期
            string isOverTime = Request["searchIsOverTime"];
            if (!string.IsNullOrEmpty(isOverTime))
            {
                if (isOverTime.Equals("0"))
                    where = string.Format("{0} and '{1:yyyy-MM-dd}'<=DueEndDate", where, DateTime.Now);//未超期
                else
                    where = string.Format("{0} and '{1:yyyy-MM-dd}'>DueEndDate", where, DateTime.Now);//已超期
            }

            //所属部门
            //string orgName = Request["searchBelongDepart"];
            //if (!string.IsNullOrEmpty(orgName))
            //{
            //    IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll().Where(o => o.OrgName == orgName).ToList();
            //    string strFilte = "";
            //    foreach (Global.Common.Models.OrgModel org in orgList)
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
        //    string GetAllAuthorityStr = "InstrumentChoose-CheckAll";
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

        #region 待送检仪器
        public ActionResult PreSendList()
        {
            //ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            //ViewBag.JsonBranchCompany = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(UtilConstants.SysParamType.BranchCompany).ToString();
            ////todo:所属部门...........生成一个下拉框树所需的数据源
            //ViewBag.OrgList =Global.Business.ServiceProvider.OrgService.GetAll();
            return View();
        }
        /// <summary>
        /// 获取待送检列表
        /// </summary>
        /// <returns></returns>
        public string GetPreSendOrderJsonData()
        {
            IList<InstrumentWaitSendModel> preSendList = GetPreSendList();
            var instrumentList = ServiceProvider.InstrumentService.GetByIdList(preSendList.Select(p => p.InstrumentId).ToList());

            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            Global.Common.Models.OrgModel orgModel = new Global.Common.Models.OrgModel();

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"data\":[");
            DateTime dueEndDate;
            bool isOverTime;
            InstrumentModel instrument = null;
            foreach (var item in preSendList)
            {
                instrument = instrumentList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                if (instrument == null) continue;
                if (!string.IsNullOrWhiteSpace(string.Format("{0}", instrument.DueEndDate)))
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", instrument.DueEndDate));
                else
                    dueEndDate = DateTime.MinValue;
                //预警天数
                isOverTime = dueEndDate < Convert.ToDateTime(string.Format("{0:d}", DateTime.Now));


                sb.AppendFormat("[\"<input type='checkbox' name='chk' value='{0}' />\"", item.InstrumentId);
                sb.AppendFormat(",\"{0}\"", isOverTime ? "已超期" : "未超期");
                sb.AppendFormat(",\"{0}\"", instrument.InstrumentName);
                sb.AppendFormat(",\"{0}\"", instrument.ManageNo);
                sb.AppendFormat(",\"{0}\"", instrument.Specification);
                sb.AppendFormat(",\"{0:d}\"", dueEndDate);
                //所属部门
                orgModel = orgList.SingleOrDefault(o => o.OrgCode == instrument.BelongDepart);
                sb.AppendFormat(",\"{0}\"", orgModel == null ? "" : orgModel.OrgName);
                sb.AppendFormat(",\"{0}\"", item.Remark);
                sb.Append("],");
            }
            sb.Append("]}");
            if (instrumentList.Count > 0)
                sb.Remove(sb.Length - 3, 1);
            return sb.ToString();
        }

        private IList<InstrumentWaitSendModel> GetPreSendList()
        {
            IList<InstrumentWaitSendModel> preSendList = null;
            string GetAllAuthorityStr = "PreSendList-CheckAll";
            bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());
            if (!IsGetAllAuthority)
                preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            else
                preSendList = ServiceProvider.InstrumentWaitSendService.GetAll();
            return preSendList;
        }

        /// <summary>
        /// 导出待检仪器
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportPreSendList()
        {
            //查询标准器具
            IList<InstrumentWaitSendModel> preSendList = GetPreSendList();
            var instrumentList = ServiceProvider.InstrumentService.GetByIdList(preSendList.Select(p => p.InstrumentId).ToList());
            if (0 == preSendList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            DateTime dueEndDate;
            bool isOverTime;
            InstrumentModel instrument = null;
            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            Global.Common.Models.OrgModel orgModel = new Global.Common.Models.OrgModel();
            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("isOverTime", typeof(string));    //状态
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("ManageNo", typeof(string));   //资产编号
            dtData.Columns.Add("Specification", typeof(string));    //型号
            dtData.Columns.Add("DueEndDate", typeof(string));    //设备分类
            dtData.Columns.Add("OrgName", typeof(string));    //所属部门
            foreach (var item in instrumentList)
            {
                DataRow drData = dtData.NewRow();
                instrument = instrumentList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                if (instrument == null) continue;
                if (instrument.DueEndDate == null)
                {
                    isOverTime = false;
                }
                else
                {
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", instrument.DueEndDate));
                    //预警天数
                    isOverTime = DateTime.Now.CompareTo(dueEndDate) > 1 ? true : false;
                }
                drData["isOverTime"] = isOverTime ? "已超期" : "未超期";
                drData["InstrumentName"] = item.InstrumentName;
                drData["ManageNo"] = item.ManageNo;
                drData["Specification"] = item.Specification;
                drData["DueEndDate"] = item.DueEndDate;
                //所属部门
                orgModel = orgList.SingleOrDefault(o => o.OrgCode == instrument.BelongDepart);
                drData["OrgName"] = orgModel == null ? "" : orgModel.OrgName;
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
               "证书超期", "仪器名称", "管理编号", "仪器型号", "有效日期", "所属部门"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "待检仪器", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}待检仪器{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="instrumentIds"></param>
        /// <returns></returns>
        public ActionResult DeleteFromSendList(string instrumentIds)
        {
            try
            {
                var instrumentIdArr = instrumentIds.Split(',');
                var instrumentIdList = instrumentIdArr.Select(l => Convert.ToInt32(l)).ToList();
                ServiceProvider.InstrumentWaitSendService.DeleteByInstrumentIds(instrumentIdList, LoginHelper.LoginUser.UserId);
                return Content("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("操作失败");
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        /// <param name="instrumentIds"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public ActionResult RemarkFromSendList(string instrumentIds, string Remark)
        {
            try
            {
                var instrumentIdArr = instrumentIds.Split(',');
                var instrumentIdList = instrumentIdArr.Select(l => Convert.ToInt32(l)).ToList();
                ServiceProvider.InstrumentWaitSendService.UpdateRemark(instrumentIdList, Remark, LoginHelper.LoginUser.UserId);
                return Content("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("操作失败");
            }
        }

        /// <summary>
        /// 联系信息页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddContractInfo()
        {
            IList<ParamItemModel> companyInfo= Global.Business.ServiceProvider.ParamService.GetAll().FirstOrDefault(s=>s.ParamCode==Instrument.Common.Constants.SysParamType.CompanyInfo).itemsList;
            ViewBag.CompanyName = companyInfo.FirstOrDefault(s => s.ParamItemName == "公司名称").ParamItemValue;
            return View();
        }

        /// <summary>
        /// 送检
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendInstrument(string instrumentIds, FormCollection collection)
        {
            //接口数据组装
            JObject orderObj = new JObject();
            orderObj.Add("SendUser", LoginHelper.LoginUser.UserName);//送检人
            orderObj.Add("CertCompany", collection["CertCompany"]);
            orderObj.Add("CertAddress", collection["CertAddress"]);
            orderObj.Add("Contactor", collection["Contactor"]);
            orderObj.Add("Mobile", collection["Mobile"]);
            orderObj.Add("Tel", collection["Tel"]);
            orderObj.Add("Fax", collection["Fax"]);
            orderObj.Add("SendAddress", collection["SendAddress"]);
            orderObj.Add("Remark", collection["Remark"]);

            Global.Common.Models.OrgModel orgmodel = Global.Business.ServiceProvider.OrgService.GetByCode(GRGTCommonUtils.LoginHelper.LoginUser.BelongDepart);
            string result = ServiceProvider.OrderSendInstrumentService.SendOrder(instrumentIds, orderObj.ToString(), orgmodel == null ? "" : orgmodel.OrgName, LoginHelper.LoginUser.UserName, LoginHelper.LoginUser.UserId);
            //删除待送检清单
            if (result.Equals("OK"))
            {
                ServiceProvider.InstrumentWaitSendService.DeleteByInstrumentIds(instrumentIds.Split(',').Select(l => Convert.ToInt32(l)).ToList(), LoginHelper.LoginUser.UserId);
            }
            return Content(result);
        }

        #endregion

        #region 已送检仪器
        public ActionResult SendList()
        {
            ////todo:所属部门...........生成一个下拉框树所需的数据源
            //ViewBag.OrgList =Global.Business.ServiceProvider.OrgService.GetAll();

            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            //ViewBag.JsonBranchCompany = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(UtilConstants.SysParamType.BranchCompany).ToString();
            return View();
        }
        /// <summary>
        /// 获取已送检列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSendOrderJsonData()
        {
            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            Global.Common.Models.OrgModel orgModel = new Global.Common.Models.OrgModel();

            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"AutoId,OrderId,InstrumentId,InstrumentName,Specification,CertificationNumber,SerialNo,ManageNo,IsCompleteCert,IsComplete";
            //paging.Where = GetSearchCondition() + " and UserId=" + LoginHelper.LoginUser.UserId; //当前用户待送检仪器
            #region***注释***
            //             paging.Where = "1=1";
            //             if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            //             {
            //                 paging.Where = dtm.FieldCondition;
            //             }
            //            
            //             string condition2 = "1=1";
            // 
            //              string GetAllAuthorityStr = "SendList-CheckAll";
            //             bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());
            // 
            //             if (!IsGetAllAuthority)
            //             {
            //                 condition2 = "UserId=" + LoginHelper.LoginUser.UserId; //当前用户待送检仪器
            //             }
            // 
            //             //所属部门
            //             string strFilte = "";
            //             string orgName = Request["searchBelongDepart"];
            //             if (!string.IsNullOrEmpty(orgName))
            //             {
            //                 IList<Global.Common.Models.OrgModel> newOrgList = orgList.Where(o => o.OrgName == orgName).ToList();
            // 
            //                 foreach (Global.Common.Models.OrgModel org in newOrgList)
            //                 {
            //                     if (string.IsNullOrWhiteSpace(strFilte))
            //                         strFilte = string.Format("BelongDepart = '{0}'", org.OrgCode);
            //                     else
            //                         strFilte = string.Format("{0} or BelongDepart = '{1}'", strFilte, org.OrgCode);
            //                 }
            //             }
            //             if (string.IsNullOrWhiteSpace(strFilte)) strFilte = "1=1";
            // 
            //             //添加单号查询条件.
            //             string orderParam = Request["OrderParam"];
            //             if (!string.IsNullOrEmpty(orderParam))
            //                 paging.Where = string.Format("{0} and (OrderId In (Select OrderId From Order_BaseInfo Where {1} and {2}))", paging.Where, condition2, orderParam);
            //             else
            //                 paging.Where = string.Format(" {0} and (OrderId In (Select OrderId From Order_BaseInfo Where {1}))", paging.Where, condition2);
            //             //添加仪器查询条件.
            //             string instrumentParam = Request["InstrumentParam"];
            //             if (!string.IsNullOrEmpty(instrumentParam))
            //                 paging.Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {1} and ({2})))", paging.Where, instrumentParam,strFilte);
            //             else if (strFilte != "1=1")
            //                 paging.Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {1}))", paging.Where, strFilte);
            #endregion
            paging.Where = GetSendListCondition(dtm, orgList);
            IList<Hashtable> sendInstrumentList = ServiceProvider.OrderSendInstrumentService.GetAllSendListForPaging(paging);

            IList<int> instrumentIds = sendInstrumentList.Select(s => Convert.ToInt32(s["InstrumentId"])).Distinct().ToList();
            IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);

            IList<int> orderIds = sendInstrumentList.Select(s => Convert.ToInt32(s["OrderId"])).Distinct().ToList();
            IList<OrderModel> orderList = ServiceProvider.OrderService.GetByIdList(orderIds);

            //系统参数
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            //ParamModel branchCompany = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.BranchCompany);    //分公司
            //if (null == branchCompany) branchCompany = new ParamModel();
            //ParamItemModel mBranchCompany = null;

            Global.Common.Models.ParamItemModel mInstrumentCate = null;



            InstrumentModel instrumentModel;
            OrderModel orderModel;

            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            //DateTime dueEndDate;
            foreach (var item in sendInstrumentList)
            {
                //dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item["DueEndDate"]));
                dtm.aaData.Add(new List<string>());
                //sbOperate.Clear();    //操作
                //sbOperate.AppendFormat("<input type='checkbox'  name='chk' value='{0}' />", UtilsHelper.Encrypt(item["AutoId"].ToString()));
                //dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                orderModel = orderList.SingleOrDefault(o => o.OrderId == Convert.ToInt32(item["OrderId"]));
                instrumentModel = instrumentList.SingleOrDefault(i => i.InstrumentId == Convert.ToInt32(item["InstrumentId"]));

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", orderModel.OrderNumber));    //送检单号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["InstrumentName"]));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", Convert.ToBoolean(item["IsCompleteCert"]) ? "完工" : "未完工"));    //证书完工
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", Convert.ToBoolean(item["IsComplete"]) ? "完工" : "未完工"));     //试验完工
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));    //管理编号

                mInstrumentCate = InstrumentCate.itemsList.SingleOrDefault(t => t.ParamItemValue == instrumentModel.InstrumentCate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));    //出厂编号
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", instrumentModel..ManuDate));    //出厂日期
                //所属部门
                orgModel = orgList.SingleOrDefault(o => o.OrgCode == instrumentModel.BelongDepart);
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", orgModel == null ? "" : orgModel.OrgName));    //所属部门
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", orderModel.SendDate));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Remark"])); 
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

        public ActionResult ExportSendList()
        {

            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            string where = GetSendListCondition(dtm, orgList);
            IList<OrderSendInstrumentModel> sendInstrumentList = ServiceProvider.OrderSendInstrumentService.GetAllSendInstrumentListByWhere(where);

            if (0 == sendInstrumentList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            Global.Common.Models.OrgModel orgModel = new Global.Common.Models.OrgModel();

            IList<int> instrumentIds = sendInstrumentList.Select(s => s.InstrumentId).Distinct().ToList();
            IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);

            IList<int> orderIds = sendInstrumentList.Select(s => s.OrderId).Distinct().ToList();
            IList<OrderModel> orderList = ServiceProvider.OrderService.GetByIdList(orderIds);

            //系统参数
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            Global.Common.Models.ParamItemModel mInstrumentCate = null;
            InstrumentModel instrumentModel;
            OrderModel orderModel;

            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("OrderNumber", typeof(string));    //送检单号
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("IsCompleteCert", typeof(string));   //证书完工
            dtData.Columns.Add("IsComplete", typeof(string));   //试验完工
            dtData.Columns.Add("Specification", typeof(string));    //型号
            dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
            dtData.Columns.Add("InstrumentCate", typeof(string));    //分类
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
            dtData.Columns.Add("OrgName", typeof(string));    //所属部门
            dtData.Columns.Add("Remark",typeof(string));    //备注
            foreach (var item in sendInstrumentList)
            {
                DataRow drData = dtData.NewRow();
                orderModel = orderList.SingleOrDefault(o => o.OrderId == item.OrderId);
                instrumentModel = instrumentList.SingleOrDefault(i => i.InstrumentId == item.InstrumentId);
                mInstrumentCate = InstrumentCate.itemsList.SingleOrDefault(t => t.ParamItemValue == instrumentModel.InstrumentCate.ToString());
                orgModel = orgList.SingleOrDefault(o => o.OrgCode == instrumentModel.BelongDepart);
                drData["OrderNumber"] = orderModel == null ? "" : orderModel.OrderNumber;
                drData["InstrumentName"] = item.InstrumentName;
                drData["IsCompleteCert"] = item.IsCompleteCert ? "完工" : "未完工";
                drData["IsComplete"] = item.IsComplete ? "完工" : "未完工";
                drData["Specification"] = item.Specification;
                drData["ManageNo"] = item.ManageNo;
                drData["InstrumentCate"] = mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName;
                drData["SerialNo"] = item.SerialNo;
                drData["OrgName"] = orgModel == null ? "" : orgModel.OrgName;
                drData["Remark"] = item.Remark;


                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
               "送检单号", "仪器名称", "证书完工", "试验完工", "仪器型号", "管理编号","分类","出厂编号","所属部门"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "已送仪器", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}已送仪器{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        private string GetSendListCondition(DataTableUtils.DataTableModel dtm, IList<Global.Common.Models.OrgModel> orgList)
        {

            string Where = "1=1";
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                Where = dtm.FieldCondition;
            }

            string condition2 = "1=1";

            string GetAllAuthorityStr = "SendList-CheckAll";
            bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

            if (!IsGetAllAuthority)
            {
                condition2 = "UserId=" + LoginHelper.LoginUser.UserId; //当前用户待送检仪器
            }

            //所属部门
            string strFilte = "";
            string orgName = Request["searchBelongDepart"];
            if (!string.IsNullOrEmpty(orgName))
            {
                IList<Global.Common.Models.OrgModel> newOrgList = orgList.Where(o => o.OrgName == orgName).ToList();

                foreach (Global.Common.Models.OrgModel org in newOrgList)
                {
                    if (string.IsNullOrWhiteSpace(strFilte))
                        strFilte = string.Format("BelongDepart like '{0}%'", org.OrgCode);
                    else
                        strFilte = string.Format("{0} or BelongDepart like '{1}%'", strFilte, org.OrgCode);
                }
            }
            if (string.IsNullOrWhiteSpace(strFilte)) strFilte = "1=1";

            //添加单号、送检日期查询条件.
            IList<string> orderFilter = new List<string>();
            string orderParam = Request["OrderParam"];
            string sendDateParam = Request["SendDate"];
            if (!string.IsNullOrEmpty(orderParam)) orderFilter.Add(string.Format("{0}", orderParam));
            if (!string.IsNullOrEmpty(sendDateParam)) orderFilter.Add(string.Format("{0}", sendDateParam));
            if (orderFilter.Count>0)
                Where = string.Format("{0} and (OrderId In (Select OrderId From Order_BaseInfo Where {1} and {2}))", Where, condition2, string.Join(" and ",orderFilter));
            else
                Where = string.Format(" {0} and (OrderId In (Select OrderId From Order_BaseInfo Where {1}))", Where, condition2);

            //添加仪器查询条件.
            string instrumentParam = Request["InstrumentParam"];
            if (!string.IsNullOrEmpty(instrumentParam))
                Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {1} and ({2})))", Where, instrumentParam, strFilte);
            else if (strFilte != "1=1")
                Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {1}))", Where, strFilte);

            return Where;
        }
        #endregion

        #region 已送检批次
        public ActionResult OrderList()
        {
            return View();
        }

        /// <summary>
        /// 获取已送检列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrderListJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"OrderId,OrderNumber,QuotationNumber,UserId,SendUser,SendDate,InstrumentCount,DownloadCertState,DownloadDate,ReceivedState,ReceivedUser,ReceivedDate,UpdateState,UpdateDate,CreateDate,CreateUser";
            paging.Where = string.IsNullOrEmpty(dtm.FieldCondition) ? "1=1" : dtm.FieldCondition;
            IList<Hashtable> orderList = ServiceProvider.OrderService.GetAllOrderListForPaging(paging);

            IList<int> orderIds = orderList.Select(s => Convert.ToInt32(s["OrderId"])).Distinct().ToList();
            IList<OrderSendInstrumentModel> sendInstrumentList = ServiceProvider.OrderSendInstrumentService.GetByOrderIdList(orderIds);

            bool IsReceived = false;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            foreach (var item in orderList)
            {
                IsReceived = Convert.ToBoolean(item["ReceivedState"]);
                dtm.aaData.Add(new List<string>());
                sbOperate.AppendFormat("<div orderId='{0}' >", UtilsHelper.Encrypt(item["OrderId"].ToString()));
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/SendInstrument/OrderScheduleList".ToLower()))
                {
                    sbOperate.Append("<a href='#' onclick='fnOrderSchedule(this);'>查看进度</a>&nbsp;&nbsp;");
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/SendInstrument/DownloadCertification".ToLower()) && IsReceived)
                {
                    sbOperate.Append("<a href='#' onclick='fnDownloadCert(this);'>证书下载</a>&nbsp;&nbsp;");
                    //sbOperate.AppendFormat("<a target='_blank' href='/SendInstrument/DownloadCertification?orderId={0}'>证书下载</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(item["OrderId"].ToString()));
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/SendInstrument/UpdateOrderState".ToLower()) && !IsReceived)
                {
                    sbOperate.Append("<a href='#' onclick='fnUpdateState(this);'>受理更新</a>&nbsp;&nbsp;");
                    sbOperate.Append("<a href='#' onclick='RemoveOrder(this);'>撤销送检单</a>&nbsp;&nbsp;");
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/SendInstrument/SynSendInstrumentCompleteState".ToLower()) && IsReceived)
                {
                    sbOperate.Append("<a href='#' onclick='fnSynCertState(this);'>同步周期校准记录</a>&nbsp;&nbsp;");
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/SendInstrument/PayOrder".ToLower()) && IsReceived)
                {
                    sbOperate.Append("<a href='#' onclick='fnPayOrder(this);'>在线支付</a>&nbsp;");
                }
                sbOperate.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString()); sbOperate.Clear();
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["OrderNumber"]));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["QuotationNumber"]));    //GRGT报价单号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", Convert.ToBoolean(item["ReceivedState"]) ? "已受理" : "未受理"));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["ReceivedDate"] == null ? "" : item["ReceivedDate"]));    //
                ////下载证书状态
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["InstrumentCount"]));    //仪器数量
                //已同步仪器数量
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", sendInstrumentList.Where(s => s.OrderId==int.Parse(string.Format("{0}",item["OrderId"])) && s.IsCompleteCert == true).Count())); 
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SendUser"]));    //送检人
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["IsCompleteCert"]));    //送检部门
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:d}", item["SendDate"]));     //送检时间
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
        /// 撤销送检
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveOrder(string orderId)
        {
            OrderModel model = ServiceProvider.OrderService.GetById(UtilsHelper.Decrypt2Int(orderId));
            string msg = "OK";
            string jsonData = GRGTCommonUtils.WSProvider.EbusinessProvider.RemoveOrder(model.OrderNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            if (dic["Msg"].ToString().Equals("OK"))
            {
                IList<OrderSendInstrumentModel> sourceList = ServiceProvider.OrderSendInstrumentService.GetByOrderId(UtilsHelper.Decrypt2Int(orderId));
                InstrumentWaitSendModel instrumentWaitSendModel = new InstrumentWaitSendModel();
                var preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
                var preInstrumentIdList = preSendList.Select(o => o.InstrumentId).ToList();
                foreach (var item in sourceList)
                {
                    if (preInstrumentIdList.Contains(item.InstrumentId)) continue;//已经送检的不再添加
                    instrumentWaitSendModel = new InstrumentWaitSendModel();
                    instrumentWaitSendModel.InstrumentId = item.InstrumentId;
                    instrumentWaitSendModel.UserId = LoginHelper.LoginUser.UserId;
                    ServiceProvider.InstrumentWaitSendService.Save(instrumentWaitSendModel);
                }
                ServiceProvider.OrderService.DeleteById(UtilsHelper.Decrypt2Int(orderId));
            }
            else
            {
                string result = dic["Data"].ToString();
                //result = "OK_administrator_2015-5-8";
                model.ReceivedState = true;
                model.QuotationNumber = result.Split('_')[0];
                model.ReceivedDate = Convert.ToDateTime(result.Split('_')[1]);
                model.UpdateDate = DateTime.Now;
                model.UpdateState = true;
                ServiceProvider.OrderService.UpdateReceivedInfo(model);
                msg = "送检已经受理，不能撤销！";
            }
            return Content(msg);
        }

        public ActionResult ExportDataBySearchCondition()
        {

            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = "1=1";
            string GetAllAuthorityStr = "Order-CheckAll";
            bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
                where = dtm.FieldCondition;

            if (!IsGetAllAuthority)
            {
                //获取当前用户
                if (!string.IsNullOrWhiteSpace(where))
                    where = string.Format(" {0} and {1}", where, " UserId=" + LoginHelper.LoginUser.UserId);
                else
                    where = " UserId=" + LoginHelper.LoginUser.UserId;
            }
            IList<OrderModel> instrumentList = ServiceProvider.OrderService.GetAllOrderListByWhere(where);


            DataTable dtData = new DataTable();
            dtData.Columns.Add("OrderNumber", typeof(string));    //送检单号
            dtData.Columns.Add("QuotationNumber", typeof(string));    //报价单号
            dtData.Columns.Add("ReceivedState", typeof(string));   //受理状态
            dtData.Columns.Add("ReceivedDate", typeof(string));   //受理时间
            dtData.Columns.Add("InstrumentCount", typeof(string));    //仪器数量
            dtData.Columns.Add("SendUser", typeof(string));    //送检人
            dtData.Columns.Add("SendDate", typeof(string));    //送检时间

            foreach (var item in instrumentList)
            {
                DataRow drData = dtData.NewRow();

                drData["OrderNumber"] = item.OrderNumber;
                drData["QuotationNumber"] = item.QuotationNumber;
                drData["ReceivedState"] = item.ReceivedState ? "已受理" : "未受理";
                drData["ReceivedDate"] = item.ReceivedDate;
                drData["InstrumentCount"] = item.InstrumentCount;
                drData["SendUser"] = item.SendUser;
                drData["SendDate"] = item.SendDate;


                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
               "送检单号", "受理人", "受理状态", "受理时间", "仪器数量", "送检人","送检时间"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "进度查询", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}进度查询{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        /// <summary>
        /// 查看进度
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult OrderScheduleList(string orderId)
        {
            return View();
        }

        /// <summary>
        /// 订单下所有仪器
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string GetOrderScheduleList(string orderId)
        {
            IList<Hashtable> sendList = new List<Hashtable>();
            //获取已送检批次仪器列表
            var orderModel = ServiceProvider.OrderService.GetById(UtilsHelper.Decrypt2Int(orderId));
            if (orderModel.ReceivedState)
            {
                //业务系统中的送检仪器
                string jsonData = WSProvider.MeasureLabProvider.QuotationInstrumentSearch(orderModel.QuotationNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
                Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
                if (dic["Msg"].ToString() == "OK")
                    sendList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(IList<Hashtable>)) as IList<Hashtable>;
                else
                    return "{\"data\":[]}";
            }
            //仪器管理系统中的送检单下仪器的ItemCode
            IList<OrderSendInstrumentModel> sourceList = ServiceProvider.OrderSendInstrumentService.GetByOrderId(UtilsHelper.Decrypt2Int(orderId));
            List<string> sendItemCodes = sendList.Select(s => s["ItemCode"].ToString()).ToList();
            List<string> sourceItemCodes = sourceList.Select(s => s.ItemCode).ToList();

            List<string> unionList = sendItemCodes.Union(sourceItemCodes).ToList();//并集(所有仪器)
            List<string> ownList = sourceItemCodes.Except(sendItemCodes).ToList();//（仅在仪器管理系统，不在业务系统的集合）
            List<string> intersectList = sendItemCodes.Intersect(sourceItemCodes).ToList();//交集

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"data\":[");
            string InstrumentType = "";
            OrderSendInstrumentModel model = null;
            foreach (string itemCode in unionList)
            {
                model = null;
                //在仪器管理系统中，不在业务系统中的仪器
                if (ownList.Contains(itemCode))
                {
                    //已受理,说明仪器被删除，状态为删除，未受理，正常
                    if (orderModel.ReceivedState) InstrumentType = "删除";
                    else InstrumentType = "正常";
                    model = sourceList.SingleOrDefault(s => s.ItemCode.Equals(itemCode));
                }
                else if (intersectList.Contains(itemCode))
                {
                    InstrumentType = "正常";//送检过去的仪器
                    model = sourceList.SingleOrDefault(s => s.ItemCode.Equals(itemCode));
                }
                else
                    InstrumentType = "新增";//业务系统新增的仪器

                if (model != null)
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentDetail"))
                        sb.AppendFormat("[\"<a href='#' onclick='fnDetails(this)' instrumentId='{0}'>详细</a>&nbsp;&nbsp;\"", UtilsHelper.Encrypt(model.InstrumentId.ToString()));
                    else
                        sb.Append("[\"\"");
                    sb.AppendFormat(",\"{0}\"", InstrumentType);
                    sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(model.InstrumentName));
                    sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(model.Specification));
                    sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(model.SerialNo));
                    sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(model.ManageNo));
                    if (intersectList.Contains(model.ItemCode))//两边都有的，显示送检过去的报价
                    {
                        Hashtable ht = sendList.SingleOrDefault(s => s["ItemCode"].Equals(itemCode));
                        sb.AppendFormat(",\"{0:C2}\"", ht["ServiceFee"]);
                    }
                    else
                        sb.AppendFormat(",\"{0:C2}\"", model.PerPrice);
                    sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(model.Remark));
                }
                else
                {
                    Hashtable ht = sendList.SingleOrDefault(s => s["ItemCode"].Equals(itemCode));
                    sb.Append("[\"\"");//操作
                    sb.AppendFormat(",\"{0}\"", InstrumentType);
                    sb.AppendFormat(",\"{0}\"", ht["InstrumentName"]);
                    sb.AppendFormat(",\"{0}\"", ht["Specification"]);
                    sb.AppendFormat(",\"{0}\"", ht["SerialNo"]);
                    sb.AppendFormat(",\"{0}\"", ht["ManageNo"]);
                    sb.AppendFormat(",\"{0:C2}\"", ht["ServiceFee"]);
                    sb.AppendFormat(",\"{0}\"", ht["Remark"]);
                }

                sb.Append("],");
            }
            sb.Append("]}");
            if (unionList.Count > 0)
                sb.Remove(sb.Length - 3, 1);
            return sb.ToString();
        }

        /// <summary>
        /// 更新仪器受理状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult UpdateOrderState(string orderId)
        {
            OrderModel model = ServiceProvider.OrderService.GetById(UtilsHelper.Decrypt2Int(orderId));
            string msg = "OK";
            string jsonData = GRGTCommonUtils.WSProvider.EbusinessProvider.UpdateOrderState(model.OrderNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;

            if (dic["Msg"].ToString().Equals("OK"))
            {
                string result = dic["Data"].ToString();
                //result = "OK_administrator_2015-5-8";
                model.ReceivedState = true;
                model.QuotationNumber = result.Split('_')[0];
                model.ReceivedDate = Convert.ToDateTime(result.Split('_')[1]);
                model.UpdateDate = DateTime.Now;
                model.UpdateState = true;
                ServiceProvider.OrderService.UpdateReceivedInfo(model);
            }
            else
                msg = dic["Msg"].ToString();
            return Content(msg);
        }

        /// <summary>
        /// 同步周期校准记录，同时下载已完工的证书，上传到文件服务器
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult SynSendInstrumentCompleteState(string orderId)
        {
            OrderModel order = ServiceProvider.OrderService.GetById(UtilsHelper.Decrypt2Int(orderId));
            if (!order.ReceivedState) return Content("送检单未受理，请先更新受理状态！");
            IList<OrderSendInstrumentModel> sendInstrumentList = ServiceProvider.OrderSendInstrumentService.GetByOrderId(UtilsHelper.Decrypt2Int(orderId)).Where(s => !s.IsCompleteCert).ToList();
            string jsonData = GRGTCommonUtils.WSProvider.MeasureLabProvider.GetCertInfoByQuotationNumber(order.QuotationNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            IList<Hashtable> certificationInfoList = new List<Hashtable>();
            if (dic["Msg"].ToString() == "OK")
            {
                certificationInfoList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(IList<Hashtable>)) as IList<Hashtable>;
                OrderSendInstrumentModel sendInstrumentModel = null;

                foreach (var item in certificationInfoList)
                {
                    sendInstrumentModel = sendInstrumentList.SingleOrDefault(s => s.ItemCode.Equals(item["ItemCode"].ToString()) );
                    if (sendInstrumentModel == null) continue;

                    sendInstrumentModel.IsComplete = true;
                    sendInstrumentModel.IsCompleteCert = true;
                    ServiceProvider.OrderSendInstrumentService.UpdateIsComplete(sendInstrumentModel);

                    //新增周期校准记录
                    InstrumentCertificationModel certModerl = new InstrumentCertificationModel();

                    certModerl.InstrumentId = sendInstrumentModel.InstrumentId;
                    certModerl.CheckDate = Convert.ToDateTime(item["DueStartDate"]);
                    certModerl.EndDate = Convert.ToDateTime(item["DueEndDate"]);
                    certModerl.CertificationCode = item["CertificateNumber"].ToString();
                    certModerl.MeasureOrg = "广电计量";
                    certModerl.CreateUser = LoginHelper.LoginUser.UserName;
                    certModerl.ItemCode = Guid.NewGuid().ToString();
                    certModerl.RecordState = Constants.InstrumentCertificationState.完成周检.GetHashCode();
                    ServiceProvider.InstrumentCertificationService.Insert(certModerl);

                }
            }

            return Json(new { Msg = dic["Msg"].ToString(), Data =  certificationInfoList.Count  }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载证书-根据送检单号打包下载所有已完工证书
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult DownloadCertification(string orderId)
        {
            var orderModel = ServiceProvider.OrderService.GetById(UtilsHelper.Decrypt2Int(orderId));
            byte[] bytes = WSProvider.MeasureLabProvider.BatchDownloadCertificationByQuotationNumber(orderModel.QuotationNumber, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            if (bytes.Length > 0)
            {
                MemoryStream stream = new MemoryStream(bytes);
                WebServer.DownLoadFile(stream, orderModel.QuotationNumber + ".zip");
                return Content("OK");
            }
            return Content("无证书可下载");
        }
        public ActionResult SynSendInstrumentLog(string OrderId,int Currcount)
        {
            int succeed = 0;
            ViewBag.RowsData = GetSynSendInstrumentLog(OrderId, ref succeed);
            ViewBag.TitleInfo = string.Format("已成功同步{0}条记录，本次成功同步{1}条", succeed, Currcount);
            return View(); }
        public string GetSynSendInstrumentLog(string OrderId,ref int succeed)
        {
            IList<OrderSendInstrumentModel> list = ServiceProvider.OrderSendInstrumentService.GetByOrderId(UtilsHelper.Decrypt2Int(OrderId));
           
            StringBuilder sb = new StringBuilder();
           
            sb.Insert(0, "[");
            foreach (OrderSendInstrumentModel item in list)
            {
                sb.Append("[");
                sb.AppendFormat("\"{0}\"", item.IsCompleteCert ? "已同步" : "未同步");
                sb.AppendFormat(",\"{0}\"", item.InstrumentName); //仪器名称
                sb.AppendFormat(",\"{0}\"", item.Specification);//仪器型号
                sb.AppendFormat(",\"{0}\"", item.SerialNo);//出厂编号 
                sb.AppendFormat(",\"{0}\"", item.ManageNo);//管理编号

                sb.Append("],");
                if (item.IsCompleteCert)
                    succeed++;
            }

            if (list.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return sb.ToString();
        }
        #endregion

    }
}
