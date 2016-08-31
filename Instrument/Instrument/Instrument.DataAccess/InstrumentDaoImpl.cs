using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class InstrumentDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add4Instrument(InstrumentModel model)
        {
            DBProvider.dbMapper.Insert("Instrument_BaseInfo.InsertInstrument", model);
        }

        public void Add4Assets(InstrumentModel model)
        {
            DBProvider.dbMapper.Insert("Instrument_BaseInfo.InsertAssets", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update4Instrument(InstrumentModel model)
        {
            DBProvider.dbMapper.Update("Instrument_BaseInfo.Update4Instrument", model);
        }

        /// <summary>
        /// 修改固定资产.
        /// </summary>
        public void Update4Assets(InstrumentModel model)
        {
            DBProvider.dbMapper.Update("Instrument_BaseInfo.Update4Assets", model);
        }

        //public void UpdateAssetsCheckResult(InstrumentModel model)
        //{
        //    DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateAssetsCheckResult", model);
        //}

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int InstrumentId)
        {
            DBProvider.dbMapper.Delete("Instrument_BaseInfo.DeleteById", InstrumentId);
        }

        public IList<InstrumentModel> GetAllInstrumentListByWhere(string where)
        {
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetAllInstrumentListByWhere", where);
        }

        public IList<InstrumentModel> GetByIdList(IList<int> instrumentIdList)
        {
            if (instrumentIdList.Count == 0)
                instrumentIdList.Add(0);
            Hashtable ht = new Hashtable();
            ht.Add("InstrumentIdList", instrumentIdList);
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetByIdList", ht);
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public InstrumentModel GetById(int InstrumentId)
        {
            return DBProvider.dbMapper.SelectObject<InstrumentModel>("Instrument_BaseInfo.GetByID", InstrumentId);
        }

        public InstrumentModel GetByBarCode(string barCode)
        {
            return DBProvider.dbMapper.SelectObject<InstrumentModel>("Instrument_BaseInfo.GetByBarCode", barCode);
        }

        public void UpdateInstrumentOfCraft(int craftId, int instrumentId)
        {
            Hashtable ht = new Hashtable();
            ht["CraftId"] = craftId;
            ht["InstrumentId"] = instrumentId;
            DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateInstrumentOfCraft", ht);
        }

        public void UpdateCertificationInfo(InstrumentModel model)
        {
            DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateCertificationInfo", model);
        }

        /// <summary>
        /// 上一次修改人和修改时间
        /// </summary>
        /// <param name="model"></param>
        public void UpdateLastUpdateInfo(InstrumentModel model)
        {
            DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateLastUpdateInfo", model);
        }

        /// <summary>
        /// 最近盘点人和最近盘点时间
        /// </summary>
        /// <param name="model"></param>
        public void UpdateLastCheckInfo(InstrumentModel model)
        {
            DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateLastCheckInfo", model);
        }

        /// <summary>
        /// 通过craftId获得所有记录.
        /// </summary>
        public IList<InstrumentModel> GetByCraftIdList(int craftId)
        {
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetByCraftIdList", craftId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<InstrumentModel> GetByWhere(string where)
        {
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetByWhere", where);
        }
        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<InstrumentModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetAll");
        }

        public virtual IList<Hashtable> GetAllInstrumentListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_BaseInfo";
            paging.FieldKey = "InstrumentId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"InstrumentId,BelongDepart,StorePalce,InstrumentName,EnglishName,Specification,Manufacturer,SerialNo
                ,ManageNo,AssetsNo,ManageLevel,InspectCycle,InspectOrg,CertificateNo,DueStartDate,DueEndDate,LeaderName,ProjectTeam,MeasureCharacter
                ,TechniqueCharacter,BuyDate,InstrumentType,Price,DurableYears,CraftId,InstrumentCate,CalibrationType,VerificationType,Number,BarCode,RecordState,CreateDate,CreateUser,LastUpdateDate";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "LastUpdateDate desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

        public virtual IList<Hashtable> GetAllCertificationListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_BaseInfo a left join Instrument_Certification b on a.InstrumentId=b.InstrumentId";
            paging.FieldKey = "InstrumentId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "InstrumentId desc";
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);
            return list;
        }

        public virtual IList<Hashtable> GetAllPeriodcheckListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_Periodcheck";
            paging.FieldKey = "PeriodcheckId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "PeriodcheckId desc";
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);
            return list;
        }
        public virtual IList<Hashtable> GetAllInnerCheckListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_InnerCheck";
            paging.FieldKey = "InnerCheckId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "InnerCheckId desc";
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);
            return list;
        }
        
        

        /// <summary>
        /// 通过ID列表得到对象实体集合.
        /// </summary>
        public IList<InstrumentModel> GetByIds(IList<int> InstrumentIdList)
        {
            Hashtable ht = new Hashtable();
            if (InstrumentIdList == null || InstrumentIdList.Count == 0)
                InstrumentIdList = new List<int>() { 0 };
            ht.Add("InstrumentIdList", InstrumentIdList);
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetByIdList", ht);
        }

        public void UpdateInstrumentSetCraftNull(int  craftId)
        {
            DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateInstrumentSetCraftNull", craftId);
        }

        /// <summary>
        /// 是否存在管理编号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistManageNo(InstrumentModel model)
        {
            int count = DBProvider.dbMapper.SelectObject<int>("Instrument_BaseInfo.IsExistManageNo", model);
            return count > 0;
        }

        /// <summary>
        /// 是否存在资产编号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistAssetsNo(InstrumentModel model)
        {
            int count = DBProvider.dbMapper.SelectObject<int>("Instrument_BaseInfo.IsExistAssetsNo", model);
            return count > 0;
        }

        /// <summary>
        /// 更新器具库(有效期过期)
        /// </summary>
        public void UpdateForOverDue()
        {

            DBProvider.dbMapper.Update("Instrument_BaseInfo.UpdateForOverDue", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 用in的方式批量删除
        /// </summary>
        public void DeleteBatchByIds(string InstrumentIds)
        {
            DBProvider.dbMapper.Delete("Instrument_BaseInfo.DeleteBatchByIds", InstrumentIds);
        }

        /// <summary>
        /// 获取关键字查询仪器信息;
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IList<InstrumentModel> GetInstrumentListByKeyWorks(Hashtable hs)
        {
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetInstrumentListByKeyWorks", hs);
        }

        /// <summary>
        /// 根据管理编号得到一个对象实体.
        /// </summary>
        public InstrumentModel GetByManageNo(string manageNo)
        {
            return DBProvider.dbMapper.SelectObject<InstrumentModel>("Instrument_BaseInfo.GetByManageNo", manageNo);
        }

        public IList<InstrumentModel> GetInstrumentByParentId(int parentId)
        {
            return DBProvider.dbMapper.SelectList<InstrumentModel>("Instrument_BaseInfo.GetByParentId",parentId);
        }
       
    }
}
