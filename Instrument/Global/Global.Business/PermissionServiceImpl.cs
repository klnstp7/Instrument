using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;


namespace Global.Business
{
    public class PermissionServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int PermissionId)
        {
            DBProvider.PermissionDAO.DeleteById(PermissionId);
            ServiceProvider.RoleService.DeletePermissionByPermissionId(PermissionId);//删除角色拥有的该权限点
            IList<PermissionModel> list = DBProvider.PermissionDAO.GetByParentPermissionID(PermissionId);
            foreach (PermissionModel m in list)
            {
                DeleteById(m.PermissionId);
            }
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(PermissionModel model)
        {
            if (model.PermissionId == 0) DBProvider.PermissionDAO.Add(model);
            else DBProvider.PermissionDAO.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public PermissionModel GetById(int PermissionId)
        {
            return DBProvider.PermissionDAO.GetById(PermissionId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<PermissionModel> GetAll()
        {
            return DBProvider.PermissionDAO.GetAll();
        }

        public IList<PermissionModel> GetByRoleId(int roleId)
        {
            return DBProvider.PermissionDAO.GetByRoleId(roleId);
        }

        ///// <summary>
        ///// 获取直接子权限点集合
        ///// </summary>
        ///// <param name="permissionId"></param>
        ///// <returns></returns>
        //public IList<PermissionModel> GetByParentPermissionID(int permissionId)
        //{
        //    return DBProvider.PermissionDAO.GetByParentPermissionID(permissionId);
        //}


        public string GetdhtmlxTree(IList<PermissionModel> list,int roleId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.Append("<tree id='0'>" + Environment.NewLine);
            RecursiveDump(list, 0, 1, sb);
            sb.Append("</tree>");

            return sb.ToString();
        }

        private void RecursiveDump(IList<PermissionModel> list, int parentId, int level, StringBuilder sb)
        {
            IEnumerable<PermissionModel> tempOrg = list.Where<PermissionModel>(m => m.ParentPermissionId == parentId);

            string msg = "";
            foreach (PermissionModel m in tempOrg)
            {
                msg = "<item text='{0}' id='{1}' open='1'>{2}";

                sb.AppendFormat(msg, m.PermissionName, m.PermissionId, Environment.NewLine);
                RecursiveDump(list, m.PermissionId, level + 1, sb);
                sb.Append("</item>");
            }
        }
    }
}
