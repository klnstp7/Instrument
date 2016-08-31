using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Business;
using Global.Common.Models;
using System.Text;
using ToolsLib.Utility;
using GRGTCommonUtils;


namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class RoleController : Controller
    {
        //
        // GET: /SysManage/Role/

        public ActionResult Index()
        {
            IList<RoleModel> roleList = ServiceProvider.RoleService.GetAll();
            return View("RoleList", roleList);
        }

        //
        // GET: /SysManage/Role/Details/5

        public ActionResult Details(int roleId)
        {
            RoleModel m = ServiceProvider.RoleService.GetById(roleId);
            if (m == null)
                m = new RoleModel();
            return View("RoleDetail", m);
        }

        //
        // GET: /SysManage/Role/Create

        public string SaveRolePermissions(string permissionId,int roleId)
        {
            ServiceProvider.RoleService.SaveRoleOwnPermission(roleId, permissionId);
            return "";
        }

        public string SaveRoleMenus(string menuId, int roleId)
        {
            ServiceProvider.RoleService.SaveRoleOwnMenu(roleId, menuId);
            return "";
        } 

        /// <summary>
        /// 设置用户角色页面
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult RoleTree(int userId=0)
        {
            ViewBag.RoleList= ServiceProvider.RoleService.GetAllEnabled();
            IList<int> checkedRoleList = ServiceProvider.RoleService.GetByUserId(userId).Select(r=>r.RoleId).ToList();
            ViewBag.CheckRoleList = checkedRoleList;
            return View();
        }

        public ActionResult LoadRole4dhtmlxTree()
        {
            IList<RoleModel> roleList = ServiceProvider.RoleService.GetAllEnabled();
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.RoleService.GetdhtmlxTree(roleList);

            return cr;
        }

        public ActionResult LoadJavascript(int userId)
        {
            StringBuilder js = new StringBuilder();
            try
            {
                //userId = UtilsHelper.Decrypt(userId);
                IList<RoleModel> checkedRole = ServiceProvider.RoleService.GetByUserId(userId);
                if (checkedRole != null)
                {
                    foreach (RoleModel r in checkedRole)
                        js.AppendFormat("tree.setCheck({0},1);{1}", r.RoleId, Environment.NewLine);
                }
                return JavaScript(js.ToString());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        //
        // POST: /SysManage/Role/Edit/5

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

        [HttpPost]
        public ActionResult Save(RoleModel model, FormCollection collection)
        {
            try
            {
                ServiceProvider.RoleService.Save(model);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //
        // GET: /SysManage/Role/Delete/5
 
        public ActionResult Delete(int roleId)
        {
            try
            {
                ServiceProvider.RoleService.DeleteById(roleId);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //
        // POST: /SysManage/Role/Delete/5

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

        public ActionResult DisableRole(int roleId)
        {
            try
            {
                ServiceProvider.RoleService.SetRoleState(roleId, false);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult EnableRole(int roleId)
        {
            try
            {
                ServiceProvider.RoleService.SetRoleState(roleId, true);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}
