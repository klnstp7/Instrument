using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class SalesManageDepartDaoImpl
    {   
        /// <summary>
        /// 通过用户ID得到一个对象实体.
        /// </summary>
        //public IList<SalesManageDepartModel> GetByUserId(int UserId)
        //{
        //    return null;
        //    //return DBProvider.dbMapper.SelectList<SalesManageDepartModel>("Sales_ManageDepart.GetByUserId", UserId);
        //}

        /// <summary>
        /// 管理的用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public IList<UserModel> GetManageUserByUserId(int userId, string orgCode)
        {
            Hashtable ht = new Hashtable();
            ht["UserId"] = userId;
            ht["OrgCode"] = orgCode;
            return DefaultMapper.GetMapper().SelectList<UserModel>("Sales_ManageDepart.GetManageUserByUserId", ht);
        }

        /// <summary>
        /// 通过用户工号JobNo得到该用户所管辖的对象实体.
        /// </summary>
        /// <param name="JobNo">工号</param>
        /// <returns></returns>
        public IList<SalesManageDepartModel> GetByJobNo(string JobNo)
        {
            return DBProvider.dbMapper.SelectList<SalesManageDepartModel>("Sales_ManageDepart.GetByJobNo", JobNo);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<SalesManageDepartModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<SalesManageDepartModel>("Sales_ManageDepart.GetAll");
        }

    }
}
