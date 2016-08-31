using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using Global.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;


namespace Global.Business
{
    public class UserServiceImpl
    {
        /// <summary>
        /// 验证用户登录信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns>返回“OK”表示登录成功</returns>
        public string Login(ref UserModel user)
        {
            string errMsg = "OK";
            UserModel loginUser = GetByLoginName(user.JobNo);
            //string pwd = ToolsLib.Utility.StrUtils.Encrypt(user.LoginPwd, ToolsLib.LibConst.EncryptFormat.SHA1);
            //if (loginUser == null) errMsg = "用户名不存在！";
            //else if (loginUser.IsEnabled == false) errMsg = string.Format("用户{0}已被禁用，请联系管理员。", loginUser.JobNo);
            //else if (loginUser.LoginPwd != pwd) errMsg = "密码输入错误！";

            if (errMsg == "OK") user = loginUser;

            return errMsg;
        }

        public void SaveUserRoles(int userId, string roles)
        {
            if (string.IsNullOrEmpty(roles))
            {
                DBProvider.UserDAO.DeleteRolesByUserId(userId);
                return;
            }

            DBProvider.UserDAO.DeleteRolesByUserId(userId);

            string[] roleArr = roles.Split(',');
            for (int i = 0; i < roleArr.Length; i++)
            {
                if (string.IsNullOrEmpty(roleArr[i])) continue;

                DBProvider.UserDAO.AddUserRoles(userId, Convert.ToInt16(roleArr[i]));
            }
        }

        /// <summary>
        /// 获取一个记录对象
        /// </summary>
        public UserModel GetById(int UserId)
        {
            return DBProvider.UserDAO.GetById(UserId);
        }
        /// <summary>
        /// 通过ID集合或工号集合得到对象实体集合.
        /// </summary>
        public IList<UserModel> GetByIdsOrJobNos(IList<int> IdList, IList<string> JobNoList)
        {
            return DBProvider.UserDAO.GetByIdsOrJobNos(IdList, JobNoList);
        }



        /// <summary>
        /// 获取一个记录对象
        /// </summary>
        public UserModel GetByLoginName(string JobNo)
        {
            return DBProvider.UserDAO.GetByLoginName(JobNo);
        }

        /// <summary>
        /// 获取所有的记录对象
        /// </summary>
        public IList<UserModel> GetAll()
        {
            return DBProvider.UserDAO.GetAll();
        }
        public IList<Hashtable> GetByOrgId(int orgId)
        {
            return DBProvider.UserDAO.GetByOrgId(orgId);
        }

        /// <summary>
        /// 根据组织Id获取用户
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns></returns>
        public IList<UserModel> GetListByOrgId(int orgId)
        {
            return DBProvider.UserDAO.GetListByOrgId(orgId);
        }

        /// <summary>
        /// 获取所有用户角色
        /// </summary>
        /// <returns></returns>
        public IList<Hashtable> GetAllUserRole()
        {
            return DBProvider.UserDAO.GetAllUserRole();
        }


        /// <summary>
        /// 根据工号或姓名模糊查找
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IList<Hashtable> SearchByNameJobNumb(string input, string orgCode)
        {
            return DBProvider.UserDAO.SearchByNameJobNumb(input, orgCode);
        }
        /// <summary>
        /// 根据工号或姓名模糊查找通讯信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IList<Hashtable> Search4MsgByNameJobNumb(string input, string orgCode)
        {
            return DBProvider.UserDAO.Search4MsgByNameJobNumb(input, orgCode);
        }

        ///// <summary>
        ///// 根据工号或姓名模糊查找
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="orgId">组织代码</param>
        ///// <returns></returns>
        //public IList<Hashtable> SearchByNameJobNumb(string input,string orgId)
        //{
        //    if (string.IsNullOrWhiteSpace(orgId))
        //        return DBProvider.UserDAO.SearchByNameJobNumb(input);
        //    else
        //    {
        //        Hashtable table = new Hashtable();
        //        table.Add("UserName", input);
        //        table.Add("BelongDepart", orgId.Trim());
        //        return DBProvider.UserDAO.SearchByNameJobNumbAndOrgId(table);
        //    }
        //}

        public IList<Hashtable> GetListForPaging(PagingModel paging)
        {
            return DBProvider.UserDAO.GetListForPaging(paging);
        }

