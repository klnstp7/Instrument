using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;


namespace Global.Business
{
    public class MenuServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int MenuId)
        {
            DBProvider.MenuDAO.DeleteById(MenuId);//删除系统菜单
            ServiceProvider.RoleService.DeleteMenusByMenuId(MenuId);//删除角色拥有的该菜单
            IList<MenuModel> list = DBProvider.MenuDAO.GetByParentMenuID(MenuId);
            foreach (MenuModel m in list)
            {
                DeleteById(m.MenuId);
            }
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(MenuModel model)
        {
            if (model.MenuId == 0) DBProvider.MenuDAO.Add(model);
            else DBProvider.MenuDAO.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public MenuModel GetById(int MenuId)
        {
            return DBProvider.MenuDAO.GetById(MenuId);
        }

        public IList<MenuModel> GetByRoleId(int roleId)
        {
            return DBProvider.MenuDAO.GetByRoleId(roleId);
        }

        public string GetdhtmlxTree(IList<MenuModel> list, int roleId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.Append("<tree id='0'>" + Environment.NewLine);
            string msg = "<item text='{0}' id='{1}' open='1'>{2}";
            sb.AppendFormat(msg, "系统菜单", -1, Environment.NewLine);
            RecursiveDump(list, 0, 2, sb);
            sb.Append("</item>");            
            sb.Append("</tree>");

            return sb.ToString();
        }

        private void RecursiveDump(IList<MenuModel> list, int parentId, int level, StringBuilder sb)
        {
            IEnumerable<MenuModel> tempOrg = list.Where<MenuModel>(m => m.ParentMenuId == parentId);

            string msg = "";
            foreach (MenuModel m in tempOrg)
            {
                msg = "<item text='{0}' id='{1}' open='1'>{2}";
                //Alte by Reven 2014-08-04   更改被禁用的菜单显示名称
                if (!m.IsEnabled)
                {
                    m.MenuName = string.Format("{0}(已禁用)", m.MenuName);
                }
                sb.AppendFormat(msg, m.MenuName, m.MenuId, Environment.NewLine);
                RecursiveDump(list, m.MenuId, level + 1, sb);
                sb.Append("</item>");
            }
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<MenuModel> GetAll()
        {
            return DBProvider.MenuDAO.GetAll();
        }
    }
}
