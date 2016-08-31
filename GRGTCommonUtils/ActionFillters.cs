using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Global.Common.Models;
using System.Collections;

namespace GRGTCommonUtils
{
    /// <summary>
    /// 不同用户在访问同一个页面（主键值相同）的拦截器
    /// </summary>
    public class ActionFillters : FilterAttribute, IActionFilter
    {
        //执行action前执行这个方法，使用该拦截器，第一个参数必须是业务主键
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //清除过期的访问记录
            ApplicationCacheHelper.Clear(false);
            
            string key = string.Format("{0}_{1}_{2}", filterContext.ActionDescriptor.ActionName, HttpContext.Current.Request.QueryString.GetValues(0)[0], GRGTCommonUtils.LoginHelper.LoginUser.UserName);
            //先清除当前登录人访问该页面的的记录，防止刷新界面会提示
            DictionaryEntry de = ApplicationCacheHelper.IsExistRequest(key);
            if (de.Value != null)
            {
                filterContext.Controller.ViewBag.FilterMsg = string.Format("{0}正在访问", de.Value);
            }
        }

        //执行action后执行这个方法
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {            
            //key：action_业务主键Id_当前登录用户姓名
            string key = string.Format("{0}_{1}_{2}", filterContext.ActionDescriptor.ActionName, HttpContext.Current.Request.QueryString.GetValues(0)[0], GRGTCommonUtils.LoginHelper.LoginUser.UserName);
           
            ApplicationCacheHelper.SetRequestKey(key, DateTime.Now);
        }
    }
}
