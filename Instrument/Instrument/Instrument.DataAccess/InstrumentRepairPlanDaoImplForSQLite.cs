using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.Common.Models;
using System.Collections;
using ToolsLib.IBatisNet;

namespace Instrument.DataAccess
{

    public class InstrumentRepairPlanDaoImplForSQLite: InstrumentRepairPlanDaoImpl
    { 
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public override IList<Hashtable> GetInstrumentRepairPlanForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_RepairPlan";
            paging.FieldKey = "PlanId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "PlanId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            return list;
        }
    }
}
