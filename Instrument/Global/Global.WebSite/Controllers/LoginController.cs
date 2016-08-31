using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections;
using System.Text;
//using GRGTCommonUtils.WS;
using GRGTCommonUtils;
using System.Configuration;
using ToolsLib.Utility;
using Global.Business;
using Global.Common.Models;
using System.Web.UI;

namespace Global.WebSite.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            LoginHelper.RememberMe(this, WebUtils.GetSettingsValue("SystemCookieName"));
            return View();
        }

        [HttpPost]
        public string Login(FormCollection collection)
        {
            string JobNo = collection["JobNo"];
            string loginPwd = collection["LoginPwd"];

            string errMsg = "OK";
            bool IsSSOLogin = Convert.ToBoolean(WebUtils.GetSettingsValue("IsSSOLogin"));
            //单点登录
            if (IsSSOLogin)
            {
                errMsg = GRGTCommonUtils.WSProvider.HRProvider.Login(JobNo, loginPwd, string.Empty);
                if (errMsg != "OK") return errMsg;
            }
            UserModel user = ServiceProvider.UserService.GetByLoginName(JobNo);
            if (user == null) return "用户不存在！";
            if (!IsSSOLogin) //本地登录，验证密码
            {
                if (user.LoginPwd != ToolsLib.Utility.StrUtils.Encrypt(loginPwd, ToolsLib.LibConst.EncryptFormat.SHA1))
                {
                    return "密码错误！";
                }
            }
            // 在跳转到其他系统,创建cookie时使用
            user.LoginPwd = loginPwd;
            //初始化用户身份验证票据和权限资源
            LoginHelper.InitPermission(user);

            UtilsHelper.CreateLoginCookie(collection["JobNo"], WebUtils.GetSettingsValue("SystemCookieName"));

            return errMsg;
        }



        public ActionResult LoginUser(string userId)
        {
            UserModel user = null;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                userId = UtilsHelper.Decrypt(userId);
                user = ServiceProvider.UserService.GetById(int.Parse(userId));
            }
            else
            {
                user = Session["LoginUser"] as UserModel;
            }
            string org = "";
            foreach (UserManageDepartModel u in user.manageDepartList)
            {
                org = org + ServiceProvider.OrgService.GetByCode(u.OrgCode).OrgName + "   ";
            }
            ViewBag.org = org;
            return View(user);
        }

        //跳转到登录后的首页
        public ActionResult LoggedinDefault()
        {
            return View("LoggedinDefault", Session["LoginUser"] as UserModel);
        }

        public string Logout()
        {
            return LoginHelper.Logout(this);
        }

        public ActionResult TopMenu()
        {
            UserModel user = LoginHelper.LoginUser;
            LoginHelper.TopMenu(this, user);

            return View(user);
        }

        public ActionResult Error(string message)
        {
            ViewBag.ErrorMessage = string.IsNullOrWhiteSpace(message) ? "没有权限访问该页面，请联系管理员." : message;
            if (TempData["ErrorMessage"] != null) ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            else if (Session["ErrorMessage"] != null) ViewBag.ErrorMessage = Session["ErrorMessage"].ToString();

            return View();
        }

        /// <summary>
        /// Autocomplete功能提供查询支持，查询用户，
        /// </summary>
        /// <param name="term">输入内容</param>
        /// <returns></returns>
        public ActionResult SearchUser(string term, bool IsNullSelectAll = false)
        {
            string orgCode = Request["labOrgCode"];
            return Content(ServiceProvider.UserService.QuickSearchUserBykeyword(term, orgCode, IsNullSelectAll).ToString());
        }

        /// <summary>
        /// Autocomplete功能提供查询支持，查询用户，
        /// </summary>
        /// <param name="term">输入内容</param>
        /// <returns></returns>
        public ActionResult SearchUser4Msg(string term)
        {
            string orgCode = Request["labOrgCode"];
            return Content(ServiceProvider.UserService.QuickSearchUser4MsgBykeyword(term, orgCode).ToString());
        }

        //关闭页面时，移除当前登录人访问当前页面的记录
        //value:action_业务主键Id
        //public void Clear(string key)
        //{
        //    ApplicationCacheHelper.Clear(string.Format("{0}_{1}", key, GRGTCommonUtils.LoginHelper.LoginUser.UserName));
        //}
        
        /// <summary>
        /// 帮助-版本查看
        /// </summary>
        /// <returns></returns>
        public ActionResult Help()
        {
            string Version = VesionCode;
            ViewBag.Version = Version;
            ViewBag.PublishDate = PublishDate;
            return View();
        }

        /// <summary>
        /// 版本编号
        /// </summary>
        private string VesionCode = Global.Common.GlobalConstants.VesionCode;
        private string PublishDate = Global.Common.GlobalConstants.PublishDate;
        /// <summary>
        /// 帮助-版本更新
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckVersion()
        {
            //获取令牌
            string accessToken = Global.Business.ServiceProvider.ParamService.GetaccessToken(Global.Common.GlobalConstants.SysParamType.CompanyInfo);
            string Msg=string.Empty;
            string info = WSProvider.EbusinessProvider.CheckInstrumentVersion(VesionCode, accessToken);
            Dictionary<string, object> dic = (Dictionary<string, object>)ToolsLib.Utility.CommonUtils.JsonDeserialize(info, typeof(Dictionary<string, object>));
            string Url = "";
            Msg = dic["Msg"].ToString();
            if (Msg.ToUpper().Equals("OK"))
            {
                InstrumentVersionModel model = (InstrumentVersionModel)ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(InstrumentVersionModel));
                if (model != null && !string.IsNullOrWhiteSpace(model.DownloadUrl))
                {
                    Url = model.DownloadUrl;
                }
                else
                    Msg = "未发现新版本";
            }
            return Json(new
            {
                Msg = Msg,
                URL = Url
            }, JsonRequestBehavior.AllowGet);
        }
      
    }
}
