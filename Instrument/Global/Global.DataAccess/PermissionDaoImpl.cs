using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;


namespace Global.DataAccess
{
    public class PermissionDaoImpl
    {

        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(PermissionModel model)
        {
            DBProvider.dbMapper.Insert("Sys_Permission.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(PermissionModel model)
        {
            DBProvider.dbMapper.Update("Sys_Permission.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int PermissionId)
        {
            DBProvider.dbMapper.Delete("Sys_Permission.DeleteById", PermissionId);
        }


        

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public PermissionModel GetById(int PermissionId)
        {
            return DBProvider.dbMapper.SelectObject<PermissionModel>("Sys_Permission.GetByID", PermissionId);
        }

        public IList<PermissionModel> GetByRoleId(int roleId)
        {
            return DBProvider.dbMapper.SelectList<PermissionModel>("Sys_Permission.GetByRoleID", roleId);
        }

        /// <summary>
        /// 获取直接子权限点集合
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public IList<PermissionModel> GetByParentPermissionID(int permissionId)
        {
            return DBProvider.dbMapper.SelectList<PermissionModel>("Sys_Permission.GetByParentPermissionID", permissionId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<PermissionModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<PermissionModel>("Sys_Permission.GetAll");
        }
    }
}
