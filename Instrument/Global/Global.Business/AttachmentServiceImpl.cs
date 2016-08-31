using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using System.Collections;
using ToolsLib.IBatisNet;

using GRGTCommonUtils;

namespace Global.Business
{
    public class AttachmentServiceImpl
    {
        /// <summary>
        /// 删除记录及对应的文件
        /// </summary>
        public void DeleteById(int FileId)
        {
            //先删除文件再删除记录
            AttachmentModel old = DBProvider.AttachmentDAO.GetById(FileId);

            if (old != null)
            {
                switch (old.FileServerType)
                {
                    case (int)GRGTCommonUtils.UtilConstants.ServerType.WebFileService:
                        GRGTCommonUtils.UtilsHelper.DeleteServerFile(old.FileAccessPrefix, old.FileVirtualPath, GRGTCommonUtils.UtilConstants.ServerType.WebFileService);
                        break;
                    case (int)GRGTCommonUtils.UtilConstants.ServerType.FTPService:
                        break;
                    case (int)GRGTCommonUtils.UtilConstants.ServerType.WebService:
                        break;
                    default:
                        break;
                }
            }
            DBProvider.AttachmentDAO.DeleteById(FileId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(AttachmentModel model)
        {
            if (model.FileId == 0) DBProvider.AttachmentDAO.Add(model);
            else DBProvider.AttachmentDAO.Update(model);
        }
        
        

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public AttachmentModel GetById(int FileId)
        {
            return DBProvider.AttachmentDAO.GetById(FileId);
        }
        /// <summary>
        /// 获取多个记录对象.
        /// </summary>
        public IList<AttachmentModel> GetById(int[] FileId)
        {
            return DBProvider.AttachmentDAO.GetByIdAaary(FileId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<AttachmentModel> GetAll()
        {
            return DBProvider.AttachmentDAO.GetAll();
        }

        /// <summary>
        /// 已经附件类型获取数据
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public IList<AttachmentModel> GetByFileType(UtilConstants.AttachmentType fileType)
        {
            return DBProvider.AttachmentDAO.GetByFileType(fileType);
        }

        public IList<Hashtable> GetAttachmentListForPaging(PagingModel paging)
        {
            return DBProvider.AttachmentDAO.GetAttachmentListForPaging(paging);
        }

    }
}
