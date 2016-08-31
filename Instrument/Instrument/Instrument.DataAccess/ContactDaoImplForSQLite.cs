using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;

namespace Instrument.DataAccess
{
    public class ContactDaoImplForSQLite : ContactDaoImpl
    {
        public override IList<Hashtable> GetAllContactListForPaging(PagingModel paging)
        {
            paging.TableName = "Contact";
            paging.FieldKey = "ContactId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"[ContactId],[CompanyName],[CaseType],[Abstract],[State],[ContactContent],[FeedbackContent],[FeedbackDate],[Creator],CreatId,[CreatDate],[ItemCode]";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "ContactId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            if (list == null) list = new List<Hashtable>();

            return list;
        }

    }
}
