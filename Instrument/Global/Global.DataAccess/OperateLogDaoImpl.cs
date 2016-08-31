using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using ToolsLib.IBatisNet;
using System.Collections;


namespace Global.DataAccess
{
    public class OperateLogDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(OperateLogModel model)
        {
            DBProvider.dbMapper.Insert("Sys_OperateLog.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(OperateLogModel model)
        {
            DBProvider.dbMapper.Update("Sys_OperateLog.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int LogId)
        {
            DBProvider.dbMapper.Delete("Sys_OperateLog.DeleteById", LogId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public OperateLogModel GetById(int LogId)
        {
            return DBProvider.dbMapper.SelectObject<OperateLogModel>("Sys_OperateLog.GetByID", LogId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<OperateLogModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<OperateLogModel>("Sys_OperateLog.GetAll");
        }

        public IList<OperateLogModel> GetByTargetPKAndType(string targetPK, int targetType)
        {
            OperateLogModel model = new OperateLogModel() { TargetPK = targetPK, TargetType = targetType };
            return DBProvider.dbMapper.SelectList<OperateLogModel>("Sys_OperateLog.GetByTargetPKAndType", model);
        }

        public IList<Hashtable> GetAllOperateLogListForPaging(PagingModel paging)
        {
            paging.TableName = "Sys_OperateLog";
            paging.FieldKey = "LogId";
            if (string.IsNullOrEmpty(paging.FieldShow))
                paging.FieldShow = "OperateType,Operator,OperateDate,OperateIP,OperateContent,TargetPK,TargetType";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "OperateDate desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging2", paging);

            return list;
        }
    }
}
