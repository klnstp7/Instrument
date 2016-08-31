using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;

namespace Instrument.DataAccess
{
    public class AssetCheckPlanDaoImplForSQLite : AssetCheckPlanDaoImpl
    {
        public override IList<Hashtable> GetAllAssetCheckPlanListForPaging(PagingModel paging)
        {
            paging.TableName = "AssetCheck_Plan";
            paging.FieldKey = "PlanId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"[PlanId],[PlanType],[PlanName],[StartDate],[EndDate],[Remark],[CreateUser],[CreateDate]";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "PlanId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            if (list == null) list = new List<Hashtable>();

            return list;
        }
    }
}
