using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class UserManageDepartDaoImpl
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
            return DBProvider.dbMapper.SelectList<UserModel>("User_ManageDepart.GetManageUserByUserId", ht);
        }

        /// <summary>
        /// 通过用户userId得到该用户所管辖的对象实体.
        /// </summary>
        /// <param name="userId">工号</param>
        /// <returns></returns>
        public IList<UserManageDepartModel> GetByUserId(int userId)
        {
            return DBProvider.dbMapper.SelectList<UserManageDepartModel>("User_ManageDepart.GetByUserId", userId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<UserManageDepartModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<UserManageDepartModel>("User_ManageDepart.GetAll");
        }
       
        /// <summary>
        /// 主管部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<OrgModel> GetUserManageDepartByUserId(int userId)
        {
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetUserManageDepartByUserId", userId);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>       
        public void DeleteManageDepartByUserId(int UserId)
        {
            DBProvider.dbMapper.Delete("User_ManageDepart.DeleteManageDepartByUserId", UserId);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>       
        public void AddManageDepart(int userId, int orgId, string orgCode)
        {
            Hashtable ht = new Hashtable();
            //int orgId = DefaultMapper.GetMapper().SelectObject<UserManageDepartModel>("User_ManageDepart.GetMaxIdModel", "").OrgId + 1;
            ht.Add("OrgId", orgId);
            ht.Add("UserID", userId);
            //ht.Add("JobNo", jobNo);
            ht.Add("OrgCode", orgCode);

            DBProvider.dbMapper.Insert("User_ManageDepart.InsertManageDepart", ht);
        }
    }
}
