using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToolsLib.Utility;
using GRGTCommonUtils;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using Global.Common;
using Global.Common.Models;
using Global.Business;
using System.Text;

namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class SaleController : Controller
    {
        //
        // GET: /SysManage/Sale/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LoadArea4dhtmlxTree(string userId)
        {
            int tempUserId = UtilsHelper.Decrypt2Int(userId);
            IList<ParamItemModel> ownList = ServiceProvider.SalesService.GetSaleOwnAreaByUserId(tempUserId);// Utils.LoginUser.saleAreaList;
            if (LoginHelper.IsSuperAdmin) ownList = Global.Business.ServiceProvider.ParamService.GetByCode(GlobalConstants.SysParamType.SaleAreaCode).itemsList;

            ContentResult cr = new ContentResult();
            cr.ContentType = "text/xml";
            cr.ContentEncoding = Encoding.UTF8;
            cr.Content = ServiceProvider.SalesService.GetOwnSaleAreadhtmlxTree(ownList);

            return cr;
        }

        public ActionResult SaleManageDepart()
        {
            return View();
        }

        public ActionResult LoadOrgJavascript(string userId)
        {
            //userId = UtilsHelper.Decrypt(userId);

            StringBuilder js = new StringBuilder();
            IList<OrgModel> checkedOrg = ServiceProvider.SalesService.GetSaleManageDepartByUserId(int.Parse(userId));
            if (checkedOrg != null)
            {
                foreach (OrgModel item in checkedOrg)
                {
                    IEnumerable<OrgModel> parentOrg = checkedOrg.Where(org => org.OrgCode.IndexOf(item.OrgCode) == 0 && org.OrgCode != item.OrgCode);
                    //当前组织仅仅为叶节点时
                    if (parentOrg.Count() == 0)
                        js.AppendFormat("tree.setCheck('{0}',1);{1}", UtilsHelper.Encrypt(item.OrgId.ToString()), Environment.NewLine);
                }
            }
            return JavaScript(js.ToString());
        }

        public ActionResult SaveManageDepart(string orgCodeCheck, string orgCodePartialCheck, string userId)
        {
            try
            {
                //userId = UtilsHelper.Decrypt(userId);
                if (!string.IsNullOrEmpty(orgCodePartialCheck))
                    orgCodeCheck = orgCodeCheck.Replace(orgCodePartialCheck, "");
                ServiceProvider.SalesService.SaveUserManageDepart(int.Parse(userId), orgCodeCheck);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}
