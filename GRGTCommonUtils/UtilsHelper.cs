using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolsLib.EncryptService;
using ToolsLib.Utility;
using System.Collections;
using System.IO;
using ToolsLib.FileService;
using System.Text;
using System.Web.Mvc;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Printing;
using System.Runtime.InteropServices;
using log4net;
using Global.Common.Models;
using System.Xml;
using System.Data;

namespace GRGTCommonUtils
{
    public class UtilsHelper
    {        

        protected static readonly ILog log = LogManager.GetLogger(typeof(UtilsHelper));

        private static string desKey = "sdji*&^125";

        /// <summary>
        /// url参数加密
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string Encrypt(string paramValue)
        {
            Triple_DES des = new Triple_DES((HttpContext.Current==null || HttpContext.Current.Session == null) ? desKey : HttpContext.Current.Session.SessionID);
            paramValue = des.Encrypt(paramValue);
            paramValue = StrUtils.ToBase64(paramValue);
            ////去掉最后一个“=”字符
            //paramValue = paramValue.Substring(0, paramValue.Length - 1);

            return paramValue;
        }

        /// <summary>
        /// url参数解密
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string Decrypt(string paramValue)
        {
            try
            {
                if (paramValue == "0")
                    return paramValue;
                ////把空格替换成“+”，字符串后面加上“=”
                //paramValue = paramValue.Replace(' ', '+') + "=";
                paramValue = StrUtils.FromBase64(paramValue);
                Triple_DES des = new Triple_DES(((HttpContext.Current == null || HttpContext.Current.Session == null) ? desKey : HttpContext.Current.Session.SessionID));
                paramValue = des.Decrypt(paramValue);
            }
            catch (System.Exception ex)
            {
                HttpContext.Current.Response.Redirect(string.Format("/Login/Error?message={0}", HttpContext.Current.Server.UrlEncode(ex.Message)), true);
            }

            return paramValue;
        }

        /// <summary>
        /// url参数解密
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static int Decrypt2Int(string paramValue)
        {
            if (paramValue == "0")
                return 0;

            int v = -99;

            v = ToolsLib.Utility.StrUtils.ToInt32(Decrypt(paramValue), -99);
            return v;
        }

        /// <summary>
        /// Cookie数据解密
        /// </summary>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public static string DecryptCookie(string cookieValue)
        {
            try
            {
                ////把空格替换成“+”，字符串后面加上“=”
                //cookieValue = cookieValue.Replace(' ', '+') + "=";
                Triple_DES des = new Triple_DES(desKey);
                //cookieValue = StrUtils.FromBase64(cookieValue);
                cookieValue = des.Decrypt(cookieValue);
            }
            catch 
            {
                cookieValue = "";
            }

            return cookieValue;
        }

        /// <summary>
        /// 加密处理登录信息并写入cookie，cookie值解密后的形式为：JobNo|pwd
        /// </summary>
        /// <param name="JobNo">登录名</param>
        /// <param name="pwd">密码</param>
        /// <param name="cookieName">cookie名</param>
        public static void CreateLoginCookie(string JobNo, string cookieName)
        {
            Triple_DES des = new Triple_DES(desKey);
            string loginInfo = JobNo;
            loginInfo = des.Encrypt(loginInfo);
            HttpCookie cookie = new HttpCookie(cookieName, loginInfo);
            cookie.Expires = DateTime.Now.AddDays(15);
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }



        /// <summary>
        /// 客户端文件下载
        /// </summary>
        /// <param name="webFileServer">可以为空,web文件服务器文件访问前缀,从web服务器下载</param>
        /// <param name="filePath">文件虚拟路径地址</param>
        /// <param name="newFileName">客户端显示文件名</param>
        /// <param name="targetServer"></param>
        public static void FileDownload(string webFileServer, string filePath, string newFileName, UtilConstants.ServerType targetServer)
        {
            string fileExtension = Path.GetExtension(filePath);
            string newFileNameExtension = Path.GetExtension(newFileName);

            //newFileName无后缀名或带有点号但却无后缀名
            if (string.IsNullOrWhiteSpace(newFileNameExtension) || fileExtension != newFileNameExtension)
            {
                newFileName = newFileName + fileExtension;
            }
            newFileName = newFileName.Replace(" ", "_").Replace(",", "_");

            using (Stream input = FileDownload(webFileServer, filePath, targetServer))
            {
                WebServer.DownLoadFile(input, newFileName);
            }

        }

