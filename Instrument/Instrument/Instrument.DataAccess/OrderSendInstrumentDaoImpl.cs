using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class OrderSendInstrumentDaoImpl
    {

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteByOrderId(int OrderId)
        {
            DBProvider.dbMapper.Delete("Order_SendInstrument.DeleteByOrderId", OrderId);
        }

                    /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.dbMapper.Delete("Order_SendInstrument.DeleteByInstrumentId", InstrumentId);
        }

        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(OrderSendInstrumentModel model)
        {
            DBProvider.dbMapper.Insert("Order_SendInstrument.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(OrderSendInstrumentModel model)
        {
            DBProvider.dbMapper.Update("Order_SendInstrument.Update", model);
        }

        public void UpdateIsComplete(OrderSendInstrumentModel model)
        {
            DBProvider.dbMapper.Update("Order_SendInstrument.UpdateIsComplete", model);
        }

        public void UpdateIsDownloadCert(int autoId)
        {
            DBProvider.dbMapper.Update("Order_SendInstrument.UpdateIsDownloadCert", autoId);
        }



        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int AutoId)
        {
            DBProvider.dbMapper.Delete("Order_SendInstrument.DeleteById", AutoId);
        }

        public void DeleteByIds(IList<int> autoIdList)
        {
            Hashtable ht = new Hashtable();
            ht["AutoIdList"] = autoIdList;
            DBProvider.dbMapper.Delete("Order_SendInstrument.DeleteByIds", ht);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public OrderSendInstrumentModel GetById(int AutoId)
        {
            return DBProvider.dbMapper.SelectObject<OrderSendInstrumentModel>("Order_SendInstrument.GetByID", AutoId);
        }

        public OrderSendInstrumentModel GetByItemCode(string ItemCode)
        {
            return DBProvider.dbMapper.SelectObject<OrderSendInstrumentModel>("Order_SendInstrument.GetByItemCode", ItemCode);
        }

        public IList<OrderSendInstrumentModel> GetByOrderId(int orderId)
        {
            return DBProvider.dbMapper.SelectList<OrderSendInstrumentModel>("Order_SendInstrument.GetByOrderId", orderId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        /// <param name="orderIdList">送检单ID列表</param>
        /// <returns>送检单仪器列表</returns>
        public IList<OrderSendInstrumentModel> GetByOrderIdList(IList<int> orderIdList)
        {
            if (orderIdList.Count == 0)
                orderIdList.Add(0);
            Hashtable ht = new Hashtable();
            ht.Add("OrderIdList", orderIdList);
            return DBProvider.dbMapper.SelectList<OrderSendInstrumentModel>("Order_SendInstrument.GetByOrderIdList", ht);
        }

        public virtual IList<Hashtable> GetAllSendListForPaging(PagingModel paging)
        {
//            paging.TableName = "View_SendInstrument";
//            paging.FieldKey = "AutoId";
//            if (string.IsNullOrEmpty(paging.FieldShow))
//            {
//                paging.FieldShow = @" AutoId,InstrumentId,CertificationNumber,InspectDate,DueEndDate,InspectOrg,IsDownloadCert,IsComplete,IsCompleteCert
//                 ,IsSend,MD5Code,OrderId,OrderNumber,UserId,SendUser,SendDate,InstrumentCount,BelongDepart,InstrumentName,Specification,Manufacturer
//                ,SerialNo,ManageNo,CertificateNo,DueStartDate,LeaderName,SpecificationCode,ProjectTeam,InstrumentType,ManuDate,InstrumentCate,InstrumentForm,CalibrationType,Number,BarCode";
//            }
            paging.TableName = "Order_SendInstrument";
            paging.FieldKey = "AutoId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @" AutoId,OrderId,InstrumentId,InstrumentName,Specification,CertificationNumber,SerialNo,ManageNo,InspectOrg,IsDownloadCert,IsComplete,IsCompleteCert,IsSend,MD5Code";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "AutoId desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

        /// <summary>
        /// 获取已经加入送检清单的仪器
        /// </summary>
        /// <param name="InstrumentIdList"></param>
        /// <returns></returns>
        public IList<OrderSendInstrumentModel> GetByIds(IList<int> InstrumentIdList)
        {
            Hashtable ht = new Hashtable();
            if (InstrumentIdList == null || InstrumentIdList.Count == 0)
                InstrumentIdList = new List<int>() { 0 };
            ht.Add("InstrumentIdList", InstrumentIdList);
            return DBProvider.dbMapper.SelectList<OrderSendInstrumentModel>("Order_SendInstrument.GetByIDList", ht);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<OrderSendInstrumentModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<OrderSendInstrumentModel>("Order_SendInstrument.GetAll");
        }
        /// <summary>
        /// 根据条件获得记录.
        /// </summary>
        public IList<OrderSendInstrumentModel> GetAllSendInstrumentListByWhere(string sqlwhere)
        {
            return DBProvider.dbMapper.SelectList<OrderSendInstrumentModel>("Order_SendInstrument.GetAllSendInstrumentListByWhere",sqlwhere);
        }


    }
}
