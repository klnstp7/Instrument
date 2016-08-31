using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class AssetCheckPlanDetailDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(AssetCheckPlanDetailModel model)
        {
            DBProvider.dbMapper.Insert("AssetCheck_PlanDetail.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(AssetCheckPlanDetailModel model)
        {
            DBProvider.dbMapper.Update("AssetCheck_PlanDetail.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int PlanDetailId)
        {
            DBProvider.dbMapper.Delete("AssetCheck_PlanDetail.DeleteById", PlanDetailId);
        }

        public void DeleteByIdList(IList<int> detailIdList)
        {
            if (detailIdList.Count == 0)
                return;
            Hashtable ht = new Hashtable();
            ht.Add("DetailIdList", detailIdList);
            DBProvider.dbMapper.Delete("AssetCheck_PlanDetail.DeleteByIdList", ht);
        }


        public void DeleteByPlanId(int planId)
        {
            DBProvider.dbMapper.Delete("AssetCheck_PlanDetail.DeleteByPlanId", planId);
        }

        public void BatchImportFromInstrument(int instrumentForm, int planId, string createUser)
        {
            Hashtable ht = new Hashtable();
            ht["InstrumentForm"] = instrumentForm;
            ht["PlanId"] = planId;
            ht["CreateUser"] = createUser;
            ht["Checkor"] = createUser;
            DBProvider.dbMapper.Insert("AssetCheck_PlanDetail.BatchImportFromInstrument", ht);
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public AssetCheckPlanDetailModel GetById(int PlanDetailId)
        {
            return DBProvider.dbMapper.SelectObject<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetByID", PlanDetailId);
        }

        public AssetCheckPlanDetailModel GetByInstrumentIdAndPlanId(int instrumentId, int planId)
        {
            Hashtable ht = new Hashtable();
            ht["InstrumentId"] = instrumentId;
            ht["PlanId"] = planId;
            return DBProvider.dbMapper.SelectObject<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetByInstrumentIdAndPlanId", ht);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<AssetCheckPlanDetailModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetAll");
        }

        public IList<AssetCheckPlanDetailModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetByInstrumentId", instrumentId);
        }
        /// <summary>
        /// 获取条码获取未过期的盘点计划的盘点明细信息；
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public IList<AssetCheckPlanDetailModel> GetCheckingPlanDetailByBarCode(string barCode)
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetCheckingPlanDetailByBarCode", barCode);
        }

        /// <summary>
        /// 根据仪器标识和当前用户批量更新所有资产盘点明细
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <param name="state"></param>
        /// <param name="userId"></param>
        public bool IsExistPlanDetail(string barCode, int userId)
        {
            Hashtable ht = new Hashtable();
            ht["BarCode"] = barCode;
            ht["UserId"] = userId;
            int count = DBProvider.dbMapper.SelectObject<int>("AssetCheck_PlanDetail.IsExistPlanDetail", ht);
            bool result = count == 0 ? false : true;
            return result;
        }


        public void UpdateState(Hashtable ht)
        {
            DBProvider.dbMapper.Update("AssetCheck_PlanDetail.UpdateState", ht);
        }

        public IList<AssetCheckPlanDetailModel> GetByPlanIdAndStatus(int planId, int state)
        {
            Hashtable ht = new Hashtable();
            ht["PlanId"] = planId;
            ht["Statuse"] = state;
            return DBProvider.dbMapper.SelectList<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetByPlanIdAndStatus", ht);
        }

        public virtual IList<Hashtable> GetPlanAllCheckAssetsListForPaging(PagingModel paging)
        {
            paging.TableName = "AssetCheck_PlanDetail";
            paging.FieldKey = "PlanDetailId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"[PlanDetailId],[PlanId],[InstrumentId],[Statuse],[BelongDepart],[InstrumentName],[Specification],[Manufacturer],[SerialNo],[ManageNo],[AssetsNo],[Remark],[CreateUser],[CreateDate],[IsRightAddress]";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "PlanDetailId desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

        public void AssetCheck(int detailId, int statuse, string Checkor)
        {
            Hashtable ht = new Hashtable();
            ht["PlanDetailId"] = detailId;
            ht["Statuse"] = statuse;
            ht["Checkor"] = Checkor;
            DBProvider.dbMapper.Update("AssetCheck_PlanDetail.AssetCheck", ht);
        }
        /// <summary>
        /// 盘点地点是否一致和备注
        /// </summary>
        /// <param name="detailId"></param>
        /// <param name="statuse"></param>
        /// <param name="Checkor"></param>
        public void AssetIsRightAddressAndRemark(int detailId, int isRightAddress, string remark)
        {
            Hashtable ht = new Hashtable();
            ht["PlanDetailId"] = detailId;
            ht["IsRightAddress"] = isRightAddress;
            ht["Remark"] = remark;
            DBProvider.dbMapper.Update("AssetCheck_PlanDetail.AssetIsRightAddressAndRemark", ht);
        }
        
        public IList<AssetCheckPlanDetailModel> GetByPlanId(int planId)
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetByPlanId", planId);
        }
        public IList<AssetCheckPlanDetailModel> GetByWhere(string where)
        {
            return DBProvider.dbMapper.SelectList<AssetCheckPlanDetailModel>("AssetCheck_PlanDetail.GetByWhere", where);
        }
    } 
}
