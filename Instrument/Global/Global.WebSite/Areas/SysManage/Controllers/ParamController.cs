using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Common.Models;
using Global.Business;
using System.Text;
using System.Collections;

namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class ParamController : Controller
    {
        //
        // GET: /SysManage/Param/

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult ParamItemList(int paramId = 0)
        //{
        //    IList<ParamModel> paramList = new List<ParamModel>();
        //    paramList = ServiceProvider.ParamService.GetAll();
        //    ViewBag.ParamItemList = new List<ParamItemModel>();
        //    foreach (ParamModel m in paramList)
        //    {
        //        if (paramId == 0)
        //        {
        //            ViewBag.ParamItemList = m.itemsList;
        //            break;
        //        }
        //        else
        //        {
        //            if (m.ParamId == paramId)
        //            {
        //                ViewBag.ParamItemList = m.itemsList;
        //                break;
        //            }
        //        }
        //    }

        //    return View("ParamItemListPage", ViewBag.ParamItemList);
        //}

        public ActionResult ParamTreePage()
        {
            return View();
        }

        public ActionResult ParamListPage(int paramId)
        {
            IList<ParamModel> paramList = new List<ParamModel>();
            paramList = ServiceProvider.ParamService.GetAll();
            return View("ParamListPage", paramList);
        }

        public ActionResult ParamItemListPage(int paramId)
        {
            IList<ParamItemModel> paramItemList = new List<ParamItemModel>();
            ParamModel m = ServiceProvider.ParamService.GetById(paramId);
            if (m != null)
            {
                if (m.itemsList != null)
                {
                    paramItemList = m.itemsList;
                }
                ViewBag.ParamItemCode = m.ParamCode;
            }
            return View(paramItemList);
        }

        public ActionResult LoadParam4dhtmlxTree()
        {
            IList<ParamModel> paramList = ServiceProvider.ParamService.GetAll();
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.ParamService.GetdhtmlxTree(paramList);
            return cr;
        }

        public ActionResult JudgeParamCode(int paramId,string paramCode)
        {
            bool IsExist = false;
            int count = ServiceProvider.ParamService.IsExistParamCode(paramId, paramCode);
            if (count > 0)
            {
                IsExist = true;
            }
            return Content(IsExist.ToString());
        }

        /// <summary>
        /// 判断是否存在指定的参数项值
        /// </summary>
        /// <param name="paramItemId">要排除比较的参数项的Id（排除与自身比较）</param>
        /// <param name="paramId">参数项所属的分类的Id</param>
        /// <param name="paramItemValue">要比较的值</param>
        /// <returns></returns>
        public ActionResult JudgeParamItemValue(int paramItemId, int paramId, string paramItemValue)
        {
            int count = ServiceProvider.ParamItemService.IsExistParamItemValue(paramItemId, paramId, paramItemValue);
            bool result = count > 0;
            return Content(result.ToString());
        }

        //
        // GET: /SysManage/Param/Details/5
        public ActionResult Details(int paramId)
        {
            ParamModel model = ServiceProvider.ParamService.GetById(paramId);
            if (model == null)
            {
                model = new ParamModel();
            }           
            UpdateModel(model);
            return View("ParamDetail", model);
        }

        public ActionResult ItemDetails(int paramId, int itemId)
        {
            ParamItemModel model = ServiceProvider.ParamItemService.GetById(itemId);
            if (model == null)
            {
                model = new ParamItemModel();
            }
            UpdateModel(model);
            return View("ParamItemDetail", model);
        }
        //
        // GET: /SysManage/Param/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /SysManage/Param/Create

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
        
        //
        // GET: /SysManage/Param/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SysManage/Param/Edit/5

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
        public ActionResult Save(ParamModel model, FormCollection collection)
        {
            try
            {
                ServiceProvider.ParamService.Save(model);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
        [HttpPost]
        public ActionResult SaveItem(ParamItemModel model, FormCollection collection)
        {
            return Content(ServiceProvider.ParamItemService.SaveParamItem(model));
        }


        //
        // GET: /SysManage/Param/Delete/5
 
        public ActionResult Delete(int paramId)
        {
            try
            {
                ServiceProvider.ParamService.DeleteStatusById(paramId);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            

            //return RedirectToAction("ParamListPage", new { paramId = 0 });
        }

        //删除参数明细项
        public ActionResult ItemDelete(int paramId, int itemId,string itemValue)
        {

            return Content(ServiceProvider.ParamItemService.DeleteParamItem(itemId, itemValue));
            //return RedirectToAction("ParamItemListPage", new { paramId = paramId });
        }
        //
        // POST: /SysManage/Param/Delete/5

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
