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
    public  class AttachmentDaoImplForSQLite : AttachmentDaoImpl
    {
        /// <summary>
        /// 附件分页查询
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public override IList<Hashtable> GetAttachmentListForPaging(PagingModel paging)
        {
            paging.TableName = "Sys_Attachment";
            paging.FieldKey = "FileId";
            if (string.IsNullOrEmpty(paging.FieldShow))
                paging.FieldShow = @"FileId,FileVirtualPath,FileSize,FileName,ServerIP,FileServerType,FileAccessPrefix,UserId,UserName,CreateDate,Remark,FileType";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "CreateDate desc";

            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);

            return list;
        }

    }
}
