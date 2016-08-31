using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Business;
using Global.Common.Models;
using System.Text;
using ToolsLib.Utility;


namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class MenuController : Controller
    {
        //
        // GET: /SysManage/Menu/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuTreePage()
        {
            return View();
        }

        // GET: /SysManage/Menu/Details/5
        public ActionResult Details(int menuId)
        {
            MenuModel m = ServiceProvider.MenuService.GetById(menuId);
            if (m != null)
            {
                MenuModel pm = new MenuModel();
                //父级菜单
                pm = ServiceProvider.MenuService.GetById(m.ParentMenuId);
                if (pm != null)
                    ViewBag.ParentMenuName = pm.MenuName;
            }
            else
            {
                m = new MenuModel();
            }
            return View("MenuDetailPage", m);
        }

        public ActionResult RoleOwnMenu()
        {
            return View();
        } 

        public ActionResult LoadMenu4dhtmlxTree(int roleId=0)
        {
            IList<MenuModel> menuList = ServiceProvider.MenuService.GetAll();
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.MenuService.GetdhtmlxTree(menuList, roleId);

            return cr;
        }

        /// <summary>
        /// 设置dhtmlxTree中未被选中的checkbox
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ActionResult LoadJavascript(int roleId)
        {
            StringBuilder js = new StringBuilder();
            IList<MenuModel> checkedMenu = ServiceProvider.MenuService.GetByRoleId(roleId);
            if (checkedMenu != null)
            {
                foreach (MenuModel item in checkedMenu)
                {
                    IEnumerable<MenuModel> parentMenu = checkedMenu.Where(menu => menu.ParentMenuId == item.MenuId);
                    if (0 == parentMenu.Count())
                    {
                        js.AppendFormat("tree.setCheck({0},1);{1}", item.MenuId, Environment.NewLine);
                    }
                }
            }
            return JavaScript(js.ToString());
        }

        //
        // POST: /SysManage/Menu/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Save(string done,MenuModel m, FormCollection collection)
        {
            try
            {
                if (done == "add")
                {
                    m.ParentMenuId = m.MenuId;
                    m.MenuId = 0;                    
                }         
                ServiceProvider.MenuService.Save(m);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //
        // GET: /SysManage/Menu/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SysManage/Menu/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SysManage/Menu/Delete/5

        public ActionResult Delete(int menuId)
        {
            try
            {
                ServiceProvider.MenuService.DeleteById(menuId);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //
        // POST: /SysManage/Menu/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
