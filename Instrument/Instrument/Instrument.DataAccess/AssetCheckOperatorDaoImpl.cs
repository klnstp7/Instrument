using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class AssetCheckOperatorDaoImpl
    {
        	/// <summary>
		/// 增加一条数据.
		/// </summary>
        public void Add(AssetCheckOperatorModel model)
		{
			DBProvider.dbMapper.Insert("AssetCheck_Operator.Insert", model);
		}

		/// <summary>
		/// 更新一条数据.
		/// </summary>
        public void Update(AssetCheckOperatorModel model)
		{
			DBProvider.dbMapper.Update("AssetCheck_Operator.Update", model);
		}

		/// <summary>
		/// 删除一条数据.
		/// </summary>
		public void DeleteById(int AutoId)
		{
			DBProvider.dbMapper.Delete("AssetCheck_Operator.DeleteById", AutoId);
		}

        public void DeleteByPlanId(int planId)
		{
            DBProvider.dbMapper.Delete("AssetCheck_Operator.DeleteByPlanId", planId);
		}
        

        public void DeleteByIdList(IList<int> autoIdList)
        {
            if (autoIdList.Count == 0)
                return;
            Hashtable ht = new Hashtable();
            ht.Add("AutoIdList", autoIdList);
            DBProvider.dbMapper.Delete("AssetCheck_Operator.DeleteByIdList", ht);
        }


		/// <summary>
		/// 得到一个对象实体.
		/// </summary>
        public AssetCheckOperatorModel GetById(int AutoId)
		{
            return DBProvider.dbMapper.SelectObject<AssetCheckOperatorModel>("AssetCheck_Operator.GetByID", AutoId);
		}

        public AssetCheckOperatorModel GetByPlanIdAndUserId(int planId,int userId)
        {
            Hashtable ht = new Hashtable();
            ht["PlanId"] = planId;
            ht["UserId"] = userId;
            return DBProvider.dbMapper.SelectObject<AssetCheckOperatorModel>("AssetCheck_Operator.GetByPlanIdAndUserId", ht);
        }

        public IList<AssetCheckOperatorModel> GetByUserId(int userId)
        {
            return DBProvider.dbMapper.SelectList<AssetCheckOperatorModel>("AssetCheck_Operator.GetByUserId", userId);
        }


		/// <summary>
		/// 获得所有记录.
		/// </summary>
        public IList<AssetCheckOperatorModel> GetAll()
		{
            return DBProvider.dbMapper.SelectList<AssetCheckOperatorModel>("AssetCheck_Operator.GetAll");
		}

        public IList<AssetCheckOperatorModel> GetByPlanId(int planId)
		{
            return DBProvider.dbMapper.SelectList<AssetCheckOperatorModel>("AssetCheck_Operator.GetByPlanId", planId);
		}
        
    }
}
