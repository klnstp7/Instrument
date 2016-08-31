using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using Global.DataAccess;

namespace Global.Business
{
    public class Sys_BusinessLogServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int LogId)
        {
            DBProvider.Sys_BusinessLogDao.DeleteById(LogId);
        }

        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(string[] LogId)
        {
            DBProvider.Sys_BusinessLogDao.DeleteByIdArray(LogId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(Sys_BusinessLogModel model)
        {
            if (model.LogId == 0) DBProvider.Sys_BusinessLogDao.Add(model);
            else DBProvider.Sys_BusinessLogDao.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public Sys_BusinessLogModel GetById(int LogId)
        {
            return DBProvider.Sys_BusinessLogDao.GetById(LogId);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public IList<Sys_BusinessLogModel> GetById(string[] LogId)
        {
            return DBProvider.Sys_BusinessLogDao.GetByIdArray(LogId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<Sys_BusinessLogModel> GetAll()
        {
            return DBProvider.Sys_BusinessLogDao.GetAll();
        }

        /// <summary>
        /// 根据外键获取记录
        /// </summary>
        /// <param name="FKValue">外键</param>
        /// <returns></returns>
        public IList<Sys_BusinessLogModel> GetByFKValue(int FKValue, int FKType)
        {
            
            return DBProvider.Sys_BusinessLogDao.GetByFKValue(FKValue, FKType);

        }

        /// <summary>
        /// 根据外键获取记录
        /// </summary>
        /// <param name="FKValue">外键</param>
        /// <returns></returns>
        public IList<Sys_BusinessLogModel> GetByFKValue(string[] FKValue, int FKType)
        {

            List<Sys_BusinessLogModel> businesslist = new List<Sys_BusinessLogModel>();
            int maxLen = 500;
            //确定要循环查询的次数(数组条件的长度/报价单审核 一次可传入的最大查询条件个数)
            int queryCount = FKValue.Length / maxLen + (FKValue.Length % maxLen == 0 ? 0 : 1);
            for (int i = 0; i < queryCount; i++)
            {
                var tempArray = FKValue.Skip(i * maxLen).Take(maxLen).ToList();
                var FKValueArary = FKValue.Select(o => Convert.ToInt32(o.ToString())).ToArray();
                IList<Sys_BusinessLogModel> businesslistmodle = DBProvider.Sys_BusinessLogDao.GetByFKValueAndFKType(FKValueArary, FKType);
                businesslist.AddRange(businesslistmodle);
            }
            return businesslist;
                
                

        }
        /// <summary>
        /// 删除日志及附件
        /// </summary>
        /// <param name="Sys_BusinessLogModelList"></param>
        public void DeleteLogAndAttach(IList<Sys_BusinessLogModel> Sys_BusinessLogModelList)
        {

            if (Sys_BusinessLogModelList.Count > 0)
            {
                var attachIdList = Sys_BusinessLogModelList.Select(o => o.FileId).ToArray();
                var attachList = Global.Business.ServiceProvider.AttachmentService.GetById(attachIdList);
                //删除服务器的附件
                foreach (var attach in attachList)
                {
                    if (attach.FileId != 0)
                    {
                        Global.Business.ServiceProvider.AttachmentService.DeleteById(attach.FileId);
                    }
                }

            }
        }

        /// <summary>
        /// 根据日志记录获取图标
        /// </summary>
        /// <param name="imgUrlEmpty"></param>
        /// <param name="imgUrl"></param>
        /// <param name="businessLogList"></param>
        /// <param name="FKValue"></param>
        /// <returns></returns>
        public string GetIamgeUrl(IList<Sys_BusinessLogModel> businessLogList, int FKValue)
        {

            string imgUrlEmpty = "../Content/themes/webcss/img/Dialogbox-empty.gif";
            string imgUrl = "/Content/themes/webcss/img/Dialogbox.gif";
            foreach (var business in businessLogList)
            {

                if (business != null && business.FKValue.Equals(FKValue))
                {
                    imgUrlEmpty = imgUrl;
                }
            }
            return imgUrlEmpty;
        }
     
    }
}
