using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Instrument.Common.Models;
using Instrument.Business;
using GRGTCommonUtils;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using Global.Common;
using ToolsLib.Utility;
using System.Text;
using System.Collections;
using System.Data;
using Global.Common.Models;
using Instrument.Common;

namespace Instrument.WebSite.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompanyInformation()
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            ParamModel model = Global.Business.ServiceProvider.ParamService.GetAll().SingleOrDefault(p=>p.ParamCode == Constants.SysParamType.CompanyInfo);

            foreach (ParamItemModel item in model.itemsList)
            {
                if (count % 2 == 0)
                    sb.AppendFormat("<tr>");

                sb.AppendFormat("<th style='width: 200px;'>{0}</th>", item.ParamItemName);
                sb.AppendFormat("<td>{0}</td>", item.ParamItemValue);
                count++;
                if (count % 2 == 0 && count > 0)
                    sb.AppendFormat("</tr>");
            }
            if (count % 2 > 0)
            {
                sb.AppendFormat("<th style='width: 200px;'></th>");
                sb.AppendFormat("<td></td>");
                sb.AppendFormat("</tr>");
            }

            //IList<CompanyModel> list = ServiceProvider.CompanyService.GetAll();
            //CompanyModel companyModel = null;
            //if (null != list && list.Count > 0)
            //{
            //    companyModel = (CompanyModel)list[0];
            //}
            ViewBag.CompanyInfo = sb.ToString();
            return View();
        }       
    }
}
