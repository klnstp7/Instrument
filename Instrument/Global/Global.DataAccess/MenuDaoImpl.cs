using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;


namespace Global.DataAccess
{
    public class MenuDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(MenuModel model)
        {
            DBProvider.dbMapper.Insert("Sys_Menu.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(MenuModel model)
        {
            DBProvider.dbMapper.Update("Sys_Menu.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int MenuId)
        {
            DBProvider.dbMapper.Delete("Sys_Menu.DeleteById", MenuId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public MenuModel GetById(int MenuId)
        {
            return DBProvider.dbMapper.SelectObject<MenuModel>("Sys_Menu.GetByID", MenuId);
        }

        public IList<MenuModel> GetByRoleId(int roleId)
        {
            return DBProvider.dbMapper.SelectList<MenuModel>("Sys_Menu.GetByRoleId", roleId);
        }

        /// <summary>
        /// 获取直接子菜单集合
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public IList<MenuModel> GetByParentMenuID(int menuId)
        {
            return DBProvider.dbMapper.SelectList<MenuModel>("Sys_Menu.GetByParentMenuID", menuId);
        }
        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<MenuModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<MenuModel>("Sys_Menu.GetAll");
        }
    }
}
