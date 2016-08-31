using System;
using System.Web;
using System.Web.Mvc;
using Global.Common;
using Global.Business;
using Global.Common.Models;
using System.Collections;
using ToolsLib.IBatisNet;
using System.Text;
using GRGTCommonUtils;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ToolsLib.FileService;
using ToolsLib.Utility.Jquery;
using ToolsLib.Utility;

namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class EmployeeController : Controller
    {
        //
        // GET: /SysManage/Exmployee/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string userId, int orgId = 1)
        {
            UserModel userModel = null;
            int user_id = 0;

            //解密处理userid
            if (!string.IsNullOrWhiteSpace(userId) && userId != "0")
            {
                //userId = UtilsHelper.Decrypt(userId);
                user_id = int.Parse(userId);
                userModel = ServiceProvider.UserService.GetById(user_id);
            }

            if (userModel == null)
            {
                userModel = new UserModel();
                userModel.BelongDepart = ServiceProvider.OrgService.GetCodeById(orgId);
            }

            //部门...........生成一个下拉框树所需的数据源
            ViewBag.OrgList = ServiceProvider.OrgService.GetAll();

            ////员工状态
            //ParamModel paramModel = ServiceProvider.ParamService.GetByCode(GlobalConstants.SysParamType.EmployeeState);
            ParamModel paramModel = ServiceProvider.ParamService.GetByCode(GlobalConstants.SysParamType.EmployeeState);
            ViewBag.EmployeeStateList = new SelectList(paramModel.itemsList, "ParamItemValue", "ParamItemName", userModel.EmployeeState);

            if (user_id != 0) ViewBag.JobNumb = ServiceProvider.UserService.GetById(user_id).UserName;

            return View("Details", userModel);
        }

        public ActionResult Save(UserModel user, FormCollection collection)
        {
            //UserModel user = new UserModel();
            user.UserName = collection["UserName"];
            user.Sex = collection["Sex"].Equals("True");
            user.JobNo = collection["JobNo"];
            user.BelongDepart = collection["OrgName"];
            user.DepartName = ServiceProvider.OrgService.GetByCode(user.BelongDepart).OrgName;
            user.Duty = collection["Duty"];
            user.Mobile1 = collection["Mobile1"];
            user.Email1 = collection["Email1"];
            user.EmployeeState = int.Parse(collection["EmployeeState"]);
            ServiceProvider.UserService.SaveUser(user);       //新增或修改员工
            return Content("OK");
        }


        public ActionResult JudgeJobNumb(int userId, string jobNo)
        {
            bool IsExist = false;
            int count = ServiceProvider.UserService.IsExistjobNo(userId, jobNo);
            if (count > 0)
            {
                IsExist = true;
            }
            return Content(IsExist.ToString());
        }
    }


}
