using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Common.Models;
using Global.Business;
using System.Text;
using ToolsLib.Utility;


namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class PermissionController : Controller
    {
        //
        // GET: /SysManage/Permission/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PermissionTreePage()
        {
            return View();
        }

        //
        // GET: /SysManage/Permission/Details/5

        public ActionResult Details(int permId)
        {
            PermissionModel p = ServiceProvider.PermissionService.GetById(permId);
            if (p != null)
            {
                PermissionModel pp = new PermissionModel();
                pp = ServiceProvider.PermissionService.GetById(p.ParentPermissionId);
                if (pp != null)
                    ViewBag.ParentPermissionName = pp.PermissionName;
            }
            else
            {
                p = new PermissionModel();
            }
            return View("PermissionDetailPage",p);
        }

        //
        // GET: /SysManage/Permission/Create

        public ActionResult RoleOwnPermission()
        {
            return View();
        }

        public ActionResult LoadPermission4dhtmlxTree(int roleId)
        {
            IList<PermissionModel> permissionList = ServiceProvider.PermissionService.GetAll();
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.PermissionService.GetdhtmlxTree(permissionList,roleId);

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
            IList<PermissionModel> checkedPermission = ServiceProvider.PermissionService.GetByRoleId(roleId);
            if (checkedPermission != null)
            {
                foreach (PermissionModel item in checkedPermission)
                {
                    IEnumerable<PermissionModel> parentPermission = checkedPermission.Where(permission => permission.ParentPermissionId == item.PermissionId);
                    if (0 == parentPermission.Count())
                    {
                        js.AppendFormat("tree.setCheck({0},1);{1}", item.PermissionId, Environment.NewLine);
                    }
                }
            }
            return JavaScript(js.ToString());
        }

        //
        // POST: /SysManage/Permission/Create

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
        public ActionResult Save(string done,PermissionModel m, FormCollection collection)
        {
            try
            {
                if (done == "add")
                {
                    m.ParentPermissionId = m.PermissionId;
                    m.PermissionId = 0;
                }
                ServiceProvider.PermissionService.Save(m);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
        
        //
        // GET: /SysManage/Permission/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SysManage/Permission/Edit/5

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
        // GET: /SysManage/Permission/Delete/5

        public ActionResult Delete(int permId)
        {
            try
            {
                ServiceProvider.PermissionService.DeleteById(permId);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //
        // POST: /SysManage/Permission/Delete/5

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
