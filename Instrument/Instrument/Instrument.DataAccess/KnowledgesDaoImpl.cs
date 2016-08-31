using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;

namespace Instrument.DataAccess
{
    public class KnowledgesDaoImpl
    {

        public IList<Hashtable> GetAllKnowledgesListForPaging(PagingModel paging)
        {
            paging.TableName = "Knowledges";
            paging.FieldKey = "KnowledgeId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"KnowledgeId ,Abstract ,State ,FileId ,Creator ,CreatDate ,KType ,Title";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "KnowledgeId desc";
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }


        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(KnowledgesModel model)
        {
            DBProvider.dbMapper.Insert("Knowledges.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(KnowledgesModel model)
        {
            DBProvider.dbMapper.Update("Knowledges.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int KnowledgeId)
        {
            DBProvider.dbMapper.Delete("Knowledges.DeleteById", KnowledgeId);
        }

        public void DeleteByIdList(IList<int> knowledgeIdList)
        {
            DBProvider.dbMapper.Delete("Knowledges.DeleteByIdList", knowledgeIdList);
        }

        public void UpdateStateByIdList(IList<int> idList, int state)
        {
            Hashtable ht = new Hashtable();
            ht["KnowledgeIdList"] = idList;
            ht["State"] = state;
            DBProvider.dbMapper.Update("Knowledges.UpdateStateByIdList", ht);
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public KnowledgesModel GetById(int KnowledgeId)
        {
            return DBProvider.dbMapper.SelectObject<KnowledgesModel>("Knowledges.GetByID", KnowledgeId);
        }

        public IList<KnowledgesModel> GetByIdList(IList<int> KnowledgeIdList)
        {
            return DBProvider.dbMapper.SelectList<KnowledgesModel>("Knowledges.GetByIdList", KnowledgeIdList);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<KnowledgesModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<KnowledgesModel>("Knowledges.GetAll");
        }

        public IList<Hashtable> GetKnowledgesCountByType(int state)
        {
            return DBProvider.dbMapper.SelectList<Hashtable>("Knowledges.GetCountByKType", state);
        }
        
      
    }
}
