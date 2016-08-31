using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Instrument.Common.Models;
using ToolsLib.Utility.Jquery;
using GRGTCommonUtils;
using Instrument.Business;
using Instrument.Common;
using System.Text;

namespace Instrument.WebSite.Controllers
{
    public class BusinessAttachmentController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BusinessAttachmentController));
        #region ===  获取业务附件列表（编辑页面） ===
        public string GetBusinessAttachmentList(int attachmentType,string bkId)
        {
            IList<BusinessAttachmentModel> attachmentList = new List<BusinessAttachmentModel>();
            if (bkId != "0")
                attachmentList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(attachmentType, UtilsHelper.Decrypt2Int(bkId));
            StringBuilder sb = new StringBuilder();

            sb.Insert(0, "{\"data\":[");
            int count = 0;
            foreach (var item in attachmentList)
            {
                count++;
                sb.AppendFormat("[\"<input type='checkbox' name='chk{0}' value='{1}|{2}|{3}' />\"", item.BusinessType, item.Id, item.FileId, 0);
                sb.AppendFormat(",\"{0}\"", count);
                
                sb.AppendFormat(",\"<a href='/SysManage/Attachment/DownLoad?fileId={0}'> {1}</a>\"", UtilsHelper.Encrypt(item.FileId.ToString()), item.FileName);
                sb.AppendFormat(",\" <input class='long_width' id='Remark{0}' type='text'  value='{1}' />&nbsp;&nbsp;<a href='#' onclick='fnUpdateAttachmentRemark({0})'>更新</a>\"", item.Id, string.IsNullOrWhiteSpace(item.Remark) ? string.Empty : item.Remark.Replace("\r", "").Replace("\n", ""));
                
                sb.Append("],");
            }

            if (attachmentList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");

            return sb.ToString();
        }
        #endregion

        #region ===  获取业务附件列表（详细页面） ===
        public string GetBusinessAttachmentList4Detail(int attachmentType, string bkId)
        {
            IList<BusinessAttachmentModel> attachmentList = new List<BusinessAttachmentModel>();
            if (bkId != "0")
                attachmentList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(attachmentType, UtilsHelper.Decrypt2Int(bkId));
            StringBuilder sb = new StringBuilder();

            sb.Insert(0, "{\"data\":[");
            int count = 0;
            foreach (var item in attachmentList)
            {
                sb.AppendFormat("[\"{0}\"", ++count);
                sb.AppendFormat(",\"<a href='/SysManage/Attachment/DownLoad?fileId={0}'>{1}</a>\"", UtilsHelper.Encrypt(item.FileId.ToString()), item.FileName);
                sb.AppendFormat(",\"{0}\"", string.IsNullOrWhiteSpace(item.Remark) ? string.Empty : item.Remark.Replace("\r", "").Replace("\n", ""));
                
                sb.Append("],");
            }

            if (attachmentList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");

            return sb.ToString();
        }
        #endregion

        public ActionResult UploadBusinessAttachmentList()
        {
            return View();
        }

        /// <summary>
        /// 上传业务附件
        /// </summary>
        /// <param name="businessType"></param>
        /// <param name="businessKeyId"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadBusinessAttachment(int businessType, string businessKeyId, string businessNumber, FormCollection collection)
        {
            BusinessAttachmentModel model = new BusinessAttachmentModel();
            try
            {
                //是否需要自动解压缩
                bool isExtract = collection["isExtract"] == null ? false : true;

                model.BusinessType = businessType;
                model.BusinessKeyId = UtilsHelper.Decrypt2Int(businessKeyId);
                model.UserName = LoginHelper.LoginUser.UserName;   
                //获取后缀名
                string Extension = System.IO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (isExtract)
                {
                    switch (Extension)
                    {
                        case ".zip":
                        case ".bz":
                        case ".gz":
                            ServiceProvider.BusinessAttachmentService.UploadBusinessAttachmentbyExtract(businessNumber, model, Request.Files);
                            break;
                        default:
                            ServiceProvider.BusinessAttachmentService.UploadBusinessAttachment(businessNumber, model, Request.Files);
                            break;
                    }          
                }
                else
                {
                    ServiceProvider.BusinessAttachmentService.UploadBusinessAttachment(businessNumber, model, Request.Files);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Content(ex.Message + "_-1");
               
            }
            return Content("OK_"+model.FileId);
           
        }

        /// <summary>
        /// 更新业务附件的备注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateRemark(int id,string remark)
        {
            try
            {
                BusinessAttachmentModel model = new BusinessAttachmentModel();
                model.Id = id;
                model.Remark =  UtilsHelper.SpecialCharValidate(remark);
                ServiceProvider.BusinessAttachmentService.UpdateRemark(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Content(ex.Message);
            }
            return Content("OK");
        }

       

        /// <summary>
        /// 删除业务附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteBusinessAttachment(string ids)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                    return Content("OK");
                string[] arrId = ids.Split(',');
                foreach (string id in arrId)
                {
                    string[] attachmentInfo = id.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    ServiceProvider.BusinessAttachmentService.DeleteById(Convert.ToInt32(attachmentInfo[0])); 
                    Global.Business.ServiceProvider.AttachmentService.DeleteById(Convert.ToInt32(attachmentInfo[1]));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return Content(ex.Message);
            }
            return Content("OK");
        }

    }
}
