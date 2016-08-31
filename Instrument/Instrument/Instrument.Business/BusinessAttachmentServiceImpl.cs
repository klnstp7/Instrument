using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using Instrument.Common.Models;
using Instrument.DataAccess;
using Instrument.Common;
using ToolsLib.Utility;
using System.Web;
using GRGTCommonUtils;
using System.IO;

namespace Instrument.Business
{
    public class BusinessAttachmentServiceImpl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BusinessAttachmentServiceImpl));
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int Id)
        {
            //找出记录
            BusinessAttachmentModel model = GetById(Id);
            if (model != null)
            {
                Global.Business.ServiceProvider.AttachmentService.DeleteById(model.FileId);
            }
            DBProvider.BusinessAttachmentDao.DeleteById(Id);
        }
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int Id, int fileId)
        {
            DBProvider.BusinessAttachmentDao.DeleteById(Id);
            //
            Global.Business.ServiceProvider.AttachmentService.DeleteById(fileId);
        }
        /// <summary>
        /// 根据businessKeyId和businessType删除记录
        /// </summary>
        public void DeleteByKeyIdAndType(int businessKeyId, string businessType)
        {
            IList<BusinessAttachmentModel> list = DBProvider.BusinessAttachmentDao.GetByBusinessId(businessKeyId, businessType);
            foreach (BusinessAttachmentModel b in list)
            {
                DBProvider.BusinessAttachmentDao.DeleteById(b.Id);
                Global.Business.ServiceProvider.AttachmentService.DeleteById(b.FileId);
            }
        }

        /// <summary>
        /// 批量删除.
        /// </summary>
        public void DeleteByIds(string ids)
        {
            DBProvider.BusinessAttachmentDao.DeleteByIds(ids);
        }

        /// <summary>
        /// 更新备注
        /// </summary>
        public void UpdateRemark(BusinessAttachmentModel model)
        {
            DBProvider.BusinessAttachmentDao.UpdateRemark(model);
        }
        /// <summary>
        /// 更新已申请受控
        /// </summary>
        public void UpdateApplyControlled(int id)
        {
            DBProvider.BusinessAttachmentDao.UpdateApplyControlled(id);
        }


        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(BusinessAttachmentModel model)
        {
            if (model.Id == 0) DBProvider.BusinessAttachmentDao.Add(model);
            else DBProvider.BusinessAttachmentDao.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public BusinessAttachmentModel GetById(int Id)
        {
            return DBProvider.BusinessAttachmentDao.GetById(Id);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<BusinessAttachmentModel> GetAll()
        {
            return DBProvider.BusinessAttachmentDao.GetAll();
        }

        /// <summary>
        /// 获取某业务类型下的某张单的所有记录对象.
        /// <param name="businessId"></param>
        /// <param name="businessType"></param>
        /// </summary>
        public IList<BusinessAttachmentModel> GetByBusinessId(int businessKeyId, string businessType)
        {
            return DBProvider.BusinessAttachmentDao.GetByBusinessId(businessKeyId, businessType);
        }
        /// <summary>
        /// 获取某业务类型下的某张单的所有记录对象.
        /// <param name="businessId"></param>
        /// <param name="businessType"></param>
        /// </summary>
        public BusinessAttachmentModel GetByBusinesId(int businessKeyId, string businessType)
        {
            return DBProvider.BusinessAttachmentDao.GetByBusinesId(businessKeyId, businessType);
        }
        /// <summary>
        /// 获取已经申请受控，待审核的记录 
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public IList<BusinessAttachmentModel> GetVerifyList()
        {
            return DBProvider.BusinessAttachmentDao.GetVerifyList();
        }

        /// <summary>
        /// 获取某业务类型下的某张单的所有记录对象.
        /// <param name="businessId"></param>
        /// <param name="businessType"></param>
        /// </summary>
        public IList<BusinessAttachmentModel> GetByBusinessTypeAndId(int businessType, int businessId)
        {
            return DBProvider.BusinessAttachmentDao.GetByBusinessTypeAndId(businessType, businessId);
        }

        /// <summary>
        /// 获取某业务类型下的某张单的附件存放路径.
        /// <param name="businessType"></param>
        /// <param name="businessNumber"></param>
        /// </summary>
        public string GetByBusinessTypeAndId(Constants.AttachmentBusinessType businessType, string businessNumber)
        {
            string returnPath = string.Empty;
            StringBuilder pathBuilder = new StringBuilder();
            switch (businessType)
            {
                case Constants.AttachmentBusinessType.联络单:
                    pathBuilder.Append(WebUtils.GetSettingsValue("AttachmentPathContact"));
                    break;
                case Constants.AttachmentBusinessType.维修单:
                    pathBuilder.Append(WebUtils.GetSettingsValue("AttachmentInstrumentRepair"));
                    break;
                case Constants.AttachmentBusinessType.设备档案:
                    pathBuilder.Append(WebUtils.GetSettingsValue("InstrumentAttachmentFilePath"));
                    break;
                case Constants.AttachmentBusinessType.仪器照片:
                    pathBuilder.Append(WebUtils.GetSettingsValue("InstrumentPicFilePath"));
                    break;
                case Constants.AttachmentBusinessType.内部核查:
                    pathBuilder.Append(WebUtils.GetSettingsValue("InnerCheckAttachmentFilePath"));
                    break;
                case Constants.AttachmentBusinessType.期间核查:
                    pathBuilder.Append(WebUtils.GetSettingsValue("PeriodcheckAttachmentFilePath"));
                    break;
                case Constants.AttachmentBusinessType.本地知识库:
                    pathBuilder.Append(WebUtils.GetSettingsValue("KnowledgeOtherFilesPath"));
                    break;
                default:
                    break;
            }
            if (businessNumber.Length >= 13)
            {
                returnPath = UtilsHelper.CreateDir(pathBuilder.ToString() + "/", businessNumber.Replace("H", ""));
            }
            else
                returnPath = pathBuilder.Append("/").ToString();

            //log.InfoFormat("BusinessType:{0}; BusinessNumber:{1}; SavePath:{2}", businessType, businessNumber, returnPath);

            return returnPath;
        }

        /// <summary>
        /// 上传业务附件
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <param name="model"></param>
        /// <param name="Files"></param>
        public void UploadBusinessAttachment(string businessNumber, BusinessAttachmentModel model, HttpFileCollectionBase Files)
        {
            AttachmentModel attachment = new AttachmentModel();
            string targetDir = GetByBusinessTypeAndId((Constants.AttachmentBusinessType)model.BusinessType, businessNumber);


            //默认随机名
            string fileName = Guid.NewGuid().ToString();

            string targetFile = string.Format(@"{0}{1}{2}", targetDir, fileName, System.IO.Path.GetExtension(Files[0].FileName));
            
            //原始记录上传操作
            attachment = UtilsHelper.FileUpload(Files[0].InputStream, Files[0].FileName, targetFile, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));

            //入库
            Global.Business.ServiceProvider.AttachmentService.Save(attachment);
            model.FileId = attachment.FileId;
            model.FileName = attachment.FileName;

            Save(model);

        }

        

        /// <summary>
        /// 上传业务附件-自动解压 Add by Raven 2014-12-31 
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <param name="model"></param>
        /// <param name="Files"></param>
        public void UploadBusinessAttachmentbyExtract(string businessNumber, BusinessAttachmentModel model, HttpFileCollectionBase Files)
        {
            string targetDir = GetByBusinessTypeAndId((Constants.AttachmentBusinessType)model.BusinessType, businessNumber);
            //获取后缀名
            string Extension = System.IO.Path.GetExtension(Files[0].FileName).ToLower();
            ToolsLib.FileService.FileCompress fc = new ToolsLib.FileService.FileCompress();
            //解压包的路径
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + Extension);
            //保存
            Files[0].SaveAs(newFile);
            string ExtractDir = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString());
            string ext = fc.Extract(newFile, ExtractDir);
            //解压后删除临时文件
            File.Delete(newFile);
            //获取目录下的文件     
            string[] fileList = Directory.GetFiles(ExtractDir, "*", SearchOption.AllDirectories); 

            AttachmentModel attachment = null;
            BusinessAttachmentModel newmodel = null;
            foreach (var file in fileList)
            {
                attachment = new AttachmentModel();
                newmodel = new BusinessAttachmentModel();
                newmodel.BusinessKeyId = model.BusinessKeyId;
                newmodel.BusinessType = model.BusinessType;
                newmodel.UserName = LoginHelper.LoginUser.UserName;
                string fileName = Guid.NewGuid().ToString();
                string targetFileName = string.Format(@"{0}{1}{2}", targetDir, fileName, Path.GetExtension(file));

                //原始记录上传操作
                attachment = UtilsHelper.FileUpload(file, targetFileName, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
                //入库
                Global.Business.ServiceProvider.AttachmentService.Save(attachment);
                newmodel.FileId = attachment.FileId;
                newmodel.FileName = attachment.FileName;
                Save(newmodel);
                //删除临时文件
                File.Delete(file);

            }
            //删除目录
            Directory.Delete(ExtractDir,true);

        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IList<Hashtable> GetInstrumentManualForPaging(PagingModel paging)
        {
            return DBProvider.BusinessAttachmentDao.GetInstrumentManualForPaging(paging);
        }
    }
}
