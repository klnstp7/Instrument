using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;
using GRGTCommonUtils;

namespace Global.DataAccess
{
    public  class AttachmentDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(AttachmentModel model)
        {
            DBProvider.dbMapper.Insert("Sys_Attachment.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(AttachmentModel model)
        {
            DBProvider.dbMapper.Update("Sys_Attachment.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int FileId)
        {
            DBProvider.dbMapper.Delete("Sys_Attachment.DeleteById", FileId);
        }



        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public AttachmentModel GetById(int FileId)
        {
            return DBProvider.dbMapper.SelectObject<AttachmentModel>("Sys_Attachment.GetByID", FileId);
        }
        /// <summary>
        /// 得到多个对象实体
        /// </summary>
        public IList<AttachmentModel> GetByIdAaary(int[] FileId)
        {
            return DBProvider.dbMapper.SelectList<AttachmentModel>("Sys_Attachment.GetByIdAaary", FileId);
        }


        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<AttachmentModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<AttachmentModel>("Sys_Attachment.GetAll");
        }

        /// <summary>
        /// 已经附件类型获取数据
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public IList<AttachmentModel> GetByFileType(UtilConstants.AttachmentType fileType)
        {
            return DBProvider.dbMapper.SelectList<AttachmentModel>("Sys_Attachment.GetByFileType", (int)fileType);
        }

        /// <summary>
        /// 附件分页查询
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public virtual IList<Hashtable> GetAttachmentListForPaging(PagingModel paging)
        {
            paging.TableName = "Sys_Attachment";
            paging.FieldKey = "FileId";
            if (string.IsNullOrEmpty(paging.FieldShow))
                paging.FieldShow = @"FileId,FileVirtualPath,FileSize,FileName,ServerIP,FileServerType,FileAccessPrefix,UserId,UserName,CreateDate,Remark,FileType";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "CreateDate desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }

    }
}
