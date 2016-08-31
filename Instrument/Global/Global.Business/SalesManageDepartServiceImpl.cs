using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using System.Data;
using System.Collections;
using GRGTCommonUtils;
using ToolsLib.Utility;


namespace Global.Business
{
    public class SalesManageDepartServiceImpl
    {
        /// <summary>
        /// 通过用户ID获取一个记录对象.
        /// </summary>
        //public IList<SalesManageDepartModel> GetByUserId(int UserId)
        //{
        //    IList<SalesManageDepartModel> model = DBProvider.SalesManageDepartDAO.GetByUserId(UserId);
        //    if (model == null) model = new List<SalesManageDepartModel>();

        //    return model;
        //}


        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<SalesManageDepartModel> GetAll()
        {
            return DBProvider.SalesManageDepartDAO.GetAll();
        }

        /// <summary>
        /// 管理的用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public IList<UserModel> GetManageUserByUserId(int userId, string orgCode)
        {
            return DBProvider.SalesManageDepartDAO.GetManageUserByUserId(userId, orgCode);
        }

        /// <summary>
        /// 获取当前用户所管辖的片区的员工信息的SQL语句.
        /// 调用说明:若某表的用户字段为SaleId,SaleId作为参数传入该方法.调用者的SQL语句应为:XX='xx' And {0},再将该方法返回的SQL语句替换到{0}即可.
        /// </summary>
        /// <param name="UserFieldName">某表的用户的字段名</param>
        /// <returns></returns>
        public StringBuilder GetSQL2MyMangeDepart(string UserFieldName)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            //判断是否为区域经理
            IList<SalesManageDepartModel> dataList = DBProvider.SalesManageDepartDAO.GetByJobNo(LoginHelper.LoginUser.JobNo);
            if (dataList == null || dataList.Count == 0)
                sqlBuilder.AppendFormat("{0} = {1}", UserFieldName, LoginHelper.LoginUser.UserId);
            else
            {
                sqlBuilder.AppendFormat("({0} IN (Select u.UserId From Sys_User u Join Sales_ManageDepart m On u.BelongDepart = m.OrgCode And m.JobNo = '{1}') Or {0}={2})", UserFieldName, LoginHelper.LoginUser.JobNo, LoginHelper.LoginUser.UserId);
            }
            return sqlBuilder;
        }
    }
}
