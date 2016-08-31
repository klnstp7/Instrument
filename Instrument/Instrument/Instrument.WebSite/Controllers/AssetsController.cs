using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using System.Text;
using Global.Common.Models;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using System.Collections;
using Instrument.Business;
using System.Data;
using Instrument.Common;
using Instrument.Common.Models;
using ToolsLib.FileService;
using ToolsLib.Utility;
using System.IO;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Instrument.WebSite.Controllers
{
    public class AssetsController : Controller
    {

        #region 资产维护
        public ActionResult Index()
        {
            ViewBag.JsonInstrumentState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.AssetsState).ToString();
            ViewBag.JsonCalibrationType = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.CalibrationType).ToString();
            return View();
        }

        /// <summary>
        /// 编辑资产
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <param name="planDetailId">盘点明细标识，默认为null，在盘盈的时候传入用于初始化资产信息</param>
        /// <returns></returns>
        public ActionResult Edit(string instrumentId, string planDetailId=null)
        {
            Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(UtilsHelper.Decrypt2Int(instrumentId));
            if (model == null) model = new Common.Models.InstrumentModel(); model.BelongDepart = "GRGT";
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //资产属性
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ViewBag.CalibrationTypeList = new SelectList(calibrationType.itemsList, "ParamItemValue", "ParamItemName");
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.AssetsState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ViewBag.InstrumentRecordStateList = new SelectList(instrumentState.itemsList, "ParamItemValue", "ParamItemName");
            //所属部门(生成一个下拉框树所需的数据源)
            ViewBag.BelongDepartList = Global.Business.ServiceProvider.OrgService.GetAll();
            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            //加载一级分类
            IList<ParamItemModel> cateList = instrumentCate.itemsList.Where(c => c.ParentCode == "0").ToList();
            ViewBag.InstrumentCateList = new SelectList(cateList, "ParamItemValue", "ParamItemName");
            //二级分类
            ViewBag.SubInstrumentCateList = new SelectList(instrumentCate.itemsList.Where(c => Convert.ToInt32(c.ParentCode) > 0), "ParamItemValue", "ParamItemName");
            //盘盈操作初始化数据(盘盈操作)
            if (planDetailId != null)
            {
                AssetCheckPlanDetailModel planDetailModel = ServiceProvider.AssetCheckPlanDetailService.GetById(UtilsHelper.Decrypt2Int(planDetailId));
                if (planDetailModel != null)
                {
                    model.InstrumentName = planDetailModel.InstrumentName;
                    model.Specification = planDetailModel.Specification;
                    model.BelongDepart = planDetailModel.BelongDepart == null ? "" : planDetailModel.BelongDepart;
                    model.Manufacturer = planDetailModel.Manufacturer;
                    model.SerialNo = planDetailModel.SerialNo;
                    model.ManageNo = planDetailModel.ManageNo;
                    model.AssetsNo = planDetailModel.AssetsNo;
                    model.Remark = planDetailModel.Remark;
                }
            }
            return View(model);
        }


        #endregion

        #region 资产查询
        public ActionResult AssetsList()
        {
            ViewBag.JsonInstrumentState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.AssetsState).ToString();
            ViewBag.JsonCalibrationType = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.CalibrationType).ToString();
            return View();
        }

        #endregion

        #region 导出
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult ExportDataBySearchCondition()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = GetSearchCondition(dtm,Constants.InstrumentForm.固定资产.GetHashCode());
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(where);
            if (0 == instrumentList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel CalibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);    //资产属性
            ParamItemModel mCalibrationType = null;
            //设备状态
            IList<ParamItemModel> RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetsState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();

            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("RecordState", typeof(string));    //状态
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("AssetsNo", typeof(string));   //资产编号
            dtData.Columns.Add("Specification", typeof(string));    //型号
            dtData.Columns.Add("BarCode", typeof(string));    //条码
            dtData.Columns.Add("CalibrationType", typeof(string));    //资产属性
            dtData.Columns.Add("BelongDepart", typeof(string));    //所属部门
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
            dtData.Columns.Add("ManageNo", typeof(string)); 
            dtData.Columns.Add("BuyDate", typeof(string));  
            dtData.Columns.Add("Price", typeof(decimal));  
            dtData.Columns.Add("Manufacturer", typeof(string)); 
            dtData.Columns.Add("ManufactureContactor", typeof(string));
            dtData.Columns.Add("LeaderName", typeof(string));
            dtData.Columns.Add("StorePalce", typeof(string));  
            dtData.Columns.Add("LastCheckDate", typeof(string));
            dtData.Columns.Add("LastCheckUser", typeof(string)); 
            foreach (var item in instrumentList)
            {

                DataRow drData = dtData.NewRow();
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item.RecordState));
                drData["RecordState"] =string.Format("{0}", mParamItem == null ? "" : mParamItem.ParamItemName);   //状态
                drData["InstrumentName"] = string.Format("{0}",item.InstrumentName);    //仪器名称
                drData["AssetsNo"] = string.Format("{0}",item.AssetsNo);
                drData["Specification"] =string.Format("{0}", item.Specification);    //型号
                drData["BarCode"] =string.Format("{0}", item.BarCode);    //BarCode
                //分类
                mCalibrationType = CalibrationType.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item.CalibrationType));
                drData["CalibrationType"] = string.Format("{0}", mCalibrationType == null ? "" : mCalibrationType.ParamItemName);
                //所属部门
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == item.BelongDepart);
                drData["BelongDepart"] = string.Format("{0}", belongDeptModel == null ? "" : OrgHelper.GetOrgNameTreeByOrgId(belongDeptModel.ParentOrgId, orgList, belongDeptModel.OrgName));

                drData["SerialNo"] = string.Format("{0}", item.SerialNo);    //出厂编号
                drData["ManageNo"] = string.Format("{0}", item.ManageNo);
                drData["BuyDate"] = string.Format("{0:yyyy-MM-dd}", item.BuyDate);
                drData["Price"] = string.Format("{0:F2}", item.Price);
                drData["Manufacturer"] = string.Format("{0}", item.Manufacturer);    //生产厂家
                drData["ManufactureContactor"] = string.Format("{0}", item.ManufactureContactor);    //厂家联系信息
                drData["LeaderName"] = string.Format("{0}", item.LeaderName);
                drData["StorePalce"] = string.Format("{0}", item.StorePalce);
                drData["LastCheckDate"] = string.Format("{0:yyyy-MM-dd}", item.LastCheckDate);
                drData["LastCheckUser"] = string.Format("{0}", item.LastCheckUser);
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
               "状态", "仪器名称", "资产编号", "型号规格", "条码", "资产属性", "所属部门","出厂编号","管理编号", "购置日期", "购置金额", "生产厂家", "厂家联系信息", "保管人", "存放地点", "最近盘点时间", "最近盘点人"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "固定资产", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}固定资产{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        /// <summary>
        /// 盘点计划相关仪器导出Export
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public void ExportCheckPlanDetail(int planId,string type)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //PagingModel paging = new PagingModel();
            //paging.FieldShow = @"PlanDetailId,PlanId,InstrumentId,Statuse,BelongDepart,InstrumentName,BarCode,Specification,Manufacturer,SerialNo,ManageNo,AssetsNo,Remark,CreateUser,CreateDate,Checkor,CheckDate";
            string where = "PlanId=" + planId;
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition)) where = string.Format("{0} and {1}", dtm.FieldCondition, where);
            string fileName = Instrument.Business.ServiceProvider.AssetCheckPlanDetailService.CreateExcel(planId, where);
            if (!string.IsNullOrEmpty(fileName))
            {
                string[] showName = fileName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                ToolsLib.FileService.WebServer.DownLoadFile(showName[0], showName[1], true);
            }
            else
            {
                Response.Write("<script type='text/javascript'>alert('模板不存在,无法导出Excel')</script>");
                Response.Write("<script type='text/javascript'>window.history.go(-1) </script>");
            }
        }
        /// <summary>
        /// 盘点计划盘盈仪器导出Export
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public void ExportForOverAge(string planId)
        {
            int PlanID = UtilsHelper.Decrypt2Int(planId);
            string fileName = Instrument.Business.ServiceProvider.AssetCheckPlanDetailService.CreateExcelForOverAge(PlanID);
            if (!string.IsNullOrEmpty(fileName))
            {
                string[] showName = fileName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                ToolsLib.FileService.WebServer.DownLoadFile(showName[0], showName[1], true);
            }
            else
            {
                Response.Write("<script type='text/javascript'>alert('模板不存在,无法导出Excel')</script>");
                Response.Write("<script type='text/javascript'>window.history.go(-1) </script>");
            }
        }

        #endregion


        #region 固定资产查询/维护获取列表数据公用方法
        /// <summary>
        /// 固定资产查询、固定资产维护
        /// </summary>
        /// <param name="type">Search:固定资产查询，Maintain：固定资产维护</param>
        /// <returns></returns>
        public JsonResult GetAllAssetsJsonData(string type)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InstrumentId,AssetsNo,BuyDate,Price,CalibrationType,RecordState,InstrumentName,ManageNo,BarCode,CertificateNo,Specification,InstrumentCate,Manufacturer,ManufactureContactor,SerialNo,LastCheckDate,LastCheckUser,DueStartDate,DueEndDate,LeaderName,BelongDepart,StorePalce,CreateDate,CreateUser";
            paging.Where = GetSearchCondition(dtm, Constants.InstrumentForm.固定资产.GetHashCode());
            IList<Hashtable> instrumentList = Instrument.Business.ServiceProvider.InstrumentService.GetAllInstrumentListForPaging(paging);
            ////系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();
            ParamItemModel mCalibrationType = null;
            //资产属性
            ParamModel CalibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);    //设备分类
            IList<ParamItemModel> paramItemList = CalibrationType.itemsList;
            ///设备状态
            IList<ParamItemModel> RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetsState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();
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

                //仪器维护才有操作
                if (type.Equals("Maintain"))
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("新增资产-修改".ToLower()))
                    {
                        //修改
                        sbOperate.Append("<a href='#' onclick='fnSelectInstrument(this);'>修改</a>&nbsp;&nbsp;");
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/Delete".ToLower()))
                    {
                        //删除
                        sbOperate.Append("<a href='#' onclick='fnDelInstr(this);'>删除</a>&nbsp;&nbsp;");
                    }
                 
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/Details".ToLower()))
                {
                    sbOperate.Append("<a href='#' onclick='fnInstrumentDetail(this);'>详细</a>&nbsp;&nbsp;");
                }

                sbOperate.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item["RecordState"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mParamItem == null ? "" : mParamItem.ParamItemName);    //状态
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["InstrumentName"]));    //资产名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["AssetsNo"]));    //资产编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));    //型号规格
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["BarCode"]));    //条码
                //资产属性
                mCalibrationType = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["CalibrationType"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mCalibrationType == null ? "" : mCalibrationType.ParamItemName);
                //所属部门
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", item["BelongDepart"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));    //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["BuyDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:F2}", item["Price"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Manufacturer"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManufactureContactor"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["LeaderName"]));    //保管人
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["StorePalce"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["LastCheckDate"]));    //最近盘点时间
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["LastCheckUser"]));    //最近盘点人
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
        private string GetSearchCondition(DataTableUtils.DataTableModel dtm,int instrumentForm)
        {
            //所属部门
            string orgName = Request["searchBelongDepart"];
            string checkAllAuthority = instrumentForm == (int)Constants.InstrumentForm.仪器 ? "Instrument-CheckAll" : "Assets-CheckAll";
            string where = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + instrumentForm, checkAllAuthority, orgName);

            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                where = string.Format("{0} and {1}", dtm.FieldCondition, where);
            }

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

        #endregion

        #region 保存
        public ActionResult Save(Instrument.Common.Models.InstrumentModel model, FormCollection collection)
        {
            model.BelongDepart = collection["OrgName"];
            model.CertificateNo = "";
            ServiceProvider.InstrumentService.Save4Assets(model);
            return Content("OK");
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult Delete(string instrumentId)
        {
            ServiceProvider.InstrumentService.DeleteById(UtilsHelper.Decrypt2Int(instrumentId));
            return Content("OK");
        }
        #endregion

        #region 详细
        /// <summary>
        /// 盘点计划下资产详细
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetsDetail(string planDetailId,string instrumnetId)
        {
            AssetCheckPlanDetailModel model = ServiceProvider.AssetCheckPlanDetailService.GetById(UtilsHelper.Decrypt2Int(planDetailId));
            return View( model);
        }

        /// <summary>
        /// 资产新增/查询下查看详细
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult Details(string instrumentId)
        {
            InstrumentModel model = ServiceProvider.InstrumentService.GetById(UtilsHelper.Decrypt2Int(instrumentId));
             //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel mState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.AssetsState);
            ParamItemModel item=mState.itemsList.SingleOrDefault(p=>p.ParamItemValue==model.RecordState.ToString());
            ViewBag.State = item == null ? "" : item.ParamItemName;
            return View(model);
        }

        //public string GetAssetsCheckLogList(string instrumentId)
        //{
        //    IList<Hashtable> checkLogList = ServiceProvider.InstrumentCheckLogService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
        //    StringBuilder sbData = new StringBuilder();
        //    sbData.Append("{\"data\":[");
        //    foreach (var item in checkLogList)
        //    {
        //        sbData.Append("[");
        //        //sbData.AppendFormat("\"{0}\"", item["InstrumentName"]);
        //        //sbData.AppendFormat(",\"{0}\"", item["AssetsNo"]);
        //        //sbData.AppendFormat(",\"{0}\"", item["Specification"]);
        //        sbData.AppendFormat("\"{0}\"", item["CheckResult"]);
        //        sbData.AppendFormat(",\"{0}\"", item["CheckUser"]);
        //        sbData.AppendFormat(",\"{0:yyyy-MM-dd HH:mm:ss}\"", item["CheckDate"]);
        //        sbData.Append("],");
        //    }
        //    if (checkLogList.Count > 0)
        //    {
        //        sbData.Remove(sbData.Length - 1, 1);
        //    }
        //    sbData.Append("]}");
        //    return sbData.ToString();
        //}
        

        #endregion

        #region 打印标签
        public ActionResult PrintQRCode()
        {
            ViewBag.JsonInstrumentState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.AssetsState).ToString();
            ViewBag.InstrumentForm = Constants.InstrumentForm.固定资产.GetHashCode();
            return View();
        }

        public ActionResult PrintQRCode2()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            ViewBag.JsonInstrumentState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.InstrumentState).ToString();
            ViewBag.InstrumentForm = Constants.InstrumentForm.仪器.GetHashCode();
            return View("PrintQRCode",null);
        }

        public JsonResult GetAllInstrumentList(int InstrumentForm)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InstrumentId,RecordState,InstrumentName,ManageNo,AssetsNo,BarCode,CertificateNo,Specification,InstrumentCate,Manufacturer,SerialNo,DueStartDate,DueEndDate,LeaderName,BelongDepart,CreateDate,CreateUser";
            paging.Where = GetSearchCondition(dtm, InstrumentForm);
            IList<Hashtable> instrumentList = Instrument.Business.ServiceProvider.InstrumentService.GetAllInstrumentListForPaging(paging);
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();
            //ParamItemModel mInstrumentCate = null;
            IList<ParamItemModel> RecordStateparamItemList = null;
            //设备状态
            if (InstrumentForm.Equals(Constants.InstrumentForm.仪器.GetHashCode()))
                RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState).itemsList;
            else
                RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetsState).itemsList;

            ParamItemModel mParamItem = new ParamItemModel();
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            foreach (var item in instrumentList)
            {
                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<input type='checkbox' name='chk' value='{0}' />", UtilsHelper.Encrypt(item["InstrumentId"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item["RecordState"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mParamItem == null ? "" : mParamItem.ParamItemName);    //状态
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' InstrumentId='{1}' InstrumentForm='{2}' onclick='fnInstrumentDetail(this);'>{0}</a>", item["InstrumentName"], UtilsHelper.Encrypt(item["InstrumentId"].ToString()), InstrumentForm));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["AssetsNo"]));    //资产编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Manufacturer"]));    //生产厂家
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));    //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["BarCode"]));    //条码
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", item["BelongDepart"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["LeaderName"]));
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
        /// 打印条码
        /// </summary>
        /// <param name="InstrumentIds"></param>
        /// <returns></returns>
        public string PrintQR(string InstrumentIds)
        {
            string result = "打印除失败！";
            try
            {
                string[] Ids = InstrumentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                InstrumentModel m = new InstrumentModel();
                foreach (string id in Ids)
                {
                    m = ServiceProvider.InstrumentService.GetById(UtilsHelper.Decrypt2Int(id));
                    TSCPrint(m);
                }
                result = "OK";
            }
            catch(Exception e)
            {
                result = e.Message;
            }
            return result;
        }
        /// <summary>
        /// 调用TSC打印机打印
        /// </summary>
        /// <param name="title">打印的标题</param>
        /// <param name="barCode">打印的条码编号</param>
        public static void TSCPrint(InstrumentModel m)
        {
            // 打开 打印机 端口.
            TSCLIB_DLL.openport("TSC  TTP-243E Pro");
            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            TSCLIB_DLL.sendcommand("DIRECTION 0");
            string strP = "QRCODE 100,70,H,6,A,0,M2,S1,\"" + m.BarCode + "\"";
            TSCLIB_DLL.sendcommand(strP);
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title
            TSCLIB_DLL.windowsfont(235, 50, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);
            TSCLIB_DLL.windowsfont(235, 90, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(235, 130, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(235, 170, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(235, 210, 25, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode
            //TSCLIB_DLL.barcode("176", "66", "39", "130", "1", "0", "7", "12", barCode);
            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }
        /// <summary>
        /// 下载打印机安装示例
        /// </summary>
        public void DownloadFiles()
        {
            UtilsHelper.FileDownload("", "/App_Data/打印机调试.zip", "打印机调试.zip", UtilConstants.ServerType.WebService);
        }
        #endregion

        #region 盘点
        public ActionResult CheckLogList()
        {
            //系统参数
            //IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //ParamModel AssetCheckResult = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.AssetCheckResult.ToString());
            //if (null == AssetCheckResult) AssetCheckResult = new ParamModel();
            //ViewBag.AssetCheckResultList = new SelectList(AssetCheckResult.itemsList, "ParamItemName", "ParamItemName");
            return View();
        }


        public string GetCheckLogList(string barCode)
        {
            Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetByBarCode(barCode);
            IList<AssetCheckPlanDetailModel> detailList = ServiceProvider.AssetCheckPlanDetailService.GetByInstrumentId(model.InstrumentId);
            //IList<AssetCheckPlanModel> planList = ServiceProvider.AssetCheckPlanService.GetAll();
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            foreach (var item in detailList)
            {
                sbData.Append("[");
                sbData.AppendFormat("\"{0}\"", item.InstrumentName);
                sbData.AppendFormat(",\"{0}\"", item.AssetsNo);
                sbData.AppendFormat(",\"{0}\"", item.Specification);
                sbData.AppendFormat(",\"{0}\"",(Constants.AssetsCheckStatus) item.Statuse);
                sbData.AppendFormat(",\"{0}\"", item.Checkor);
                sbData.AppendFormat(",\"{0:yyyy-MM-dd HH:mm:ss}\"", item.CheckDate);
                sbData.Append("],");
            }
            if (detailList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult CheckAssets(string barCode,FormCollection collection)
        {
            string msg = ServiceProvider.AssetCheckPlanDetailService.AssetCheck(barCode, LoginHelper.LoginUser.UserId, LoginHelper.LoginUser.UserName);
            //ServiceProvider.AssetCheckPlanDetailService.UpdateState(barCode, LoginHelper.LoginUser.UserId);
            return Content(msg);
        }

        #endregion

        #region 盘点记录
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult CheckLogFlowList(string instrumentId, string planDetailId)
        {
            ViewBag.InstrumentId = instrumentId;
            return View();
        }

        public string GetCheckLogFlowList(string instrumentId, string planDetailId)
        {
            IList<AssetCheckPlanDetailModel> detailList = new List<AssetCheckPlanDetailModel>();
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            if (UtilsHelper.Decrypt2Int(instrumentId) == 0)//盘盈资产的只显示该盘盈记录信息
            {
                AssetCheckPlanDetailModel model = ServiceProvider.AssetCheckPlanDetailService.GetById(UtilsHelper.Decrypt2Int(planDetailId));
                sbData.AppendFormat("[\"{0}\"", (Constants.AssetsCheckStatus)model.Statuse);
                sbData.AppendFormat(",\"{0}\"", model.Checkor);
                sbData.AppendFormat(",\"{0}\"",model.CheckDate);
                sbData.Append("]]}");
                return sbData.ToString();
            }
            detailList = ServiceProvider.AssetCheckPlanDetailService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
            detailList = detailList.Where(p =>!p.Statuse.Equals(Constants.AssetsCheckStatus.盘亏.GetHashCode())).ToList();
            foreach (var item in detailList)
            {
                sbData.Append("[");
                sbData.AppendFormat("\"{0}\"",(Constants.AssetsCheckStatus)item.Statuse);
                sbData.AppendFormat(",\"{0}\"", item.CreateUser);
                sbData.AppendFormat(",\"{0:yyyy-MM-dd HH:mm:ss}\"", item.CreateDate);
                sbData.Append("],");
            }
            if (detailList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }
        #endregion

        #region === 下载模板 ===
        public void DownloadTemplate()
        {
            UtilsHelper.FileDownload("", "/App_Data/盘点资产模板.xls", "盘点资产模板.xls", UtilConstants.ServerType.WebService);
        }

        /// <summary>
        /// 根据参数表构造固定资产下载模板
        /// </summary>
        public void DownloadAssetsTemplate()
        {
            //UtilsHelper.FileDownload("", "/App_Data/固定资产模板.xls", "固定资产模板.xls", UtilConstants.ServerType.WebService);
            Stream workBookStream = UtilsHelper.FileDownload("", "/App_Data/固定资产模板.xls", UtilConstants.ServerType.WebService);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(workBookStream);
            ISheet sheet1 = hssfworkbook.GetSheet("sheet1");
            //设备状态	资产属性
            CellRangeAddressList regionInstrumentState = new CellRangeAddressList(0, 65535, 9, 9);
            CellRangeAddressList regionCalibrationType = new CellRangeAddressList(0, 65535, 2, 2);
     
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel pInstrumentState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetsState);    //设备状态
            ParamModel pCalibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);    //资产属性

            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(pInstrumentState.itemsList.Select(p => p.ParamItemName).ToArray());
            HSSFDataValidation paramValidate = new HSSFDataValidation(regionInstrumentState, constraint);
            sheet1.AddValidationData(paramValidate);
            constraint = DVConstraint.CreateExplicitListConstraint(pCalibrationType.itemsList.Select(p => p.ParamItemName).ToArray());
            paramValidate = new HSSFDataValidation(regionCalibrationType, constraint);
            sheet1.AddValidationData(paramValidate);
         
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + ".xls");
            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            //写入内存流
            hssfworkbook.Write(fs);
            fs.Close();
            fs.Dispose();
            ToolsLib.FileService.WebServer.DownLoadFile(newFile, "固定资产模板.xls", true);
        }
        #endregion

        #region 批量导入固定资产
        [HttpPost]
        public ActionResult BatchImportAssets(FormCollection collection)
        {
            bool isOK = true;
            string errMsg = "";
            string sucessMsg = "";
            //上传到服务器
            HttpPostedFileBase file = Request.Files[0];
            string fileName = ToolsLib.FileService.WebServer.UpLoadFile(file.InputStream, WebUtils.Resulve("/tempFile/" + Path.GetFileName(file.FileName)));
            DataTable dt = new DataTable();
            dt = ExcelFile.ReadExcelFile(fileName, "Sheet1", true);

            if (dt.Columns.Count != 15) errMsg = "提交的文件格式与模板不同，请按照模板格式上传数据。";
            if (dt.Rows.Count == 0) errMsg = "上传数据为空，请重新上传。";
            if (errMsg != "") isOK = false;
            if (isOK)
            {
                errMsg = ServiceProvider.InstrumentService.BatchImportAssets(dt, ref sucessMsg);
            }

            return Json(new
            {
                sMsg = errMsg,
                Result = sucessMsg
            }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 盘点计划
        /// <summary>
        /// 盘点计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckPlanList()
        {
            ViewBag.PlanType = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.AssetCheckPlanType).ToString();
            return View();
        }

        public JsonResult GetAllCheckPlanJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"PlanId,PlanType,PlanName,StartDate,EndDate,Remark,CreateUser,CreateDate";
            paging.Where = dtm.FieldCondition;
            IList<Hashtable> planList = Instrument.Business.ServiceProvider.AssetCheckPlanService.GetAllAssetCheckPlanListForPaging(paging);
            ////系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamItemModel mPlaType = null;
            //计划类型
            ParamModel pPlanTyle = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetCheckPlanType);
            IList<ParamItemModel> paramItemList = pPlanTyle.itemsList;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            foreach (var item in planList)
            {
                bool isCanEdit = false;
                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<div planId='{0}' planName='{1}'>", UtilsHelper.Encrypt(item["PlanId"].ToString()), item["PlanName"]);
                
                if (Convert.ToDateTime(item["StartDate"]) > DateTime.Now) isCanEdit = true;//盘点开始时间大于今天，不可删除和编辑

                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/EditCheckPlan".ToLower()) && isCanEdit)
                {
                    //编辑
                    sbOperate.Append("<a href='#' onclick='fnEditCheckPlan(this);'>编辑</a>&nbsp;&nbsp;");
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/DeleteCheckPlan".ToLower()) && isCanEdit)
                {
                    //删除
                    sbOperate.Append("<a href='#' onclick='fnDeletePlan(this);'>删除</a>&nbsp;&nbsp;");
                }

                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/CheckPlanDetail".ToLower()))
                {
                    sbOperate.Append("<a href='#' onclick='fnAssetsPlanDetail(this);'>查看</a>&nbsp;&nbsp;");
                }

                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/AssetsOverage".ToLower()))
                {
                    sbOperate.Append("<a href='#' onclick='fnCheckPlanOverage(this);'>盘盈信息</a>&nbsp;&nbsp;");
                }

                sbOperate.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["PlanName"]));

                mPlaType = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["PlanType"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mPlaType == null ? "" : mPlaType.ParamItemName);

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["StartDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["EndDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["CreateDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["CreateUser"]));
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

        /// <summary>
        /// 获取计划下所有盘点资产
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="type">Edit:编辑，Detail:详细</param>
        /// <returns></returns>
        public JsonResult GetPlanAllCheckAssetsJsonData(int planId, string type)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"PlanDetailId,PlanId,InstrumentId,Statuse,BelongDepart,InstrumentName,BarCode,Specification,Manufacturer,SerialNo,ManageNo,AssetsNo,Remark,CreateUser,CreateDate,Checkor,CheckDate,IsRightAddress";
            paging.Where = string.Format("{0} and PlanId=" + planId, string.IsNullOrEmpty(dtm.FieldCondition) ? "1=1" : dtm.FieldCondition);
            if (!string.IsNullOrWhiteSpace(dtm.KeyWord))
                paging.Where = string.Format(@"{0} and (InstrumentName like '{1}%' or Specification like '{1}%' or Manufacturer like '{1}%' or BarCode like '{1}%' 
                                                        or ManageNo like '{1}%' or AssetsNo like '{1}%' or SerialNo like '{1}%'  )", paging.Where, dtm.KeyWord);

            IList<Hashtable> planList = Instrument.Business.ServiceProvider.AssetCheckPlanDetailService.GetPlanAllCheckAssetsListForPaging(paging);
            ////系统参数
            //IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            foreach (var item in planList)
            {
                dtm.aaData.Add(new List<string>());
                if (type.Equals("Edit"))
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<input type='checkbox' name='chk' value='{0}'/>", UtilsHelper.Encrypt(item["PlanDetailId"].ToString())));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' PlanDetailId='{1}' instrumentId='{2}' onclick='fnCheckDetail(this);'>{0}</a>&nbsp;&nbsp;", item["InstrumentName"], UtilsHelper.Encrypt(item["PlanDetailId"].ToString()), UtilsHelper.Encrypt(item["InstrumentId"].ToString())));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", (Constants.AssetsCheckStatus)Convert.ToInt32(item["Statuse"])));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["AssetsNo"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["BarCode"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Manufacturer"]));
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", item["BelongDepart"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);
                if (Convert.ToInt32(item["IsRightAddress"]) == (Constants.AssetsIsRightAddress.一致.GetHashCode()))
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", "一致"));//一致
                else
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", "不一致"));//不一致
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", (Constants.AssetsCheckStatus)Convert.ToInt32(item["Statuse"]) == Constants.AssetsCheckStatus.已盘点 ? item["Checkor"] : ""));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", (Constants.AssetsCheckStatus)Convert.ToInt32(item["Statuse"]) == Constants.AssetsCheckStatus.已盘点 ? item["CheckDate"] : ""));
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

        public ActionResult BatchDeletePlanDetailByIds(string detailIds)
        {
            IList<int> detailIdList = detailIds.Split(',').Select(a =>UtilsHelper.Decrypt2Int(a)).ToList();
            ServiceProvider.AssetCheckPlanDetailService.DeleteByIdList(detailIdList);
            return Content("OK");
        }

        /// <summary>
        /// 编辑盘点计划
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult EditCheckPlan(string planId)
        {
            AssetCheckPlanModel model=new AssetCheckPlanModel();
            model.PlanName = string.Format("{0:yyyyMMdd}盘点计划",DateTime.Now);
            if (planId != "0")
                model = ServiceProvider.AssetCheckPlanService.GetById(UtilsHelper.Decrypt2Int(planId));
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //计划类型
            ParamModel pPlanType = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.AssetCheckPlanType);
            if (null == pPlanType) pPlanType = new ParamModel();
            ViewBag.PlanType = new SelectList(pPlanType.itemsList, "ParamItemValue", "ParamItemName", model.PlanType);
            ViewBag.StatusJson = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new Constants.AssetsCheckStatus()).ToString();

            IList<AssetCheckPlanDetailModel> list = ServiceProvider.AssetCheckPlanDetailService.GetByPlanId(UtilsHelper.Decrypt2Int(planId));
            StringBuilder sb = new StringBuilder();
            if (list.Count > 0)
            {
                IList<AssetCheckPlanDetailModel> list1 = list.Where(p => p.Statuse.Equals(Constants.AssetsCheckStatus.盘亏.GetHashCode())).ToList();
                IList<AssetCheckPlanDetailModel> list2 = list.Where(p => p.Statuse.Equals(Constants.AssetsCheckStatus.盘盈.GetHashCode())).ToList();
                IList<AssetCheckPlanDetailModel> list3 = list.Where(p => p.Statuse.Equals(Constants.AssetsCheckStatus.已盘点.GetHashCode())).ToList();
                sb.AppendFormat("盘亏：{0}({1}%),盘盈：{2}({3}%),已盘点：{4}({5}%)", list1.Count, string.Format("{0:F2}", Convert.ToDecimal(list1.Count) / Convert.ToDecimal(list.Count) * 100), list2.Count, string.Format("{0:F2}", Convert.ToDecimal(list2.Count) / Convert.ToDecimal(list.Count) * 100), list3.Count, string.Format("{0:F2}", Convert.ToDecimal(list3.Count) / Convert.ToDecimal(list.Count) * 100));
            }
            ViewBag.TotalInfo = sb.ToString();

            return View(model);
        }

      
        /// <summary>
        /// 保存盘点计划
        /// </summary>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult SaveCheckPlan(AssetCheckPlanModel model, FormCollection collection)
        {
            model.CreateUser = LoginHelper.LoginUser.UserName;
            ServiceProvider.AssetCheckPlanService.Save(model);
            JsonResult jr = Json(new
            {
                Msg="OK",
                PlanId=model.PlanId
            }, JsonRequestBehavior.AllowGet);
            return jr;
        }

        /// <summary>
        /// 删除盘点计划
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult DeleteCheckPlan(string planId)
        {
            string result = ServiceProvider.AssetCheckPlanService.DeleteById(UtilsHelper.Decrypt2Int(planId));
            return Content(result);
        }

        /// <summary>
        ///  获取盘点计划操作用户列表
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string GetCheckPlanOperatorList(int planId, string type)
        {
            IList<AssetCheckOperatorModel> opetatorList = ServiceProvider.AssetCheckOperatorService.GetByPlanId(planId);
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            foreach (var item in opetatorList)
            {
                sbData.Append("[");
                if (type.Equals("Edit"))
                {
                    sbData.AppendFormat("\"<input type='checkbox' name='chk_op' value='{0}' />\"", item.AutoId);
                    sbData.AppendFormat(",\"{0}\"", item.UserName);
                }
                else
                    sbData.AppendFormat("\"{0}\"", item.UserName);

                UserModel user = Global.Business.ServiceProvider.UserService.GetById(item.UserId);
                sbData.AppendFormat(",\"{0}\"", user == null ? "" : user.JobNo);
                sbData.Append("],");
            }
            if (opetatorList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 关键字查询盘点用户
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult QuickSearchOperatorNameByKeyWord(string term)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(term)) return Content("[]");
            IList<Hashtable> userList = Global.Business.ServiceProvider.UserService.SearchByNameJobNumb(term, "");
            if (userList == null) Content("[]");
            sb.Append("[");
            foreach (Hashtable user in userList)
            {
                sb.Append("{");
                sb.AppendFormat("\"value\":\"{0}\"", user["UserId"]);
                sb.AppendFormat(",\"label\":\"{0}\"", user["UserName"]);
                sb.Append("},");
            }
            if (userList.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return Content(sb.ToString());
        }

        /// <summary>
        /// 添加盘点用户
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult AddPlanOperator(int planId, FormCollection collection)
        {
            int userId = Convert.ToInt32(collection["UserId"]);
            AssetCheckOperatorModel model = null;
            model = ServiceProvider.AssetCheckOperatorService.GetByPlanIdAndUserId(planId, userId);
            if (model != null)
                return Content("不能添加相同的用户");
            model = new AssetCheckOperatorModel();
            model.PlanId=planId;
            model.UserName = collection["UserName"];
            model.UserId = userId;
            model.Creator = LoginHelper.LoginUser.UserName;
            ServiceProvider.AssetCheckOperatorService.Save(model);
            return Content("OK");
        }


        /// <summary>
        /// 根据多个标识删除用户
        /// </summary>
        /// <param name="autoIds"></param>
        /// <returns></returns>
        public ActionResult BatchDeleteOperatorByIds(string autoIds)
        {
            IList<int> autoIdList = autoIds.Split(',').Select(a => Convert.ToInt32(a)).ToList();
            ServiceProvider.AssetCheckOperatorService.DeleteByIdList(autoIdList);
            return Content("OK");
        }

        /// <summary>
        /// 计划盘点资产
        /// </summary>
        /// <returns></returns>
        public ActionResult PlanAssetsList()
        {
            return View();
        }

        /// <summary>
        /// 盘盈信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult AssetsOverage(string planId)
        {
            return View();
        }

        /// <summary>
        /// 盘盈信息列表
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string GetAssetsOverageList(string planId)
        {
            AssetCheckPlanModel planModel = ServiceProvider.AssetCheckPlanService.GetById(UtilsHelper.Decrypt2Int(planId));
            IList<AssetCheckPlanDetailModel> OverageList = ServiceProvider.AssetCheckPlanDetailService.GetByPlanIdAndStatus(UtilsHelper.Decrypt2Int(planId), Constants.AssetsCheckStatus.盘盈.GetHashCode());
            StringBuilder sbData = new StringBuilder();
            StringBuilder sbOperator = new StringBuilder();
            sbData.Append("{\"data\":[");
            foreach (var item in OverageList)
            {
                sbOperator.AppendFormat("<div planDetailId='{0}'>",UtilsHelper.Encrypt(item.PlanDetailId.ToString()));
                if (LoginHelper.LoginUserAuthorize.ContainsKey("Overage-AddToInstrument".ToLower()))
                    sbOperator.Append("<a href='#' onclick='fnAddToInstrument(this)'>添加到仪器库</a>&nbsp;|&nbsp;");
                if (LoginHelper.LoginUserAuthorize.ContainsKey("Overage-AddToAssets".ToLower()))
                    sbOperator.Append("<a href='#' onclick='fnAddToAssets(this)'>添加到资产库</a>&nbsp;|&nbsp;");
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Assets/CancelOverage".ToLower()))
                    sbOperator.Append("<a href='#' onclick='fnCancelOverage(this)'>取消盘盈</a>");
                sbOperator.AppendFormat("</div>");
                sbData.Append("[");
                sbData.AppendFormat("\"{0}\"", sbOperator);
                sbOperator.Clear();
                sbData.AppendFormat(",\"{0}\"", planModel.PlanName);
                sbData.AppendFormat(",\"{0}\"", item.InstrumentName);
                sbData.AppendFormat(",\"{0}\"", item.Specification);
                sbData.AppendFormat(",\"{0}\"", item.Manufacturer);
                sbData.AppendFormat(",\"{0}\"", item.SerialNo);
                sbData.AppendFormat(",\"{0}\"", item.Remark);
                sbData.AppendFormat(",\"{0}\"", item.CreateUser);
                sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", item.CreateDate);
                sbData.Append("],");
            }
            if (OverageList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 取消盘盈
        /// </summary>
        /// <param name="planDetailId"></param>
        /// <returns></returns>
        public ActionResult CancelOverage(string planDetailId)
        {
            ServiceProvider.AssetCheckPlanDetailService.DeleteById(UtilsHelper.Decrypt2Int(planDetailId));
            return Content("OK");
        }

        /// <summary>
        /// 盘盈添加
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPlanDetail()
        {
            IList<AssetCheckPlanModel> planList = ServiceProvider.AssetCheckPlanService.GetByUserId(LoginHelper.LoginUser.UserId);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var model in planList)
            {
                list.Add(new SelectListItem { Text = model.PlanName, Value = model.PlanId.ToString() });
            }
            SelectList planSelectList = new SelectList(list, "Value", "Text", "");
            ViewBag.PlanSelectList = planSelectList;
            return View();
        }

        /// <summary>
        /// 保存盘盈添加信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SavePlanDetail(AssetCheckPlanDetailModel model,FormCollection collection)
        {

            model.Checkor = LoginHelper.LoginUser.UserName;
            model.Statuse = Constants.AssetsCheckStatus.盘盈.GetHashCode();
            ServiceProvider.AssetCheckPlanDetailService.Add(model);
            return Content("OK");
        }

        /// <summary>
        /// 查看计划详细
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult CheckPlanDetail(string planId)
        {
            AssetCheckPlanModel model = ServiceProvider.AssetCheckPlanService.GetById(UtilsHelper.Decrypt2Int(planId));
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //计划类型
            ParamModel pPlanType = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.AssetCheckPlanType);
            if (null == pPlanType) pPlanType = new ParamModel();
            ViewBag.PlanType = pPlanType.itemsList.SingleOrDefault(p=>p.ParamItemValue.Equals(model.PlanType.ToString())).ParamItemName;
            ViewBag.StatusJson = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new Constants.AssetsCheckStatus()).ToString();
            ViewBag.AddressJson = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new Constants.AssetsIsRightAddress()).ToString();

            IList<AssetCheckPlanDetailModel> list = ServiceProvider.AssetCheckPlanDetailService.GetByPlanId(UtilsHelper.Decrypt2Int(planId));
            StringBuilder sb = new StringBuilder();
            if (list.Count > 0)
            {
                IList<AssetCheckPlanDetailModel> list1 = list.Where(p => p.Statuse.Equals(Constants.AssetsCheckStatus.盘亏.GetHashCode())).ToList();
                IList<AssetCheckPlanDetailModel> list2 = list.Where(p => p.Statuse.Equals(Constants.AssetsCheckStatus.盘盈.GetHashCode())).ToList();
                IList<AssetCheckPlanDetailModel> list3 = list.Where(p => p.Statuse.Equals(Constants.AssetsCheckStatus.已盘点.GetHashCode())).ToList();
                sb.AppendFormat("盘亏：{0}({1}%),盘盈：{2}({3}%)，已盘点：{4}({5}%)", list1.Count, string.Format("{0:F2}", Convert.ToDecimal(list1.Count) / Convert.ToDecimal(list.Count) * 100), list2.Count, string.Format("{0:F2}", Convert.ToDecimal(list2.Count) / Convert.ToDecimal(list.Count) * 100), list3.Count, string.Format("{0:F2}", Convert.ToDecimal(list3.Count) / Convert.ToDecimal(list.Count) * 100));
            }
            ViewBag.TotalInfo = sb.ToString();

            return View(model);
        }

        /// <summary>
        /// 模板批量导入资产到盘点计划列表
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult BatchImportAssetsForCheckPlan(FormCollection collection)
        {
            string planId = collection["PlanId"];
            bool isOK = true;
            string errMsg = "";
            string sucessMsg = "";
            //上传到服务器
            HttpPostedFileBase file = Request.Files[0];
            string fileName = ToolsLib.FileService.WebServer.UpLoadFile(file.InputStream, WebUtils.Resulve("/tempFile/" + Path.GetFileName(file.FileName)));
            DataTable dt = new DataTable();
            dt = ExcelFile.ReadExcelFile(fileName, "Sheet1", true);

            if (dt.Columns["条码"]==null) errMsg = "提交的文件格式与模板不同，请按照模板格式上传数据。";
            if (dt.Rows.Count == 0) errMsg = "上传数据为空，请重新上传。";
            if (errMsg != "") isOK = false;
            if (isOK)
            {
                errMsg = ServiceProvider.AssetCheckPlanDetailService.BatchImportAssetsForCheckPlan(Convert.ToInt32(planId), dt, ref sucessMsg);
            }

            return Json(new
            {
                sMsg = errMsg,
                Result = sucessMsg
            }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加所有仪器资产
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchImportAllInstrument(int planId)
        {
            ServiceProvider.AssetCheckPlanDetailService.BatchImportFromInstrument(Constants.InstrumentForm.仪器.GetHashCode(), planId);
            return Content("OK");
        }

        /// <summary>
        /// 添加所有非仪器资产
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchImportAllAssets(int planId)
        {
            ServiceProvider.AssetCheckPlanDetailService.BatchImportFromInstrument(Constants.InstrumentForm.固定资产.GetHashCode(), planId);
            return Content("OK");
        }
        #endregion
    }
}
