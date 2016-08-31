using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Instrument.Business;
using GRGTCommonUtils;
using System.Text;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using Global.Common;
using ToolsLib.Utility;
using System.Data;
using System.IO;
using Global.Common.Models;
using Instrument.Common.Models;
using Instrument.Common;
using ToolsLib.FileService;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Instrument.WebSite.Controllers
{
    public class InstrumentController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InstrumentController));

        #region 仪器维护
        public ActionResult InstrumentMaintain()
        {
            ViewBag.JsonWarnDay = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.WarnDay).ToString();
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            ViewBag.JsonInstrumentRecordState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.InstrumentState).ToString();
            return View();
        }
        /// <summary>
        /// 批量导入证书
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadInstrumentCert()
        {
            return View();
        }
        /// <summary>
        /// 导入证书
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadCerts(FormCollection collection)
        {
            string LogFiledName = "";
            string msg = ServiceProvider.InstrumentService.UploadCerts(Request.Files[0], ref LogFiledName);
            JsonResult jr = Json(new
            {
                MSG = msg,
                Data = LogFiledName
            }, JsonRequestBehavior.AllowGet);
            return jr;
        }
        public ActionResult ExportLogFiled(string FiledName)
        {
            string result = this.Server.MapPath(string.Format("~/tempFile/{0}.xlsx", FiledName));
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}证书上传结果清单{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }
        #endregion

        #region === 下载仪器模板 ===
        /// <summary>
        /// 从系统参数对模板的下拉框进行绑定，注意：模板本来列的数据有效性必须先删除
        /// </summary>
        public void DownloadTemplate()
        {
            Stream workBookStream = UtilsHelper.FileDownload("", "/App_Data/仪器模板.xls", UtilConstants.ServerType.WebService);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(workBookStream);
            ISheet sheet1 = hssfworkbook.GetSheet("sheet1");
            //设备状态	设备分类	资产属性	设备类别	管理级别	计量类别
            CellRangeAddressList regionInstrumentState = new CellRangeAddressList(1, 65535, 6, 6);//
            CellRangeAddressList regionInstrumentCate = new CellRangeAddressList(1, 65535, 7, 7);//
            CellRangeAddressList regionpCalibrationType = new CellRangeAddressList(1, 65535, 8, 8);//
            CellRangeAddressList regionpInstrumentType = new CellRangeAddressList(1, 65535, 9, 9);//
            CellRangeAddressList regionpManageLevel = new CellRangeAddressList(1, 65535, 10, 10);//
            CellRangeAddressList regionpVerificationType = new CellRangeAddressList(1, 65535, 11, 11);//
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel pInstrumentState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState);    //设备状态
            ParamModel pInstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            ParamModel pVerificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);    //检定类别
            ParamModel pManageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);    //管理级别
            ParamModel pCalibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);    //资产属性
            ParamModel pInstrumentType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentType);    //设备类别

            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(pInstrumentState.itemsList.Select(p => p.ParamItemName).ToArray());
            HSSFDataValidation paramValidate = new HSSFDataValidation(regionInstrumentState, constraint);
            sheet1.AddValidationData(paramValidate);
            constraint = DVConstraint.CreateExplicitListConstraint(pInstrumentCate.itemsList.Where(p => p.ParentCode.Equals("0")).Select(p => p.ParamItemName).ToArray());
            paramValidate = new HSSFDataValidation(regionInstrumentCate, constraint);
            sheet1.AddValidationData(paramValidate);
            constraint = DVConstraint.CreateExplicitListConstraint(pCalibrationType.itemsList.Select(p => p.ParamItemName).ToArray());
            paramValidate = new HSSFDataValidation(regionpCalibrationType, constraint);
            sheet1.AddValidationData(paramValidate);
            constraint = DVConstraint.CreateExplicitListConstraint(pInstrumentType.itemsList.Select(p => p.ParamItemName).ToArray());
            paramValidate = new HSSFDataValidation(regionpInstrumentType, constraint);
            sheet1.AddValidationData(paramValidate);
            constraint = DVConstraint.CreateExplicitListConstraint(pManageLevel.itemsList.Select(p => p.ParamItemName).ToArray());
            paramValidate = new HSSFDataValidation(regionpManageLevel, constraint);
            sheet1.AddValidationData(paramValidate);
            constraint = DVConstraint.CreateExplicitListConstraint(pVerificationType.itemsList.Select(p => p.ParamItemName).ToArray());
            paramValidate = new HSSFDataValidation(regionpVerificationType, constraint);
            sheet1.AddValidationData(paramValidate);
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + ".xls");
            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            //写入内存流
            hssfworkbook.Write(fs);
            fs.Close();
            fs.Dispose();
            ToolsLib.FileService.WebServer.DownLoadFile(newFile, "仪器模板.xls", true);
        }
        #endregion
        #region === 下载维修信息模板 ===
        /// <summary>
        /// 从系统参数对模板的下拉框进行绑定，注意：模板本来列的数据有效性必须先删除
        /// </summary>
        public void DownloadRepairTemplate()
        {
            Stream workBookStream = UtilsHelper.FileDownload("", "/App_Data/维修信息模板.xls", UtilConstants.ServerType.WebService);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(workBookStream);
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + ".xls");
            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            //写入内存流
            hssfworkbook.Write(fs);
            fs.Close();
            fs.Dispose();
            ToolsLib.FileService.WebServer.DownLoadFile(newFile, "维修信息模板.xls", true);
        }
        #endregion

        #region 批量导入仪器
        [HttpPost]
        public ActionResult BatchImportInstrument(FormCollection collection)
        {
            bool isOK = true;
            string errMsg = "";
            string sucessMsg = "";
            //上传到服务器
            HttpPostedFileBase file = Request.Files[0];
            string fileName = ToolsLib.FileService.WebServer.UpLoadFile(file.InputStream, WebUtils.Resulve("/tempFile/" + Path.GetFileName(file.FileName)));
            DataTable dt = new DataTable();
            dt = ExcelFile.ReadExcelFile(fileName, "Sheet1", true);
            if (dt.Columns.Count != 29) errMsg = "提交的文件格式与模板不同，请按照模板格式上传数据。";
            if (dt.Rows.Count == 0) errMsg = "上传数据为空，请重新上传。";
            if (errMsg != "") isOK = false;
            if (isOK)
            {
                errMsg = ServiceProvider.InstrumentService.BatchImportInstrument(dt, ref sucessMsg);
            }

            return Json(new
            {
                sMsg = errMsg,
                Result = sucessMsg
            }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 批量导入维修信息
        [HttpPost]
        public ActionResult BatchImportRepairPlan(FormCollection collection)
        {
            bool isOK = true;
            string errMsg = "";
            string sucessMsg = "";
            //上传到服务器
            HttpPostedFileBase file = Request.Files[0];
            string fileName = ToolsLib.FileService.WebServer.UpLoadFile(file.InputStream, WebUtils.Resulve("/tempFile/" + Path.GetFileName(file.FileName)));
            DataTable dt = new DataTable();
            dt = ExcelFile.ReadExcelFile(fileName, "Sheet1", true);
            if (dt.Columns.Count != 16) errMsg = "提交的文件格式与模板不同，请按照模板格式上传数据。";
            if (dt.Rows.Count == 0) errMsg = "上传数据为空，请重新上传。";
            if (errMsg != "") isOK = false;
            if (isOK)
            {
                errMsg = ServiceProvider.InstrumentRepairPlanService.BatchImportRepairPlan(dt, ref sucessMsg);
            }

            return Json(new
            {
                sMsg = errMsg,
                Result = sucessMsg
            }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 仪器查询
        public ActionResult Index()
        {
            ViewBag.JsonWarnDay = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.WarnDay).ToString();
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            ViewBag.JsonInstrumentRecordState = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.InstrumentState).ToString();
            //ViewBag.JsonBranchCompany = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(UtilConstants.SysParamType.BranchCompany).ToString();
            return View();
        }

        #endregion

        #region 仪器查询/维护获取列表数据公用方法
        /// <summary>
        /// 仪器查询、仪器维护
        /// </summary>
        /// <param name="type">Search:仪器查询，Maintain：仪器维护</param>
        /// <returns></returns>
        public JsonResult GetAllInstrumentJsonData(string type)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.FieldOrder = dtm.FieldOrder == "" ? "InstrumentId desc" : dtm.FieldOrder;
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InstrumentId,RecordState,InstrumentName,ManageNo,CertificateNo,BarCode,Specification,InstrumentCate,Manufacturer,SerialNo,DueStartDate,DueEndDate,LeaderName,BelongDepart,StorePalce,CreateDate,CreateUser,LastUpdateDate";
            paging.Where = GetSearchCondition(dtm, type);
            IList<Hashtable> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListForPaging(paging);
            ////系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();
            ParamItemModel mInstrumentCate = null;
            //分类
            ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            IList<ParamItemModel> paramItemList = InstrumentCate.itemsList;
            ///设备状态
            IList<ParamItemModel> RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState).itemsList;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            DateTime dueEndDate;
            bool isOverTime = false;
            int warnDay = 0;
            ParamItemModel mParamItem = new ParamItemModel();
            foreach (var item in instrumentList)
            {
                if (!string.IsNullOrWhiteSpace(string.Format("{0}", item["DueEndDate"])))
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item["DueEndDate"]));
                else
                    dueEndDate = DateTime.MinValue;
                //预警天数
                isOverTime = dueEndDate < Convert.ToDateTime(string.Format("{0:d}", DateTime.Now));
                warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                warnDay = warnDay < 0 ? 0 : warnDay;
                //超期无预警天数
                if (isOverTime) warnDay = 0;

                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<div instrumentId='{0}' instrumentName='{1}'>", UtilsHelper.Encrypt(item["InstrumentId"].ToString()), item["InstrumentName"]);
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentDetail".ToLower()))
                {
                    //详细
                    sbOperate.Append("<a href='#' onclick='fnInstrumentDetail(this);'>详细</a>&nbsp;&nbsp;");
                }
                //仪器维护才有操作
                if (type.Equals("Maintain"))
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/Edit".ToLower()))
                    {
                        //修改
                        sbOperate.Append("<a href='#' onclick='fnSelectInstrument(this);'>修改</a>&nbsp;&nbsp;");
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/Delete".ToLower()))
                    {
                        //删除
                        sbOperate.Append("<a href='#' onclick='fnDelInstr(this);'>删除</a>&nbsp;&nbsp;");
                    }

                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/SynInstrument".ToLower()))
                    {
                        //同步
                        sbOperate.Append("<a href='#' onclick='fnSynInstrument(this);'>同步到业务系统</a>");
                    }
                }

                sbOperate.Append("</div>");
                if (type.Equals("Maintain") && LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DeleteInstruments".ToLower()))
                {
                    dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<input type='checkbox' name='chk' value={0} />", item["InstrumentId"]));
                }
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["DueEndDate"] == null ? "" : isOverTime ? "已超期" : "未超期");    //证书超期
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", (warnDay == 0) ? "无预警" : warnDay.ToString() + "天"));//预警天数
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["DueEndDate"]));    //到期日期
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item["RecordState"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mParamItem == null ? "" : mParamItem.ParamItemName);    //状态
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["InstrumentName"]));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Specification"]));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["SerialNo"]));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["ManageNo"]));    //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["CertificateNo"]));    //证书编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["BarCode"]));    //条码

                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["InstrumentCate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                //所属部门
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", item["BelongDepart"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);
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
        /// 检索条件
        /// </summary>
        /// <returns></returns>
        private string GetSearchCondition(DataTableUtils.DataTableModel dtm, string type)
        {
            string where = "InstrumentForm=" + (int)Constants.InstrumentForm.仪器;
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition)) where = string.Format("{0} and {1}", dtm.FieldCondition, where);
            string orgName = Request["searchBelongDepart"];
            if (type.Equals("Maintain"))
                where = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition(where, "Instrument-CheckAll", orgName);
            else
                where = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition(where, "Instrument-SearchCheckAll", orgName);
            //预警天数
            string isWarnDay = Request["searchIsWarnDay"];
            if (!string.IsNullOrEmpty(isWarnDay))
            {
                int iWarnDay = Convert.ToInt32(isWarnDay);
                where = string.Format("{0} and DueEndDate<='{1:yyyy-MM-dd}' and DueEndDate>='{2:yyyy-MM-dd}'", where, DateTime.Now.AddDays(iWarnDay), DateTime.Now);
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
            return where;
        }

        ///// <summary>
        ///// 获取用户管理部门的条件语句(仪器查询使用)
        ///// </summary>
        ///// <param name="where"></param>
        ///// <returns></returns>
        //private string GetManageConditionForInstrumentSearch(string where)
        //{
        //    string GetAllAuthorityStr = "Instrument-SearchCheckAll";
        //    bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

        //    if (!IsGetAllAuthority)
        //    {
        //        //获取当前用户所管辖的所有区域下的仪器SQL语句.
        //        StringBuilder subSqlStr = Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart");
        //        where = string.Format(" {0} and {1}", subSqlStr, where);
        //    }
        //    return where;
        //}

        ///// <summary>
        ///// 获取用户管理部门的条件语句(仪器维护使用)
        ///// </summary>
        ///// <param name="where"></param>
        ///// <returns></returns>
        //private string GetManageCondition(string where)
        //{
        //    string GetAllAuthorityStr = "Instrument-CheckAll";
        //    bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

        //    if (!IsGetAllAuthority)
        //    {
        //        //获取当前用户所管辖的所有区域下的仪器SQL语句.
        //        StringBuilder subSqlStr = Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart");
        //     where = string.Format(" {0} and {1}", subSqlStr, where);
        //    }
        //    return where;
        //}
        #endregion

        #region 同步数据方法
        /// <summary>
        /// 新增/修改仪器
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult SynInstrument(string instrumentId)
        {
            string result = "OK";
            result = ServiceProvider.InstrumentService.BeginSynInstrument(UtilsHelper.Decrypt2Int(instrumentId));
            return Content(result);
        }

        #endregion

        #region 仪器新增、修改、保存
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(string planDetailId = null)
        {
            Instrument.Common.Models.InstrumentModel model = new Instrument.Common.Models.InstrumentModel();
            model.BelongDepart = "";

            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //所属部门...........生成一个下拉框树所需的数据源
            ViewBag.BelongDepartList = Global.Business.ServiceProvider.OrgService.GetAll();
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ViewBag.InstrumentRecordStateList = new SelectList(instrumentState.itemsList, "ParamItemValue", "ParamItemName");
            //器具种类(设备类别)
            ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new ParamModel();
            ViewBag.InstrumentTypeList = new SelectList(instrumentType.itemsList, "ParamItemValue", "ParamItemName");

            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            //加载一级分类
            IList<ParamItemModel> cateList = instrumentCate.itemsList.Where(c => c.ParentCode == "0").ToList();
            ViewBag.InstrumentCateList = new SelectList(cateList, "ParamItemValue", "ParamItemName");
            //二级分类
            ViewBag.SubInstrumentCateList = new SelectList(instrumentCate.itemsList.Where(c => Convert.ToInt32(c.ParentCode) > 0), "ParamItemValue", "ParamItemName");

            //资产属性
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ViewBag.CalibrationTypeList = new SelectList(calibrationType.itemsList, "ParamItemValue", "ParamItemName");

            //管理级别
            ParamModel manageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);
            if (null == manageLevel) manageLevel = new ParamModel();
            ViewBag.ManageLevelList = new SelectList(manageLevel.itemsList, "ParamItemValue", "ParamItemName");

            //检定类别
            ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new ParamModel();
            ViewBag.VerificationTypeList = new SelectList(verificationType.itemsList, "ParamItemValue", "ParamItemName");

            ParamModel instrumentCertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState);
            if (null == instrumentCertificationState) instrumentCertificationState = new ParamModel();

            ViewBag.RecordStateList = new SelectList(instrumentCertificationState.itemsList, "ParamItemValue", "ParamItemName", -99);

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

        /// <summary>
        /// 新增：仪器基本信息和证书信息
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Insert(Instrument.Common.Models.InstrumentModel model, InstrumentCertificationModel certModel, FormCollection collection)
        {
            model.BelongDepart = collection["OrgName"];
            try
            {
                ServiceProvider.InstrumentService.Save4Instrument(model);
                if (Request.Files.AllKeys.Length > 0)
                    ServiceProvider.InstrumentService.SaveInstrumentPic(Request.Files, model.InstrumentId);
                //周检记录信息
                if (collection["RecordState2"] != null)
                {
                    if (Convert.ToInt32(collection["RecordState2"]) != Constants.InstrumentCertificationState.未检.GetHashCode())
                    {
                        //仪器的证书信息
                        if (!string.IsNullOrWhiteSpace(collection["CheckDate"])) certModel.CheckDate = Convert.ToDateTime(collection["CheckDate"]);
                        if (!string.IsNullOrWhiteSpace(collection["EndDate"])) certModel.EndDate = Convert.ToDateTime(collection["EndDate"]);
                        certModel.CreateUser = LoginHelper.LoginUser.UserName;
                        certModel.CertificationCode = collection["CertificationCode"];
                        certModel.MeasureOrg = collection["MeasureOrg"];
                        certModel.OrderNo = collection["OrderNo"];
                        certModel.RecordState = Convert.ToInt32(collection["RecordState2"]);
                        if (!string.IsNullOrWhiteSpace(collection["SendInstrumentDate"])) certModel.SendInstrumentDate = Convert.ToDateTime(collection["SendInstrumentDate"]);
                        if (!string.IsNullOrWhiteSpace(collection["ReturnInstrumentDate"])) certModel.ReturnInstrumentDate = Convert.ToDateTime(collection["ReturnInstrumentDate"]);
                        if (!string.IsNullOrWhiteSpace(collection["GetCertificateDate"])) certModel.GetCertificateDate = Convert.ToDateTime(collection["GetCertificateDate"]);
                        if (!string.IsNullOrWhiteSpace(collection["CertificateConfirmDate"])) certModel.CertificateConfirmDate = Convert.ToDateTime(collection["CertificateConfirmDate"]);
                        certModel.Remark = collection["Remark2"];
                        certModel.InstrumentId = model.InstrumentId;
                        ServiceProvider.InstrumentCertificationService.SaveCerAndPic(certModel, Request.Files);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json(new { Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Msg = "OK" }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult Edit(string instrumentId)
        {
            int instrumentIdDecrpt = UtilsHelper.Decrypt2Int(instrumentId);
            //获取仪器照片
            GetInstrumentPic(instrumentIdDecrpt);
            Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(instrumentIdDecrpt);
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            //所属部门...........生成一个下拉框树所需的数据源
            ViewBag.BelongDepartList = Global.Business.ServiceProvider.OrgService.GetAll();
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ViewBag.InstrumentRecordStateList = new SelectList(instrumentState.itemsList, "ParamItemValue", "ParamItemName");
            //器具种类(设备类别)
            ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new ParamModel();
            ViewBag.InstrumentTypeList = new SelectList(instrumentType.itemsList, "ParamItemValue", "ParamItemName");

            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            //加载一级分类
            IList<ParamItemModel> cateList = instrumentCate.itemsList.Where(c => c.ParentCode == "0").ToList();
            ViewBag.InstrumentCateList = new SelectList(cateList, "ParamItemValue", "ParamItemName", model.InstrumentCate);
            //二级分类
            IEnumerable<ParamItemModel> subList = instrumentCate.itemsList.Where(c => c.ParentCode == model.InstrumentCate.ToString());
            if (subList.Count() > 0) ViewBag.IsShowSubCate = true;
            else ViewBag.IsShowSubCate = false;

            ViewBag.SubInstrumentCateList = new SelectList(instrumentCate.itemsList.Where(c => c.ParentCode == model.InstrumentCate.ToString()), "ParamItemValue", "ParamItemName", model.SubInstrumentCate);


            //资产属性
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ViewBag.CalibrationTypeList = new SelectList(calibrationType.itemsList, "ParamItemValue", "ParamItemName");

            //管理级别
            ParamModel manageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);
            if (null == manageLevel) manageLevel = new ParamModel();
            ViewBag.ManageLevelList = new SelectList(manageLevel.itemsList, "ParamItemValue", "ParamItemName");

            //检定类别
            ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new ParamModel();
            ViewBag.VerificationTypeList = new SelectList(verificationType.itemsList, "ParamItemValue", "ParamItemName");
            return View(model);
        }

        /// <summary>
        /// 仪器照片
        /// </summary>
        /// <returns></returns>
        public ActionResult InstrumentPic(string picSrc)
        {
            return View();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public string Save(Instrument.Common.Models.InstrumentModel model, FormCollection collection)
        {
            try
            {
                model.BelongDepart = collection["OrgName"];
                if (collection["InstrumentId"] != "0")
                {
                    model.InstrumentId = UtilsHelper.Decrypt2Int(collection["InstrumentId"]);
                }
                //删除以前上传的仪器照片
                if (Request.Files.Count != 0)
                    ServiceProvider.BusinessAttachmentService.DeleteByKeyIdAndType(model.InstrumentId, Convert.ToString(Instrument.Common.Constants.AttachmentBusinessType.仪器照片.GetHashCode()));
                ServiceProvider.InstrumentService.SaveInstrumentPic(Request.Files, model.InstrumentId);
                ServiceProvider.InstrumentService.Save4Instrument(model);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 新增仪器时使用
        /// </summary>
        /// <param name="manageNo"></param>
        /// <returns></returns>
        public string ChkManageNo(string instrumentId, string manageNo)
        {
            bool IsExist = ServiceProvider.InstrumentService.IsExistManageNo(UtilsHelper.Decrypt2Int(instrumentId), manageNo);
            return IsExist ? "false" : "true";
        }

        /// <summary>
        /// 存在资产编号
        /// </summary>
        /// <param name="manageNo"></param>
        /// <returns></returns>
        public string ChkAssetsNo(string instrumentId, string assetsNo)
        {
            bool IsExist = true;
            if (!string.IsNullOrWhiteSpace(assetsNo)) IsExist = ServiceProvider.InstrumentService.IsExistAssetsNo(UtilsHelper.Decrypt2Int(instrumentId), assetsNo);
            return IsExist ? "false" : "true";
        }

        /// <summary>
        /// 新增仪器时使用
        /// </summary>
        /// <param name="certificateNo"></param>
        /// <returns></returns>
        public string ChkCertificateNo(string certificateNo)
        {
            var temCertModel = ServiceProvider.InstrumentCertificationService.GetByCertificationCode(certificateNo);
            if (temCertModel != null) return "false"; //存在相同的证书编号
            else return "true";        //不存在    
        }

        /// <summary>
        /// 编辑周期校准记录时使用
        /// </summary>
        /// <param name="certificateNo"></param>
        /// <returns></returns>
        public string ChkCertificateNo2(int logId, string certificateNo)
        {
            bool IsExist = ServiceProvider.InstrumentCertificationService.IsExistCertificationCode(logId, certificateNo);
            return IsExist ? "false" : "true";
        }

        /// <summary>
        /// 一级分类改变，获取二级分类
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public JsonResult GetSubInstrumentCate(string parentCode)
        {
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentCate);
            IList<ParamItemModel> subInstrumentCateList = instrumentCate.itemsList.Where(c => c.ParentCode == parentCode).ToList();

            return Json(subInstrumentCateList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 周期校准记录

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentCertificationList(string instrumentId, int dataType)
        {
            ViewBag.InstrumentId = instrumentId;
            ViewBag.DataType = dataType;
            return View();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public string GetInstrumentCertificationList(string instrumentId, int dataType)
        {
            IList<InstrumentCertificationModel> logList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();

            ///周检状态
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            IList<ParamItemModel> CertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();
            foreach (var log in logList)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                if (dataType == 1)
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/EditInstrumentCertification".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnUpdateInstrumentCertification({0})'>修改</a>&nbsp;&nbsp;", log.LogId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DeleteInstrumentCertification".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnDeleteInstrumentCertification({0},{1})'>删除</a>&nbsp;&nbsp;", log.LogId, log.InstrumentId);
                    }
                }
                else
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/GetInstrumentRepairPlanDetail".ToLower()))
                        sbOperate.AppendFormat("<a href='#' onclick='fnDetails({0})'>详细</a>&nbsp;&nbsp;", log.LogId);
                }
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DownloadCertification".ToLower()) && log.FileId != 0)
                {
                    sbOperate.AppendFormat("<a href='#' onclick='fnDownFile(this)' fileId='{0}'>下载</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(log.FileId.ToString()));
                    //sbOperate.Append(string.Format("<a href='/Certification/ReadCert?Id={1}' target='_blank' >{0}</a>", "浏 览", UtilsHelper.Encrypt(log.FileId.ToString())));
                }
                sbData.AppendFormat("\"{0}\"", sbOperate);
                mParamItem = CertificationState.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", log.RecordState));
                sbData.AppendFormat(",\"{0}\"", mParamItem == null ? "" : mParamItem.ParamItemName);    //状态
                sbData.AppendFormat(",\"{0}\"", log.CertificationCode); //证书编号
                sbData.AppendFormat(",\"{0:F2}\"", log.CertMoney);    //费用
                //sbData.AppendFormat(",\"{0}\"", log.OrderNo);    //送检单号
                sbData.AppendFormat(",\"{0}\"", log.MeasureOrg);    //计量机构
                sbData.AppendFormat(",\"{0}\"", log.CheckResult);    //检测结果
                sbData.AppendFormat(",\"{0}\"", log.ErrorValue);    //误差
                sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", log.CheckDate);    //校准日期
                sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", log.EndDate);    //到期日期
                //sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", log.SendInstrumentDate);    //送检日期
                //sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", log.ReturnInstrumentDate);    //返回日期
                //sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", log.GetCertificateDate);    //证书取回日期
                //sbData.AppendFormat(",\"{0:yyyy-MM-dd}\"", log.CertificateConfirmDate);    //证书确认日期
                //sbData.AppendFormat(",\"{0}\"", log.Remark);    //备注
                sbData.Append("],");
            }
            if (logList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }


        /// <summary>
        /// 关键字查询机构
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult QuickSearchOperatorNameByKeyWord(string term)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(term)) return Content("[]");
            ParamModel param = Global.Business.ServiceProvider.ParamService.GetByCode(Instrument.Common.Constants.SysParamType.TestOrg);
            IList<Hashtable> paramItem = Global.Business.ServiceProvider.ParamItemService.SearchByName(term, param.ParamId);
            if (paramItem == null) Content("[]");
            sb.Append("[");
            foreach (Hashtable item in paramItem)
            {
                sb.Append("{");
                sb.AppendFormat("\"label\":\"{0}\"", item["ParamItemName"]);
                sb.Append("},");
            }
            if (paramItem.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return Content(sb.ToString());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult EditInstrumentCertification(int logId, string instrumentId)
        {
            InstrumentCertificationModel model = null;
            if (0 == logId)
            {
                model = new InstrumentCertificationModel();
                model.InstrumentId = UtilsHelper.Decrypt2Int(instrumentId);
                model.RecordState = -99;
            }
            else
            {
                model = ServiceProvider.InstrumentCertificationService.GetById(logId);
            }
            AttachmentModel attModel = Global.Business.ServiceProvider.AttachmentService.GetById(model.FileId);
            if (attModel == null) attModel = new AttachmentModel();
            ViewBag.FileName = attModel.FileName + Path.GetExtension(attModel.FileVirtualPath);
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel instrumentCertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState);
            if (null == instrumentCertificationState) instrumentCertificationState = new ParamModel();
            IList<ParamItemModel> recordStateList = instrumentCertificationState.itemsList.Where(r => Convert.ToInt32(r.ParamItemValue) != Constants.InstrumentCertificationState.未检.GetHashCode()).ToList();
            ViewBag.RecordStateList = new SelectList(recordStateList, "ParamItemValue", "ParamItemName", model.RecordState);
            return View("InstrumentCertificationEdit", model);
        }

        /// <summary>
        /// 同步周检记录
        /// </summary>
        /// <param name="CertificationCode"></param>
        /// <returns></returns>
        public ActionResult SynchronousInstrumentCert(string certificationCode, string InstrumentId, string LogId)
        {
            string jsonData = GRGTCommonUtils.WSProvider.MeasureLabProvider.GetCertInfoByCertificateNumber(SSOHelper.Encrypt(certificationCode), Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            Hashtable certificationInfoList = new Hashtable();
            InstrumentCertificationModel certModerl = new InstrumentCertificationModel();
            if (dic["Msg"].ToString() == "OK")
            {
                certificationInfoList = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(Hashtable)) as Hashtable;
                certModerl.CheckDate = Convert.ToDateTime(certificationInfoList["DueStartDate"]);
                certModerl.EndDate = Convert.ToDateTime(certificationInfoList["DueEndDate"]);
                certModerl.CertificationCode = certificationInfoList["CertificateNumber"].ToString();
                certModerl.MeasureOrg = "广电计量";
                certModerl.CreateUser = LoginHelper.LoginUser.UserName;
                certModerl.ItemCode = Guid.NewGuid().ToString();
                certModerl.RecordState = Constants.InstrumentCertificationState.完成周检.GetHashCode();
                certModerl.CertMoney = Convert.ToDecimal(certificationInfoList["ServiceFee"]);
                certModerl.InstrumentId = Convert.ToInt32(InstrumentId);
                certModerl.LogId = Convert.ToInt32(LogId);
                //ServiceProvider.InstrumentCertificationService.Insert(certModerl);
                //同步证书文件
                byte[] bytes = WSProvider.MeasureLabProvider.DownloadCertification(certificationCode, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
                ServiceProvider.InstrumentCertificationService.Synchronous(certModerl, new MemoryStream(bytes), string.Format("{0}.pdf", certificationCode), certificationCode);
            }
            else
                return Json(new { Msg = dic["Msg"] }, JsonRequestBehavior.AllowGet);
            return Json(new { Msg = "OK", Data = certModerl, JsonRequestBehavior.AllowGet });
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="collection"></param>
        [HttpPost]
        public ActionResult SaveInstrumentCertification(InstrumentCertificationModel model, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                string msg = UtilsHelper.GetValidateErrorMsg(ModelState.Values).ToString();
                return Json(new { Msg = msg }, "text/html", JsonRequestBehavior.AllowGet);
            }
            try
            {
                ServiceProvider.InstrumentCertificationService.Save(model, Request.Files);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Json(new { Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Msg = "OK" }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="collection"></param>
        public ActionResult DeleteInstrumentCertification(int logId, string instrumentId)
        {
            string msg = ServiceProvider.InstrumentCertificationService.DeleteById(logId, UtilsHelper.Decrypt2Int(instrumentId));
            return Content(msg);
        }

        #endregion

        #region 维修计划

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentRepairPlanList(string instrumentId, int dataType)
        {
            ViewBag.InstrumentId = instrumentId;
            ViewBag.DataType = dataType;
            return View();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public string GetInstrumentRepairPlanList(string instrumentId, int DataType)
        {
            instrumentId = UtilsHelper.Decrypt(instrumentId);
            IList<InstrumentRepairPlanModel> planList = ServiceProvider.InstrumentRepairPlanService.GetByInstrumentId(Convert.ToInt32(instrumentId));
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();
            foreach (var plan in planList)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                if (DataType == 1)
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/EditInstrumentRepairPlan".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnUpdateInstrumentRepairPlan({0})'>修改</a>&nbsp;&nbsp;", plan.PlanId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DeleteInstrumentRepairPlan".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnDeleteInstrumentRepairPlan({0})'>删除</a>&nbsp;&nbsp;", plan.PlanId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentRepairPlanAttachmentList".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnUploadRepairPlanAttachment({0})'>上传附件</a>&nbsp;&nbsp;", plan.PlanId);
                    }
                }
                else
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/GetInstrumentRepairPlanDetail".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnGetPlanDetail({0})'>详细</a>&nbsp;&nbsp;", plan.PlanId);
                    }
                }
                sbData.AppendFormat("\"{0}\"", sbOperate);
                sbData.AppendFormat(",\"{0}\"", plan.RepairCompany);    //维修公司
                sbData.AppendFormat(",\"{0}\"", plan.Repairer);    //维修人员
                sbData.AppendFormat(",\"{0}\"", plan.Mobile);    //联系电话
                sbData.AppendFormat(",\"{0:F2}\"", plan.RepairMoney);    //维修金额
                sbData.AppendFormat(",\"{0:yyyy-MM-dd HH:mm:ss}\"", plan.DueStartDate);    //开始时间
                sbData.AppendFormat(",\"{0:yyyy-MM-dd HH:mm:ss}\"", plan.DueEndDate);    //结束时间
                sbData.AppendFormat(",\"{0}\"", plan.Leader);    //报修人
                //sbData.AppendFormat(",\"{0}\"", plan.Remark);    //备注
                sbData.Append("],");
            }
            if (planList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult EditInstrumentRepairPlan(int planId, string instrumentId)
        {
            InstrumentRepairPlanModel model = null;
            if (0 == planId)
            {
                instrumentId = UtilsHelper.Decrypt(instrumentId);
                model = new InstrumentRepairPlanModel();
                model.InstrumentId = Convert.ToInt32(instrumentId);
            }
            else
            {
                model = ServiceProvider.InstrumentRepairPlanService.GetById(planId);
            }
            return View("InstrumentRepairPlanEdit", model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult SaveInstrumentRepairPlan(FormCollection collection)
        {
            InstrumentRepairPlanModel model = new InstrumentRepairPlanModel();
            TryUpdateModel(model);
            ServiceProvider.InstrumentRepairPlanService.Save(model);
            return Content("OK");
        }

        /// <summary>
        /// 维修附件列表
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult InstrumentRepairPlanAttachmentList(string planId)
        {
            ViewBag.PlanId = planId;
            return View();
        }


        /// <summary>
        /// 删除维修记录
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult DeleteInstrumentRepairPlan(int planId)
        {
            ServiceProvider.InstrumentRepairPlanService.DeleteById(planId);
            ServiceProvider.BusinessAttachmentService.DeleteByKeyIdAndType(planId, Convert.ToString(Instrument.Common.Constants.AttachmentBusinessType.维修单.GetHashCode()));
            return Content("OK");
        }


        /// <summary>
        /// 获取维修计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult InstrumentRepairPlanListForPage()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            return View();
        }

        /// <summary>
        /// 构建维修计划列表JSON
        /// </summary>
        /// <param name="iType"></param>
        /// <returns></returns>
        public JsonResult GetInstrumentRepairPlanJson()
        {
            //提取DataTable参数
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = "PlanId,InstrumentId,Repairer,RepairCompany,Mobile,DueStartDate,DueEndDate,Leader,Remark,CreateDate,Creator,ItemCode,ReportCode,RepairMoney,Reason,TermService";
            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? dtm.FieldCondition = "1=1" : dtm.FieldCondition;
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            //所属部门
            string orgName = Request["searchBelongDepart"];
            //string strFilte = "";
            //if (!string.IsNullOrEmpty(orgName))
            //{
            //    strFilte = " and (1=0 ";
            //    orgList = orgList.Where(o => o.OrgName == orgName).ToList();
            //    foreach (OrgModel org in orgList)
            //    {
            //        strFilte = string.Format("{0} or BelongDepart like '{1}%'", strFilte, org.OrgCode);
            //    }
            //    strFilte = string.Format("{0})", strFilte);
            //}
            //添加委托单查询条件.
            string orderParam = Request["InstrumentParam"];
            if (!string.IsNullOrWhiteSpace(orderParam))
            {
                orderParam = string.Format(" and {0}", orderParam);
            }
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", orgName);
            paging.Where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {2}{1}))", paging.Where, orderParam, sManageCondition);

            //数据库查询数据
            IList<Hashtable> RepairPlanList = ServiceProvider.InstrumentRepairPlanService.GetInstrumentRepairPlanForPaging(paging);
            IList<int> instrumentIds = RepairPlanList.Select(s => Convert.ToInt32(s["InstrumentId"])).Distinct().ToList();
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);
            ////系统参数
            OrgModel belongDeptModel = new OrgModel();
            ParamItemModel mInstrumentCate = null;
            IList<ParamItemModel> paramItemList = Global.Business.ServiceProvider.ParamService.GetByCode(Instrument.Common.Constants.SysParamType.InstrumentCate).itemsList;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            Instrument.Common.Models.InstrumentModel instrumentModel;
            StringBuilder sbOperate = new StringBuilder();
            foreach (Hashtable row in RepairPlanList)
            {
                instrumentModel = instrumentList.SingleOrDefault(w => w.InstrumentId == Convert.ToInt32(row["InstrumentId"]));
                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<div PlanId='{0}'>", row["PlanId"].ToString());
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/GetAttachments".ToLower()))
                {
                    //详细
                    sbOperate.Append("<a href='#' onclick='fnGetDetails(this);'>详细</a>&nbsp;&nbsp;");
                }
                sbOperate.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.InstrumentName)); //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.ManageNo)); //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.Specification)); //型号规格
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.SerialNo)); //出厂编号
                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrumentModel.InstrumentCate));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", instrumentModel.BelongDepart));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["RepairCompany"])); //维修公司
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Repairer"])); //维修人
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Mobile"])); //联系电话
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["RepairMoney"])); //维修金额
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Leader"])); //保修人
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["ReportCode"])); //报告编号
                dtm.aaData[dtm.aaData.Count - 1].Add(row["DueStartDate"] == null ? null : string.Format("{0:yyyy-MM-dd HH:mm:ss}", row["DueStartDate"].ToString()));   //开始日期 
                dtm.aaData[dtm.aaData.Count - 1].Add(row["DueEndDate"] == null ? null : string.Format("{0:yyyy-MM-dd HH:mm:ss}", row["DueEndDate"].ToString())); //结束日期
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Reason"])); //故障原因
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["TermService"])); //保修期限
                //dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Remark"])); //故障描述
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
        /// 导出维护信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult ExportRepairDataBySearchCondition()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            string where = GetRepairSearchCondition(dtm, orgList);
            IList<Instrument.Common.Models.InstrumentRepairPlanModel> RepairPlanList = ServiceProvider.InstrumentRepairPlanService.GetByWhere(where);
            if (0 == RepairPlanList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            IList<int> instrumentIds = RepairPlanList.Select(s => s.InstrumentId).Distinct().ToList();
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);
            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("InstrumentName", typeof(string));
            dtData.Columns.Add("ManageNo", typeof(string));
            dtData.Columns.Add("Specification", typeof(string));
            dtData.Columns.Add("SerialNo", typeof(string));
            dtData.Columns.Add("InstrumentCate", typeof(string));
            dtData.Columns.Add("BelongDepart", typeof(string));
            dtData.Columns.Add("RepairCompany", typeof(string));
            dtData.Columns.Add("Repairer", typeof(string));
            dtData.Columns.Add("Mobile", typeof(string));
            dtData.Columns.Add("RepairMoney", typeof(string));
            dtData.Columns.Add("Leader", typeof(string));
            dtData.Columns.Add("ReportCode", typeof(string));
            dtData.Columns.Add("DueStartDate", typeof(string));
            dtData.Columns.Add("DueEndDate", typeof(string));
            dtData.Columns.Add("Reason", typeof(string));
            dtData.Columns.Add("TermService", typeof(string));
            dtData.Columns.Add("Remark", typeof(string));
            OrgModel belongDeptModel = new OrgModel();
            ParamItemModel mInstrumentCate = null;
            IList<ParamItemModel> paramItemList = Global.Business.ServiceProvider.ParamService.GetByCode(Instrument.Common.Constants.SysParamType.InstrumentCate).itemsList;
            Instrument.Common.Models.InstrumentModel instrumentModel;
            foreach (var item in RepairPlanList)
            {
                DataRow drData = dtData.NewRow();
                instrumentModel = instrumentList.SingleOrDefault(w => w.InstrumentId == item.InstrumentId);
                drData["InstrumentName"] = instrumentModel == null ? "" : instrumentModel.InstrumentName; //仪器名称
                drData["ManageNo"] = instrumentModel == null ? "" : instrumentModel.ManageNo; //管理编号
                drData["Specification"] = instrumentModel == null ? "" : instrumentModel.Specification; //型号规格
                drData["SerialNo"] = instrumentModel == null ? "" : instrumentModel.SerialNo; //出厂编号
                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrumentModel.InstrumentCate));
                drData["InstrumentCate"] = (mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", instrumentModel.BelongDepart));
                drData["BelongDepart"] = (belongDeptModel == null ? "" : belongDeptModel.OrgName);
                drData["RepairCompany"] = item.RepairCompany; //维修公司
                drData["Repairer"] = item.Repairer; //维修人
                drData["Mobile"] = item.Mobile; //联系电话
                drData["RepairMoney"] = item.RepairMoney; //维修金额
                drData["Leader"] = item.Leader; //保修人
                drData["ReportCode"] = item.ReportCode; //报告编号
                drData["DueStartDate"] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.DueStartDate); //开始日期
                drData["DueEndDate"] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.DueStartDate); //结束日期
                drData["Reason"] = item.Reason; //故障原因
                drData["TermService"] = item.TermService; //保修期限
                drData["Remark"] = item.Remark; //故障描述
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
                "仪器名称", "管理编号", "仪器型号", "出厂编号","分类","所属部门", "维修公司","维修人","联系电话","维修金额","报修人", "报告编号", "报修日期", "修复日期", "故障原因", "保修期限", "故障描述"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "维修", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}仪器维修记录{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }
        /// <summary>
        /// 检索条件
        /// </summary>
        /// <returns></returns>
        private string GetRepairSearchCondition(DataTableUtils.DataTableModel dtm, IList<OrgModel> orgList)
        {
            string where = "1=1";
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                where = string.Format("{0} and {1}", dtm.FieldCondition, where);
            }
            string orderParam = Request["InstrumentParam"];
            if (!string.IsNullOrWhiteSpace(orderParam))
            {
                orderParam = string.Format(" and {0}", orderParam);
            }

            orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            //所属部门
            string orgName = Request["searchBelongDepart"];
            //string strFilte = "";
            //if (!string.IsNullOrEmpty(orgName))
            //{
            //    strFilte = " and (1=0 ";
            //    orgList = orgList.Where(o => o.OrgName == orgName).ToList();
            //    foreach (OrgModel org in orgList)
            //    {
            //        strFilte = string.Format("{0} or BelongDepart like '{1}%'", strFilte, org.OrgCode);
            //    }
            //    strFilte = string.Format("{0})", strFilte);
            //}
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", orgName);
            where = string.Format("{0} and (InstrumentId In (Select InstrumentId From Instrument_BaseInfo Where {2}{1}))", where, orderParam, sManageCondition);
            return where;
        }
        /// <summary>
        /// 查看附件信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult GetAttachments(int AttachmentBusinessType, string BusinessId)
        {
            ViewBag.AttachmentBusinessType = AttachmentBusinessType;
            ViewBag.BusinessId = BusinessId;
            return View();
        }
        /// <summary>
        /// 查看维修详细
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInstrumentRepairPlanDetail(int planId)
        {
            InstrumentRepairPlanModel model = ServiceProvider.InstrumentRepairPlanService.GetById(planId);
            return View("InstrumentRepairPlanDetail", model);
        }
        #endregion

        #region 设备说明书

        /// <summary>
        /// 设备说明书列表
        /// </summary>
        /// <returns></returns>
        public ActionResult InstrumentManualListForPage()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            return View();
        }

        /// <summary>
        /// 构建设备说明书列表JSON
        /// </summary>
        /// <param name="iType"></param>
        /// <returns></returns>
        public JsonResult GetInstrumentManualJson()
        {
            //提取DataTable参数
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = "Id,BusinessType,BusinessKeyId,FileId,FileName,UserName,CreateDate,Remark";
            paging.Where = string.Format("({0} and BusinessType={1})", string.IsNullOrWhiteSpace(dtm.FieldCondition) ? dtm.FieldCondition = "1=1" : dtm.FieldCondition, Instrument.Common.Constants.AttachmentBusinessType.设备档案.GetHashCode());
            string orderParam = Request["InstrumentParam"];
            if (!string.IsNullOrWhiteSpace(orderParam))
            {
                orderParam = string.Format(" and {0}", orderParam);
            }
            //所属部门
            string orgName = Request["searchBelongDepart"];
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", orgName);
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();

            paging.Where = string.Format("{0} and (BusinessKeyId In (Select InstrumentId From Instrument_BaseInfo Where {2}{1}))", paging.Where, orderParam, sManageCondition);
            //paging.Where = string.Format("{0} and (BusinessKeyId In (Select InstrumentId From Instrument_BaseInfo Where {1}))", sType, paging.Where);

            //数据库查询数据
            IList<Hashtable> ManualList = ServiceProvider.BusinessAttachmentService.GetInstrumentManualForPaging(paging);
            IList<int> instrumentIds = ManualList.Select(s => Convert.ToInt32(s["BusinessKeyId"])).Distinct().ToList();
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(instrumentIds);
            ////系统参数
            OrgModel belongDeptModel = new OrgModel();
            ParamItemModel mInstrumentCate = null;
            IList<ParamItemModel> paramItemList = Global.Business.ServiceProvider.ParamService.GetByCode(Instrument.Common.Constants.SysParamType.InstrumentCate).itemsList;
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            Instrument.Common.Models.InstrumentModel instrumentModel;
            StringBuilder sbOperate = new StringBuilder();
            foreach (Hashtable row in ManualList)
            {
                instrumentModel = instrumentList.SingleOrDefault(w => w.InstrumentId == Convert.ToInt32(row["BusinessKeyId"]));
                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                //sbOperate.AppendFormat("<div Id='{0}'>", row["Id"].ToString());
                //    sbOperate.Append("<a href='#' onclick='fnGetDetails(this);'>下载</a>&nbsp;&nbsp;");
                //sbOperate.Append("</div>");
                if (row["FileName"] != "")
                {
                    sbOperate.Append("<a href='#' onclick='fnDownFile(\"" + UtilsHelper.Encrypt(row["FileId"].ToString()) + "\");'>下 载</a>&nbsp;&nbsp;");
                }
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.InstrumentName)); //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.ManageNo)); //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.Specification)); //型号规格
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrumentModel == null ? "" : instrumentModel.SerialNo)); //出厂编号
                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrumentModel.InstrumentCate));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);//分类
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", instrumentModel.BelongDepart));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);//所属部门
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["FileName"]));//文件名字
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["CreateDate"]));//创建时间
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Remark"]));//备注
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

        #region 工艺过程
        public ActionResult CraftEdit(string instrumentId)
        {
            Instrument.Common.Models.InstrumentModel mIntrument = ServiceProvider.InstrumentService.GetById(UtilsHelper.Decrypt2Int(instrumentId));
            CraftModel mCraft = ServiceProvider.CraftService.GetById(mIntrument.CraftId);
            if (mCraft == null) mCraft = new CraftModel();
            ViewBag.InstrumentId = instrumentId;
            return View(mCraft);
        }

        /// <summary>
        /// 选择工艺
        /// </summary>
        /// <returns></returns>
        public ActionResult ChoseCraft()
        {
            return View("ChooseCraft");
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

        /// <summary>
        /// 更新仪器下对应的工艺
        /// </summary>
        /// <param name="craftId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult UpdateInstrumentOfCraft(string craftId, string instrumentId)
        {
            try
            {
                ServiceProvider.InstrumentService.UpdateInstrumentOfCraft(Convert.ToInt32(craftId), UtilsHelper.Decrypt2Int(instrumentId));
                return Content("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("更新失败");
            }
        }
        #endregion

        #region 体系文件
        public ActionResult InstrumentOwnDocumentList(string instrumentId, string dataType)
        {
            ViewBag.DataType = dataType;
            ViewBag.InstrumentId = instrumentId;
            return View();
        }

        public ActionResult ChooseDocument()
        {
            return View();
        }

        /// <summary>
        /// 仪器关联的体系文件列表/体系文件库
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <param name="type">1:体系文件库，2：仪器已关联的体系文件</param>
        /// <returns></returns>
        public string GetDocumentJsonData(string instrumentId, string type)
        {
            //文件分类
            IList<ParamModel> list = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel paramModel = list.SingleOrDefault(p => p.ParamCode == Constants.SysParamType.DocumentType);
            IList<DocumentModel> documentList = new List<DocumentModel>();
            if (type.Equals("2"))
                documentList = ServiceProvider.DocumentService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
            else
                documentList = ServiceProvider.DocumentService.GetAll();
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();
            foreach (var document in documentList)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                if (type.Equals("2"))
                {
                    if (LoginHelper.LoginUserAuthorize.Contains("/Instrument/DeleteInstrumentOwnDocument".ToLower()))
                        sbOperate.AppendFormat("<a href='#' id='{0}' onclick='fnDeleteRelation(this)'>删除关联</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(document.DocumentId.ToString()));
                    else
                        sbOperate.Append("");
                }
                else
                    sbOperate.AppendFormat("<a href='#' id='{0}' onclick='fnChooseDocumentSelect(this)'>选择</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(document.DocumentId.ToString()));
                sbData.AppendFormat("\"{0}\"", sbOperate);

                sbData.AppendFormat(",\"<a target='_blank' href='/SysManage/Attachment/DownLoad?fileId={0}'>{1}</a>\"", UtilsHelper.Encrypt(document.FileId.ToString()), document.FileName);
                ParamItemModel item = paramModel.itemsList.SingleOrDefault(p => p.ParamItemValue == document.DocCategory.ToString());
                sbData.AppendFormat(",\"{0}\"", item == null ? "" : item.ParamItemName);
                //sbData.AppendFormat(",\"{0}\"", document.DocCategory);
                sbData.AppendFormat(",\"{0:d}\"", document.CreateDate);
                sbData.AppendFormat(",\"{0}\"", document.Remark);
                sbData.Append("],");
            }
            if (documentList.Count > 0)
                sbData.Remove(sbData.Length - 1, 1);
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 关联体系文件
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult AddMoreDocument(string documentId, string instrumentId)
        {
            try
            {
                string result = ServiceProvider.DocumentService.AddMoreDocument(UtilsHelper.Decrypt2Int(documentId), UtilsHelper.Decrypt(instrumentId));
                return Content(result);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("关联失败");
            }
        }
        public ActionResult AddOwnDocument(string documentId, string instrumentId)
        {
            try
            {
                string result = ServiceProvider.DocumentService.AddOwnDocument(UtilsHelper.Decrypt2Int(documentId), UtilsHelper.Decrypt2Int(instrumentId));
                return Content(result);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("关联失败");
            }
        }


        /// <summary>
        /// 删除关联
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult DeleteInstrumentOwnDocument(string documentId, string instrumentId)
        {
            try
            {
                ServiceProvider.DocumentService.DeleteInstrumentOwnDocument(UtilsHelper.Decrypt2Int(documentId), UtilsHelper.Decrypt2Int(instrumentId));
                return Content("OK");
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                return Content("删除失败");
            }
        }

        #endregion

        #region 导出
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult ExportDataBySearchCondition(string type)
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = GetSearchCondition(dtm, type);
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(where);
            if (0 == instrumentList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            ParamModel ManageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);    //管理级别
            ParamModel CalibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);    //校准类别
            ParamModel VerificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);    //检定类别
            ///设备状态
            IList<ParamItemModel> RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetsState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();

            ParamItemModel mVerificationType = null;
            ParamItemModel mCalibrationType = null;
            ParamItemModel mInstrumentCate = null;
            ParamItemModel mManageLevel = null;

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();

            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("IsOverTime", typeof(string));    //证书超期
            dtData.Columns.Add("WarnDay", typeof(string));    //预警天数
            dtData.Columns.Add("RecordState", typeof(string));    //状态
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("EnglishName", typeof(string));    //
            dtData.Columns.Add("Specification", typeof(string));    //型号
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
            dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
            dtData.Columns.Add("CertificateNo", typeof(string));    //证书编号
            dtData.Columns.Add("BarCode", typeof(string));    //条码
            dtData.Columns.Add("DurableYears", typeof(string));    //
            dtData.Columns.Add("AssetsNo", typeof(string));    //
            dtData.Columns.Add("ManageLevel", typeof(string));    //
            dtData.Columns.Add("InspectCycle", typeof(string));    //
            dtData.Columns.Add("InspectOrg", typeof(string));    //
            dtData.Columns.Add("MeasureCharacter", typeof(string));    //
            dtData.Columns.Add("TechniqueCharacter", typeof(string));    //
            dtData.Columns.Add("ProjectTeam", typeof(string));    //
            //dtData.Columns.Add("SpecificationCode", typeof(string));    //
            dtData.Columns.Add("CalibrationType", typeof(string));    //
            dtData.Columns.Add("VerificationType", typeof(string));    //
            dtData.Columns.Add("BuyDate", typeof(string));    //
            dtData.Columns.Add("Price", typeof(string));    //
            dtData.Columns.Add("InstrumentCate", typeof(string));    //设备分类
            dtData.Columns.Add("Manufacturer", typeof(string));    //生产厂家
            dtData.Columns.Add("ManufactureContactor", typeof(string));    //
            dtData.Columns.Add("BelongDepart", typeof(string));    //所属部门
            dtData.Columns.Add("DueStartDate", typeof(string));    //校准日期
            dtData.Columns.Add("DueEndDate", typeof(string));    //到期日期
            dtData.Columns.Add("LeaderName", typeof(string));    //保管人
            dtData.Columns.Add("StorePalce", typeof(string));    //存放地址
            dtData.Columns.Add("CreateUser", typeof(string));    //创建人
            dtData.Columns.Add("Remark", typeof(string));    //备注
            DateTime dueEndDate; bool isOverTime = false;
            foreach (var item in instrumentList)
            {
                if (item.DueEndDate == null) item.DueEndDate = DateTime.MinValue;
                dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item.DueEndDate));

                //isOverTime = DateTime.Now.CompareTo(dueEndDate) > 1 ? true : false;
                //超期
                isOverTime = dueEndDate < Convert.ToDateTime(string.Format("{0:d}", DateTime.Now));
                //预警天数
                int warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                warnDay = (warnDay < 0) ? 0 : warnDay;
                //超期无预警天数
                if (isOverTime) warnDay = 0;

                DataRow drData = dtData.NewRow();
                drData["IsOverTime"] = isOverTime ? "已超期" : "未超期";   //证书超期
                drData["WarnDay"] = (warnDay == 0) ? "无预警" : warnDay.ToString() + "天";   //预警天数
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item.RecordState));
                drData["RecordState"] = string.Format("{0}", mParamItem == null ? "" : mParamItem.ParamItemName);   //状态
                drData["InstrumentName"] = string.Format("{0}", item.InstrumentName);    //仪器名称
                drData["EnglishName"] = string.Format("{0}", item.EnglishName);    //英文名称
                drData["Specification"] = string.Format("{0}", item.Specification);    //型号
                drData["SerialNo"] = string.Format("{0}", item.SerialNo);    //出厂编号
                drData["ManageNo"] = string.Format("{0}", item.ManageNo);    //管理编号
                drData["CertificateNo"] = string.Format("{0}", item.CertificateNo);    //证书编号
                drData["BarCode"] = string.Format("{0}", item.BarCode);
                drData["DurableYears"] = string.Format("{0}", item.DurableYears);    //使用年限
                drData["AssetsNo"] = string.Format("{0}", item.AssetsNo);    //资产编号
                //管理级别
                mManageLevel = ManageLevel.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item.ManageLevel));
                drData["ManageLevel"] = string.Format("{0}", mManageLevel == null ? "" : mManageLevel.ParamItemName);
                drData["InspectCycle"] = string.Format("{0}", item.InspectCycle);    //检验周期
                drData["InspectOrg"] = string.Format("{0}", item.InspectOrg);    //检验机构
                drData["MeasureCharacter"] = string.Format("{0}", item.MeasureCharacter);    //计量特性
                drData["TechniqueCharacter"] = string.Format("{0}", item.TechniqueCharacter);    //技术特征
                drData["ProjectTeam"] = string.Format("{0}", item.ProjectTeam);    //项目组
                //drData["SpecificationCode"] = item.SpecificationCode;    //说明书编号
                //校准类别
                mCalibrationType = ManageLevel.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item.CalibrationType));
                drData["CalibrationType"] = string.Format("{0}", mCalibrationType == null ? "" : mCalibrationType.ParamItemName);
                //检定类别
                mVerificationType = ManageLevel.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item.VerificationType));
                drData["VerificationType"] = string.Format("{0}", mVerificationType == null ? "" : mVerificationType.ParamItemName);

                drData["BuyDate"] = string.Format("{0}", item.BuyDate);    //购置日期
                drData["Price"] = string.Format("{0}", item.Price);    //购置金额
                //分类
                mInstrumentCate = InstrumentCate.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item.InstrumentCate));
                drData["InstrumentCate"] = string.Format("{0}", mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                drData["Manufacturer"] = string.Format("{0}", item.Manufacturer);    //生产厂家
                drData["ManufactureContactor"] = string.Format("{0}", item.ManufactureContactor);    //生产厂家联系信息

                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == item.BelongDepart);
                drData["BelongDepart"] = string.Format("{0}", belongDeptModel == null ? "" : OrgHelper.GetOrgNameTreeByOrgId(belongDeptModel.ParentOrgId, orgList, belongDeptModel.OrgName));

                drData["DueStartDate"] = string.Format("{0}", item.DueStartDate == null ? "" : item.DueStartDate.Value.ToString("yyyy-MM-dd"));  //校准日期
                drData["DueEndDate"] = string.Format("{0}", item.DueEndDate == null ? "" : item.DueEndDate.Value.ToString("yyyy-MM-dd"));   //到期日期
                drData["LeaderName"] = string.Format("{0}", item.LeaderName);  //保管人
                drData["StorePalce"] = string.Format("{0}", item.StorePalce);  //存放地址
                drData["CreateUser"] = string.Format("{0}", item.CreateUser);  //创建人
                drData["Remark"] = string.Format("{0}", item.Remark);  //备注
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
                "证书超期","预警天数","状态", "仪器名称", "英文名称", "仪器型号", "出厂编号", "管理编号", "证书编号","条码", "使用年限", "资产编号", "管理级别", "检验周期", "检验机构", "计量特性", "技术特征", "项目组",
                  "校准类别", "检定类别", "购置日期", "购置金额",
                "分类","生产厂家","生产厂家联系信息","所属部门", "校准日期", "到期日期", "保管人", "存放地址","创建人","备注"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "Sheet1", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}仪器{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        /// <summary>
        /// 导出(周期校准记录)
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportInstrumentCheckLogList()
        //{
        //    //查询周期校准记录
        //    DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
        //    string where = GetSearchCondition();
        //    IList<Hashtable> logList = ServiceProvider.InstrumentCheckLogService.GetInstrumentCheckLogListByWhere(where);
        //    if (0 == logList.Count)
        //    {
        //        Response.Write("没有要导出的内容。");
        //        Response.End();
        //        return Content("");
        //    }
        //    //系统参数
        //    IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
        //    ParamModel branchCompany = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.BranchCompany);    //分公司
        //    if (null == branchCompany)
        //    {
        //        branchCompany = new ParamModel();
        //    }
        //    ParamItemModel mBranchCompany = null;
        //    //数据列表
        //    DataTable dtData = new DataTable();
        //    dtData.Columns.Add("BelongSubCompany", typeof(string));    //分公司
        //    dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
        //    dtData.Columns.Add("InstrumentName", typeof(string));    //设备名称
        //    dtData.Columns.Add("Specification", typeof(string));    //规格型号
        //    dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
        //    dtData.Columns.Add("MeasureOrg", typeof(string));    //计量机构
        //    dtData.Columns.Add("CheckDate", typeof(DateTime));    //周检日期
        //    dtData.Columns.Add("SendInstrumentDate", typeof(DateTime));    //仪器送出日期
        //    dtData.Columns.Add("OrderNo", typeof(string));    //委托单号
        //    dtData.Columns.Add("ReturnInstrumentDate", typeof(DateTime));    //仪器返还日期  
        //    dtData.Columns.Add("GetCertificateDate", typeof(DateTime));    //证书取回日期 
        //    dtData.Columns.Add("CertificateConfirmDate", typeof(DateTime));    //证书计量确认日期
        //    foreach (var item in logList)
        //    {
        //        DataRow drData = dtData.NewRow();
        //        mBranchCompany = branchCompany.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(item["BelongSubCompany"]));    //分公司
        //        if (null == mBranchCompany)
        //        {
        //            mBranchCompany = new ParamItemModel();
        //        }
        //        drData["BelongSubCompany"] = mBranchCompany.ParamItemName;
        //        drData["ManageNo"] = Convert.ToString(item["ManageNo"]);   //管理编号
        //        drData["InstrumentName"] = Convert.ToString(item["InstrumentName"]);    //设备名称
        //        drData["Specification"] = Convert.ToString(item["Specification"]);    //规格型号
        //        drData["SerialNo"] = Convert.ToString(item["SerialNo"]);    //出厂编号
        //        drData["MeasureOrg"] = Convert.ToString(item["MeasureOrg"]);    //计量机构
        //        drData["CheckDate"] = Convert.ToDateTime(item["CheckDate"]);   //周检日期
        //        drData["SendInstrumentDate"] = Convert.ToDateTime(item["SendInstrumentDate"]);    //仪器送出日期
        //        drData["OrderNo"] = Convert.ToString(item["OrderNo"]);    //委托单号
        //        drData["ReturnInstrumentDate"] = Convert.ToDateTime(item["ReturnInstrumentDate"]);    //仪器返还日期
        //        drData["GetCertificateDate"] = Convert.ToDateTime(item["GetCertificateDate"]);    //证书取回日期
        //        drData["CertificateConfirmDate"] = Convert.ToDateTime(item["CertificateConfirmDate"]);    //证书计量确认日期
        //        dtData.Rows.Add(drData);
        //    }
        //    //导出
        //    List<string> headerList = new List<string>(new string[] { 
        //        "部门", "管理编号", "设备名称", "规格型号", "出厂编号", "计量机构", "周检日期", "仪器送出日期", "委托单号", "仪器返还日期", "证书取回日期", "证书计量确认日期" });
        //    string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "周期校准记录", ToolsLib.LibConst.ExcelVersion.Excel2007);
        //    ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}周期校准记录{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
        //    return Content("OK");
        //}

        #endregion

        #region 删除仪器
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult Delete(string instrumentId)
        {
            //同步删除
            string result = "OK";
            try
            {
                if (LoginHelper.LoginUserAuthorize.Contains("/Instrument/SynInstrument".ToLower()))
                {
                    Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(UtilsHelper.Decrypt2Int(instrumentId));
                    result = WSProvider.CommonProvider.DeleteInstrumentData(ToolsLib.Utility.CommonUtils.JsonSerialize(model));
                }
                if (result.Equals("OK"))
                    ServiceProvider.InstrumentService.DeleteById(UtilsHelper.Decrypt2Int(instrumentId));
            }
            catch
            {
                result = "删除失败！";
            }

            return Content(result);
        }

        public string DeleteInstruments(string InstrumentIds)
        {
            string result = "删除失败！";
            try
            {
                string[] Ids = InstrumentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in Ids)
                {
                    ServiceProvider.InstrumentService.DeleteById(Convert.ToInt32(id));
                }
                result = "OK";
            }
            catch
            { }
            return result;
        }
        #endregion

        #region 详细
        /// <summary>
        /// 详细
        /// </summary>
        /// <returns></returns>
        public ActionResult InstrumentDetail(string instrumentId)
        {
            int instrumentIdDecrpt = UtilsHelper.Decrypt2Int(instrumentId);
            //获取仪器照片
            GetInstrumentPic(instrumentIdDecrpt);
            Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(instrumentIdDecrpt);
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            //所属部门
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel org = orgList.SingleOrDefault(o => o.OrgCode == model.BelongDepart);
            ViewBag.BelongDepart = OrgHelper.GetOrgNameTreeByOrgId(org == null ? 0 : org.ParentOrgId, orgList, org == null ? "" : org.OrgName);
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ParamItemModel InstrumentRecordState = instrumentState.itemsList.SingleOrDefault(p => p.ParamItemValue == model.RecordState.ToString());
            ViewBag.InstrumentRecordState = InstrumentRecordState == null ? new ParamItemModel() : InstrumentRecordState;
            //设备类别
            ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new ParamModel();
            ParamItemModel InstrumentType = instrumentType.itemsList.SingleOrDefault(p => p.ParamItemValue == model.InstrumentType.ToString());
            ViewBag.InstrumentType = InstrumentType == null ? new ParamItemModel() : InstrumentType;

            //管理级别
            ParamModel instrumentManageLevel = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.ManageLevel);
            if (null == instrumentManageLevel) instrumentManageLevel = new ParamModel();
            if (model.ManageLevel == null)
                ViewBag.ManageLevel = null;
            else
            {
                ParamItemModel InstrumentManageLeve = instrumentManageLevel.itemsList.SingleOrDefault(p => p.ParamItemValue == model.ManageLevel.ToString());
                ViewBag.ManageLevel = InstrumentManageLeve.ParamItemName == null ? null : InstrumentManageLeve.ParamItemName;
            }
            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            ParamItemModel InstrumentCate = instrumentCate.itemsList.SingleOrDefault(p => p.ParamItemValue == model.InstrumentCate.ToString());
            ViewBag.InstrumentCate = InstrumentCate == null ? new ParamItemModel() : InstrumentCate;
            ParamItemModel subInstrumentCate = instrumentCate.itemsList.SingleOrDefault(p => p.ParamItemValue == model.SubInstrumentCate.ToString());
            if (subInstrumentCate != null && subInstrumentCate.ParentCode == InstrumentCate.ParamItemValue)
            {
                ViewBag.SubInstrumentCate = subInstrumentCate;
                ViewBag.IsShowSubCate = "block";
            }
            else
            {
                ViewBag.SubInstrumentCate = new ParamItemModel();
                ViewBag.IsShowSubCate = "none";
            }
            //校准类别
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ParamItemModel CalibrationType = calibrationType.itemsList.SingleOrDefault(p => p.ParamItemValue == model.CalibrationType.ToString());
            ViewBag.CalibrationType = CalibrationType == null ? new ParamItemModel() : CalibrationType;

            //检定类别
            ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new ParamModel();
            ParamItemModel VerificationType = verificationType.itemsList.SingleOrDefault(p => p.ParamItemValue == model.VerificationType.ToString());
            ViewBag.VerificationType = VerificationType == null ? new ParamItemModel() : VerificationType;
            CraftModel mCraft = ServiceProvider.CraftService.GetById(model.CraftId);
            if (mCraft == null) mCraft = new CraftModel();
            ViewBag.CraftModel = mCraft;

            return View("InstrumentDetail", model);
        }

        #endregion

        #region 期间核查

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentPeriodcheckList(string instrumentId, int dataType)
        {
            ViewBag.InstrumentId = instrumentId;
            ViewBag.DataType = dataType;
            return View();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public string GetInstrumentPeriodcheckList(string instrumentId, int DataType)
        {
            IList<PeriodcheckModel> periodCheckList = ServiceProvider.InstrumentPeriodcheckService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
            Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetById(UtilsHelper.Decrypt2Int(instrumentId));

            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            foreach (var p in periodCheckList)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                if (DataType == 1)
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentPeriodcheckEdit".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnPeriodcheckEdit({0})'>修改</a>&nbsp;&nbsp;", p.PeriodcheckId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DeleteInstrumentPeriodcheck".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnPeriodcheckDelete({0})'>删除</a>&nbsp;&nbsp;", p.PeriodcheckId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentPeriodcheckAttachmentList".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnUploadPeriodcheckAttachment({0})'>上传附件</a>&nbsp;&nbsp;", p.PeriodcheckId);
                    }
                }
                else
                {
                    sbOperate.AppendFormat("<a href='#' onclick='fnPeriodcheckDetail(this)' id='{0}'>详细</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(Convert.ToString(p.PeriodcheckId)));
                }

                ParamModel result = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.PeriodcheckResult);    //设备分类
                ParamItemModel mResult = result.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(p.Result.ToString()));
                sbData.AppendFormat("\"{0}\"", sbOperate);
                sbData.AppendFormat(",\"{0}\"", p.Frequency);
                sbData.AppendFormat(",\"{0:d}\"", p.PlanDate);
                sbData.AppendFormat(",\"{0:d}\"", p.CompleteDate);
                sbData.AppendFormat(",\"{0}\"", p.Leader);
                sbData.AppendFormat(",\"{0}\"", mResult == null ? "" : mResult.ParamItemName);
                sbData.Append("],");
            }
            if (periodCheckList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentPeriodcheckAttachmentList(string PeriodcheckId)
        {
            ViewBag.PeriodcheckId = PeriodcheckId;
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="periodcheckId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentPeriodcheckEdit(int periodcheckId, string instrumentId)
        {
            PeriodcheckModel model = null;
            if (0 == periodcheckId)
            {
                model = new PeriodcheckModel();
                model.InstrumentId = UtilsHelper.Decrypt2Int(instrumentId);
            }
            else
            {
                model = ServiceProvider.InstrumentPeriodcheckService.GetById(periodcheckId);
            }
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel periodcheckResult = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.PeriodcheckResult);
            if (null == periodcheckResult) periodcheckResult = new ParamModel();

            ViewBag.PeriodcheckResultList = new SelectList(periodcheckResult.itemsList, "ParamItemValue", "ParamItemName", model.Result);
            return View(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="collection"></param>
        [HttpPost]
        public ActionResult SaveInstrumentPeriodcheck(PeriodcheckModel model, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                string msg = UtilsHelper.GetValidateErrorMsg(ModelState.Values).ToString();
                return Json(new { Msg = msg }, "text/html", JsonRequestBehavior.AllowGet);
            }
            try
            {
                ServiceProvider.InstrumentPeriodcheckService.Save(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Content(ex.Message);
            }
            return Content("OK");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="collection"></param>
        public ActionResult DeleteInstrumentPeriodcheck(int periodcheckId)
        {
            ServiceProvider.InstrumentPeriodcheckService.DeleteById(periodcheckId);
            ServiceProvider.BusinessAttachmentService.DeleteByKeyIdAndType(periodcheckId, Convert.ToString(Instrument.Common.Constants.AttachmentBusinessType.期间核查.GetHashCode()));
            return Content("OK");
        }

        /// <summary>
        /// 期间核查
        /// </summary>
        /// <param name="collection"></param>
        public ActionResult AllPeriodcheckList()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            ViewBag.PeriodcheckResult = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.PeriodcheckResult).ToString();
            return View();
        }
        /// <summary>
        /// 期间核查数据列表
        /// </summary>
        /// <param name="collection"></param>
        public JsonResult GetAllPeriodcheckListJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"PeriodcheckId,InstrumentId,Frequency,PlanDate,CompleteDate,Leader,Result,Remark";
            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? " 1=1 " : dtm.FieldCondition;

            string instrumentParam = Request["instrumentParam"];
            if (!string.IsNullOrWhiteSpace(instrumentParam))
            {
                instrumentParam = string.Format(" and {0}", instrumentParam);
            }
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", "");
            paging.Where = string.Format("{0} and InstrumentId in ( select InstrumentId from Instrument_BaseInfo where {2}{1})", paging.Where, instrumentParam, sManageCondition);

            IList<Hashtable> periodList = ServiceProvider.InstrumentService.GetAllPeriodcheckListForPaging(paging);
            ////系统参数
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
            foreach (var item in periodList)
            {

                IList<InstrumentCertificationModel> certList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(Convert.ToInt32(item["InstrumentId"]));
                DateTime? maxCheckDate = null;
                if (certList.Count > 0)
                {
                    maxCheckDate = certList.OrderByDescending(c => c.CheckDate).First().CheckDate;
                }

                Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetById(Convert.ToInt32(item["InstrumentId"]));
                dtm.aaData.Add(new List<string>());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' onclick='fnDetail(this)' id='{0}'>详细</a>", UtilsHelper.Encrypt(item["PeriodcheckId"].ToString())));
                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrument.InstrumentCate));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", instrument.BelongDepart));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.InstrumentName));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.Specification));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.SerialNo));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.ManageNo));    //证书编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.StorePalce));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Frequency"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", string.Format("{0:yyyy-MM-dd}", item["PlanDate"])));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", string.Format("{0:yyyy-MM-dd}", item["CompleteDate"])));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Leader"]));
                //结论
                ParamModel result = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.PeriodcheckResult);    //设备分类
                ParamItemModel mResult = result.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(item["Result"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", mResult == null ? "" : mResult.ParamItemName));

                dtm.aaData[dtm.aaData.Count - 1].Add(maxCheckDate == null ? "" : string.Format("{0:yyyy-MM-dd}", maxCheckDate));    //校准日期
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
        /// 详细
        /// </summary>
        /// <param name="periodcheckId"></param>
        /// <returns></returns>
        public ActionResult InstrumentPeriodcheckDetail(string periodcheckId)
        {
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            PeriodcheckModel model = ServiceProvider.InstrumentPeriodcheckService.GetById(UtilsHelper.Decrypt2Int(periodcheckId));
            ParamModel result = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.PeriodcheckResult);    //设备分类
            ParamItemModel mResult = result.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(model.Result.ToString()));

            ViewBag.Result = mResult == null ? "" : mResult.ParamItemName;
            return View(model);
        }

        #region 导出
        /// <summary>
        ///  导出期间核查
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult PeriodcheckEditBySearchCondition()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? " 1=1 " : dtm.FieldCondition;
            string instrumentParam = Request["instrumentParam"];
            if (!string.IsNullOrWhiteSpace(instrumentParam))
            {
                instrumentParam = string.Format(" and {0}", instrumentParam);
            }
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", "");
            where = string.Format("{0} and InstrumentId in ( select InstrumentId from Instrument_BaseInfo where {2}{1})", where, instrumentParam, sManageCondition);
            IList<PeriodcheckModel> periodList = ServiceProvider.InstrumentPeriodcheckService.GetPeriodcheckListByWhere(where);
            if (0 == periodList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            ParamItemModel mInstrumentCate = null;

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();

            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("InstrumentCate", typeof(string));    //设备分类
            dtData.Columns.Add("BelongDepart", typeof(string));    //所属部门
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("Specification", typeof(string));    //型号
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
            dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
            dtData.Columns.Add("StorePalce", typeof(string));    //存放地址
            dtData.Columns.Add("Frequency", typeof(string));    //频次
            dtData.Columns.Add("PlanDate", typeof(string));    //计划日期
            dtData.Columns.Add("CompleteDate", typeof(string));    //完成日期
            dtData.Columns.Add("Leader", typeof(string));    //负责人
            dtData.Columns.Add("Result", typeof(string));    //结果
            dtData.Columns.Add("LastCheckDate", typeof(string));    //最新校准日期
            foreach (var item in periodList)
            {
                IList<InstrumentCertificationModel> certList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(Convert.ToInt32(item.InstrumentId));
                DateTime? maxCheckDate = null;
                if (certList.Count > 0)
                {
                    maxCheckDate = certList.OrderByDescending(c => c.CheckDate).First().CheckDate;
                }
                Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetById(item.InstrumentId);

                DataRow drData = dtData.NewRow();
                //分类
                mInstrumentCate = InstrumentCate.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrument.InstrumentCate));
                drData["InstrumentCate"] = (mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == instrument.BelongDepart);
                drData["BelongDepart"] = belongDeptModel == null ? "" : OrgHelper.GetOrgNameTreeByOrgId(belongDeptModel.ParentOrgId, orgList, belongDeptModel.OrgName);
                drData["InstrumentName"] = instrument.InstrumentName;    //仪器名称
                drData["Specification"] = instrument.Specification;
                drData["SerialNo"] = instrument.SerialNo;
                drData["ManageNo"] = instrument.ManageNo;
                drData["StorePalce"] = instrument.StorePalce;
                drData["Frequency"] = item.Frequency;
                drData["PlanDate"] = string.Format("{0:yyyy-MM-dd}", item.PlanDate);
                drData["CompleteDate"] = string.Format("{0:yyyy-MM-dd}", item.CompleteDate);
                drData["Leader"] = item.Leader;
                //结论
                ParamModel resultModel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.PeriodcheckResult);    //设备分类
                ParamItemModel mResult = resultModel.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(item.Result.ToString()));
                drData["Result"] = mResult == null ? "" : mResult.ParamItemName;

                drData["LastCheckDate"] = string.Format("{0:yyyy-MM-dd}", maxCheckDate);
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
               "设备分类","所属部门","仪器名称", "仪器型号", "出厂编号", "管理编号" , "存放地址", "频次", "计划日期", "完成日期", "负责人", "结果", "最新校准日期"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "期间核查", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}期间核查{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        /// <summary>
        ///  导出内部核查
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult InnerCheckEditBySearchCondition()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            string where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? " 1=1 " : dtm.FieldCondition;
            string instrumentParam = Request["instrumentParam"];
            if (!string.IsNullOrWhiteSpace(instrumentParam))
            {
                instrumentParam = string.Format(" and {0}", instrumentParam);
            }
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", "");
            where = string.Format("{0} and InstrumentId in ( select InstrumentId from Instrument_BaseInfo where {2}{1})", where, instrumentParam, sManageCondition);
            IList<InnerCheckModel> periodList = ServiceProvider.InstrumentInnerCheckService.GetInnerCheckListByWhere(where);
            if (0 == periodList.Count)
            {
                Response.Write("没有要导出的内容。");
                Response.End();
                return Content("");
            }
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);    //设备分类
            ParamItemModel mInstrumentCate = null;

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel belongDeptModel = new OrgModel();

            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("InstrumentCate", typeof(string));    //设备分类
            dtData.Columns.Add("BelongDepart", typeof(string));    //所属部门
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名称
            dtData.Columns.Add("Specification", typeof(string));    //型号
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号
            dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
            dtData.Columns.Add("CertificateNo", typeof(string));    //证书编号
            dtData.Columns.Add("StorePalce", typeof(string));    //存放地址
            dtData.Columns.Add("CheckDate", typeof(string));    //核查日期
            dtData.Columns.Add("PeriodDate", typeof(string));    //有效日期
            dtData.Columns.Add("Leader", typeof(string));    //负责人
            dtData.Columns.Add("CreateDate", typeof(string));    //创建日期
            dtData.Columns.Add("Creator", typeof(string));    //创建人
            dtData.Columns.Add("Result", typeof(string));    //结论

            foreach (var item in periodList)
            {
                IList<InstrumentCertificationModel> certList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(Convert.ToInt32(item.InstrumentId));
                DateTime? maxCheckDate = null;
                if (certList.Count > 0)
                {
                    maxCheckDate = certList.OrderByDescending(c => c.CheckDate).First().CheckDate;
                }
                Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetById(item.InstrumentId);

                DataRow drData = dtData.NewRow();
                //分类
                mInstrumentCate = InstrumentCate.itemsList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrument.InstrumentCate));
                drData["InstrumentCate"] = (mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == instrument.BelongDepart);
                drData["BelongDepart"] = belongDeptModel == null ? "" : OrgHelper.GetOrgNameTreeByOrgId(belongDeptModel.ParentOrgId, orgList, belongDeptModel.OrgName);
                drData["InstrumentName"] = instrument.InstrumentName;    //仪器名称
                drData["Specification"] = instrument.Specification;
                drData["SerialNo"] = instrument.SerialNo;
                drData["ManageNo"] = instrument.ManageNo;
                drData["CertificateNo"] = instrument.CertificateNo;
                drData["StorePalce"] = instrument.StorePalce;
                drData["CheckDate"] = string.Format("{0:yyyy-MM-dd}", item.CheckDate);
                drData["PeriodDate"] = string.Format("{0:yyyy-MM-dd}", item.PeriodDate);
                drData["Leader"] = item.Leader;
                drData["CreateDate"] = string.Format("{0:yyyy-MM-dd}", item.CreateDate);
                drData["Creator"] = item.Creator;
                //结论
                ParamModel resultModel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InnerCheckResult);    //设备分类
                ParamItemModel mResult = resultModel.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(item.Result.ToString()));
                drData["Result"] = mResult == null ? "" : mResult.ParamItemName;
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
               "设备分类","所属部门","仪器名称", "仪器型号", "出厂编号", "管理编号","证书编号" , "存放地址", "核查日期", "有效日期", "负责人", "创建日期", "创建人", "结论"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "期间核查", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}内部核查{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
            return Content("OK");
        }

        #endregion

        #endregion

        #region === 预警提醒 ===
        public ActionResult OverTimeAndWarnList()
        {
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel warnday = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.WarnDay);
            if (null == warnday) warnday = new ParamModel();
            IList<ParamItemModel> warnDayList = new List<ParamItemModel>();
            warnDayList = warnDayList.Union(warnday.itemsList).ToList();
            ViewBag.WarnDayList = new SelectList(warnDayList, "ParamItemValue", "ParamItemName", "28");
            return View();
        }
        /// <summary>
        /// 超期提醒列表
        /// </summary>
        /// <returns></returns>
        public string GetOverTimeInstrumentList()
        {
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.Append("1=1");
            if (!LoginHelper.LoginUserAuthorize.ContainsKey("Instrument-CheckAll".ToLower()))
            {
                sqlWhere.AppendFormat(" and {0}", Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart"));
            }
            sqlWhere.AppendFormat(" and '{0:yyyy-MM-dd}'>DueEndDate and ManageLevel !='C' and RecordState={1}", DateTime.Now, UtilConstants.InstrumentState.过期禁用.GetHashCode());

            IList<Instrument.Common.Models.InstrumentModel> overTimeList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(sqlWhere.ToString());
            //当前用户下已加入清单但未送检的仪器
            IList<InstrumentWaitSendModel> preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            ///设备状态
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            IList<ParamItemModel> RecordStateparamItemList = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState).itemsList;
            ParamItemModel mParamItem = new ParamItemModel();
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"data\":[");
            InstrumentWaitSendModel instrumentWaitSendModel = null;
            bool isHas = false;
            foreach (Instrument.Common.Models.InstrumentModel item in overTimeList)
            {
                instrumentWaitSendModel = preSendList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                if (instrumentWaitSendModel != null) continue;
                isHas = true;
                sb.AppendFormat("[\"<input type='checkbox' name='OverChk' value='{0}' />\"", UtilsHelper.Encrypt(item.InstrumentId.ToString()));
                sb.AppendFormat(",\"<a href='/Instrument/InstrumentDetail?instrumentId={0}' target='_blank'>详细</a>\"", UtilsHelper.Encrypt(item.InstrumentId.ToString()));
                mParamItem = RecordStateparamItemList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item.RecordState));
                sb.AppendFormat(",\"{0}\"", mParamItem == null ? "" : mParamItem.ParamItemName);
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.InstrumentName));
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.CertificateNo));
                sb.AppendFormat(",\"{0:d}\"", item.DueEndDate);
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.Specification));
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.ManageNo));
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.SerialNo));
                sb.Append("],");
            }
            sb.Append("]}");
            if (isHas)
                sb.Remove(sb.Length - 3, 1);
            return sb.ToString();
        }

        /// <summary>
        /// 预警提醒列表
        /// </summary>
        /// <returns></returns>
        public string GetWarnInstrumentList(int day)
        {
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.Append("1=1");
            if (!LoginHelper.LoginUserAuthorize.ContainsKey("Instrument-CheckAll".ToLower()))
            {
                sqlWhere.AppendFormat(" and {0}", Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart"));
            }

            if (day > 0)
            {
                sqlWhere.AppendFormat(" and DueEndDate<='{0:yyyy-MM-dd}' and DueEndDate>='{1:yyyy-MM-dd}' ", DateTime.Now.AddDays(day), DateTime.Now);
            }
            sqlWhere.Append(" and ManageLevel !='C' ");
            IList<Instrument.Common.Models.InstrumentModel> warnList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(sqlWhere.ToString());
            //当前用户下已加入清单但未送检的仪器
            IList<InstrumentWaitSendModel> preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"data\":[");
            DateTime dueEndDate;
            int warnDay = 0;
            InstrumentWaitSendModel instrumentWaitSendModel = null;
            bool isHas = false;
            foreach (Instrument.Common.Models.InstrumentModel item in warnList)
            {
                instrumentWaitSendModel = preSendList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                if (instrumentWaitSendModel != null) continue;

                if (!string.IsNullOrWhiteSpace(string.Format("{0}", item.DueEndDate)))
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item.DueEndDate));
                else
                    dueEndDate = DateTime.MinValue;
                warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                warnDay = warnDay < 0 ? 0 : warnDay;

                isHas = true;
                sb.AppendFormat("[\"<input type='checkbox' name='WarnChk' value='{0}' />\"", UtilsHelper.Encrypt(item.InstrumentId.ToString()));
                sb.AppendFormat(",\"<a href='/Instrument/InstrumentDetail?instrumentId={0}' target='_blank'>详细</a>\"", UtilsHelper.Encrypt(item.InstrumentId.ToString()));
                sb.AppendFormat(",\"{0}\"", warnDay);
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.InstrumentName));
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.CertificateNo));
                sb.AppendFormat(",\"{0:d}\"", item.DueEndDate);
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.Specification));
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.ManageNo));
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(item.SerialNo));
                sb.Append("],");
            }
            sb.Append("]}");
            if (isHas)
                sb.Remove(sb.Length - 3, 1);
            return sb.ToString();
        }
        /// <summary>
        /// 预警导出
        /// </summary>
        /// <returns></returns>
        public void ExportOverTimeAndWarnList(int state, int day)
        {
            ServiceProvider.InstrumentService.ExportOverTimeAndWarnList(state, day);
        }
        #endregion

        #region === 内部核查 ===
        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentInnerCheckList(string instrumentId, int dataType)
        {
            ViewBag.InstrumentId = instrumentId;
            ViewBag.DataType = dataType;
            return View();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public string GetInstrumentInnerCheckList(string instrumentId, int dataType)
        {
            IList<InnerCheckModel> innerCheckList = ServiceProvider.InstrumentInnerCheckService.GetByInstrumentId(UtilsHelper.Decrypt2Int(instrumentId));
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();
            foreach (var p in innerCheckList)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                if (dataType == 1)
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentInnerCheckEdit".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnInnerEdit({0})'>修改</a>&nbsp;&nbsp;", p.InnerCheckId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/DeleteInstrumentInnerCheck".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnInnerDelete({0})'>删除</a>&nbsp;&nbsp;", p.InnerCheckId);
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentInnerCheckAttachmentList".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnUploadInnerCheckAttachment({0})'>上传附件</a>&nbsp;&nbsp;", p.InnerCheckId);
                    }
                }
                else
                {
                    sbOperate.AppendFormat("<a href='#' onclick='fnInnerDetail(this)'id='{0}'>详细</a>", UtilsHelper.Encrypt(p.InnerCheckId.ToString()));
                }
                sbData.AppendFormat("\"{0}\"", sbOperate);
                sbData.AppendFormat(",\"{0}\"", null == p.CheckDate ? string.Empty : p.CheckDate.Value.ToString("yyyy-MM-dd"));//检查日期
                sbData.AppendFormat(",\"{0}\"", p.PeriodDate == null ? string.Empty : p.PeriodDate.Value.ToString("yyyy-MM-dd"));//有效日期
                //结论
                ParamModel result = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InnerCheckResult);    //设备分类
                ParamItemModel mResult = result.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(p.Result.ToString()));//结论
                sbData.AppendFormat(",\"{0}\"", string.Format("{0}", mResult == null ? "" : mResult.ParamItemName));
                sbData.AppendFormat(",\"{0}\"", p.Leader);//负责人
                sbData.AppendFormat(",\"{0}\"", p.CreateDate == null ? string.Empty : p.CreateDate.Value.ToString("yyyy-MM-dd"));//创建日期
                sbData.AppendFormat(",\"{0}\"", p.Creator);//创建人
                sbData.AppendFormat(",\"{0}\"", p.Remark);//备注
                sbData.Append("],");
            }
            if (innerCheckList.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="innerCheck"></param>
        /// <returns></returns>
        public ActionResult InstrumentInnerCheckAttachmentList(string innerCheck)
        {
            ViewBag.InnerCheck = innerCheck;
            return View();
        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="innerCheckId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentInnerCheckEdit(int innerCheckId, string instrumentId)
        {
            InnerCheckModel model = null;
            if (0 == innerCheckId)
            {
                model = new InnerCheckModel();
                model.InstrumentId = UtilsHelper.Decrypt2Int(instrumentId);
            }
            else
            {
                model = ServiceProvider.InstrumentInnerCheckService.GetById(innerCheckId);
            }
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel periodcheckResult = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InnerCheckResult);
            ViewBag.InnerCheckResultList = new SelectList(periodcheckResult.itemsList, "ParamItemValue", "ParamItemName", model.Result);
            return View(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="collection"></param>
        public ActionResult DeleteInstrumentInnerCheck(int innerCheckId)
        {
            ServiceProvider.InstrumentInnerCheckService.Delect(innerCheckId);
            ServiceProvider.BusinessAttachmentService.DeleteByKeyIdAndType(innerCheckId, Convert.ToString(Instrument.Common.Constants.AttachmentBusinessType.内部核查.GetHashCode()));
            return Content("OK");
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="collection"></param>
        [HttpPost]
        public ActionResult SaveInstrumentInnerCheck(InnerCheckModel model, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                string msg = UtilsHelper.GetValidateErrorMsg(ModelState.Values).ToString();
                return Json(new { Msg = msg }, "text/html", JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (model.InnerCheckId == 0)
                {
                    model.CreateDate = DateTime.Now;
                    model.Creator = LoginHelper.LoginUser.UserName;
                }
                ServiceProvider.InstrumentInnerCheckService.Save(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Content(ex.Message);
            }
            return Content("OK");
        }

        /// <summary>
        /// 详细
        /// </summary>
        /// <param name="periodcheckId"></param>
        /// <returns></returns>
        public ActionResult InstrumentInnerCheckDetail(string innerCheckId)
        {
            InnerCheckModel model = ServiceProvider.InstrumentInnerCheckService.GetById(UtilsHelper.Decrypt2Int(innerCheckId));
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel result = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InnerCheckResult);    //设备分类
            ParamItemModel mResult = result.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(model.Result.ToString()));
            ViewBag.Result = mResult == null ? "" : mResult.ParamItemName;
            return View(model);
        }

        /// <summary>
        /// 内部核查
        /// </summary>
        /// <param name="collection"></param>
        public ActionResult AllInnerCheckList()
        {
            ViewBag.JsonInstrumentCate = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue2(Instrument.Common.Constants.SysParamType.InstrumentCate).ToString();
            ViewBag.InnerCheckResult = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Instrument.Common.Constants.SysParamType.InnerCheckResult).ToString();
            return View();
        }
        /// <summary>
        /// 内部核查数据列表
        /// </summary>
        /// <param name="collection"></param>
        public JsonResult GetAllInnerCheckListJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"InnerCheckId,InstrumentId,CheckDate,PeriodDate,Remark,Creator,CreateDate,Leader,Result";
            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? " 1=1 " : dtm.FieldCondition;

            string instrumentParam = Request["instrumentParam"];
            if (!string.IsNullOrWhiteSpace(instrumentParam))
            {
                instrumentParam = string.Format(" and {0}", instrumentParam);
            }
            string sManageCondition = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition("InstrumentForm= " + (int)Constants.InstrumentForm.仪器, "Instrument-CheckAll", "");
            paging.Where = string.Format("{0} and InstrumentId in ( select InstrumentId from Instrument_BaseInfo where {2}{1})", paging.Where, instrumentParam, sManageCondition);

            IList<Hashtable> periodList = ServiceProvider.InstrumentService.GetAllInnerCheckListForPaging(paging);
            ////系统参数
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
            foreach (var item in periodList)
            {

                IList<InstrumentCertificationModel> certList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(Convert.ToInt32(item["InstrumentId"]));
                DateTime? maxCheckDate = null;
                if (certList.Count > 0)
                {
                    maxCheckDate = certList.OrderByDescending(c => c.CheckDate).First().CheckDate;
                }

                Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetById(Convert.ToInt32(item["InstrumentId"]));
                dtm.aaData.Add(new List<string>());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' onclick='fnDetail(this)' id='{0}'>详细</a>", UtilsHelper.Encrypt(item["InnerCheckId"].ToString())));
                mInstrumentCate = paramItemList.SingleOrDefault(t => t.ParamItemValue == Convert.ToString(instrument.InstrumentCate));
                dtm.aaData[dtm.aaData.Count - 1].Add(mInstrumentCate == null ? "" : mInstrumentCate.ParamItemName);//分类
                belongDeptModel = orgList.SingleOrDefault(o => o.OrgCode == string.Format("{0}", instrument.BelongDepart));
                dtm.aaData[dtm.aaData.Count - 1].Add(belongDeptModel == null ? "" : belongDeptModel.OrgName);//所属部门

                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.InstrumentName));    //仪器名称
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.Specification));    //型号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.SerialNo));    //出厂编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.ManageNo));    //管理编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.CertificateNo));    //证书编号
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", instrument == null ? "" : instrument.StorePalce));//存放地点
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", string.Format("{0:yyyy-MM-dd}", item["CheckDate"])));//核查日期
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", string.Format("{0:yyyy-MM-dd}", item["PeriodDate"])));//有效日期
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Leader"]));//负责人
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", string.Format("{0:yyyy-MM-dd}", item["CreateDate"])));//创建日期
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Creator"]));//创建人
                //结论
                ParamModel result = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InnerCheckResult);    //设备分类
                ParamItemModel mResult = result.itemsList.SingleOrDefault(Q => Q.ParamItemValue.Equals(item["Result"].ToString()));//结论
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", mResult == null ? "" : mResult.ParamItemName));
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

        #region === 配件设备 ===
        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult AccessoriesInstrument(string instrumentId, int dataType)
        {
            ViewBag.InstrumentId = instrumentId;
            ViewBag.DataType = dataType;
            return View();
        }
        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public string GetAccessoriesInstrumentList(string parentID, int dataType)
        {
            IList<InstrumentModel> instrumentModel = ServiceProvider.InstrumentService.GetInstrumentByParentId(UtilsHelper.Decrypt2Int(parentID));
            StringBuilder sbData = new StringBuilder();
            sbData.Append("{\"data\":[");
            StringBuilder sbOperate = new StringBuilder();
            foreach (var item in instrumentModel)
            {
                sbData.Append("[");
                sbOperate.Clear();  //操作
                if (dataType == 1)
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/InstrumentAccessoriesInstrumentEdit".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnAccessoriseEdit(this)'id='{0}'>修改</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(item.InstrumentId.ToString()));
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Instrument/Delete".ToLower()))
                    {
                        sbOperate.AppendFormat("<a href='#' onclick='fnAccessoriseDelete(this)'id='{0}'name='{1}'>删除</a>&nbsp;&nbsp;", UtilsHelper.Encrypt(item.InstrumentId.ToString()), item.InstrumentName);
                    }
                }
                else
                {
                    sbOperate.AppendFormat("<a href='#' onclick='fnAccessoriesInstrumentDetail(this)'id='{0}'>详细</a>", UtilsHelper.Encrypt(item.InstrumentId.ToString()));
                }
                sbData.AppendFormat("\"{0}\"", sbOperate);
                sbData.AppendFormat(",\"{0}\"", item.InstrumentName);//配件名字
                sbData.AppendFormat(",\"{0}\"", item.ManageNo);//管理编号
                sbData.AppendFormat(",\"{0}\"", item.Specification);//型号规格
                sbData.AppendFormat(",\"{0}\"", item.SerialNo);//出厂编号
                sbData.AppendFormat(",\"{0}\"", item.Manufacturer);//生产厂家
                sbData.AppendFormat(",\"{0}\"", item.LeaderName);//保管人
                sbData.AppendFormat(",\"{0}\"", item.Remark);//备注
                sbData.Append("],");
            }
            if (instrumentModel.Count > 0)
            {
                sbData.Remove(sbData.Length - 1, 1);
            }
            sbData.Append("]}");
            return sbData.ToString();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult AddAccessoriesInstrument(string instrumentId)
        {
            Instrument.Common.Models.InstrumentModel model = new Instrument.Common.Models.InstrumentModel();
            model.BelongDepart = "";
            model.ParentID = UtilsHelper.Decrypt2Int(instrumentId);

            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //所属部门...........生成一个下拉框树所需的数据源
            ViewBag.BelongDepartList = Global.Business.ServiceProvider.OrgService.GetAll();
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ViewBag.InstrumentRecordStateList = new SelectList(instrumentState.itemsList, "ParamItemValue", "ParamItemName");
            //器具种类(设备类别)
            ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new ParamModel();
            ViewBag.InstrumentTypeList = new SelectList(instrumentType.itemsList, "ParamItemValue", "ParamItemName");

            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            //加载一级分类
            IList<ParamItemModel> cateList = instrumentCate.itemsList.Where(c => c.ParentCode == "0").ToList();
            ViewBag.InstrumentCateList = new SelectList(cateList, "ParamItemValue", "ParamItemName");
            //二级分类
            ViewBag.SubInstrumentCateList = new SelectList(instrumentCate.itemsList.Where(c => Convert.ToInt32(c.ParentCode) > 0), "ParamItemValue", "ParamItemName");

            //资产属性
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ViewBag.CalibrationTypeList = new SelectList(calibrationType.itemsList, "ParamItemValue", "ParamItemName");

            //管理级别
            ParamModel manageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);
            if (null == manageLevel) manageLevel = new ParamModel();
            ViewBag.ManageLevelList = new SelectList(manageLevel.itemsList, "ParamItemValue", "ParamItemName");

            //检定类别
            ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new ParamModel();
            ViewBag.VerificationTypeList = new SelectList(verificationType.itemsList, "ParamItemValue", "ParamItemName");

            //配件类别
            ParamModel combinedType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AccessoriesType);
            if (null == combinedType) combinedType = new ParamModel();
            ViewBag.CombinedTypeList = new SelectList(combinedType.itemsList, "ParamItemValue", "ParamItemName");

            ParamModel instrumentCertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState);
            if (null == instrumentCertificationState) instrumentCertificationState = new ParamModel();

            ViewBag.RecordStateList = new SelectList(instrumentCertificationState.itemsList, "ParamItemValue", "ParamItemName", -99);

            return View(model);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult InstrumentAccessoriesInstrumentEdit(string instrumentId)
        {
            int instrumentIdDecrpt = UtilsHelper.Decrypt2Int(instrumentId);
            //获取仪器照片
            GetInstrumentPic(instrumentIdDecrpt);
            Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(instrumentIdDecrpt);
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            //所属部门...........生成一个下拉框树所需的数据源
            ViewBag.BelongDepartList = Global.Business.ServiceProvider.OrgService.GetAll();
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ViewBag.InstrumentRecordStateList = new SelectList(instrumentState.itemsList, "ParamItemValue", "ParamItemName");
            //器具种类(设备类别)
            ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new ParamModel();
            ViewBag.InstrumentTypeList = new SelectList(instrumentType.itemsList, "ParamItemValue", "ParamItemName");

            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            //加载一级分类
            IList<ParamItemModel> cateList = instrumentCate.itemsList.Where(c => c.ParentCode == "0").ToList();
            ViewBag.InstrumentCateList = new SelectList(cateList, "ParamItemValue", "ParamItemName", model.InstrumentCate);
            //二级分类
            IEnumerable<ParamItemModel> subList = instrumentCate.itemsList.Where(c => c.ParentCode == model.InstrumentCate.ToString());
            if (subList.Count() > 0) ViewBag.IsShowSubCate = true;
            else ViewBag.IsShowSubCate = false;

            ViewBag.SubInstrumentCateList = new SelectList(instrumentCate.itemsList.Where(c => c.ParentCode == model.InstrumentCate.ToString()), "ParamItemValue", "ParamItemName", model.SubInstrumentCate);

            //资产属性
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ViewBag.CalibrationTypeList = new SelectList(calibrationType.itemsList, "ParamItemValue", "ParamItemName");

            //管理级别
            ParamModel manageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);
            if (null == manageLevel) manageLevel = new ParamModel();
            ViewBag.ManageLevelList = new SelectList(manageLevel.itemsList, "ParamItemValue", "ParamItemName");

            //配件类型
            ParamModel combinedType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AccessoriesType);
            if (null == combinedType) combinedType = new ParamModel();
            ViewBag.CombinedTypeList = new SelectList(combinedType.itemsList, "ParamItemValue", "ParamItemName");

            //检定类别
            ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new ParamModel();
            ViewBag.VerificationTypeList = new SelectList(verificationType.itemsList, "ParamItemValue", "ParamItemName");
            return View("AddAccessoriesInstrument", model);
        }
        /// <summary>
        /// 详细
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public ActionResult AccessoriesInstrumentDetail(string instrumentId)
        {
            int instrumentIdDecrpt = UtilsHelper.Decrypt2Int(instrumentId);
            //获取仪器照片
            GetInstrumentPic(instrumentIdDecrpt);
            Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(instrumentIdDecrpt);
            //系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();

            //所属部门
            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            OrgModel org = orgList.SingleOrDefault(o => o.OrgCode == model.BelongDepart);
            ViewBag.BelongDepart = OrgHelper.GetOrgNameTreeByOrgId(org == null ? 0 : org.ParentOrgId, orgList, org == null ? "" : org.OrgName);
            //器具状态
            ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new ParamModel();
            ParamItemModel InstrumentRecordState = instrumentState.itemsList.SingleOrDefault(p => p.ParamItemValue == model.RecordState.ToString());
            ViewBag.InstrumentRecordState = InstrumentRecordState == null ? new ParamItemModel() : InstrumentRecordState;
            //设备类别
            ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new ParamModel();
            ParamItemModel InstrumentType = instrumentType.itemsList.SingleOrDefault(p => p.ParamItemValue == model.InstrumentType.ToString());
            ViewBag.InstrumentType = InstrumentType == null ? new ParamItemModel() : InstrumentType;
            //设备分类
            ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new ParamModel();
            ParamItemModel InstrumentCate = instrumentCate.itemsList.SingleOrDefault(p => p.ParamItemValue == model.InstrumentCate.ToString());
            ViewBag.InstrumentCate = InstrumentCate == null ? new ParamItemModel() : InstrumentCate;
            ParamItemModel subInstrumentCate = instrumentCate.itemsList.SingleOrDefault(p => p.ParamItemValue == model.SubInstrumentCate.ToString());
            if (subInstrumentCate != null && subInstrumentCate.ParentCode == InstrumentCate.ParamItemValue)
            {
                ViewBag.SubInstrumentCate = subInstrumentCate;
                ViewBag.IsShowSubCate = "block";
            }
            else
            {
                ViewBag.SubInstrumentCate = new ParamItemModel();
                ViewBag.IsShowSubCate = "none";
            }
            //校准类别
            ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new ParamModel();
            ParamItemModel CalibrationType = calibrationType.itemsList.SingleOrDefault(p => p.ParamItemValue == model.CalibrationType.ToString());
            ViewBag.CalibrationType = CalibrationType == null ? new ParamItemModel() : CalibrationType;
            //管理级别
            ParamModel instrumentManageLevel = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.ManageLevel);
            if (null == instrumentManageLevel) instrumentManageLevel = new ParamModel();
            if (model.ManageLevel == null)
                ViewBag.ManageLevel = null;
            else
            {
                ParamItemModel InstrumentManageLeve = instrumentManageLevel.itemsList.SingleOrDefault(p => p.ParamItemValue == model.ManageLevel.ToString());
                ViewBag.ManageLevel = InstrumentManageLeve.ParamItemName == null ? null : InstrumentManageLeve.ParamItemName;
            }

            //检定类别
            ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new ParamModel();
            ParamItemModel VerificationType = verificationType.itemsList.SingleOrDefault(p => p.ParamItemValue == model.VerificationType.ToString());
            ViewBag.VerificationType = VerificationType == null ? new ParamItemModel() : VerificationType;
            CraftModel mCraft = ServiceProvider.CraftService.GetById(model.CraftId);
            if (mCraft == null) mCraft = new CraftModel();
            ViewBag.CraftModel = mCraft;
            return View(model);
        }
        #endregion
        #region === 公用方法 ===
        private void GetInstrumentPic(int instrumentId)
        {
            BusinessAttachmentModel businessAttachmentModel = ServiceProvider.BusinessAttachmentService.GetByBusinesId(instrumentId, Convert.ToString(Instrument.Common.Constants.AttachmentBusinessType.仪器照片.GetHashCode()));
            ViewBag.PicPath = "/Content/themes/webcss/img/notPic.jpg";
            if (businessAttachmentModel != null)
            {
                Global.Common.Models.AttachmentModel attachModel = Global.Business.ServiceProvider.AttachmentService.GetById(businessAttachmentModel.FileId);
                string userId = WebUtils.GetSettingsValue("WebFileServerUser");
                string pwd = WebUtils.GetSettingsValue("WebFileServerPwd");
                string tempPath = "/tempFile";

                try
                {
                    Stream picStream = UtilsHelper.FileDownload(attachModel.FileAccessPrefix, attachModel.FileVirtualPath, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
                    attachModel.FileVirtualPath = string.Format("{0}/{1}{2}", tempPath, Guid.NewGuid().ToString(), Path.GetExtension(attachModel.FileVirtualPath));
                    ToolsLib.FileService.NormalFile.SaveInfoToFile(picStream, attachModel.FileVirtualPath);
                    ViewBag.PicPath = attachModel.FileVirtualPath;
                }
                catch
                {

                }
            }
        }
        #endregion
    }
}
