using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Global.Common.Models;


namespace GRGTCommonUtils
{
    public class LoginHelper
    {
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static UserModel LoginUser
        {
            get
            {
                UserModel model = new UserModel();
                if (System.Web.HttpContext.Current.Session != null)
                    model = System.Web.HttpContext.Current.Session["LoginUser"] as UserModel;
                return model;
            }
        }

        /// <summary>
        /// 当前登录用户权限点
        /// </summary>
        public static Hashtable LoginUserAuthorize
        {
            get
            {
                Hashtable ht = System.Web.HttpContext.Current.Session["ACL"] as Hashtable;
                return ht;
            }
        }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public static bool IsSuperAdmin
        {
            get { return LoginUserAuthorize.ContainsKey("superadmin"); }
        }

        /// <summary>
        /// 是否具有某个权限
        /// </summary>
        /// <param name="authorize">权限点</param>
        /// <returns></returns>
        public static bool IsHasAuthorize(string authorize)
        {
            return LoginUserAuthorize.ContainsKey(authorize.ToLower());
        }

        /// <summary>
        /// 初始化用户身份验证票据和权限资源
        /// </summary>
        /// <param name="JobNo"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public static void InitPermission(UserModel mUser)
        {
            //创建身份验证票据
            FormsAuthentication.SetAuthCookie(string.Format("{0}|{1}", mUser.UserName, mUser.JobNo), false);
            //用户权限资源
            int c = mUser.manageDepartList.Count;//强制执行被配置为延迟加载的Sql
            Hashtable ht = new Hashtable();
            foreach (RoleModel role in mUser.roleList)
            {
                c = role.menuList.Count;//强制执行被配置为延迟加载的Sql
                foreach (PermissionModel per in role.permissionList)
                {
                    if (string.IsNullOrEmpty(per.PermissionResource))
                    {
                        continue;
                    }
                    string[] resArray = per.PermissionResource.Split(new string[] { ",", ";", "，", "；", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < resArray.Length; i++)
                    {
                        if (string.IsNullOrEmpty(resArray[i])) continue;
                        if (!ht.ContainsKey(resArray[i].ToLower())) ht.Add(resArray[i].ToLower(), per.PermissionId);
                    }
                }
            }
            System.Web.HttpContext.Current.Session.Add("LoginUser", mUser);
            System.Web.HttpContext.Current.Session.Add("ACL", ht);
        }

        /// <summary>
        /// 记住我
        /// </summary>
        /// <param name="mct"></param>
        /// <param name="cookieName"></param>
        public static void RememberMe(Controller mController, string sCookieName)
        {
            mController.ViewBag.RememberMe = false;
            mController.ViewBag.JobNo = string.Empty;
            //mController.ViewBag.Pwd = string.Empty;
            HttpCookie mCookie = mController.HttpContext.Request.Cookies[sCookieName];

            if (null != mCookie)
            {
                //cookie中的信息未被空字符串覆盖（当未钩选记住时以空字符串覆盖原有的登录信息）
                if (!string.IsNullOrWhiteSpace(mCookie.Value))
                {
                    string[] arrlogin = UtilsHelper.DecryptCookie(mCookie.Value).Split('|'); ;
                    //if (arrlogin.Length >= 2)
                    //{
                        mController.ViewBag.JobNo = arrlogin[0];
                        //mController.ViewBag.Pwd = arrlogin.Length < 2 ? string.Empty : arrlogin[1];
                        //mController.ViewBag.RememberMe = true;
                    //}
                }
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public static string Logout(Controller mController)
        {
            ////清除用户访问的记录
            //ApplicationCacheHelper.Clear(true);
            FormsAuthentication.SignOut();
            mController.HttpContext.Session.RemoveAll();
            mController.Response.Cookies[FormsAuthentication.FormsCookieName].Value = "";
            mController.Response.Write(string.Format("<script>window.top.location.href='{0}';</script>", FormsAuthentication.LoginUrl));
            return "";
        }

        /// <summary>
        /// 系统菜单
        /// </summary>
        /// <param name="mController"></param>
        /// <param name="mUser"></param>
        public static void TopMenu(Controller mController, UserModel mUser)
        {
            //用户角色
            IList<MenuModel> menuList = new List<MenuModel>();
            ToolsLib.Utility.myComparer<MenuModel> mComparer = new ToolsLib.Utility.myComparer<MenuModel>("MenuId");
            string userRole = string.Empty;
            foreach (var role in mUser.roleList)
            {
                if (!string.IsNullOrEmpty(role.RoleName))
                {
                    userRole += role.RoleName + ",";
                }
                foreach (var menu in role.menuList)
                {
                    if (!menuList.Contains(menu, mComparer))
                        menuList.Add(menu);
                }
            }
            if (userRole.EndsWith(","))
            {
                userRole = userRole.Substring(0, userRole.Length - 1);
            }
            mController.ViewBag.UserRole = userRole;
            //用户菜单
            StringBuilder sbMenu1 = new StringBuilder();
            StringBuilder sbMenu2 = new StringBuilder();
            int count = 0;
            ToolsLib.Utility.myComparer<MenuModel> orderComparer = new ToolsLib.Utility.myComparer<MenuModel>("ShowOrder"); //排序比较器
            IEnumerable<MenuModel> leveloneMenu = menuList.Where(m => m.ParentMenuId == 0);
            //IOrderedEnumerable<MenuModel> orderleveloneMenu = leveloneMenu.OrderBy(m => m, orderComparer);    //1级菜单

            //获取没被禁用的1级菜单 Alter by Reven 2014-08-04
            IOrderedEnumerable<MenuModel> orderleveloneMenu = leveloneMenu.Where(m=>m.IsEnabled).OrderBy(m => m, orderComparer);   

            IOrderedEnumerable<MenuModel> orderleveltwoMenu = null;
            sbMenu1.Append("<ul id=\"nav\">");
            sbMenu2.Append("<div id=\"menu_con\">");
            foreach (var menu in orderleveloneMenu)    //2级菜单
            {
                IEnumerable<MenuModel> leveltwoMenu = menuList.Where(m => m.ParentMenuId == menu.MenuId);
                //orderleveltwoMenu = leveltwoMenu.OrderBy(m => m, orderComparer);
                //获取没被禁用的2级菜单 Alter by Reven 2014-08-04
                orderleveltwoMenu = leveltwoMenu.Where(m=>m.IsEnabled).OrderBy(m => m, orderComparer);
                sbMenu1.AppendFormat("<li><a class=\"{0}\" id=\"mynav{1}\" onclick=\"javascript:return qiehuan({1},{5})\" href=\"{2}\" target=\"MiddleFrame\" {3}>" 
                    + "<span>{4}</span></a></li>", count == 0 ? "nav_on" : "nav_off", count, string.IsNullOrEmpty(menu.LinkUrl) ? "#" : menu.LinkUrl, 
                    string.IsNullOrEmpty(menu.LinkUrl) ? "onclick=\"return false;\"" : "", menu.MenuName, leveltwoMenu.Count());
                sbMenu1.Append("<li class=\"menu_line\"></li>");
                sbMenu2.AppendFormat("<div id=\"qh_con{0}\" style=\"display: {1}\">", count, count == 0 ? "block" : "none");
                sbMenu2.Append("<ul>");
                int secCount = 0;
                foreach (var menu2 in orderleveltwoMenu)
                {
                    sbMenu2.AppendFormat("<li><a href=\"{0}\" target=\"MiddleFrame\" id=\"myTwoMenu{2}{3}\" onclick=\"javascript:return fnSelected('myTwoMenu{2}{3}')\">" 
                        + "<span>{1}</span></a></li><li class=\"menu_line2\"></li>", menu2.LinkUrl, menu2.MenuName, count, secCount);
                    secCount++;
                }
                sbMenu2.Append("</ul>");
                sbMenu2.Append("</div>");
                count += 1;
            }
            sbMenu1.Append("</ul>");
            sbMenu2.Append("</div>");
            mController.ViewBag.Menu1 = sbMenu1.ToString();
            mController.ViewBag.Menu2 = sbMenu2.ToString();
            mController.ViewBag.LevelOneCount = count;
        }


    }
}
