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

namespace Instrument.Business
{
    /// <summary>
    /// 实验室标准器具
    /// </summary>
    public class AssetCheckPlanServiceImpl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AssetCheckPlanServiceImpl));
        /// <summary>
        /// 删除记录.
        /// </summary>
        public string DeleteById(int PlanId)
        {
                DBProvider.dbMapper.mapper.BeginTransaction();
                try
                {
                    DBProvider.AssetCheckPlanDetailDAO.DeleteByPlanId(PlanId);
                    DBProvider.AssetCheckOperatorDAO.DeleteByPlanId(PlanId);
                    DBProvider.AssetCheckPlanDAO.DeleteById(PlanId);

                    DBProvider.dbMapper.mapper.CommitTransaction();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    DBProvider.dbMapper.mapper.RollBackTransaction();
                    return "删除失败";
                }
                return "OK";

        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(AssetCheckPlanModel model)
        {
            if (model.PlanId == 0) DBProvider.AssetCheckPlanDAO.Add(model);
            else DBProvider.AssetCheckPlanDAO.Update(model);
        }



        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public AssetCheckPlanModel GetById(int PlanId)
        {
            return DBProvider.AssetCheckPlanDAO.GetById(PlanId);
        }

        public IList<AssetCheckPlanModel> GetByUserId(int userId)
        {
            return DBProvider.AssetCheckPlanDAO.GetByUserId(userId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<AssetCheckPlanModel> GetAll()
        {
            return DBProvider.AssetCheckPlanDAO.GetAll();
        }

        public IList<Hashtable> GetAllAssetCheckPlanListForPaging(PagingModel paging)
        {
            return DBProvider.AssetCheckPlanDAO.GetAllAssetCheckPlanListForPaging(paging);
        }

        /// <summary>
        /// 获取正在进行的盘点计划;
        /// </summary>
        /// <returns></returns>
        public IList<AssetCheckPlanModel> GetCheckingPlan()
        {
            return DBProvider.AssetCheckPlanDAO.GetCheckingPlan();
        }
    }
}
