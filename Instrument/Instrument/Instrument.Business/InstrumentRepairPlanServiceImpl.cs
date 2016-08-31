using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Instrument.Common.Models;
using Instrument.DataAccess;
using GRGTCommonUtils;
using System.Collections;
using ToolsLib.IBatisNet;

namespace Instrument.Business
{
    public class InstrumentRepairPlanServiceImpl
    {
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.InstrumentRepairPlanDAO.DeleteByInstrumentId(InstrumentId);
        }
        /// <summary>
        /// 删除维护计划.同时删除计划所有附件和文件
        /// </summary>
        public void DeleteById(int PlanId)
        {
            DBProvider.InstrumentRepairPlanDAO.DeleteById(PlanId);
        }

        /// <summary>
        /// 保存维护计划.
        /// </summary>
        public void Save(InstrumentRepairPlanModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Remark))
            {
                model.Remark = UtilsHelper.SpecialCharValidate(model.Remark);
            }
            if (model.PlanId == 0)
            {
                model.Creator = GRGTCommonUtils.LoginHelper.LoginUser.UserName;
                model.ItemCode = Guid.NewGuid().ToString();
                DBProvider.InstrumentRepairPlanDAO.Add(model);
            }
            else
            {
                DBProvider.InstrumentRepairPlanDAO.Update(model);
            }
        }

        /// <summary>
        /// 批量保存维修信息
        /// </summary>
        public string BatchImportRepairPlan(DataTable dt, ref string sucessMsg)
        {
            StringBuilder result = new StringBuilder();
            InstrumentRepairPlanModel repairPlan = new InstrumentRepairPlanModel();
            int sucessCount = 0;
            InstrumentModel instrument = new InstrumentModel();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    instrument = DBProvider.InstrumentDAO.GetByBarCode(dr["条码"].ToString());
                    if (instrument == null && string.IsNullOrWhiteSpace(dr["报修日期"].ToString()) && string.IsNullOrWhiteSpace(dr["修复日期"].ToString()))
                    {
                        continue;
                    }
                    repairPlan.InstrumentId = instrument.InstrumentId;
                    repairPlan.RepairCompany =  UtilsHelper.SpecialCharValidate(dr["维修公司"].ToString());
                    repairPlan.Repairer = UtilsHelper.SpecialCharValidate(dr["维修人员"].ToString());
                    repairPlan.Mobile = UtilsHelper.SpecialCharValidate(dr["联系电话"].ToString());
                    if (!string.IsNullOrWhiteSpace(dr["维修金额"].ToString())) repairPlan.RepairMoney = Convert.ToDecimal(dr["维修金额"].ToString());
                    repairPlan.Leader = UtilsHelper.SpecialCharValidate(dr["报修人"].ToString());
                    repairPlan.ReportCode = UtilsHelper.SpecialCharValidate(dr["报告编号"].ToString());
                    repairPlan.DueStartDate = Convert.ToDateTime(dr["报修日期"].ToString());
                    repairPlan.DueEndDate = Convert.ToDateTime(dr["修复日期"].ToString());
                    repairPlan.Reason = UtilsHelper.SpecialCharValidate(dr["故障原因"].ToString());
                    repairPlan.TermService = UtilsHelper.SpecialCharValidate(dr["保修期限"].ToString());
                    repairPlan.Remark = UtilsHelper.SpecialCharValidate(dr["故障描述"].ToString());
                    repairPlan.Creator = GRGTCommonUtils.LoginHelper.LoginUser.UserName;//创建人
                    repairPlan.ItemCode = Guid.NewGuid().ToString();//获取项目编码
                    repairPlan.CreateDate = DateTime.Now;//获取系统当前时间
                    DBProvider.InstrumentRepairPlanDAO.Add(repairPlan);
                    //导入成功记录
                    sucessCount++;
                }
                catch (Exception ex)
                {
                    result.AppendLine("未成功导入维修信息:仪器条码(" + instrument.BarCode + ")");
                }
            }
            sucessMsg = "共导入" + sucessCount + "条记录";
            return result.ToString();
        }

        /// <summary>
        /// 获取一个维护计划.
        /// </summary>
        public InstrumentRepairPlanModel GetById(int PlanId)
        {
            return DBProvider.InstrumentRepairPlanDAO.GetById(PlanId);
        }

        /// <summary>
        /// 获取所有维护计划.
        /// </summary>
        public IList<InstrumentRepairPlanModel> GetAll()
        {
            return DBProvider.InstrumentRepairPlanDAO.GetAll();
        }

        /// <summary>
        /// 查询维修计划(根据仪器编号).
        /// </summary>
        public IList<InstrumentRepairPlanModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.InstrumentRepairPlanDAO.GetByInstrumentId(instrumentId);
        }

        /// <summary>
        /// 查询维修计划.
        /// </summary>
        public IList<InstrumentRepairPlanModel> GetByWhere(string where)
        {
            return DBProvider.InstrumentRepairPlanDAO.GetByWhere(where);
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IList<Hashtable> GetInstrumentRepairPlanForPaging(PagingModel paging)
        {
            return DBProvider.InstrumentRepairPlanDAO.GetInstrumentRepairPlanForPaging(paging);
        }


    }
}
