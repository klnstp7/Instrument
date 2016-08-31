using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class OrderDaoImplForSQLite : OrderDaoImpl
    {
        public override IList<Hashtable> GetAllOrderListForPaging(PagingModel paging)
        {
            paging.TableName = "Order_BaseInfo";
            paging.FieldKey = "OrderId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"OrderId,OrderNumber,UserId,SendUser,SendDate,InstrumentCount,DownloadCertState,DownloadDate,ReceivedState,ReceivedUser,ReceivedDate,UpdateState,UpdateDate,CreateDate,CreateUser";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "OrderId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);

            return list;
        }
    }
}
