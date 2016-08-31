using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class AssetCheckPlanDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(AssetCheckPlanModel model)
        {
            DBProvider.dbMapper.Insert("AssetCheck_Plan.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(AssetCheckPlanModel model)
        {
            DBProvider.dbMapper.Update("AssetCheck_Plan.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int PlanId)
        {
            DBProvider.dbMapper.Delete("AssetCheck_Plan.DeleteById", PlanId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public AssetCheckPlanModel GetById(int PlanId)
        {
            return DBProvider.dbMapper.SelectObject<AssetCheckPlanModel>("AssetCheck_Plan.GetByID", PlanId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<AssetCheckPlanModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanModel>("AssetCheck_Plan.GetAll");
        }

        public IList<AssetCheckPlanModel> GetByUserId(int userId)
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanModel>("AssetCheck_Plan.GetByUserId", userId);
        }


        public virtual IList<Hashtable> GetAllAssetCheckPlanListForPaging(PagingModel paging)
        {
            paging.TableName = "AssetCheck_Plan";
            paging.FieldKey = "PlanId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                 paging.FieldShow = @"[PlanId],[PlanType],[PlanName],[StartDate],[EndDate],[Remark],[CreateUser],[CreateDate]";
			}
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "PlanId desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

        public IList<AssetCheckPlanModel> GetCheckingPlan()
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanModel>("AssetCheck_Plan.GetCheckingPlan");
        }
    }
}
