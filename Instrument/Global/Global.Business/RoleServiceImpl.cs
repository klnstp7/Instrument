using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;


namespace Global.Business
{
    public class RoleServiceImpl
    {


        public void DeleteByUserId(int UserId)
        {
            DBProvider.RoleDAO.DeleteByUserId(UserId);
        }
        public void DeleteByRoleId(int RoleId)
        {
            DBProvider.RoleDAO.DeleteByRoleId(RoleId);
        }
        public void DeleteMenuByRoleId(int RoleId)
        {
            DBProvider.RoleDAO.DeleteMenuByRoleId(RoleId);
        }
        public void DeletePermissionByRoleId(int RoleId)
        {
            DBProvider.RoleDAO.DeletePermissionByRoleId(RoleId);
        }
        public void DeleteMenusByMenuId(int MenuId)
        {
            DBProvider.RoleDAO.DeleteMenusByMenuId(MenuId);
        }
        public void DeletePermissionByPermissionId(int PermissionId)
        {
            DBProvider.RoleDAO.DeletePermissionByPermissionId(PermissionId);
        }
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int RoleId)
        {
            RoleModel roleModel = DBProvider.RoleDAO.GetById(RoleId);

            //普通用户角色不能删除
            if (roleModel.RoleName == "普通用户")
            {
                throw new Exception("普通用户角色可禁用，但不能删除！");
            }
            DeleteByRoleId(RoleId);//用户拥有的角色
            DeleteMenuByRoleId(RoleId);//角色拥有的菜单
            DeletePermissionByRoleId(RoleId);//角色拥有的权限点
            DBProvider.RoleDAO.DeleteById(RoleId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(RoleModel model)
        {
            if (model.RoleId == 0) DBProvider.RoleDAO.Add(model);
            else DBProvider.RoleDAO.Update(model);
        }

        public void SaveRoleOwnPermission(int roleId, string permissionId)
        {
            if (string.IsNullOrEmpty(permissionId))
            {
                DBProvider.RoleDAO.DeletePermissionByRoleId(roleId);
                return;
            }

            DBProvider.RoleDAO.DeletePermissionByRoleId(roleId);
            string[] idArray = permissionId.Split(',');
            for (int i = 0; i < idArray.Length; i++)
            {
                if (string.IsNullOrEmpty(idArray[i])) break;
                DBProvider.RoleDAO.AddPermission(roleId, Convert.ToInt16(idArray[i]));
            }
        }

        public void SaveRoleOwnMenu(int roleId, string menuId)
        {
            if (string.IsNullOrEmpty(menuId)) return;

            DBProvider.RoleDAO.DeleteMenuByRoleId(roleId);
            string[] idArray = menuId.Split(',');
            for (int i = 0; i < idArray.Length; i++)
            {
                if (string.IsNullOrEmpty(idArray[i])) break;
                DBProvider.RoleDAO.AddMenu(roleId, Convert.ToInt16(idArray[i]));
            }
        }

        public void SetRoleState(int roleId, bool state)
        {
            DBProvider.RoleDAO.SetRoleState(roleId, state);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public RoleModel GetById(int RoleId)
        {
            return DBProvider.RoleDAO.GetById(RoleId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<RoleModel> GetAll()
        {
            return DBProvider.RoleDAO.GetAll();
        }

        public IList<RoleModel> GetAllEnabled()
        {
            return DBProvider.RoleDAO.GetAllEnabled();
        }

        public IList<RoleModel> GetByUserId(int userId)
        {
            return DBProvider.RoleDAO.GetByUserId(userId);
        }

        public string GetdhtmlxTree(IList<RoleModel> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.Append("<tree id='0'>" + Environment.NewLine);

            string msg = "";
            foreach (RoleModel role in list)
            {
                msg = "<item text='{0}' id='{1}'></item>{2}";
                sb.AppendFormat(msg, role.RoleName, role.RoleId, Environment.NewLine);
            }
            sb.Append(Environment.NewLine + "</tree>");

            return sb.ToString();
        }
    }
}
