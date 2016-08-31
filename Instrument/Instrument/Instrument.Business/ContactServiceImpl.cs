using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common;
using Instrument.Common.Models;
using GRGTCommonUtils;
using System.Data;
using Global.Common.Models;
using System.Web;
using ToolsLib.Utility;
using System.IO;

namespace Instrument.Business
{
    public class ContactServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int ContactId)
        {
            ContactModel model = ServiceProvider.ContactService.GetById(ContactId);
            if (model == null) return;
            IList<BusinessAttachmentModel> listbusinessAttach = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Constants.AttachmentBusinessType.联络单.GetHashCode(), model.ContactId);
            //如果原来有附件，删除原来的
            if (listbusinessAttach.Count > 0)
            {
                Global.Business.ServiceProvider.AttachmentService.DeleteById(listbusinessAttach[0].FileId);
                ServiceProvider.BusinessAttachmentService.DeleteById(listbusinessAttach[0].Id);
            }
            DBProvider.ContactDAO.DeleteById(ContactId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(ContactModel model, HttpFileCollectionBase Files)
        {
            model.ItemCode = Guid.NewGuid().ToString();
            model.CreatId = LoginHelper.LoginUser.UserId;
            model.Creator = LoginHelper.LoginUser.UserName;

            if (model.ContactId == 0) DBProvider.ContactDAO.Add(model);
            else DBProvider.ContactDAO.Update(model);

            BusinessAttachmentModel businessAttach = null;
            if (Files != null && Files.Count > 0 && Files[0].ContentLength > 0)
            {
                IList<BusinessAttachmentModel> businessAttachList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Constants.AttachmentBusinessType.联络单.GetHashCode(), model.ContactId);
                //如果原来有附件，删除原来的
                if (businessAttachList.Count>0&& businessAttachList.First() != null)
                {
                    businessAttach = businessAttachList.First();
                    Global.Business.ServiceProvider.AttachmentService.DeleteById(businessAttach.FileId);
                    ServiceProvider.BusinessAttachmentService.DeleteById(businessAttach.Id);
                }
                businessAttach = new BusinessAttachmentModel();
                businessAttach.BusinessType = Constants.AttachmentBusinessType.联络单.GetHashCode();
                businessAttach.BusinessKeyId = model.ContactId;
                businessAttach.UserName = LoginHelper.LoginUser.UserName;
                ServiceProvider.BusinessAttachmentService.UploadBusinessAttachment("", businessAttach, Files);
            }
         
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public ContactModel GetById(int ContactId)
        {
            return DBProvider.ContactDAO.GetById(ContactId);
        }

        public void UpdateState(ContactModel model)
        {
            DBProvider.ContactDAO.UpdateState(model);
        }

        public void UpdateFeedback(ContactModel model)
        {
            DBProvider.ContactDAO.UpdateFeedback(model);
        }
        

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<ContactModel> GetAll()
        {
            return DBProvider.ContactDAO.GetAll();
        }
        public IList<Hashtable> GetAllContactListForPaging(PagingModel paging)
        {
            if (!LoginHelper.LoginUserAuthorize.Contains("Contact-ViewAll".ToLower()))
                paging.Where = string.Format(" CreatId={0} and {1}", LoginHelper.LoginUser.UserId, paging.Where);
            return DBProvider.ContactDAO.GetAllContactListForPaging(paging);
        }
        
    }
}
