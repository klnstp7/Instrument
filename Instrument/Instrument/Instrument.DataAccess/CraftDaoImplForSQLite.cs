using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class CraftDaoImplForSQLite : CraftDaoImpl
    {
        public override IList<Hashtable> GetAllCraftListForPaging(PagingModel paging)
        {
            paging.TableName = "Craft_BaseInfo";
            paging.FieldKey = "CraftId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "CraftId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            return list;
        }
    }
}
