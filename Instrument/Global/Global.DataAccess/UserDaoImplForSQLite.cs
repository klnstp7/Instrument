using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using ToolsLib.IBatisNet;
using System.Collections;


namespace Global.DataAccess
{
    public class UserDaoImplForSQLite : UserDaoImpl
    {
        public override IList<Hashtable> GetListForPaging(PagingModel paging)
        {
            paging.TableName = "Sys_User";
            paging.FieldKey = "UserId";
            paging.FieldShow = "UserId,UserName,DepartName,Duty,JobNo,Mobile1,Email1,Email2,EmployeeState,Sex";
            paging.FieldOrder = "DepartName";

            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);

            return list;
        }

    }
}
