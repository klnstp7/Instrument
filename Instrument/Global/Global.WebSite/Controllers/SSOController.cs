using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using Global.Business;
using Global.Common.Models;

namespace Global.WebSite.Controllers
{
    public class SSOController : Controller
    {
        //跳转的页面调用
        public ActionResult SSO()
        {
            //先创建令牌，再创建cookie
            SSOHelper.CreateCookie(LoginHelper.LoginUser.JobNo, "ksdflkjsd", 5);
            string url = Request.Url.Query;
            url = url.Substring(5, url.Length - 5);
            int index = url.IndexOf('&');
            if (index > -1)
            {
                url = url.Remove(index, 1);
                url = url.Insert(index, "?");
            }
            Response.Redirect(url);
            return null;
            //return View();
        }
        
        public ActionResult SSOValidate()
        {
            //单点登录
            string errMsg = "";
            if (SSOHelper.ValidateToken())
            {
                //已登录，获取用户权限
                string accout = SSOHelper.GetUserAccount();
                if (!string.IsNullOrEmpty(accout))
                {
                    UserModel user = ServiceProvider.UserService.GetByLoginName(accout.Split('|')[0]);
                    if (user != null)
                    {
                        //初始化用户身份验证票据和权限资源
                        LoginHelper.InitPermission(user);
                        string url = Request.Url.Query;
                        url = url.Substring(5, url.Length - 5);
                        int index = url.IndexOf('&');
                        if (index > -1)
                        {
                            url = url.Remove(index, 1);
                            url = url.Insert(index, "?");
                        }
                        //Response.Redirect(url);
                        ViewBag.Url = url;
                    }
                    else
                        errMsg = "不存在登录帐号";
                        //Response.Write("不存在登录帐号");
                }
                else
                    errMsg = "登录帐号为空";
            }
            else
                ViewBag.Url = "/Login/LoggedinDefault";
                //errMsg = "令牌错误";
            ViewBag.ErrMsg = errMsg;
            return View();
        }

       
    }
}
