using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class InstrumentWaitSendDaoImpl
    {
                /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.dbMapper.Delete("Instrument_WaitSend.DeleteByInstrumentId", InstrumentId);
        }
        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteByInstrumentId(int InstrumentId,int UserId)
        {
            Hashtable ht = new Hashtable();
            ht["InstrumentId"] = InstrumentId;
            ht["UserId"] = UserId;
            DBProvider.dbMapper.Delete("Instrument_WaitSend.DeleteByInstrumentIdAndUserId", ht);
        }
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(InstrumentWaitSendModel model)
        {
            DBProvider.dbMapper.Insert("Instrument_WaitSend.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(InstrumentWaitSendModel model)
        {
            DBProvider.dbMapper.Update("Instrument_WaitSend.Update", model);
        }
        /// <summary>
        /// 批量更新数据.
        /// </summary>
        public void UpdateRemark(IList<int> instrumentIdList, string Remark, int userId)
        {
            Hashtable ht = new Hashtable();
            ht["UserId"] = userId;
            ht["Remark"] = Remark;
            ht["InstrumentIdList"] = instrumentIdList;
            DBProvider.dbMapper.Update("Instrument_WaitSend.UpdateRemark", ht);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int AutoId)
        {
            DBProvider.dbMapper.Delete("Instrument_WaitSend.DeleteById", AutoId);
        }

        public void DeleteByIds(IList<int> autoIdList)
        {
            Hashtable ht = new Hashtable();
            ht["AutoIdList"] = autoIdList;
            DBProvider.dbMapper.Delete("Instrument_WaitSend.DeleteByIds", ht);
        }
        /// <summary>
        /// 通过仪器id集合批量删除
        /// </summary>
        /// <param name="instrumentIdList"></param>
        public void DeleteByInstrumentIds(IList<int> instrumentIdList,int userId)
        {
            Hashtable ht = new Hashtable();
            ht["UserId"] = userId;
            ht["InstrumentIdList"] = instrumentIdList;
            DBProvider.dbMapper.Delete("Instrument_WaitSend.DeleteByInstrumentIds", ht);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public InstrumentWaitSendModel GetById(int AutoId)
        {
            return DBProvider.dbMapper.SelectObject<InstrumentWaitSendModel>("Instrument_WaitSend.GetByID", AutoId);
        }
        public IList<InstrumentWaitSendModel> GetByAutoIdList(IList<int> autoIdList)
        {
            if (autoIdList.Count == 0)
                autoIdList.Add(0);
            Hashtable ht = new Hashtable();
            ht.Add("AutoIdList", autoIdList);
            return DBProvider.dbMapper.SelectList<InstrumentWaitSendModel>("Instrument_WaitSend.GetByAutoIdList", ht);
        }
        public IList<InstrumentWaitSendModel> GetByInstrumentIdsList(IList<int> InstrumentIds,int UserId)
        {
            if (InstrumentIds.Count == 0)
                InstrumentIds.Add(0);
            Hashtable ht = new Hashtable();
            ht.Add("InstrumentIds", InstrumentIds);
            ht.Add("UserId", UserId);
            return DBProvider.dbMapper.SelectList<InstrumentWaitSendModel>("Instrument_WaitSend.GetByInstrumentIdsList", ht);
        }

        

//        public IList<Hashtable> GetAllPreSendListForPaging(PagingModel paging)
//        {
//            paging.TableName = "View_PreSendInstrument";
//            paging.FieldKey = "AutoId";
//            if (string.IsNullOrEmpty(paging.FieldShow))
//            {
//                paging.FieldShow = @" AutoId,OrderId,CertificationNumber,InspectDate, OrderDueEndDate, OrderInspectOrg,IsDownloadCert,IsComplete,IsCompleteCert,IsSend
//                ,MD5Code,InstrumentId,BelongSubCompany,BelongDepart,StorePalce,InstrumentName,EnglishName,Specification,Manufacturer,SerialNo
//                ,ManageNo,AssetsNo,ManageLevel,InspectCycle,InspectOrg,CertificateNo,DueStartDate,DueEndDate,LeaderName,SpecificationCode,ProjectTeam,MeasureCharacter
//                ,TechniqueCharacter,BuyDate,InstrumentType,Price,DurableYears,CraftId,InstrumentCate,CalibrationType,VerificationType,Number,BarCode,RecordState,CreateDate,CreateUser";
//            }
//            if (string.IsNullOrEmpty(paging.FieldOrder))
//                paging.FieldOrder = "AutoId desc";

//            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

//            return list;
//        }

        /// <summary>
        /// 获取已经加入送检清单的仪器
        /// </summary>
        /// <param name="InstrumentIdList"></param>
        /// <returns></returns>
        public IList<InstrumentWaitSendModel> GetByUserId(int userId)
        {
            return DBProvider.dbMapper.SelectList<InstrumentWaitSendModel>("Instrument_WaitSend.GetByUserId", userId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<InstrumentWaitSendModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<InstrumentWaitSendModel>("Instrument_WaitSend.GetAll");
        }

        /// <summary>
        /// 检测用户的仪器是否已存在.
        /// </summary>
        public bool ExistsInstrument(int InstrumentId, int UserId)
        {
            Hashtable ht = new Hashtable();
            ht["InstrumentId"] = InstrumentId;
            ht["UserId"] = UserId;
            return DBProvider.dbMapper.SelectObject<int>("Instrument_WaitSend.GetCountByInstrumentId", ht) > 0;
        }
        /// <summary>
        /// 获取待送仪器信息;
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns>IList 【InstrumentName,Specification ,SerialNo,ManageNo ,AssetsNo,BarCode】</returns>
        public IList<Hashtable> GetInstrumentByUserId(int Userid)
        {
            return DBProvider.dbMapper.SelectList<Hashtable>("Instrument_WaitSend.GetInstrumentByUserId", Userid);
        }
    }
}