        /// <summary>
        /// Autocomplete功能提供查询支持，查询用户，
        /// </summary>
        /// <param name="term">输入内容</param>
        /// <returns></returns>
        public StringBuilder QuickSearchUserBykeyword(string term)
        {
            return QuickSearchUserBykeyword(term, string.Empty);
        }

        /// <summary>
        /// Autocomplete功能提供查询支持，查询用户，
        /// Added By Linzz In 2014-03-28;
        /// 添加对组织代码的约束条件.
        /// </summary>
        /// <param name="term">输入内容</param>
        /// <param name="orgId">组织代码</param>
        /// <returns></returns>
        public StringBuilder QuickSearchUserBykeyword(string term, string orgCode, bool IsNullSelectAll = false)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(term) && !IsNullSelectAll) return sb.Append("[]");

            IList<Hashtable> userList = DBProvider.UserDAO.SearchByNameJobNumb(term.Trim(), orgCode);
            sb.Append("[");
            foreach (Hashtable ht in userList)
            {
                sb.Append("{");
                sb.AppendFormat("\"value\":\"{0}\"", ht["UserId"]);
                sb.AppendFormat(",\"label\":\"{0},{1},{2}\"", ht["UserName"], ht["JobNo"], ht["OrgName"]);
                sb.AppendFormat(",\"desc\":\"{0}\"", ht["BelongDepart"]);

                sb.Append("},");
            }
            if (userList.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb;
        }


        /// <summary>
        /// Autocomplete功能提供查询支持，查询用户，
        /// Added By Linzz In 2014-05-04;
        /// 添加对组织代码的约束条件.
        /// </summary>
        /// <param name="term">输入内容</param>
        /// <param name="orgId">组织代码</param>
        /// <returns></returns>
        public StringBuilder QuickSearchUser4MsgBykeyword(string term, string orgCode)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(term)) return sb.Append("[]");

            IList<Hashtable> userList = DBProvider.UserDAO.Search4MsgByNameJobNumb(term.Trim(), orgCode);
            sb.Append("[");
            foreach (Hashtable ht in userList)
            {
                sb.Append("{");
                sb.AppendFormat("\"value\":\"{0}\"", ht["UserId"]);
                sb.AppendFormat(",\"label\":\"{0},{1}\"", ht["UserName"], ht["JobNo"]);
                sb.AppendFormat(",\"desc\":\"{0},{1}\"", ht["Mobile"], ht["Email"]);

                sb.Append("},");
            }
            if (userList.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb;
        }

        /// <summary>
        /// 更新用户的信息
        /// </summary>
        public int UpdateUserModelByUserId(UserModel user)
        {

            return DBProvider.UserDAO.UpdateUserModelByUserId(user);
        }

        /// <summary>
        /// 删除一个用户
        /// </summary>
        public int DeleteUserById(int userId)
        {
            ServiceProvider.UserManageDepartService.DeleteManageDepartByUserId(userId);//用户管理的部门
            ServiceProvider.RoleService.DeleteByUserId(userId);//用户角色
            return DBProvider.UserDAO.DeleteUserById(userId);
        }

        /// <summary>
        /// 判断是否存在相同的工号
        /// </summary>
        public int IsExistjobNo(int userId, string jobNo)
        {
            return DBProvider.UserDAO.IsExistjobNo(userId, jobNo);
        }

        /// <summary>
        /// 新建一个用户
        /// </summary>
        public int SaveUser(UserModel user)
        {
            if (user.UserId == 0)
            {
                user.LoginPwd = ToolsLib.Utility.StrUtils.Encrypt(ToolsLib.Utility.WebUtils.GetSettingsValue("DefaultPassword"), ToolsLib.LibConst.EncryptFormat.SHA1);
                DBProvider.UserDAO.Add(user);
            }
            else
            {
                DBProvider.UserDAO.Update(user);
            }
            return user.UserId;
        }

        public IList<Hashtable> SearchByNameJobNumb(string input)
        {
            return DBProvider.UserDAO.SearchByNameJobNumb(input);
        }

        public int GetUserCountByOrgCode(int orgId)
        {
            //string orgCode = DBProvider.OrgDAO.GetCodeById(orgId);

            //return DefaultMapper.GetMapper().SelectObject<int>("Sys_User.GetUserCountByOrgCode", orgCode);
            return DBProvider.UserDAO.GetUserCountByOrgCode(orgId);
        }
    }
}
