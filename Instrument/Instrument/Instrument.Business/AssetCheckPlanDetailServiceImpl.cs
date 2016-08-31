using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common;
using Instrument.Common.Models;
using GRGTCommonUtils;
using System.Data;
using Global.Common.Models;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;

namespace Instrument.Business
{
    /// <summary>
    /// 实验室标准器具
    /// </summary>
    public class AssetCheckPlanDetailServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int PlanDetailId)
        {
            DBProvider.AssetCheckPlanDetailDAO.DeleteById(PlanDetailId);
        }

        public void DeleteByIdList(IList<int> detailIdList)
        {
            DBProvider.AssetCheckPlanDetailDAO.DeleteByIdList(detailIdList);
        }
        

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(AssetCheckPlanDetailModel model)
        {
            if (model.PlanDetailId == 0) DBProvider.AssetCheckPlanDetailDAO.Add(model);
            else DBProvider.AssetCheckPlanDetailDAO.Update(model);
        }

        public void Add(AssetCheckPlanDetailModel model)
        {
            DBProvider.AssetCheckPlanDetailDAO.Add(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public AssetCheckPlanDetailModel GetById(int PlanDetailId)
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetById(PlanDetailId);
        }

        /// <summary>
        /// 批量导入资产
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sucessMsg"></param>
        /// <returns></returns>
        public string BatchImportAssetsForCheckPlan(int planId, DataTable dt, ref string sucessMsg)
        {
            StringBuilder result = new StringBuilder();
            int sucessCount = 0;
            foreach (DataRow dr in dt.Rows)
            {

                try
                {
                    if (string.IsNullOrWhiteSpace(dr["条码"].ToString()))
                    {
                        result.AppendLine("条码格式不正确：【" + dr["条码"] + "】");
                        continue;
                    }

                    Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetByBarCode(dr["条码"].ToString().Trim());
                    if (model == null)
                    {
                        result.AppendLine("系统找不到该条码：【" + dr["条码"] + "】");
                        continue;
                    }
                    bool isExist = ServiceProvider.AssetCheckPlanDetailService.IsExist(model.InstrumentId, planId);
                    if (isExist)
                    {
                        //result.AppendLine("系统已存在该条码：【" + dr["条码"] + "】");
                        continue;
                    }

                    AssetCheckPlanDetailModel detailModel = new AssetCheckPlanDetailModel();
                    detailModel.PlanId = planId;
                    detailModel.Statuse = Common.Constants.AssetsCheckStatus.盘亏.GetHashCode();
                    detailModel.InstrumentId = model.InstrumentId;
                    detailModel.InstrumentName = model.InstrumentName;
                    detailModel.BelongDepart = model.BelongDepart;
                    detailModel.Specification = model.Specification;
                    detailModel.Manufacturer = model.Manufacturer;
                    detailModel.SerialNo = model.SerialNo;
                    detailModel.ManageNo = model.ManageNo;
                    detailModel.AssetsNo = model.AssetsNo;
                    detailModel.CreateUser = LoginHelper.LoginUser.UserName;
                    detailModel.BarCode = model.BarCode;
                    ServiceProvider.AssetCheckPlanDetailService.Save(detailModel);
                    sucessCount++;
                }
                catch (Exception ex)
                {
                }
            }
            sucessMsg = "共导入" + sucessCount + "条记录";
            return result.ToString();
        }

        public void BatchImportFromInstrument(int instrumentForm, int planId)
        {

            DBProvider.AssetCheckPlanDetailDAO.BatchImportFromInstrument(instrumentForm, planId,LoginHelper.LoginUser.UserName);
            //foreach (Instrument.Common.Models.InstrumentModel model in list)
            //{
            //    bool isExist = ServiceProvider.AssetCheckPlanDetailService.IsExist(model.InstrumentId, planId);
            //    if (isExist)
            //    {
            //        continue;
            //    }
            //    AssetCheckPlanDetailModel detailModel = new AssetCheckPlanDetailModel();
            //    detailModel.PlanId = planId;
            //    detailModel.Statuse = Common.Constants.AssetsCheckStatus.盘亏.GetHashCode();
            //    detailModel.InstrumentId = model.InstrumentId;
            //    detailModel.InstrumentName = model.InstrumentName;
            //    detailModel.BelongDepart = model.BelongDepart;
            //    detailModel.Specification = model.Specification;
            //    detailModel.Manufacturer = model.Manufacturer;
            //    detailModel.SerialNo = model.SerialNo;
            //    detailModel.ManageNo = model.ManageNo;
            //    detailModel.AssetsNo = model.AssetsNo;
            //    detailModel.CreateUser = LoginHelper.LoginUser.UserName;
            //    ServiceProvider.AssetCheckPlanDetailService.Save(detailModel);
            //}
        }
        

        /// <summary>
        /// 根据仪器标识和计划标识判断是否已经在盘点计划内
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public bool IsExist(int instrumentId, int planId)
        {
            bool result = false;
            AssetCheckPlanDetailModel model = DBProvider.AssetCheckPlanDetailDAO.GetByInstrumentIdAndPlanId(instrumentId, planId);
            if (model != null) result = true;
            return result;
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<AssetCheckPlanDetailModel> GetAll()
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetAll();
        }
        public IList<AssetCheckPlanDetailModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetByInstrumentId(instrumentId);
        }

        public bool IsExistPlanDetail(string barCode, int userId)
        {
            return DBProvider.AssetCheckPlanDetailDAO.IsExistPlanDetail(barCode, userId);
        }

        /// <summary>
        /// 盘点
        /// 只更新当前用户有权限的计划下所有资产
        /// 计划未开始或者已经结束的  不更新
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="userId"></param>
        public string UpdateState(string barCode, int userId)
        {
            //Common.Models.InstrumentModel instrument = ServiceProvider.InstrumentService.GetByBarCode(barCode);
            //if (instrument == null) result="Unfind";
           bool result = ServiceProvider.AssetCheckPlanDetailService.IsExistPlanDetail(barCode,userId);
           if (!result)
                return  "该资产尚未制定盘点计划";
            Hashtable ht = new Hashtable();
            ht["BarCode"] = barCode;
            ht["Statuse"] = Constants.AssetsCheckStatus.已盘点.GetHashCode();
            ht["UserId"] = userId;
            ht["StartDate"] = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            DBProvider.AssetCheckPlanDetailDAO.UpdateState(ht);
            return "OK";
        }

        public IList<AssetCheckPlanDetailModel> GetByPlanIdAndStatus(int planId, int state)
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetByPlanIdAndStatus(planId, state);
        }

        public IList<AssetCheckPlanDetailModel> GetByPlanId(int planId)
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetByPlanId(planId);
        }
        

        public IList<Hashtable> GetPlanAllCheckAssetsListForPaging(PagingModel paging)
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetPlanAllCheckAssetsListForPaging(paging);
        }

        /// <summary>
        /// 获取条码获取未过期的盘点计划的盘点明细信息；
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public IList<AssetCheckPlanDetailModel> GetCheckingPlanDetailByBarCode(string barCode)
        {
            return DBProvider.AssetCheckPlanDetailDAO.GetCheckingPlanDetailByBarCode(barCode);
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="operatorJobNo"></param>
        /// <returns></returns>
        public string AssetCheck(string barCode, int userId, string userName)
        {
            string msg = "你不是该设备盘点人员!";
            Instrument.Common.Models.InstrumentModel instrumentModel = ServiceProvider.InstrumentService.GetByBarCode(barCode);
            if (instrumentModel == null) return  msg = "该设备不存在,请盘盈！" ;

            IList<AssetCheckPlanDetailModel> detailList = ServiceProvider.AssetCheckPlanDetailService.GetCheckingPlanDetailByBarCode(barCode);
            if (detailList.Count == 0) return  msg = "没有该设备的盘点任务！" ;
            
            foreach (AssetCheckPlanDetailModel detail in detailList)
            {
                IList<AssetCheckOperatorModel> operatorList = ServiceProvider.AssetCheckOperatorService.GetByPlanId(detail.PlanId);
                if (operatorList.Where(W => W.UserId == userId).Count() > 0)
                {
                    DBProvider.AssetCheckPlanDetailDAO.AssetCheck(detail.PlanDetailId, Constants.AssetsCheckStatus.已盘点.GetHashCode(), userName);
                    msg = "OK";
                }
            }
            //更新最近盘点人和最近盘点时间
            instrumentModel.LastCheckUser = userName;
            ServiceProvider.InstrumentService.UpdateLastCheckInfo(instrumentModel);

            return msg;
        }

        /// <summary>
        /// 盘点地点是否一致
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="operatorJobNo"></param>
        /// <returns></returns>
        public string AssetIsRightAddressAndRemark(string barCode, int userId, string remark, int isRightAddress)
        {
            string msg = "你不是该设备盘点人员!";
            Instrument.Common.Models.InstrumentModel instrumentModel = ServiceProvider.InstrumentService.GetByBarCode(barCode);
            if (instrumentModel == null) return msg = "该设备不存在,请盘盈！";

            IList<AssetCheckPlanDetailModel> detailList = ServiceProvider.AssetCheckPlanDetailService.GetCheckingPlanDetailByBarCode(barCode);
            if (detailList.Count == 0) return msg = "没有该设备的盘点任务！";

            foreach (AssetCheckPlanDetailModel detail in detailList)
            {
                IList<AssetCheckOperatorModel> operatorList = ServiceProvider.AssetCheckOperatorService.GetByPlanId(detail.PlanId);
                if (operatorList.Where(W => W.UserId == userId).Count() > 0)
                {
                    DBProvider.AssetCheckPlanDetailDAO.AssetIsRightAddressAndRemark(detail.PlanDetailId, isRightAddress, remark);
                    msg = "OK";
                }
            }
            return msg;
        }

        /// <summary>
        /// 根据盘点计划ID导出仪器数据
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public string CreateExcel(int planId, string where)
        {
            //盘点计划属性
            AssetCheckPlanModel assetCheckPlanModel = ServiceProvider.AssetCheckPlanService.GetById(planId);
            IList<AssetCheckPlanDetailModel>  listModel = DBProvider.AssetCheckPlanDetailDAO.GetByWhere(where);
            //获取所有仪器的ID
            IList<int> listId = new List<int>();
            foreach (AssetCheckPlanDetailModel a in listModel)
            {
                listId.Add(a.InstrumentId);
            }
            IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIds(listId);
            ISheet sheet = null;
            IWorkbook hssfworkbook = null;
            //读取Excel模板的路径
            string templatepath = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/App_Data/盘点表及盘盈表模板.xls");
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + ".xls");

            if (IsExistSheet(ref sheet, ref hssfworkbook, templatepath, "盘点表"))
            {
                //强制要求Excel在打开时重新计算的属性
                sheet.ForceFormulaRecalculation = true;

                //获取当前工作表的索引
                int sheetIndex = hssfworkbook.GetSheetIndex(sheet);
                int sheetNumbers = hssfworkbook.NumberOfSheets;
                //删除多余的工作表
                for (int i = sheetNumbers - 1; i >= 0; i--)
                {
                    if (i != sheetIndex)
                    {
                        hssfworkbook.RemoveSheetAt(i);
                    }
                }
                //单元格样式
                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                //居中
                style.VerticalAlignment = VerticalAlignment.Center;
                style.Alignment = HorizontalAlignment.Center;
                style.WrapText = true;

                sheet.GetRow(0).GetCell(0).SetCellValue(assetCheckPlanModel.PlanName);
                int originalRowData = 4;
                int rowIndex = 0;
                if (listModel.Count > 15)
                    sheet.ShiftRows(19, 23, listModel.Count - 15, true, true);

                foreach (AssetCheckPlanDetailModel model in listModel)
                {
                    InstrumentModel instrumentModel = instrumentList.SingleOrDefault(t => t.InstrumentId == Convert.ToInt32(model.InstrumentId));
                    if (sheet.GetRow(originalRowData + rowIndex) == null)
                        sheet.CreateRow(originalRowData + rowIndex);
                    sheet.GetRow(originalRowData + rowIndex).Height = 600;
                    sheet.GetRow(originalRowData + rowIndex).CreateCell(0).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(0).SetCellValue(rowIndex+1);//行数

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(1).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(1).SetCellValue(model.ManageNo);//管理编号

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(2).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(2).SetCellValue(model.AssetsNo);//资产编号

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(3).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(3).SetCellValue(model.InstrumentName);//资产名称

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(4).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(4).SetCellValue(model.Specification);//规格型号

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(5).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(5).SetCellValue(model.SerialNo);//出厂编号

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(6).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(6).SetCellValue(instrumentModel == null ? "" : instrumentModel.LeaderName);//保管人

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(7).CellStyle = style;
                    if (model.Statuse == Constants.AssetsCheckStatus.已盘点.GetHashCode())
                        sheet.GetRow(originalRowData + rowIndex).GetCell(7).SetCellValue("√");//实物是否存在
                    else
                        sheet.GetRow(originalRowData + rowIndex).GetCell(7).SetCellValue("×");

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(8).CellStyle = style;
                    if (model.IsRightAddress == Constants.AssetsIsRightAddress.一致.GetHashCode())
                        sheet.GetRow(originalRowData + rowIndex).GetCell(8).SetCellValue("√");//地点是否一致
                    else
                        sheet.GetRow(originalRowData + rowIndex).GetCell(8).SetCellValue("×");//地点是否一致

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(9).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(9).SetCellValue(model.Remark);//备注

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(10).CellStyle = style;
                    if(model.CheckDate == DateTime.MinValue)
                        sheet.GetRow(originalRowData + rowIndex).GetCell(10).SetCellValue("");//最近盘点时间，没有等于空
                    else
                        sheet.GetRow(originalRowData + rowIndex).GetCell(10).SetCellValue(model.CheckDate.ToString("yyyy-MM-dd"));//最近盘点时间

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(11).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(11).SetCellValue(model.Checkor);//最近盘点人
                    
                    rowIndex++;
                }
                FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                //写入内存流
                hssfworkbook.Write(fs);
                fs.Close();
                fs.Dispose();
                newFile = newFile + "," + assetCheckPlanModel.PlanName;
            }
            else
            {
                newFile = "";
            }
            return newFile;
        }

        /// <summary>
        /// 根据盘点计划ID导出盘盈仪器数据
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public string CreateExcelForOverAge(int planId)
        {
            //盘点计划属性
            AssetCheckPlanModel assetCheckPlanModel = ServiceProvider.AssetCheckPlanService.GetById(planId);
            IList<AssetCheckPlanDetailModel> list = ServiceProvider.AssetCheckPlanDetailService.GetByPlanIdAndStatus(planId, Constants.AssetsCheckStatus.盘盈.GetHashCode());

            ISheet sheet = null;
            IWorkbook hssfworkbook = null;
            //读取Excel模板的路径
            string templatepath = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/App_Data/盘点表及盘盈表模板.xls");
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + ".xls");

            if (IsExistSheet(ref sheet, ref hssfworkbook, templatepath, "盘盈表"))
            {
                //强制要求Excel在打开时重新计算的属性
                sheet.ForceFormulaRecalculation = true;

                //获取当前工作表的索引
                int sheetIndex = hssfworkbook.GetSheetIndex(sheet);
                int sheetNumbers = hssfworkbook.NumberOfSheets;
                //删除多余的工作表
                for (int i = sheetNumbers - 1; i >= 0; i--)
                {
                    if (i != sheetIndex)
                    {
                        hssfworkbook.RemoveSheetAt(i);
                    }
                }
                //单元格样式
                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                //居中
                style.VerticalAlignment = VerticalAlignment.Center;
                style.Alignment = HorizontalAlignment.Center;
                style.WrapText = true;

                sheet.GetRow(0).GetCell(0).SetCellValue(assetCheckPlanModel.PlanName);
                int originalRowData = 2;
                int rowIndex = 0;

                foreach (AssetCheckPlanDetailModel model in list)
                {
                    if (sheet.GetRow(originalRowData + rowIndex) == null)
                        sheet.CreateRow(originalRowData + rowIndex);
                    //sheet.SetColumnWidth(originalRowData + rowIndex, 5*256);
                    sheet.GetRow(originalRowData + rowIndex).Height = 600;
                    sheet.GetRow(originalRowData + rowIndex).CreateCell(0).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(0).SetCellValue(rowIndex + 1);//行数

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(1).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(1).SetCellValue(model.InstrumentName);//资产名称

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(2).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(2).SetCellValue(model.Specification);//规格型号

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(3).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(3).SetCellValue(model.Manufacturer);//生产厂家

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(4).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(4).SetCellValue(model.SerialNo);//出厂编号

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(5).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(5).SetCellValue(model.BelongDepart);//单位

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(6).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(6).SetCellValue("");//数量

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(7).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(7).SetCellValue("");//存放地点

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(8).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(8).SetCellValue(model.Remark);//备注

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(9).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(9).SetCellValue(model.CreateDate.ToString("yyyy-MM-dd"));//最近盘点时间

                    sheet.GetRow(originalRowData + rowIndex).CreateCell(10).CellStyle = style;
                    sheet.GetRow(originalRowData + rowIndex).GetCell(10).SetCellValue(model.CreateUser);//最近盘点人

                    rowIndex++;
                }
                FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                //写入内存流
                hssfworkbook.Write(fs);
                fs.Close();
                fs.Dispose();
                newFile = newFile + "," + assetCheckPlanModel.PlanName;
            }
            else
            {
                newFile = "";
            }
            return newFile;
        }

        /// <summary>
        /// 工作表是否存在
        /// </summary>
        public bool IsExistSheet(ref ISheet sheet, ref IWorkbook hssfworkbook, string templatepath, string sheetName)
        {
            bool result = true;
            //读取Excel模板
            using (FileStream file = new FileStream(templatepath, FileMode.Open, FileAccess.Read))
            {
                //获取Excel工作表
                hssfworkbook = new HSSFWorkbook(file);
                sheet = hssfworkbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
