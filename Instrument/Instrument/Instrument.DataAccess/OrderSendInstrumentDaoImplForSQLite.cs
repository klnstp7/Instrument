using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class OrderSendInstrumentDaoImplForSQLite: OrderSendInstrumentDaoImpl
    {
        public override IList<Hashtable> GetAllSendListForPaging(PagingModel paging)
        {
            paging.TableName = "Order_SendInstrument";
            paging.FieldKey = "AutoId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @" AutoId,OrderId,InstrumentId,InstrumentName,Specification,CertificationNumber,SerialNo,ManageNo,InspectOrg,IsDownloadCert,IsComplete,IsCompleteCert,IsSend,MD5Code";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "AutoId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);

            return list;
        }
    }
}
