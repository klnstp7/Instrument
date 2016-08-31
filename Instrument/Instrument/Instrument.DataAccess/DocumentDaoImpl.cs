using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class DocumentDaoImpl
    {
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.dbMapper.Delete("Document_BaseInfo.DeleteByInstrumentId", InstrumentId);
        }

        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(DocumentModel model)
        {
            DBProvider.dbMapper.Insert("Document_BaseInfo.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(DocumentModel model)
        {
            DBProvider.dbMapper.Update("Document_BaseInfo.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int DocumentId)
        {
            DBProvider.dbMapper.Delete("Document_BaseInfo.DeleteById", DocumentId);
        }
        public void DeleteInstrumentOwnDocument(int documentId,int instrumentId)
        {
            Hashtable ht = new Hashtable();
            ht["DocumentId"] = documentId;
            ht["InstrumentId"] =instrumentId;
            DBProvider.dbMapper.Delete("Document_BaseInfo.DeleteInstrumentOwnDocument", ht);
        }

        public void DeleteInstrumentOwnDocumentByDocumentId(int documentId)
        {
            Hashtable ht = new Hashtable();
            ht["DocumentId"] = documentId;
            DBProvider.dbMapper.Delete("Document_BaseInfo.DeleteInstrumentOwnDocumentByDocumentId", ht);
        }

        public void AddOwnDocument(int documentId, int instrumentId)
        {
            Hashtable ht = new Hashtable();
            ht["DocumentId"] = documentId;
            ht["InstrumentId"] =instrumentId;
            DBProvider.dbMapper.Insert("Document_BaseInfo.AddOwnDocument", ht);
        }

        public Hashtable IsExitDocument(int documentId, int instrumentId)
        {
            Hashtable ht = new Hashtable();
            ht["DocumentId"] = documentId;
            ht["InstrumentId"] =instrumentId;
            return DBProvider.dbMapper.SelectObject<Hashtable>("Document_BaseInfo.IsExitDocument", ht);
        }

        public Hashtable GetByDocumentId(int documentId)
        {
            Hashtable ht = new Hashtable();
            ht["DocumentId"] = documentId;
            return DBProvider.dbMapper.SelectObject<Hashtable>("Document_BaseInfo.GetByDocumentId", ht);
        }
        


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public DocumentModel GetById(int DocumentId)
        {
            return DBProvider.dbMapper.SelectObject<DocumentModel>("Document_BaseInfo.GetByID", DocumentId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<DocumentModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<DocumentModel>("Document_BaseInfo.GetAll");
        }

        public IList<DocumentModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.dbMapper.SelectList<DocumentModel>("Document_BaseInfo.GetByInstrumentId", instrumentId);
        }
        


    }
}
