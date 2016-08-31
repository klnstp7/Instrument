using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.Common.Models;
using Instrument.DataAccess;
using System.Collections;
using GRGTCommonUtils;

namespace Instrument.Business
{
    public class InstrumentUsingPlanServiceImpl
    {
        /// <summary>
        /// 删除排期调度.
        /// </summary>
        public void DeleteById(int PlanId)
        {
            DBProvider.InstrumentUsingPlanDAO.DeleteById(PlanId);
        }

        /// <summary>
        /// 保存排期调度.
        /// </summary>
        public void Save(InstrumentUsingPlanModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Remark))
            {
                model.Remark = UtilsHelper.SpecialCharValidate(model.Remark);
            }
            if (0 == model.PlanId)
            {
                model.Creator = GRGTCommonUtils.LoginHelper.LoginUser.UserName;
                DBProvider.InstrumentUsingPlanDAO.Add(model);
            }
            else
            {
                DBProvider.InstrumentUsingPlanDAO.Update(model);
            }
        }

        /// <summary>
        /// 获取一个排期调度.
        /// </summary>
        public InstrumentUsingPlanModel GetById(int PlanId)
        {
            return DBProvider.InstrumentUsingPlanDAO.GetById(PlanId);
        }

        /// <summary>
        /// 获取所有排期调度.
        /// </summary>
        public IList<InstrumentUsingPlanModel> GetAll()
        {
            return DBProvider.InstrumentUsingPlanDAO.GetAll();
        }

        /// <summary>
        /// 查询排期调度(根据仪器标识)
        /// </summary>
        /// <returns></returns>
        public IList<InstrumentUsingPlanModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.InstrumentUsingPlanDAO.GetByInstrumentId(instrumentId);
        }

        /// <summary>
        /// 查询排期(根据仪器标识和时间)
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public IList<InstrumentUsingPlanModel> GetByInstrumentIdsAndDate(Hashtable hash)
        {
            return DBProvider.InstrumentUsingPlanDAO.GetByInstrumentIdsAndDate(hash);
        }

        /// <summary>
        /// 查询排期调度(根据测试项目标识)
        /// </summary>
        /// <returns></returns>
        public IList<InstrumentUsingPlanModel> GetByProjectNumber(string ProjectNumber)
        {
            return DBProvider.InstrumentUsingPlanDAO.GetByProjectNumber(ProjectNumber);
        }

    }
}
