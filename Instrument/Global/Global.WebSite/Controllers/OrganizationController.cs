using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ToolsLib.Utility;
using ToolsLib.Utility.Jquery;
using System.Collections;
using Global.Business;
using Global.Common.Models;
using GRGTCommonUtils;
using Global.Common;

namespace Global.WebSite.Controllers
{
    public class OrganizationController : Controller
    {
        //
        // GET: /Organization/

        public ActionResult Index()
        {
            ViewBag.orgId = UtilsHelper.Encrypt("1");
            return View();
        }

        public ActionResult OrgTree()
        {
            IList<Hashtable> orgList = ServiceProvider.OrgService.GetByParentId(1);
            return View(orgList);
        }

        public ActionResult OrgTreePage()
        {
            return View();
        }

        public ActionResult OrgListPage(string orgId, string orgName)
        {
            //"Z0pic1pKNFlYK0U9 "
            IList<Hashtable> orgList = ServiceProvider.OrgService.GetByParentId(Convert.ToInt32(UtilsHelper.Decrypt(orgId)));
            //IList<Hashtable> orgList = ServiceProvider.OrgService.GetByParentId(StrUtils.ToInt32(orgId, 0));
            return View(orgList);
        }


        public ActionResult LoadOrg4dhtmlxTree()
        {
            IList<OrgModel> orgList = ServiceProvider.OrgService.GetAll();
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.OrgService.GetdhtmlxTree(orgList);

            return cr;
        }

        public ActionResult BelongOrgTree()
        {
            //IList<Hashtable> orgList = ServiceProvider.OrgService.GetByParentId(1);
            return View("OrgTree");
        }

        public ActionResult LoadBelongOrg4dhtmlxTree()
        {
            IList<UserManageDepartModel> departList = new List<UserManageDepartModel>();
            if (LoginHelper.LoginUserAuthorize.Contains("Instrument-CheckAll".ToLower()) || LoginHelper.LoginUserAuthorize.Contains("Instrument-SearchCheckAll".ToLower()))
                departList = ServiceProvider.UserManageDepartService.GetAll();
            else
                departList = LoginHelper.LoginUser.manageDepartList;
            IList<OrgModel> orgList = ServiceProvider.OrgService.GetByOrgIds(departList.Select(d => d.OrgId).ToList());
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.OrgService.GetParentdhtmlxTree(orgList);

            return cr;
        }


        public ActionResult BelongParamTree()
        {
            //IList<Hashtable> orgList = ServiceProvider.OrgService.GetByParentId(1);
            return View("ParamTree");
        }

        public ActionResult LoadBelongParam4dhtmlxTree()
        {
            ParamModel paramModel = new ParamModel();
            paramModel = Global.Business.ServiceProvider.ParamService.GetByCode(Instrument.Common.Constants.SysParamType.DocumentType);
            IList<ParamItemModel> paramlist = Global.Business.ServiceProvider.ParamItemService.GetByParamID(paramModel.ParamId);
            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            //cr.Content = ServiceProvider.ParamItemService.GetParamdhtmlxTree(paramlist,paramModel.ParamId);
            cr.Content = ServiceProvider.ParamItemService.GetdhtmlxTree(paramlist);

            return cr;
        }

        //
        // GET: /Organization/Details/5

        public ActionResult Details(int orgId, string parentId)
        {
            //int _orgid = Convert.ToInt32(UtilsHelper.Decrypt(orgId));
            int _parentId = UtilsHelper.Decrypt2Int(parentId);
            OrgModel thisOrg = new OrgModel { ParentOrgId = _parentId };
            OrgModel parentOrg = ServiceProvider.OrgService.GetById(_parentId);
            if (parentOrg == null) parentOrg = new OrgModel();
            if (orgId == 0)
                thisOrg.OrgCode = ServiceProvider.OrgService.BuildSubOrgCode(_parentId);//自动填充子组织编号
            else
                thisOrg = ServiceProvider.OrgService.GetById(orgId);
       
            ViewBag.ParentOrg = parentOrg;

            return View("OrgDetail", thisOrg);





            //Convert.ToInt32(UtilsHelper.Decrypt(orgId))
            //OrgModel ParentOrg = ServiceProvider.OrgService.GetById(Convert.ToInt32(UtilsHelper.Decrypt(parentId.ToString())));
            //OrgModel thisOrg = ServiceProvider.OrgService.GetById(orgId);

            //以下为获取部门领导名称

            //return View("OrgDetail", thisOrg);
        }


        //
        // POST: /Organization/Create

        [HttpPost]
        public ActionResult Save(OrgModel org, FormCollection collection)
        {
            try
            {
                
                //org.AppendInfo.OfficeAddress = collection["OfficeAddress"];
                //org.AppendInfo.OfficeFax = collection["OfficeFax"];
                //org.AppendInfo.OfficeTel = collection["OfficeTel"];
                //org.AppendInfo.BusinessType = Convert.ToInt16(collection["BusinessType"]);
                //org.AppendInfo.OrgLeader = Convert.ToInt16(collection["OrgLeader"]);
                //org.AppendInfo.OrgType = int.Parse(collection["OrgType"]);

                ServiceProvider.OrgService.Save(org);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult DisableOrg(int orgId)
        {
            ServiceProvider.OrgService.SetOrgState(orgId, false);
            return Content("OK");
        }

        public ActionResult EnableOrg(int orgId)
        {
            ServiceProvider.OrgService.SetOrgState(orgId, true);
            return Content("OK");
        }

        //
        // GET: /Organization/Delete/5

        public ActionResult Delete(int orgId)
        {
            string msg = "OK";
            int count = ServiceProvider.UserService.GetUserCountByOrgCode(orgId);

            if (count == 0)
            {
                ServiceProvider.OrgService.DeleteById(orgId);
            }
            else
            {
                msg = string.Format("删除失败，有【{0}】名员工归属该部门及其下属部门。", count);
            }

            return Content(msg);
        }

        public ActionResult JudgeOrgCode(int orgId, string orgCode)
        {
            bool IsExist = false;
            return Content(IsExist.ToString());
        }
    }


}
