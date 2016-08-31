using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class OrderDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(OrderModel model)
        {
            DBProvider.dbMapper.Insert("Order_BaseInfo.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(OrderModel model)
        {
            DBProvider.dbMapper.Update("Order_BaseInfo.Update", model);
        }

        /// <summary>
        /// 更新送检单的受理状态.
        /// </summary>
        public void UpdateReceivedInfo(OrderModel model)
        {
            DBProvider.dbMapper.Update("Order_BaseInfo.UpdateReceivedInfo", model);
        }

        public void UpdateDownloadCertState(OrderModel model)
        {
            DBProvider.dbMapper.Update("Order_BaseInfo.UpdateDownloadCertState", model);
        }

        public bool IsExistOrderNumber(string orderNumber)
        {
            return DBProvider.dbMapper.SelectObject<int>("Order_BaseInfo.IsExistOrderNumber", orderNumber) == 0 ? false : true;
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int OrderId)
        {
            DBProvider.dbMapper.Delete("Order_BaseInfo.DeleteById", OrderId);
        }

        public virtual IList<Hashtable> GetAllOrderListForPaging(PagingModel paging)
        {
            paging.TableName = "Order_BaseInfo";
            paging.FieldKey = "OrderId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"OrderId,OrderNumber,UserId,SendUser,SendDate,InstrumentCount,DownloadCertState,DownloadDate,ReceivedState,ReceivedUser,ReceivedDate,UpdateState,UpdateDate,CreateDate,CreateUser";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "OrderId desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public OrderModel GetById(int OrderId)
        {
            return DBProvider.dbMapper.SelectObject<OrderModel>("Order_BaseInfo.GetByID", OrderId);
        }

        public IList<OrderModel> GetByIdList(IList<int> orderIdList)
        {
            if (orderIdList.Count == 0)
                orderIdList.Add(0);
            Hashtable ht = new Hashtable();
            ht.Add("OrderIdList", orderIdList);
            return DBProvider.dbMapper.SelectList<OrderModel>("Order_BaseInfo.GetByIdList", ht);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<OrderModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<OrderModel>("Order_BaseInfo.GetAll");
        }

        /// <summary>
        /// 根据条件获得所有记录.
        /// </summary>
        public IList<OrderModel> GetAllOrderListByWhere(string where)
        {
            return DBProvider.dbMapper.SelectList<OrderModel>("Order_BaseInfo.GetAllOrderListByWhere", where);
        }

    }
}
