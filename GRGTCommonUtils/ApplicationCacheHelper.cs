using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using ToolsLib.Utility;

namespace GRGTCommonUtils
{

    public class ApplicationCacheHelper
    {
        //public static IList<DictionaryEntry> cacheList = new List<DictionaryEntry>();
        //private static object m_SyncLock = new object();
        private static readonly string cacheName = "RequestKey";

        /// <summary>
        /// 设置（同时起到刷新值的作用）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetRequestKey(object key, object value)
        {
            HttpContext.Current.Application.Lock();
            if (key != null)
            {
                IList<DictionaryEntry> cacheList = HttpContext.Current.Application[cacheName] as List<DictionaryEntry>;
                DictionaryEntry de = new DictionaryEntry();
                de.Key = key;
                de.Value = value;
                if (cacheList != null)
                {
                    DictionaryEntry tempde = cacheList.SingleOrDefault(d => d.Key.Equals(key));
                    //存在，移除
                    if (tempde.Key != null) cacheList.Remove(tempde);
                }
                else  cacheList = new List<DictionaryEntry>();

                cacheList.Add(de);
                HttpContext.Current.Application[cacheName] = cacheList;
            }
            HttpContext.Current.Application.UnLock();
            //Clear(false); // 执行后自动清理过期缓存
        }

        /// <summary>
        /// 清空当前用户访问的过期记录请求
        /// </summary>
        /// <param name="IsAllRequest"></param>
        public static void Clear(bool IsAllRequest)
        {
            if (HttpContext.Current == null) return;
            if (HttpContext.Current.Application == null) return;
            if (GRGTCommonUtils.LoginHelper.LoginUser == null) return;

            HttpContext.Current.Application.Lock();
            IList<DictionaryEntry> cacheList = HttpContext.Current.Application[cacheName] as List<DictionaryEntry>;
            if (cacheList != null && cacheList.Count > 0)
            {
                IEnumerable<DictionaryEntry> list;
                DateTime nowDT = DateTime.Now;
                if (IsAllRequest) //移除当前登录人的记录
                    list = cacheList.Where(de => !de.Key.ToString().Contains(GRGTCommonUtils.LoginHelper.LoginUser.UserName));
                else //移除当前登录人的过期记录
                    //list = cacheList.Where(de => de.Key.ToString().Contains(GRGTCommonUtils.LoginHelper.LoginUser.UserName));
                    list = cacheList.Where(de => !de.Key.ToString().Contains(GRGTCommonUtils.LoginHelper.LoginUser.UserName) || (de.Key.ToString().Contains(GRGTCommonUtils.LoginHelper.LoginUser.UserName) && (nowDT - Convert.ToDateTime(de.Value.ToString())).TotalMilliseconds < Convert.ToInt32(WebUtils.GetSettingsValue("ApplicationCacheExpirationTime"))));
                cacheList = new List<DictionaryEntry>();
                foreach (DictionaryEntry de in list)
                {
                    cacheList.Add(de);
                }
                HttpContext.Current.Application[cacheName] = cacheList;
            }
            HttpContext.Current.Application.UnLock();
        }

        //
        //key:action_业务主键Id
        /// <summary>
        /// 移除指定key的值 关闭页面时，移除当前登录人访问当前页面的记录
        /// </summary>
        /// <param name="key"></param>
        public static void Clear(object key)
        {
            HttpContext.Current.Application.Lock();
            IList<DictionaryEntry> cacheList = HttpContext.Current.Application[cacheName] as List<DictionaryEntry>;
            if (cacheList != null)
            {
                DictionaryEntry tempde = cacheList.SingleOrDefault(de => de.Key.Equals(key));
                if (tempde.Key != null)
                    cacheList.Remove(tempde);
                HttpContext.Current.Application[cacheName] = cacheList;
            }
            HttpContext.Current.Application.UnLock();
        }


        /// <summary>
        /// 是否存在请求
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DictionaryEntry IsExistRequest(object key)
        {
            HttpContext.Current.Application.Lock();
            DictionaryEntry tempde = new DictionaryEntry();
            IList<DictionaryEntry> cacheList = HttpContext.Current.Application[cacheName] as List<DictionaryEntry>;
            if (cacheList != null)
            {
                //包含自己或其他人
                //tempde = cacheList.FirstOrDefault(de => de.Key.Equals(key) || de.Key.ToString().Contains(key.ToString().Split('_')[0]));
                //tempde = cacheList.FirstOrDefault(de => de.Key.ToString().Contains(string.Format("{0}_{1}", key.ToString().Split('_')[0], key.ToString().Split('_')[1])));
                IEnumerable<DictionaryEntry> tempList = cacheList.Where(de => de.Key.ToString().Contains(string.Format("{0}_{1}", key.ToString().Split('_')[0], key.ToString().Split('_')[1])));
                if (tempList != null)
                {
                    foreach (DictionaryEntry d in tempList)
                    {
                        if (!d.Key.ToString().Contains(key.ToString().Split('_')[2]))
                            tempde.Value = string.Format("{0},", d.Key.ToString().Split('_')[2]);
                    }
                }
                if (tempde.Value != null) tempde.Value = tempde.Value.ToString().Remove(tempde.Value.ToString().Length - 1, 1);
            }
            HttpContext.Current.Application.UnLock();

            return tempde;
        }
    }
}
