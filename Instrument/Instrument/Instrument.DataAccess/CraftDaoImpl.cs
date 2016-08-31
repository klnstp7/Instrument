using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class CraftDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(CraftModel model)
        {
            DBProvider.dbMapper.Insert("Craft_BaseInfo.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(CraftModel model)
        {
            DBProvider.dbMapper.Update("Craft_BaseInfo.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int CraftId)
        {
            DBProvider.dbMapper.Delete("Craft_BaseInfo.DeleteById", CraftId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public CraftModel GetById(int CraftId)
        {
            return DBProvider.dbMapper.SelectObject<CraftModel>("Craft_BaseInfo.GetByID", CraftId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<CraftModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<CraftModel>("Craft_BaseInfo.GetAll");
        }


        public virtual IList<Hashtable> GetAllCraftListForPaging(PagingModel paging)
        {
            paging.TableName = "Craft_BaseInfo";
            paging.FieldKey = "CraftId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "CraftId desc";
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);
            return list;
        }
    }
}
