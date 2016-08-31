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
    public class AssetCheckOperatorServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int AutoId)
        {
            DBProvider.AssetCheckOperatorDAO.DeleteById(AutoId);
        }
        public void DeleteByIdList(IList<int> autoIdList)
        {
            DBProvider.AssetCheckOperatorDAO.DeleteByIdList(autoIdList);
        }
        

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(AssetCheckOperatorModel model)
        {
            if (model.AutoId == 0) DBProvider.AssetCheckOperatorDAO.Add(model);
            else DBProvider.AssetCheckOperatorDAO.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public AssetCheckOperatorModel GetById(int AutoId)
        {
            return DBProvider.AssetCheckOperatorDAO.GetById(AutoId);
        }


        public AssetCheckOperatorModel GetByPlanIdAndUserId(int planId, int userId)
        {
            return DBProvider.AssetCheckOperatorDAO.GetByPlanIdAndUserId(planId, userId);
        }

        public  IList<AssetCheckOperatorModel>  GetByUserId(int userId)
        {
            return DBProvider.AssetCheckOperatorDAO.GetByUserId(userId);
        }
        

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<AssetCheckOperatorModel> GetAll()
        {
            return DBProvider.AssetCheckOperatorDAO.GetAll();
        }

        public IList<AssetCheckOperatorModel> GetByPlanId(int planId)
        {
            return DBProvider.AssetCheckOperatorDAO.GetByPlanId(planId);
        }

    }
}
