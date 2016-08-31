using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Business;
using Global.Common.Models;
using System.Collections;
using ToolsLib.IBatisNet;
using ToolsLib.Utility.Jquery;
using System.Text;
using GRGTCommonUtils;

using ToolsLib.Utility;

using System.IO;

namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class AttachmentController : Controller
    {
        
        /// <summary>
        /// 上传文件列表页面:
        /// GET: /SysManage/Attachment/List
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {    
            return View();
        }

        /// <summary>
        /// 上传文件列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAttachmentListJsonData()
        {
            //提取DataTable参数
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;

            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? "1=1" : dtm.FieldCondition;

            if (!string.IsNullOrWhiteSpace(dtm.KeyWord))
                paging.Where = string.Format("{0} and (FileName like '{1}%' or UserName like '{1}%')", paging.Where, dtm.KeyWord);

            IList<Hashtable> attachmentList = ServiceProvider.AttachmentService.GetAttachmentListForPaging(paging);

            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();

            //列表项：文件显示名称、文件大小、文件类型、上传人、上传时间、文件路径
            StringBuilder operate = new StringBuilder();
            foreach (var item in attachmentList)
            {
                string fileSite = item["FileAccessPrefix"].ToString() + item["FileVirtualPath"].ToString();
                string fileId = UtilsHelper.Encrypt(item["FileId"].ToString());
                int fileServerType = int.Parse(item["FileServerType"].ToString());
                int fileSize = int.Parse(item["FileSize"].ToString()) / 1024;
                dtm.aaData.Add(new List<string>());
                if (LoginHelper.LoginUserAuthorize.ContainsKey("/SysManage/Attachment/List".ToLower()))
                {
                    operate.AppendFormat("<a href='/SysManage/Attachment/DownLoad?fileId={0}' target='_blank'>下载</a>&nbsp;|&nbsp;<a href='#' fileId='{0}' fileName='{1}' onclick='fnDeleteAttachment(this);return false;' >删除</a>",  fileId, item["FileName"].ToString());
                }
                dtm.aaData[dtm.aaData.Count - 1].Add(operate.ToString());
                operate.Clear();
                dtm.aaData[dtm.aaData.Count - 1].Add(item["FileName"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(fileSize.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(((UtilConstants.AttachmentType)Convert.ToInt16(item["FileType"])).ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["UserName"] == null ?string.Empty:item["UserName"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(DateTime.Parse(item["CreateDate"].ToString()).ToString("yyyy-MM-dd"));
                dtm.aaData[dtm.aaData.Count - 1].Add(item["FileAccessPrefix"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(item["FileVirtualPath"].ToString());
            }

            JsonResult jr = Json(new
            {
                sEcho = dtm.sEcho,
                iTotalRecords = dtm.iTotalRecords,
                iTotalDisplayRecords = dtm.iTotalDisplayRecords,
                aaData = dtm.aaData
            }, JsonRequestBehavior.AllowGet);

            return jr;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        public ActionResult DeleteAttachment(string fileId)
        {
            try
            {
                fileId = UtilsHelper.Decrypt(fileId);
                ServiceProvider.AttachmentService.DeleteById(int.Parse(fileId));
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileId">文件Id</param>
        public void DownLoad(string fileId)
        {
            fileId = UtilsHelper.Decrypt(fileId);
            AttachmentModel fileMode = ServiceProvider.AttachmentService.GetById(int.Parse(fileId));
            UtilConstants.ServerType type = (UtilConstants.ServerType)fileMode.FileServerType;
            UtilsHelper.FileDownload(fileMode.FileAccessPrefix, fileMode.FileVirtualPath, fileMode.FileName, (UtilConstants.ServerType)fileMode.FileServerType);
        }

        
        
        /// <summary>
        /// 文件批量上传
        /// </summary>
        /// <param name="fileData">文件对象</param>
        /// <param name="targetPath">上传文件路径</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadMutiFile(HttpPostedFileBase fileData, string targetPath)
        {
            string ASPSESSID = Request["ASPSESSID"];
            string retureMsg = "true";
            try
            {
                AttachmentModel model = new AttachmentModel();
                if (UtilConstants.UploadSession == null)
                    UtilConstants.UploadSession = new Hashtable();
                if (!UtilConstants.UploadSession.ContainsKey(ASPSESSID))
                    UtilConstants.UploadSession.Add(ASPSESSID, new List<AttachmentModel>());

                string targetFile = "";
                //Hashtable ht = null;

                //string targetFile = string.Format(@"{0}/", WebUtils.GetSettingsValue("CertificateTemplateFilePath"));
                //Hashtable ht = UtilsHelper.FileUpload(fileData, targetFile, UtilConstants.ServerType.WebFileService);

                //targetFile = string.Format(@"{0}/{1}", targetPath, fileData.FileName);
                //targetFile = string.Format(@"{0}/", targetPath);
                targetFile = string.Format(@"{0}/{1}{2}", targetPath, StrUtils.GetUniqueFileName(null), Path.GetExtension(fileData.FileName));
                model = UtilsHelper.FileUpload(fileData, targetFile, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
                //UserModel user = new UserModel();
                //model = Utils.GetAttachmentModel(ht, user);
                ((List<AttachmentModel>)UtilConstants.UploadSession[ASPSESSID]).Add(model);               
            }
            catch
            {
                retureMsg = "false";
            }
            return Content(retureMsg);
        }

    }
}
