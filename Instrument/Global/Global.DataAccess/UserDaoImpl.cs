using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using ToolsLib.IBatisNet;
using System.Collections;


namespace Global.DataAccess
{
    public class UserDaoImpl
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>       
        public void AddUserRoles(int userId, int roleId)
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserID", userId);
            ht.Add("RoleID", roleId);

            DBProvider.dbMapper.Insert("Sys_User.InsertUserRoles", ht);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>       
        public void DeleteRolesByUserId(int UserId)
        {
            DBProvider.dbMapper.Delete("Sys_User.DeleteRolesByUserId", UserId);
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public UserModel GetById(int UserId)
        {
            return DBProvider.dbMapper.SelectObject<UserModel>("Sys_User.GetByID", UserId);
        }

        /// <summary>
        /// 通过ID集合或工号集合得到对象实体集合.
        /// </summary>
        public IList<UserModel> GetByIdsOrJobNos(IList<int> IdList, IList<string> JobNoList)
        {
            Hashtable ht = new Hashtable();
            ht.Add("IdList", IdList);
            ht.Add("JobNoList", JobNoList);
            return DBProvider.dbMapper.SelectList<UserModel>("Sys_User.GetByIdsOrJobNos", ht);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public UserModel GetByLoginName(string JobNo)
        {
            return DBProvider.dbMapper.SelectObject<UserModel>("Sys_User.GetByLoginName", JobNo);
        }

        /// <summary>
        /// 获得所有记录
        /// </summary>
        public IList<UserModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<UserModel>("Sys_User.GetAll");
        }

        public IList<Hashtable> GetByOrgId(int orgId)
        {
            string code = DBProvider.OrgDAO.GetCodeById(orgId);

            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_User.GetByOrgCode", code);
        }

        /// <summary>
        /// 根据组织Id获取用户
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns></returns>
        public IList<UserModel> GetListByOrgId(int orgId)
        {
            string code = DBProvider.OrgDAO.GetCodeById(orgId);

            return DBProvider.dbMapper.SelectList<UserModel>("Sys_User.GetByOrgCode", code);
        }

        /// <summary>
        /// 获取所有用户角色
        /// </summary>
        /// <returns></returns>
        public IList<Hashtable> GetAllUserRole()
        {
            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_User.GetAllUserRole");
        }



        /// <summary>
        /// 根据工号或姓名模糊查找
        /// </summary>
        /// <param name="input">搜索条件</param>
        /// <param name="orgCode">组织编码</param>
        /// <returns></returns>
        public IList<Hashtable> SearchByNameJobNumb(string input,string orgCode)
        {
            UserModel user = new UserModel { UserName = input, JobNo = input };
            if (string.IsNullOrWhiteSpace(orgCode)) user.BelongDepart = string.Empty;
            else
            {
                StringBuilder whereBuilder = new StringBuilder();
                string[] orgList = orgCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string orStr = string.Empty;
                foreach(string org in orgList)
                {
                    whereBuilder.AppendFormat("{0}DepartName Like '%{1}%'", orStr, org);
                    orStr = " OR ";
                }
                user.BelongDepart = whereBuilder.ToString();
            }

            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_User.SearchByNameJobNumb", user);
        }

        /// <summary>
        /// 根据工号或姓名模糊查找通讯信息
        /// </summary>
        /// <param name="input">搜索条件</param>
        /// <param name="orgCode">组织编码</param>
        /// <returns></returns>
        public IList<Hashtable> Search4MsgByNameJobNumb(string input, string orgCode)
        {
            UserModel user = new UserModel { UserName = input, JobNo = input };
            if (string.IsNullOrWhiteSpace(orgCode)) user.BelongDepart = string.Empty;
            else
            {
                StringBuilder whereBuilder = new StringBuilder();
                string[] orgList = orgCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string orStr = string.Empty;
                foreach (string org in orgList)
                {
                    whereBuilder.AppendFormat("{0}DepartName Like '%{1}%'", orStr, org);
                    orStr = " OR ";
                }
                user.BelongDepart = whereBuilder.ToString();
            }

            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_User.Search4MsgByNameJobNumb", user);
        }

        
        /// <summary>
        /// 根据工号或姓名模糊查找
        /// </summary>
        /// <param name="input">传入用户名[UserName]及组织代码[BelongDepart]的HASHTABLE集合</param>
        /// <returns></returns>
        //public IList<Hashtable> SearchByNameJobNumbAndOrgId(Hashtable input)
        //{
        //    return DBProvider.dbMapper.SelectList<Hashtable>("Sys_User.SearchByNameJobNumbAndOrgId", input);
        //}

        public virtual IList<Hashtable> GetListForPaging(PagingModel paging)
        {
            paging.TableName = "Sys_User";
            paging.FieldKey = "UserId";
            paging.FieldShow = "UserId,UserName,DepartName,Duty,JobNo,Mobile1,Email1,Email2,EmployeeState,Sex";
            paging.FieldOrder = "DepartName";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

        /// <summary>
        /// 更新用户的信息
        /// </summary>
        public int UpdateUserModelByUserId( UserModel user) {

            return DBProvider.dbMapper.Update("Sys_User.UpdateUserModelByUserId", user);
        }

            /// <summary>
        /// 删除一个用户
        /// </summary>
        public int DeleteUserById(int userId)
        {

            return DBProvider.dbMapper.Delete("Sys_User.DeleteUserByUserId", userId);
        }

        /// <summary>
        /// 判断是否存在相同的工号
        /// </summary>
        public int IsExistjobNo(int userId, string JobNo)
        {
            UserModel u = new UserModel { UserId = userId, JobNo = JobNo };
            return DBProvider.dbMapper.SelectObject<int>("Sys_User.IsExistJobNo", u);
        }

        /// <summary>
        /// 增加一个用户
        /// </summary>
        public void Add(UserModel user)
        {
            DBProvider.dbMapper.Insert("Sys_User.SaveUser", user);
        }

        /// <summary>
        /// 跟新一个用户
        /// </summary>
        public int Update(UserModel user)
        {
            return DBProvider.dbMapper.Update("Sys_User.UpdateUserModel", user);
        }

        public IList<Hashtable> SearchByNameJobNumb(string input)
        {
            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_User.SearchByNameJobNumb2", input);
        }

        public int GetUserCountByOrgCode(int orgId)
        {
            string orgCode = DBProvider.OrgDAO.GetCodeById(orgId);
            return DBProvider.dbMapper.SelectObject<int>("Sys_User.GetUserCountByOrgCode", orgCode);
        }

    }
}
