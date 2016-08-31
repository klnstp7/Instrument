using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;


namespace Global.DataAccess
{
    public class RoleDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(RoleModel model)
        {
            DBProvider.dbMapper.Insert("Sys_Role.Insert", model);
        }

        public void AddPermission(int roleId, int permissionId)
        {
            if (permissionId == -1)
                return;
            Hashtable ht = new Hashtable();
            ht.Add("RoleId", roleId);
            ht.Add("PermissionId", permissionId);

            DBProvider.dbMapper.Insert("Sys_Role.InsertPermission", ht);
        }

        public void AddMenu(int roleId, int menuId)
        {
            if (menuId == -1)
                return;
            Hashtable ht = new Hashtable();
            ht.Add("RoleId", roleId);
            ht.Add("MenuId", menuId);

            DBProvider.dbMapper.Insert("Sys_Role.InsertMenu", ht);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(RoleModel model)
        {
            DBProvider.dbMapper.Update("Sys_Role.Update", model);
        }

        public void SetRoleState(int roleId, bool state)
        {
            RoleModel r = new RoleModel { RoleId = roleId, IsEnabled = state };
            DBProvider.dbMapper.Update("Sys_Role.SetRoleState", r);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int RoleId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeleteById", RoleId);
        }

        public void DeletePermissionByRoleId(int roleId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeletePermissionByRoleId", roleId);
        }

        public void DeleteMenuByRoleId(int roleId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeleteMenuByRoleId", roleId);
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public RoleModel GetById(int RoleId)
        {
            return DBProvider.dbMapper.SelectObject<RoleModel>("Sys_Role.GetByID", RoleId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<RoleModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<RoleModel>("Sys_Role.GetAll");
        }

        public IList<RoleModel> GetAllEnabled()
        {
            return DBProvider.dbMapper.SelectList<RoleModel>("Sys_Role.GetAllEnabled");
        }

        public IList<RoleModel> GetByUserId(int userId)
        {
            return DBProvider.dbMapper.SelectList<RoleModel>("Sys_Role.GetByUserID", userId);
        }

        public void DeleteByUserId(int UserId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeleteByUserId", UserId);
        }

        public void DeleteByRoleId(int RoleId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeleteByRoleId", RoleId);
        }
        public void DeleteMenusByMenuId(int MenuId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeleteMenuByMenuId", MenuId);
        }
        public void DeletePermissionByPermissionId(int PermissionId)
        {
            DBProvider.dbMapper.Delete("Sys_Role.DeletePermissionByPermissionId", PermissionId);
        }
    }
}
