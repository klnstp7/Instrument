using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using ToolsLib.Utility;
using Global.Common.Models;
using System.Text;
using System.IO;

namespace Global.WebSite.Controllers
{
    public class BusinessLogController : Controller
    {
        //
        // GET: /RecordLog/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BusinessLogController));

        public ActionResult Index()
        {          
            return View();
        }

        /// <summary>
        /// 记录日志列表
        /// </summary>
        /// <returns></returns>
        public ActionResult RecordLog(int FKType)
        {
            string URL = Request.RawUrl;
            //截取参数(参数中含有特殊字符)
            int index = URL.IndexOf('?') + 1;
            string paramstr = URL.Substring(index, URL.Length - index);
            //先用&将参数分组
            var pramas = paramstr.Split('&');
            //再用=号将参数分组
            var pramas2 = pramas[0].Split('=');
            //解密
            int FKvalueInt = 0;
            if (!string.IsNullOrEmpty(pramas2[1]))
            {
                FKvalueInt = UtilsHelper.Decrypt2Int(pramas2[1]);
            }
            ViewBag.FKValue = pramas2[1];
            ViewBag.FKType = FKType;    
            return View("RecordLog");
        }

        public string GetOrderBusinessLog(string FKValue, int FKType)
        {
            //解密
            int FKvalueInt = 0;
            if (!string.IsNullOrEmpty(FKValue))
            {
                FKvalueInt = UtilsHelper.Decrypt2Int(FKValue);
            }

            //获取日志记录列表
            IList<Sys_BusinessLogModel> businessLogList = Global.Business.ServiceProvider.Sys_BusinessLogService.GetByFKValue(FKvalueInt, FKType);
            businessLogList = businessLogList.OrderByDescending(o => o.CreateDate).ToList();
            //姓名、时间、备注、附件（点击即下载）
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"data\":[");
            foreach (var business in businessLogList)
            {
                int index = businessLogList.IndexOf(business);
                sb.AppendFormat("[\"{0}\"", index + 1);
                sb.AppendFormat(",\"<input type='checkbox' name='chk' LogId='{0}'></input> \"", business.LogId);
                sb.AppendFormat(",\"{0}\"", business.CreateUser);
                sb.AppendFormat(",\"{0}\"", business.CreateDate);
                sb.AppendFormat(",\"{0}\"", UtilsHelper.SpecialCharValidate(business.LogConent));
                sb.AppendFormat(",\"{0}\"", business.FileName == "" ? "" : string.Format("<a onclick='fnDownload(this)' FileId='{1}' >{0}</a>", business.FileName, UtilsHelper.Encrypt(business.FileId.ToString())));  //附件           
                sb.Append("],");
            }
            sb.Append("]}");
            if (businessLogList.Count > 0)
                sb.Remove(sb.Length - 3, 1);
            return sb.ToString();
        }

        #region 保存日志
        /// <summary>
        /// 保存日志
        /// </summary>
        [HttpPost]
        public string SaveBussinessLogFile(FormCollection form, string FKValue, int? FKType, string LogConent)
        {
            int FKvalueInt = 0;
            if (!string.IsNullOrEmpty(FKValue))
            {
                FKvalueInt = UtilsHelper.Decrypt2Int(FKValue);
            }
            //记录日志
            Sys_BusinessLogModel businessLog = new Sys_BusinessLogModel()
            {
                FKValue = FKvalueInt,           //外键标识
                FKType = FKType.Value,                        //外键类别
                CreateUser = LoginHelper.LoginUser.UserName,  //创建人
                Creator = LoginHelper.LoginUser.UserId,       //创建人标识
                LogConent = LogConent == null ? "" : UtilsHelper.SpecialCharValidate(LogConent), //日志描述
                FileId = -1,                                   //文件标识
                FileName = "",                                 //文件名称
                CreateDate = DateTime.Now                     //创建时间
            };
            SaveLogandAttach(businessLog);

            return "OK";
           
        }

        public void SaveLogandAttach(Sys_BusinessLogModel businessLog)
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                string url = WebUtils.GetSettingsValue("BusinessLogFilePath") + @"/" + StrUtils.GetUniqueFileName(null) + System.IO.Path.GetExtension(Request.Files[0].FileName);

                //上传文件
                var AttachmentModel = UtilsHelper.FileUpload(Request.Files[0], url, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType"))); ;

                Global.Business.ServiceProvider.AttachmentService.Save(AttachmentModel);

                businessLog.FileId = AttachmentModel.FileId;
                businessLog.FileName = Request.Files[0].FileName;
            }
            Global.Business.ServiceProvider.Sys_BusinessLogService.Save(businessLog);
        }
        #endregion

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="LogId"></param>
        /// <returns></returns>
        public string DeleteBusinessLog(string LogId)
        {
            string[] LogIdArray = LogId.Split(',');
          
            //获取记录
            IList<Sys_BusinessLogModel> Sys_BusinessLogModelList=Global.Business.ServiceProvider.Sys_BusinessLogService.GetById(LogIdArray);
            Global.Business.ServiceProvider.Sys_BusinessLogService.DeleteLogAndAttach(Sys_BusinessLogModelList);
            //删除记录
            Global.Business.ServiceProvider.Sys_BusinessLogService.DeleteById(LogIdArray);
            return "OK";
            
        }

        
    }
}
