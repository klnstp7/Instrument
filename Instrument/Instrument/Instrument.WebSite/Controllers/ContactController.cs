using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using System.Collections;
using Global.Common.Models;
using System.Text;
using GRGTCommonUtils;
using Instrument.Business;
using Instrument.Common.Models;
using Instrument.Common;
using System.IO;

namespace Instrument.WebSite.Controllers
{
    public class ContactController : Controller
    {
        #region 数据列表
        public ActionResult ContactList()
        {
            ViewBag.State = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new Common.Constants.ContactState()).ToString();
            ViewBag.CaseType = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(Common.Constants.SysParamType.ContactCaseType).ToString();
            return View();
        }

        public JsonResult GetAllContactJsonData()
        {
            //查询标准器具
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = @"ContactId,CompanyName,CaseType,Abstract,State,ContactContent,FeedbackContent,FeedbackDate,Creator,CreatId,CreatDate,ItemCode";
            paging.Where = string.IsNullOrWhiteSpace(dtm.FieldCondition) ? dtm.FieldCondition = "1=1" : dtm.FieldCondition;
            IList<Hashtable> contractList = Instrument.Business.ServiceProvider.ContactService.GetAllContactListForPaging(paging);
            ////系统参数
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel pCaseType = paramList.SingleOrDefault(p => p.ParamCode.Equals(Common.Constants.SysParamType.ContactCaseType));
            if (pCaseType == null) pCaseType = new ParamModel();
            ParamItemModel itemCaseType = null;

            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sbOperate = new StringBuilder();
            foreach (var item in contractList)
            {
                dtm.aaData.Add(new List<string>());
                sbOperate.Clear();    //操作
                sbOperate.AppendFormat("<div contactId='{0}' >", UtilsHelper.Encrypt(item["ContactId"].ToString()));
                int state=Convert.ToInt32(item["State"]);
                if (state.Equals(Common.Constants.ContactState.草稿.GetHashCode()))
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("新增联络单-修改"))
                    {
                        sbOperate.Append("<a href='#' onclick='fnEditContact(this);'>编辑</a>&nbsp;&nbsp;");
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Contact/Delete".ToLower()))
                    {
                        sbOperate.Append("<a href='#' onclick='fnDeleteContact(this);'>删除</a>&nbsp;&nbsp;");
                    }
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Contact/Send".ToLower()))
                    {
                        sbOperate.Append("<a href='#' onclick='fnSendContact(this);'>提交</a>&nbsp;&nbsp;");
                    }
                }
                if (!state.Equals(Common.Constants.ContactState.草稿.GetHashCode()))
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Contact/Detail".ToLower()))
                    {
                        sbOperate.Append("<a href='#' onclick='fnViewContact(this);'>查看</a>&nbsp;&nbsp;");
                    }
                }
                if (state.Equals(Common.Constants.ContactState.已提交.GetHashCode()))
                {
                 
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Contact/SynContact".ToLower()))
                    {
                        sbOperate.Append("<a href='#' onclick='fnSynContact(this);'>更新同步</a>&nbsp;&nbsp;");
                    }
                }
                if (state.Equals(Common.Constants.ContactState.已反馈.GetHashCode()))
                {
                    if (LoginHelper.LoginUserAuthorize.ContainsKey("/Contact/Solve".ToLower()))
                    {
                        sbOperate.Append("<a href='#' onclick='fnSolveContact(this);'>解决</a>&nbsp;&nbsp;");
                    }
                }
                sbOperate.Append("</div>");
                dtm.aaData[dtm.aaData.Count - 1].Add(sbOperate.ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["CompanyName"]));
                //事项分类
                itemCaseType = pCaseType.itemsList.SingleOrDefault(p => p.ParamItemValue.Equals(item["CaseType"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", itemCaseType == null ? "" : itemCaseType.ParamItemName));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Abstract"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(((Constants.ContactState)Convert.ToInt32(item["State"])).ToString());    //状态
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["FeedbackDate"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item["Creator"])); 
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd}", item["CreatDate"]));
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
        #endregion

        #region 同步更新

        public ActionResult SynContact(string contactId)
        {
            ContactModel model = ServiceProvider.ContactService.GetById(UtilsHelper.Decrypt2Int(contactId));
            //获取客户关系数据
            string jsonData = WSProvider.EbusinessProvider.GetContactByItemCode(model.ItemCode, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            if (dic["Msg"].ToString() == "OK")
            {
                Hashtable temContact = ToolsLib.Utility.CommonUtils.JsonDeserialize(dic["Data"].ToString(), typeof(Hashtable)) as Hashtable;
                model.FeedbackContent =string.Format("{0}", temContact["FeedbackContent"]);
                if(temContact["FeedbackDate"]==null)
                    model.FeedbackDate =null;
                else
                    model.FeedbackDate = Convert.ToDateTime(temContact["FeedbackDate"]);
                model.State =Convert.ToInt32(temContact["State"]);
                ServiceProvider.ContactService.UpdateFeedback(model);
            }
            return Content(dic["Msg"].ToString());
        }

        #endregion


        #region 编辑、保存
        public ActionResult Edit(string contactId)
        {
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //事项分类
            Global.Common.Models.ParamModel caseType = paramList.SingleOrDefault(m => m.ParamCode.Equals(Constants.SysParamType.ContactCaseType));
            ViewBag.CaseType = new SelectList(caseType.itemsList, "ParamItemValue", "ParamItemName");

            //公司名称
            Global.Common.Models.ParamModel company = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.CompanyInfo);
            if (null == company) company = new Global.Common.Models.ParamModel();
            Global.Common.Models.ParamItemModel companyInfo = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司名称"));

            ContactModel model =null;
            if (contactId == "0")
            {
                model = new ContactModel();
                model.CompanyName = companyInfo == null ? "" : companyInfo.ParamItemValue;
            }
            else
            {
                model = ServiceProvider.ContactService.GetById(UtilsHelper.Decrypt2Int(contactId));
                IList<BusinessAttachmentModel> businessAttachList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Constants.AttachmentBusinessType.联络单.GetHashCode(), model.ContactId);
                if (businessAttachList.Count > 0 && businessAttachList.First() != null)
                {
                    ViewBag.FileId =UtilsHelper.Encrypt( businessAttachList.First().FileId.ToString());
                    ViewBag.FileName = businessAttachList.First().FileName;
                }
            }
            return View(model);
        }

        public ActionResult Save(ContactModel model, FormCollection collection)
        {
            ServiceProvider.ContactService.Save(model,Request.Files);
            return Json(new { Msg = "OK" }, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region 查看详细
        public ActionResult Detail(string contactId)
        {
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ContactModel model = ServiceProvider.ContactService.GetById(UtilsHelper.Decrypt2Int(contactId));
            //事项分类
            Global.Common.Models.ParamModel caseType = paramList.SingleOrDefault(m => m.ParamCode.Equals(Constants.SysParamType.ContactCaseType));
            ParamItemModel item = caseType.itemsList.SingleOrDefault(p => p.ParamItemValue.Equals(model.CaseType.ToString()));
            ViewBag.CaseType = item.ParamItemName;
            IList<BusinessAttachmentModel> businessAttachList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Constants.AttachmentBusinessType.联络单.GetHashCode(), model.ContactId);
            if (businessAttachList.Count > 0 && businessAttachList.First() != null)
            {
                ViewBag.FileId = UtilsHelper.Encrypt(businessAttachList.First().FileId.ToString());
                ViewBag.FileName = businessAttachList.First().FileName;
            }
            return View(model);
        }
        #endregion

        #region 提交
        public ActionResult Send(string contactId)
        {
            ContactModel model = ServiceProvider.ContactService.GetById(UtilsHelper.Decrypt2Int(contactId));

            //同步到业务系统
            IList<BusinessAttachmentModel> businessAttachList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Constants.AttachmentBusinessType.联络单.GetHashCode(), model.ContactId);
            byte[] fileData = new byte[0];
            string fileName = "";
            AttachmentModel attach = null;
            if (businessAttachList.Count > 0 && businessAttachList.First() != null)
            {
                attach = Global.Business.ServiceProvider.AttachmentService.GetById(businessAttachList.First().FileId);
                if (attach != null)
                {
                    Stream fileStream = UtilsHelper.FileDownload(attach.FileAccessPrefix, attach.FileVirtualPath, (UtilConstants.ServerType)attach.FileServerType);
                    fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, (int)fileStream.Length);
                    fileName = attach.FileName + Path.GetExtension(attach.FileVirtualPath);
                }
            }
            model.State = Constants.ContactState.已提交.GetHashCode();
            
            IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel pCaseType = paramList.SingleOrDefault(p => p.ParamCode.Equals(Common.Constants.SysParamType.ContactCaseType));
            if (pCaseType == null) pCaseType = new ParamModel();
            ParamItemModel itemCaseType = null;
            itemCaseType = pCaseType.itemsList.SingleOrDefault(c => c.ParamItemValue.Equals(model.CaseType.ToString()));
            model.CaseTypeStr = itemCaseType == null ? "" : itemCaseType.ParamItemName;

            string jsonData = WSProvider.EbusinessProvider.SynInsertContact(ToolsLib.Utility.CommonUtils.JsonSerialize(model), fileData, fileName, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
            Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            if (dic["Msg"].ToString() == "OK")
            {
                ServiceProvider.ContactService.UpdateState(model);
            }
            return Content(dic["Msg"].ToString());
        }
        #endregion        
        
        #region 解决
        public ActionResult Solve(string contactId)
        {
            ContactModel model = new ContactModel();
            model.ContactId = UtilsHelper.Decrypt2Int(contactId);
            model.State = Constants.ContactState.已解决.GetHashCode();
            ServiceProvider.ContactService.UpdateState(model);

            return Content("OK");
        }
        #endregion


        #region 删除
        public ActionResult Delete(string contactId)
        {
            ServiceProvider.ContactService.DeleteById(UtilsHelper.Decrypt2Int(contactId));
            return Content("OK");
        }
        #endregion
    }
}
