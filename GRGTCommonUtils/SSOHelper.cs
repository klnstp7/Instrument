using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ToolsLib.EncryptService;
using ToolsLib.Utility;

namespace GRGTCommonUtils
{
    public static class SSOHelper
    {
        static Triple_DES des = new Triple_DES("@#$FW12!_23$dsD");

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string Encrypt(string paramValue)
        {
            paramValue = des.Encrypt(paramValue);

            paramValue = StrUtils.ToBase64(paramValue);
            return  paramValue;
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string Decrypt(string paramValue)
        {
            try
            {
                paramValue = StrUtils.FromBase64(paramValue);
                paramValue = des.Decrypt(paramValue);
            }
            catch (System.Exception ex)
            {
                paramValue = ex.Message;
            }
            return paramValue;
        }       

        public static string GetCookieName()
        {
            return "_GRGTest-Passport_";
        }

        public static string GetUserAccount()
        {
            string account = "";

            if (!ValidateToken()) return account;

            account = HttpContext.Current.Request.Cookies[GetCookieName()].Value;
            if (!string.IsNullOrWhiteSpace(account))
                //account = Decrypt(account.Split('|')[0]) + "|" + Decrypt(account.Split('|')[1]);
                account = Decrypt(account);
            return account;
        }

        /// <summary>
        /// 调用WebService进行登录验证
        /// </summary>
        /// <returns></returns>
        public static string CheckLogin(string JobNo,string loginPwd)
        {
            //AccoutService client = new AccoutService();
            return "";
            //return client.Login(JobNo, loginPwd, string.Empty);
        }

        /// <summary>
        /// 创建Cookie
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="timeout">Cookie过期时间，单位为秒</param>
        public static void CreateCookie(string account, string password, int timeout)
        {
            string cookieValue = Encrypt(string.Format("{0}|{1}|{2}", account, password, DateTime.Now.AddSeconds(timeout)));
            HttpContext.Current.Request.Cookies.Remove(GetCookieName());
            HttpCookie cookie = new HttpCookie(GetCookieName(), cookieValue);
            //cookie.Expires = DateTime.Now.AddDays(-3);
            //cookie.Expires = DateTime.Now.AddSeconds(timeout);
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 验证Cookie中的令牌
        /// </summary>
        /// <returns></returns>
        public static bool ValidateToken()
        {
            if (HttpContext.Current.Request.Cookies == null) return false;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[GetCookieName()];
            if (cookie == null) return false;
            //验证过期
            string cookieValue = Decrypt(cookie.Value);
            if (DateTime.Now.CompareTo(Convert.ToDateTime(cookieValue.Split('|')[2])) > 0) return false;

            return true;
        }

        public static string GetToUrl(GRGTCommonUtils.UtilConstants.SysType system, string goUrl)
        {
            string[] ports = ToolsLib.Utility.WebUtils.BaseAppPreSuffix.Split(':');
            string ip = ports[1];
            string port = "8083";
            switch (system)
            {
                case GRGTCommonUtils.UtilConstants.SysType.CRM:
                    if (ip == "//localhost") port = "24127";
                    else port = "8082";
                    break;
                case GRGTCommonUtils.UtilConstants.SysType.MeasureLab:
                    if (ip == "//localhost") port = "11740";
                    else port = "8083";
                    break;
                case GRGTCommonUtils.UtilConstants.SysType.Finance:
                    if (ip == "//localhost") port = "11743";
                    else port = "8085";
                    break;
                case GRGTCommonUtils.UtilConstants.SysType.Environment:
                    if (ip == "//localhost") port = "11731";
                    else port = "8090";
                    break;
                case GRGTCommonUtils.UtilConstants.SysType.EMC:
                    if (ip == "//localhost") port = "7622";
                    else port = "8091";
                    break;
                case GRGTCommonUtils.UtilConstants.SysType.CustomerPortal:
                    if (ip.IndexOf("172.18") > 0)
                    {
                        ip = "//172.18.0.21";
                        port = "80";
                    }
                    else
                    {
                        ip = "//120.197.59.40";
                        port = "8086";
                    }
                    break;
                case GRGTCommonUtils.UtilConstants.SysType.Instrument:
                    if (ip == "//localhost") port = "6723";
                    else port = "8094";
                    break;
            }

            string ssoUrl = string.Format("/SSO/SSO?src=http:{0}:{1}/SSO/SSOValidate&url={2}", ip, port, goUrl);
            if (system == GRGTCommonUtils.UtilConstants.SysType.CustomerPortal)
                ssoUrl = string.Format("http:{0}:{1}", ip, port);

            return ssoUrl;
        }

    }
}
