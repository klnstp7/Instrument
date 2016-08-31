using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    /// <summary>
    /// 系统所有上传的文件
    /// </summary>
    public class AttachmentModel
    {
        /// <summary>
        /// 文件标识
        /// </summary>
        public int FileId { set; get; }

        /// <summary>
        /// 文件虚拟地址
        /// </summary>
        public string FileVirtualPath { set; get; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { set; get; }

        /// <summary>
        /// 文件显示名称
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// 上传文件类型
        /// </summary>
        public int FileType { get; set; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { set; get; }

        /// <summary>
        /// 文件服务器类型，Web服务器：1，WebFile服务器：2，FTP：3
        /// </summary>
        public int FileServerType { set; get; }

        /// <summary>
        /// 文件访问前缀
        /// </summary>
        public string FileAccessPrefix { set; get; }

        /// <summary>
        /// 上传人
        /// </summary>
        public int UserId { set; get; }

        /// <summary>
        /// 上传人姓名(冗余)
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
    }
}
