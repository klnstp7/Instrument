using System;
using System.Collections;
using System.Collections.Generic;
using Instrument.Common.Models;
using Instrument.DataAccess;

namespace Instrument.Business
{
    /// <summary>
    /// 体系文件关联的仪器
    /// </summary>
    public class DocumentServiceImpl
    {
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.DocumentDAO.DeleteByInstrumentId(InstrumentId);
        }

        /// <summary>
        /// 删除记录.体系文件
        /// </summary>
        public void DeleteById(int DocumentId)
        {
            DeleteInstrumentOwnDocumentByDocumentId(DocumentId);
            DBProvider.DocumentDAO.DeleteById(DocumentId);
        }
        /// <summary>
        /// 关联体系文件
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <param name="instrumentId"></param>
        public void DeleteInstrumentOwnDocument(int DocumentId, int instrumentId)
        {
            DBProvider.DocumentDAO.DeleteInstrumentOwnDocument(DocumentId, instrumentId);
        }


        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(DocumentModel model)
        {
            if (model.DocumentId == 0) DBProvider.DocumentDAO.Add(model);
            else DBProvider.DocumentDAO.Update(model);
        }

        /// <summary>
        /// 关联文件，已关联的提示已关联
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public string AddMoreDocument(int DocumentId, string instrumentIds)
        {
            string Msg = "OK";
            int success = 0;
            int failure = 0;
            try
            {
                string[] Ids = instrumentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in Ids)
                {
                    Hashtable model = DBProvider.DocumentDAO.IsExitDocument(DocumentId, Convert.ToInt32(id));
                    if (model == null)
                    {
                        DBProvider.DocumentDAO.AddOwnDocument(DocumentId, Convert.ToInt32(id));
                        success++;
                    }
                    else
                    {
                        failure++;
                    }
                }
            }
            catch 
            {
                Msg = "关联仪器失败！";
            }
            return Msg+","+success+","+failure;
        }

        /// <summary>
        /// 关联文件，已关联的提示已关联
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public string AddOwnDocument(int DocumentId, int instrumentId)
        {
            Hashtable model = DBProvider.DocumentDAO.IsExitDocument(DocumentId, instrumentId);
            if (model == null)
            {
                DBProvider.DocumentDAO.AddOwnDocument(DocumentId, instrumentId);
                return "OK";
            }
            else
                return "已关联此文件";
        }



        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public DocumentModel GetById(int DocumentId)
        {
            return DBProvider.DocumentDAO.GetById(DocumentId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<DocumentModel> GetAll()
        {
            return DBProvider.DocumentDAO.GetAll();
        }

        /// <summary>
        /// 获取仪器下所有对应的体系文件
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public IList<DocumentModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.DocumentDAO.GetByInstrumentId(instrumentId);
        }

        public Hashtable IsExitDocument(int documentId, int instrumentId)
        {
            return DBProvider.DocumentDAO.IsExitDocument(documentId, instrumentId);
        }

        public Hashtable GetByDocumentId(int documentId)
        {
            return DBProvider.DocumentDAO.GetByDocumentId(documentId);
        }

        /// <summary>
        /// 体系文件关联的仪器
        /// </summary>
        public void DeleteInstrumentOwnDocumentByDocumentId(int documentId)
        {
            DBProvider.DocumentDAO.DeleteInstrumentOwnDocumentByDocumentId(documentId);
        }
    }
}