        /// <summary>
        /// 客户端文件下载
        /// </summary>
        /// <param name="webFileServer"></param>
        /// <param name="filePath">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static Stream FileDownload(string webFileServer, string filePath, UtilConstants.ServerType targetServer)
        {
            Stream input = null;
            switch (targetServer)
            {
                case UtilConstants.ServerType.WebFileService:
                    WebDAVFileServer dav = GetWebDAVServer(webFileServer);
                    input = dav.DownLoadFile(filePath);
                    break;
                case UtilConstants.ServerType.WebService:
                     input = new FileStream(ToolsLib.Utility.WebUtils.Resulve(filePath), FileMode.Open, FileAccess.Read, FileShare.Read);
                    break;
                default:
                    break;
            }
            return input;
        }

        /// <summary>
        /// 判断fs-www文件服务器是否存在指定文件
        /// </summary>
        /// <param name="webFileServer"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsExistFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            WebDAVFileServer fileService = GetWebDAVServer2();
            return fileService.IsFileExist(fileName);
        }

        /// <summary>
        /// 判断文件服务器是否存在指定文件
        /// </summary>
        /// <param name="server"></param>
        /// <param name="fileName"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsExistInFSServer(string server, string fileName, string user, string password)
        {
            //log.InfoFormat("server={0}, fileName={1}, user={2}, password={3}", server, fileName, user, password);
            WebDAVFileServer fileService = new WebDAVFileServer(server, user, password);
            return fileService.IsFileExist(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseDir"></param>
        /// <param name="dir">4位年+2位月+2位日</param>
        public static string CreateDir(string baseDir, string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return "";
            dir = string.Format("{0}/{1}/{2}/", dir.Substring(0, 4), dir.Substring(4, 2), dir.Substring(6, 2));
            WebDAVFileServer fileService = GetWebDAVServer("");

            string[] dirs = dir.Split('/');
            foreach (string d in dirs)
            {
                if (!string.IsNullOrEmpty(d))
                {
                    baseDir += d + "/";
                    fileService.CreateDir(baseDir);
                }
            }
            return baseDir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        public static string CreateDir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return "";           
            WebDAVFileServer fileService = GetWebDAVServer("");
            fileService.CreateDir(dir);
            return dir;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="postFile"></param>
        /// <param name="targetFileName">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static AttachmentModel FileUpload(HttpPostedFileBase postFile, string targetFileName, UtilConstants.ServerType targetServer)
        {
            return FileUpload(WebUtils.GetSettingsValue("WebFileServer"), postFile, targetFileName, targetServer);
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="webFileServer">当文件服务器发生变更，必须指定旧的文件服务器地址</param>
        /// <param name="postFile"></param>
        /// <param name="targetFileName">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static AttachmentModel FileUpload(string webFileServer, HttpPostedFileBase postFile, string targetFileName, UtilConstants.ServerType targetServer)
        {
            return FileUpload(webFileServer, postFile.InputStream, postFile.FileName, targetFileName, targetServer);
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="sourceFileName">源文件物理地址</param>
        /// <param name="targetFileName">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static AttachmentModel FileUpload(string sourceFileName, string targetFileName, UtilConstants.ServerType targetServer)
        {
            return FileUpload(WebUtils.GetSettingsValue("WebFileServer"), sourceFileName, targetFileName, targetServer);
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="webFileServer">当文件服务器发生变更，必须指定旧的文件服务器地址</param>
        /// <param name="sourceFileName">源文件物理地址</param>
        /// <param name="targetFileName">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static AttachmentModel FileUpload(string webFileServer, string sourceFileName, string targetFileName, UtilConstants.ServerType targetServer)
        {
            FileStream fs = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read);
            AttachmentModel atta = FileUpload(webFileServer, fs, sourceFileName, targetFileName, targetServer);

            fs.Close();
            fs.Dispose();

            return atta;
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="sourceFileName">待上传文件名称</param>
        /// <param name="targetFileName">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static AttachmentModel FileUpload(System.IO.Stream inputStream, string sourceFileName, string targetFileName, UtilConstants.ServerType targetServer)
        {
            return FileUpload(WebUtils.GetSettingsValue("WebFileServer"), inputStream, sourceFileName, targetFileName, targetServer);
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="webFileServer">当文件服务器发生变更，必须指定旧的文件服务器地址</param>
        /// <param name="inputStream">输入流</param>
        /// <param name="sourceFileName">待上传文件名称</param>
        /// <param name="targetFileName">文件虚拟路径地址</param>
        /// <param name="targetServer"></param>
        /// <returns></returns>
        public static AttachmentModel FileUpload(string webFileServer, System.IO.Stream inputStream, string sourceFileName, string targetFileName, UtilConstants.ServerType targetServer)
        {
            ToolsLib.FileService.WebDAVFileServer webDav;

            string newFileName = targetFileName;
            if (string.IsNullOrEmpty(targetFileName))
                newFileName += string.Format("{0}{1}", StrUtils.GetUniqueFileName(null), Path.GetExtension(sourceFileName));
            if (Path.GetExtension(targetFileName) == "")
                newFileName = string.Format("{0}{1}", targetFileName, Path.GetFileName(sourceFileName));
            //if (!string.IsNullOrEmpty(targetFileName) && Path.GetExtension(targetFileName) == "")
            //    newFileName += string.Format("{0}{1}", StrUtils.GetUniqueFileName(null), Path.GetExtension(sourceFileName));
            AttachmentModel attachment = new AttachmentModel();
            attachment.FileSize = Convert.ToInt32(inputStream.Length);
            attachment.FileName = Path.GetFileNameWithoutExtension(sourceFileName);
            attachment.FileServerType = (int)targetServer;
            attachment.FileVirtualPath = newFileName;
            attachment.FileAccessPrefix = webFileServer;
            attachment.UserName = "";
            attachment.ServerIP = "";
            attachment.Remark = "";

            if (HttpContext.Current != null)
            {
                UserModel user = HttpContext.Current.Session["LoginUser"] as UserModel;
                if (user != null)
                {
                    attachment.UserName = user.UserName;
                    attachment.UserId = user.UserId;
                }
            }

            //Hashtable ht = new Hashtable();
            //ht.Add("FileSize", Convert.ToInt32(inputStream.Length));
            //ht.Add("FileName", Path.GetFileNameWithoutExtension(sourceFileName));
            //ht.Add("FileServerType", (int)targetServer);
            //ht.Add("ServerIP", "");
            //ht.Add("FileAccessPrefix", "");
            //ht.Add("FileVirtualPath", newFileName);
            //ht.Add("UserId", 0);
            //ht.Add("UserName", "");
            //ht.Add("Remark", "");

            switch (targetServer)
            {
                case UtilConstants.ServerType.WebFileService:
                    //if (string.IsNullOrWhiteSpace(webFileServer))
                    //    ht["FileAccessPrefix"] = WebUtils.GetSettingsValue("WebFileServer");
                    //else
                    //ht["FileAccessPrefix"] = webFileServer;
                    attachment.FileVirtualPath = attachment.FileVirtualPath.Replace("\\", "/");
                    webDav = GetWebDAVServer(webFileServer);
                    webDav.UploadFile(inputStream, attachment.FileVirtualPath);
                    break;
                case UtilConstants.ServerType.WebService:
                    attachment.FileAccessPrefix = "";
                    attachment.FileVirtualPath = ToolsLib.FileService.WebServer.UpLoadFile(inputStream, targetFileName);
                    attachment.FileVirtualPath = attachment.FileVirtualPath.Replace(ToolsLib.Utility.WebUtils.Resulve("/"), "/").Replace("\\", "/");
                    break;
                case UtilConstants.ServerType.FTPService:
                    attachment.ServerIP = WebUtils.GetSettingsValue("FtpHost");
                    break;
                default:
                    break;
            }

            return attachment;
        }

        /// <summary>
        /// 删除服务器上文件
        /// </summary>
        /// <param name="webFileServer"></param>
        /// <param name="fileName">虚拟路径 或 物理路径 地址</param>
        /// <param name="targetServer"></param>
        public static void DeleteServerFile(string webFileServer, string fileName, UtilConstants.ServerType targetServer)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;

            switch (targetServer)
            {
                case UtilConstants.ServerType.WebFileService:
                    if (string.IsNullOrWhiteSpace(webFileServer)) return;

                    WebDAVFileServer dav = GetWebDAVServer(webFileServer);
                    dav.DeleteFile(fileName);

                    break;
                case UtilConstants.ServerType.WebService:
                    string physicsPath = CommonUtils.GetPhysicsPath(fileName);
                    System.IO.File.Delete(physicsPath);
                    break;
                case UtilConstants.ServerType.FTPService:
                    break;
                default:
                    break;
            }
        }

        public static void DeleteServerFile(string fileName, UtilConstants.ServerType targetServer)
        {
            switch (targetServer)
            {
                case UtilConstants.ServerType.WebFileService:
                    WebDAVFileServer dav = GetWebDAVServer("");
                    dav.DeleteFile(fileName);

                    break;
                case UtilConstants.ServerType.WebService:
                    break;
                case UtilConstants.ServerType.FTPService:
                    break;
                default:
                    break;
            }
        }

        private static WebDAVFileServer GetWebDAVServer(string webFileServer)
        {
            //读取配置信息
            if (string.IsNullOrWhiteSpace(webFileServer))
                webFileServer = WebUtils.GetSettingsValue("WebFileServer");
            string userId = WebUtils.GetSettingsValue("WebFileServerUser");
            string pwd = WebUtils.GetSettingsValue("WebFileServerPwd");
            WebDAVFileServer dav = new WebDAVFileServer(webFileServer, userId, pwd);

            return dav;
        }

        private static WebDAVFileServer GetWebDAVServer2()
        {
            //读取配置信息
            string webFileServer = WebUtils.GetSettingsValue("WebFileServer2");
            string userId = WebUtils.GetSettingsValue("WebFileServerUser2");
            string pwd = WebUtils.GetSettingsValue("WebFileServerPwd2");
            WebDAVFileServer dav = new WebDAVFileServer(webFileServer, userId, pwd);

            return dav;
        }
       
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="sendTo"></param>
        /// <param name="comeFromSys"></param>
        /// <param name="msgType"></param>
        /// <param name="sendType"></param>
        public static void SendMsg(string title, string content, string sendTo, UtilConstants.SysType comeFromSys, UtilConstants.MsgType msgType, UtilConstants.SendMsgType sendType)
        {
            //LogMessageModel msgModel = new LogMessageModel();
            //msgModel.ComeFromSys = comeFromSys.GetHashCode();
            //msgModel.ComeFromUser = LoginHelper.LoginUser.UserName;
            //msgModel.IsEnabled = true;
            //msgModel.MsgTitle = title;
            //msgModel.MsgContent = content;
            //msgModel.MsgType = msgType.GetHashCode();
            //msgModel.RepeatTime = 0;
            //msgModel.SendTo = sendTo;
            //msgModel.SendType = sendType.GetHashCode();
            //msgModel.PlanDate = DateTime.Now;
            //WSProvider.CommonProvider.AddMsg(msgModel);
        }


        /// <summary>
        /// 是否通过公网IP地址访问
        /// </summary>
        /// <returns></returns>
        public static bool IsInternet()
        {
            bool flag = true;

            string preSuffix = ToolsLib.Utility.WebUtils.BaseAppPreSuffix;
            if (preSuffix.IndexOf("172.18.0") != -1 || preSuffix.IndexOf("localhost") != -1) flag = false;

            return flag;
        }


        /// <summary>
        /// 获取公网IP地址访问
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetInternetIP(string url)
        {
            string internetIP = "120.197.59.40";
            internetIP = url.Replace("172.18.0.19", internetIP);

            return internetIP;
        }       

        /// <summary>
        /// 根据枚举生成下拉列表(后台用) Add by Raven 2014-11-11
        /// </summary>
        /// <param name="name">使用 new Constants.Color() 为例 </param>
        /// <param name="defaultObj">为null,则没有默认值</param>
        /// <returns></returns>
        public static SelectList BuliderSelectList(Enum name, object defaultObj)
        {
            List<SelectListItem> SelectListItem = new List<SelectListItem>();
            foreach (object o in Enum.GetValues(name.GetType()))
            {
                var oType = (object)Enum.Parse(name.GetType(), o.ToString(), true);

                SelectListItem item = new
                SelectListItem
                {
                    Text = o.ToString(),
                    Value = ((int)oType).ToString()
                };
                SelectListItem.Add(item);
            }
            if (defaultObj != null)
            {
                return new SelectList(SelectListItem, "Value", "Text", defaultObj);
            }
            return new SelectList(SelectListItem, "Value", "Text");
        }

        /// <summary>
        /// 获取星期
        /// </summary>
        /// <returns></returns>
        public static string GetWeek(DayOfWeek dayofweek)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(dayofweek)];
            return week;
        }


        /// <summary>
        /// 获取ModelState验证错误信息
        /// </summary>
        /// <param name="ModelStateList"></param>
        /// <returns></returns>
        public static StringBuilder GetValidateErrorMsg(ICollection<ModelState> ModelStateList)
        {
            StringBuilder sb = new StringBuilder();

            foreach (ModelState ms in ModelStateList)
            {
                foreach (var error in ms.Errors)
                    sb.AppendLine(error.ErrorMessage);
            }
            return sb;
        }



        #region 构建排期表内容

        public static StringBuilder BuildHtmlTable(List<ToolsLib.Model.NameValueModel> columnDs, DataTable planDs, DateTime planDate)
        {
            StringBuilder html = new StringBuilder();

            bool currentDate = planDate == DateTime.MinValue ? false : true;
            foreach (ToolsLib.Model.NameValueModel item in columnDs)
            {
                html.Append(Environment.NewLine + "<tr>" + Environment.NewLine);
                html.AppendFormat("<td>{0}</td>", item.Name);

                if (!currentDate) planDate = Convert.ToDateTime(item.Value);
                DataRow[] drSelect = planDs.Select(string.Format("PK='{0}'", item.Value));
                buildTD(drSelect, html, planDate);

                html.Append(Environment.NewLine + "</tr>");
            }
            return html;
        }

        private static void buildTD(DataRow[] drSelect, StringBuilder html, DateTime planDate)
        {
            DateTime dt = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", planDate));
            List<ToolsLib.Model.NameValueModel> tdList = new List<ToolsLib.Model.NameValueModel>();
            List<Hashtable> hashlist = new List<Hashtable>();
            //if (drSelect.Count() > 0) dt = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", drSelect[0]["StartDate"]));
            //将每天生成48个时间段
            for (int i = 0; i < 48; i++)
            {
                tdList.Add(new ToolsLib.Model.NameValueModel() { Name = dt, Value = 0 });
                Hashtable newhash = new Hashtable();
                newhash["Value"] = 0;
                newhash["FK"] = -1;
                newhash["Name"] = dt;
                hashlist.Add(newhash);
                dt = dt.AddMinutes(30);
            }



            //计算每个时间段是否排期、是否冲突（0：未排期，1：已排期，大于1：冲突）
            foreach (DataRow dr in drSelect)
            {

                DateTime dtStart = Convert.ToDateTime(dr["StartDate"]);
                if (dtStart < planDate)
                {
                    dtStart = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", planDate));
                }
                DateTime dtEnd = Convert.ToDateTime(dr["EndDate"]);
                //foreach (ToolsLib.Model.NameValueModel item in tdList)
                //{
                //    Hashtable newhash = new Hashtable();
                //    newhash["Value"] = item.Value;
                //    newhash["FK"] = -1;

                //    if (Convert.ToDateTime(item.Name) <= dtStart && dtStart < Convert.ToDateTime(item.Name).AddMinutes(30))
                //    {
                //        item.Value = Convert.ToInt32(item.Value) + 1;
                //        newhash["Value"] = item.Value;
                //        newhash["FK"] = dr["FK"];
                //        dtStart = dtStart.AddMinutes(30);
                //        if (dtEnd < dtStart && dtEnd < Convert.ToDateTime(item.Name).AddMinutes(30)) break;

                //        if (dtStart > dtEnd) dtStart = dtEnd;
                //    }
                //}

                foreach (var item in hashlist)
                {

                    if (Convert.ToDateTime(item["Name"]) <= dtStart && dtStart < Convert.ToDateTime(item["Name"]).AddMinutes(30))
                    {
                        item["Value"] = Convert.ToInt32(item["Value"]) + 1;
                        if (dr.Table.Columns.Contains("FK"))
                        {
                            item["FK"] = dr["FK"];
                        }

                        dtStart = dtStart.AddMinutes(30);
                        if (dtEnd <= dtStart && dtEnd <= Convert.ToDateTime(item["Name"]).AddMinutes(30)) break;

                        if (dtStart > dtEnd) dtStart = dtEnd;
                    }
                }



            }

            //输出td
            //foreach (ToolsLib.Model.NameValueModel item in tdList)
            //{
            //    int value = Convert.ToInt32(item.Value);

            //    if (value == 0)
            //    {
            //        html.AppendFormat("<td></td>");
            //    }
            //    else if (value == 1)
            //    {
            //        html.AppendFormat("<td class='paiqi'></td>");
            //    }
            //    else
            //    {
            //        html.AppendFormat("<td class='chongtu'><div title='{0}'></div></td>", "有" + (value-1) + "个排期冲突");
            //    }

            //}


            foreach (var item in hashlist)
            {
                int value = Convert.ToInt32(item["Value"]);

                if (value == 0)
                {
                    html.AppendFormat("<td></td>");
                }
                else if (value == 1)
                {
                    //string link = string.Format("<a href='#' onclick='fnProjectDetail({0})' ></a>", UtilsHelper.Encrypt(item["FK"].ToString()));               

                    if (Convert.ToInt32(item["FK"]) > 0)
                    {
                        html.AppendFormat("<td class='paiqi' ProjectId={0} onclick='fnProjectDetail(this)'></td>", UtilsHelper.Encrypt(item["FK"].ToString()));
                    }
                    else
                    {
                        html.AppendFormat("<td class='paiqi' ></td>");
                    }
                }
                else
                {
                    html.AppendFormat("<td class='chongtu'><div title='{0}'></div></td>", "有" + (value - 1) + "个排期冲突");
                }

            }
        }


        #endregion

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SpecialCharValidate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            string msg = Regex.Replace(value, "[\t\r\n]", "");
            msg = Regex.Replace(msg, @"[\\]", "/");
            msg = Regex.Replace(msg, "[\"]", "'");
            return msg;
        }

        /// <summary>
        /// pdf转swf
        /// </summary>
        /// <param name="dataDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string PdfToSwf(string dataDir, string fileName)
        {
            try
            {
                string cmdStr = CommonUtils.GetPhysicsPath("/SWFTools/pdf2swf.exe");
                string savePath = CommonUtils.GetPhysicsPath("/tempFile/");
                string sourcePath = @"""" + dataDir + @"""";
                string targetPath = @"""" + savePath + @"\" + fileName + @"""";
                //@"""" 四个双引号得到一个双引号，如果你所存放的文件所在文件夹名有空格的话，要在文件名的路径前后加上双引号，才能够成功
                // -t 源文件的路径
                // -s 参数化（也就是为pdf2swf.exe 执行添加一些窗外的参数(可省略)）
                string argsStr = "  -t " + sourcePath + " -s flashversion=9 -o " + targetPath;
                //string argsStr = "  -t " + sourcePath + " -s flashversion=9 -o " + targetPath + " -T -9 -s poly2bitmap -S";


                using (Process p = new Process())
                {
                    //Logger.System.Info("cmdStr :" + cmdStr);
                    ProcessStartInfo psi = new ProcessStartInfo(cmdStr, argsStr);
                    p.StartInfo = psi;
                    p.Start();
                    p.WaitForExit();
                    //Logger.System.Info("argsStr : " + argsStr);
                }

                return CommonUtils.GetPhysicsPath("/tempFile/" + fileName);
            }
            catch
            {
                //Logger.System.Error("生成swf失败" + ex);
                return string.Empty;
            }
        }


        /// <summary>
        /// Xml格式中特殊字符转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string XmlSpecialCharValidate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            string msg = Regex.Replace(value, "[>]", "&gt;");
            msg = Regex.Replace(msg, "[<]", "&lt;");
            msg = Regex.Replace(msg, "[\"]", "&quot;");
            msg = Regex.Replace(msg, "[']", "&apos;");
            msg = Regex.Replace(msg, "[&]", "&amp;");
            return msg;
        }
    }
  
}