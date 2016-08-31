using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Common.Models;
using GRGTCommonUtils;
using Global.Business;
using ToolsLib.Utility;
using System.Text.RegularExpressions;

namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /SysManage/Common/

        /// <summary>
        /// 通用框架页,第一个参数必须是src
        /// </summary>
        /// <returns></returns>
        public ActionResult IFramePage()
        {
            string height = Request["height"];
            string src = Request.Url.Query;
            if (string.IsNullOrWhiteSpace(height))
                height = "900px";
            ViewBag.Height = height;
            src = src.Substring(5, src.Length - 5);
            int index = src.IndexOf('&');
            src = src.Remove(index, 1);
            src = src.Insert(index, "?");
            ViewBag.FrameSrc = src;

            return View("IFramePage");
        }

        #region === 浏览pdf文件 ===
        /// <summary>
        /// 浏览pdf文件
        /// </summary>
        /// <param name="url">url</param>
        public ActionResult ReadPdf(string filePath)
        {
            string height = Request["height"];
            if (string.IsNullOrEmpty(height) == false)
            {
                ViewBag.Height = height;
            }
            else
            {
                ViewBag.Height = "550px";
            }
            ViewBag.PdfUrl = filePath;
            return View("ReadPdf");
        }       
        #endregion
    }
}
