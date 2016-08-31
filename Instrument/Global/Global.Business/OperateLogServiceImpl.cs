using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using Global.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;


namespace Global.Business
{
    public class OperateLogServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int LogId)
        {
            DBProvider.OperateLogDao.DeleteById(LogId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(OperateLogModel model)
        {
            model.OperateIP = ToolsLib.Utility.WebUtils.ClientIP;
            if (model.LogId == 0) DBProvider.OperateLogDao.Add(model);
            else DBProvider.OperateLogDao.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public OperateLogModel GetById(int LogId)
        {
            return DBProvider.OperateLogDao.GetById(LogId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<OperateLogModel> GetAll()
        {
            return DBProvider.OperateLogDao.GetAll();
        }

        public IList<OperateLogModel> GetByTargetPKAndType(string targetPK,int targetType)
        {
            return DBProvider.OperateLogDao.GetByTargetPKAndType(targetPK, targetType);
        }

        public IList<Hashtable> GetAllOperateLogListForPaging(PagingModel paging)
        {
            return DBProvider.OperateLogDao.GetAllOperateLogListForPaging(paging);
        }
    }
}
