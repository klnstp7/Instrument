using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class Sys_BusinessLogDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(Sys_BusinessLogModel model)
        {
            DBProvider.dbMapper.Insert("Sys_BusinessLog.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(Sys_BusinessLogModel model)
        {
            DBProvider.dbMapper.Update("Sys_BusinessLog.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int LogId)
        {
            DBProvider.dbMapper.Delete("Sys_BusinessLog.DeleteById", LogId);
        }


        /// <summary>
        /// 删除多条数据.
        /// </summary>
        public void DeleteByIdArray(string[] LogId)
        {
            DBProvider.dbMapper.Delete("Sys_BusinessLog.DeleteByIdArray", LogId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public Sys_BusinessLogModel GetById(int LogId)
        {
            return DBProvider.dbMapper.SelectObject<Sys_BusinessLogModel>("Sys_BusinessLog.GetByID", LogId);
        }

        /// <summary>
        /// 得到多个对象实体.
        /// </summary>
        public IList<Sys_BusinessLogModel> GetByIdArray(string[] LogId)
        {
            return DBProvider.dbMapper.SelectList<Sys_BusinessLogModel>("Sys_BusinessLog.GetByIdArray", LogId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<Sys_BusinessLogModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<Sys_BusinessLogModel>("Sys_BusinessLog.GetAll");
        }

        /// <summary>
        /// 根据外键获取记录
        /// </summary>
        /// <param name="FKValue">外键</param>
        /// <returns></returns>
        public IList<Sys_BusinessLogModel> GetByFKValue(int FKValue, int FKType)
        {
            Hashtable table = new Hashtable();
            table["FKValue"] = FKValue;
            table["FKType"]=FKType;
            return DBProvider.dbMapper.SelectList<Sys_BusinessLogModel>("Sys_BusinessLog.GetByFKValue", table);
        }

        /// <summary>
        /// 根据外键获取记录
        /// </summary>
        /// <param name="FKValue">外键</param>
        /// <returns></returns>
        public IList<Sys_BusinessLogModel> GetByFKValueAndFKType(int[] FKValue, int FKType)
        {
            if (FKValue == null || FKValue.Length == 0) return new List<Sys_BusinessLogModel>() ;


            Hashtable hash = new Hashtable();
            hash.Add("businessList", FKValue);
            hash.Add("FKType", FKType);
            return DBProvider.dbMapper.SelectList<Sys_BusinessLogModel>("Sys_BusinessLog.GetByFKValueAndFKType", hash);
        }



    }
}
