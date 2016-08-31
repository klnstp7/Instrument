using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Instrument.Common.Models;
using ToolsLib.Utility;
using GRGTCommonUtils;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using Instrument.DataAccess;
using System.Collections;
using System.IO;
using System.Net;



namespace Instrument.Business
{
    public class KnowledgesServiceImpl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(KnowledgesServiceImpl));

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <param name="KnowledgeId"></param>
        public void DeleteById(int KnowledgeId)
        {
            DBProvider.KnowledgesDao.DeleteById(KnowledgeId);
        }

        /// <summary>
        /// 批量删除知识库
        /// </summary>
        /// <param name="knowledgeIdList"></param>
        public void DeleteByIdList(IList<int> knowledgeIdList)
        {
            IList<KnowledgesModel> knowledgeList = ServiceProvider.KnowledgesService.GetByIdList(knowledgeIdList);
            if (knowledgeList.Count == 0) return;
            foreach (KnowledgesModel model in knowledgeList)
            {
                Global.Business.ServiceProvider.AttachmentService.DeleteById(model.FileId);//删除PDF附件
                IList<BusinessAttachmentModel> businessList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Instrument.Common.Constants.AttachmentBusinessType.本地知识库.GetHashCode(), model.KnowledgeId);
                foreach (BusinessAttachmentModel item in businessList)//删除业务附件
                {
                    ServiceProvider.BusinessAttachmentService.DeleteById(item.Id);
                }
            }
            DBProvider.KnowledgesDao.DeleteByIdList(knowledgeIdList);
        }

        /// <summary>
        /// 通过ID更新状态
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="state"></param>
        public void UpdateStateByIdList(IList<int> idList, int state)
        {
            DBProvider.KnowledgesDao.UpdateStateByIdList(idList, state);
        }

        /// <summary>
        ///保存知识库
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Files"></param>
        public void Save(KnowledgesModel model, HttpFileCollectionBase Files)
        {
            if (Files.Count > 0 && Files[0].ContentLength > 0)
            {
                int fileServerType = Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType"));
                HttpPostedFileBase file = Files[0];
                string targetPath = WebUtils.GetSettingsValue("KnowledgesPDFFilePath");
                string SwfFilePath = WebUtils.GetSettingsValue("KnowledgesSwfFilePath");
                string targetFileName = StrUtils.GetUniqueFileName(null);
                string targetFile = string.Format(@"{0}/{1}{2}", targetPath, targetFileName, Path.GetExtension(Files[0].FileName));
                string targetSwfPath, pdfFilestr, srcSwfFile, tempFileName;
                AttachmentModel attModel = null;
                Stream fileStream = null;
                try
                {
                    attModel = UtilsHelper.FileUpload(WebUtils.GetSettingsValue("WebFileServer"), Files[0], targetFile, (UtilConstants.ServerType)fileServerType);
                    fileStream = UtilsHelper.FileDownload(WebUtils.GetSettingsValue("WebFileServer"), attModel.FileVirtualPath, (UtilConstants.ServerType)fileServerType);
                    tempFileName = Guid.NewGuid().ToString();
                    pdfFilestr = CommonUtils.GetPhysicsPath(string.Format("{0}/{1}.pdf", "/tempFile", tempFileName));
                    ToolsLib.FileService.NormalFile.SaveInfoToFile(fileStream, pdfFilestr);
                    srcSwfFile = UtilsHelper.PdfToSwf(pdfFilestr, tempFileName + ".swf");
                    targetSwfPath = string.Format("{0}/{1}.swf", Path.GetDirectoryName(attModel.FileVirtualPath).Replace("Pdf", "Swf"), Path.GetFileNameWithoutExtension(attModel.FileVirtualPath));
                    if (fileServerType == 1)
                    {
                        fileStream = new FileStream(srcSwfFile, FileMode.Open);
                        ToolsLib.FileService.NormalFile.SaveInfoToFile(fileStream, targetSwfPath);
                    }
                    else
                        UtilsHelper.FileUpload(WebUtils.GetSettingsValue("WebFileServer"), srcSwfFile, targetSwfPath, (UtilConstants.ServerType)fileServerType);
                    File.Delete(pdfFilestr);
                    File.Delete(srcSwfFile);
                }
                catch (Exception e)
                { }
                attModel.FileType = (int)Instrument.Common.Constants.AttachmentBusinessType.本地知识库.GetHashCode();
                attModel.UserId = LoginHelper.LoginUser.UserId;
                attModel.UserName = LoginHelper.LoginUser.UserName;
                if (model.FileId != 0)//重新上传删除原来文件
                    Global.Business.ServiceProvider.AttachmentService.DeleteById(model.FileId);
                Global.Business.ServiceProvider.AttachmentService.Save(attModel);
                model.FileId = attModel.FileId;//新文件位置
            }
            if (model.KnowledgeId == 0)
            {
                model.Creator = LoginHelper.LoginUser.UserName;
                DBProvider.KnowledgesDao.Add(model);
            }
            else DBProvider.KnowledgesDao.Update(model);
        }

        /// <summary>
        /// 通过ID获取知识库
        /// </summary>
        /// <param name="KnowledgeId"></param>
        /// <returns></returns>
        public KnowledgesModel GetById(int KnowledgeId)
        {
            return DBProvider.KnowledgesDao.GetById(KnowledgeId);
        }

        /// <summary>
        ///通过ID批量获取知识库
        /// </summary>
        /// <param name="KnowledgeIdList"></param>
        /// <returns></returns>
        public IList<KnowledgesModel> GetByIdList(IList<int> KnowledgeIdList)
        {
            return DBProvider.KnowledgesDao.GetByIdList(KnowledgeIdList);
        }

       /// <summary>
       /// 获取所有的知识库
       /// </summary>
       /// <returns></returns>
        public IList<KnowledgesModel> GetAll()
        {
            return DBProvider.KnowledgesDao.GetAll();
        }

        /// <summary>
        /// 获取分页的知识库
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IList<Hashtable> GetAllKnowledgesListForPaging(PagingModel paging)
        {
            return DBProvider.KnowledgesDao.GetAllKnowledgesListForPaging(paging);
        }

        /// <summary>
        ///  根据本地知识库标识获取知识库信息和业务附件表信息、PDF文件地址信息
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public KnowledgesModel GetKnowledgeDetailInfo(int knowledgeId)
        {
            KnowledgesModel model  = ServiceProvider.KnowledgesService.GetById(knowledgeId);
            //组装业务附件表信息
            IList<BusinessAttachmentModel> attachList = ServiceProvider.BusinessAttachmentService.GetByBusinessTypeAndId(Instrument.Common.Constants.AttachmentBusinessType.本地知识库.GetHashCode(), knowledgeId);
            if (attachList.Count > 0)
            {
                model.businessAttachList = new List<Hashtable>();
                foreach (BusinessAttachmentModel item in attachList)
                {
                    Hashtable ht = new Hashtable();
                    AttachmentModel attach =Global.Business.ServiceProvider.AttachmentService.GetById(item.FileId);
                    ht["FileId"] = attach.FileId;
                    ht["FileName"] = attach.FileName;
                    model.businessAttachList.Add(ht);
                }
            }
            //组装flash文件地址信息
            if (model.FileId != 0)
            {
                AttachmentModel attach = Global.Business.ServiceProvider.AttachmentService.GetById(model.FileId);
                model.swfFileByte = new byte[] { };
                if (attach != null)
                {
                    string filepath;
                    filepath = string.Format("{0}/{1}.swf", Path.GetDirectoryName(attach.FileVirtualPath).Replace("Pdf", "Swf"), Path.GetFileNameWithoutExtension(attach.FileVirtualPath));
                    Stream stream = UtilsHelper.FileDownload(attach.FileAccessPrefix, filepath, (UtilConstants.ServerType)attach.FileServerType);
                    int count = Convert.ToInt32(stream.Length);
                    Byte[] bytes = new Byte[count];
                    stream.Read(bytes, 0, count);
                    model.swfFileByte = bytes;
                }
            }
            return model;
        }
    }
}