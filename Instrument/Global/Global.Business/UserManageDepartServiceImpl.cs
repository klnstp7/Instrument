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
    public class UserManageDepartServiceImpl
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
        public IList<UserManageDepartModel> GetAll()
        {
            return DBProvider.UserManageDepartDAO.GetAll();
        }

        /// <summary>
        /// 管理的用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public IList<UserModel> GetManageUserByUserId(int userId, string orgCode)
        {
            return DBProvider.UserManageDepartDAO.GetManageUserByUserId(userId, orgCode);
        }

        /// <summary>
        /// 组装用户所管理部门和高级查询所属部门过滤条件的SQL
        /// 若无所属部门查询条件，参数orgName传空值
        /// </summary>
        /// <param name="where">SQL Where条件</param>
        /// <param name="checkAllAuthority">用户查看所有管理部门的权限点</param>
        /// <param name="orgName">高级查询过滤条件（部门名称）</param>
        /// <returns></returns>
        public string GetManageAndDepartSearchCondition(string where, string checkAllAuthority, string orgName)
        {
            bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(checkAllAuthority.ToLower());

            if (!IsGetAllAuthority)
            {
                //获取当前用户所管辖的所有区域下的SQL语句.
                StringBuilder subSqlStr = this.GetSQL2MyMangeDepart("BelongDepart");
                where = string.Format(" {0} and {1}", subSqlStr, where);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll().Where(o => o.OrgName == orgName).ToList();
                string strFilte = "";
                foreach (OrgModel org in orgList)
                {
                    if (string.IsNullOrWhiteSpace(strFilte))
                        strFilte = string.Format("BelongDepart like '{0}%'", org.OrgCode);
                    else
                        strFilte = string.Format("{0} or BelongDepart like '{1}%'", strFilte, org.OrgCode);
                }
                if (orgList.Count > 0)
                    where = string.Format("{0} and ({1})", where, strFilte);
                else
                    where = string.Format("{0} and 1=0", where);
            }
            return where;
        }

        /// <summary>
        /// 组装用户所管理部门和高级查询所属部门过滤条件的SQL(接口调用)
        /// 若无所属部门查询条件，参数orgName传空值
        /// </summary>
        /// <param name="where">SQL Where条件</param>
        /// <param name="checkAllAuthority">用户查看所有管理部门的权限点</param>
        /// <param name="orgName">高级查询过滤条件（部门名称）</param>
        /// <returns></returns>
        public string GetManageAndDepartSearchCondition(string UserId)
        {
            return this.GetSQL2MyMangeDepart("BelongDepart", UserId).ToString();
        }
        /// <summary>
        /// 获取当前用户所管辖的部门(接口调用)
        /// </summary>
        /// <param name="columnName">某表的所属部门字段名</param>
        /// <returns></returns>
        public StringBuilder GetSQL2MyMangeDepart(string columnName, string UserId)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            IList<UserManageDepartModel> List = DBProvider.UserManageDepartDAO.GetByUserId(Convert.ToInt32(UserId));
            if (List == null || List.Count == 0)
            { sqlBuilder.Append(" 1>1 "); }
            else
            {
                sqlBuilder.AppendFormat("({0} IN (  select OrgCode from User_ManageDepart where UserId={1}))", columnName, UserId);
            }
            return sqlBuilder;
        }

        /// <summary>
        /// 获取当前用户所管辖的部门
        /// </summary>
        /// <param name="columnName">某表的所属部门字段名</param>
        /// <returns></returns>
        public StringBuilder GetSQL2MyMangeDepart(string columnName)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            var dataList = LoginHelper.LoginUser.manageDepartList;
            if (dataList == null || dataList.Count == 0)
            { sqlBuilder.Append(" 1>1 "); }
            else
            {
                sqlBuilder.AppendFormat("({0} IN (  select OrgCode from User_ManageDepart where UserId={1}))", columnName, LoginHelper.LoginUser.UserId);
            }
            return sqlBuilder;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public void DeleteManageDepartByUserId(int userId)
        {
            DBProvider.UserManageDepartDAO.DeleteManageDepartByUserId(userId);
        }

        /// <summary>
        /// 主管部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<OrgModel> GetUserManageDepartByUserId(int userId)
        {
            return DBProvider.UserManageDepartDAO.GetUserManageDepartByUserId(userId);
        }

        /// <summary>
        /// 保存管理部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void SaveUserManageDepart(int userId, string orgCode)
        {
            //UserModel user = Global.Business.ServiceProvider.UserService.GetById(userId);
            DBProvider.UserManageDepartDAO.DeleteManageDepartByUserId(userId);
            if (string.IsNullOrEmpty(orgCode)) return;

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            string code = string.Empty;
            string[] idArray = orgCode.Split(',');
            for (int i = 0; i < idArray.Length; i++)
            {
                if (string.IsNullOrEmpty(idArray[i])) break;
                idArray[i] = UtilsHelper.Decrypt(idArray[i]);
                code = orgList.SingleOrDefault(o=>o.OrgId == (int.Parse(idArray[i]))).OrgCode;
                DBProvider.UserManageDepartDAO.AddManageDepart(userId, int.Parse(idArray[i]), code);
            }
        }
    }
}
