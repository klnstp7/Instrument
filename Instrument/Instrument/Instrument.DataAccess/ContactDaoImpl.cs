using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;


namespace Instrument.DataAccess
{
    public class ContactDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(ContactModel model)
        {
            DBProvider.dbMapper.Insert("Contact.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(ContactModel model)
        {
            DBProvider.dbMapper.Update("Contact.Update", model);
        }

        public void UpdateState(ContactModel model)
        {
            DBProvider.dbMapper.Update("Contact.UpdateState", model);
        }

        public void UpdateFeedback(ContactModel model)
        {
            DBProvider.dbMapper.Update("Contact.UpdateFeedback", model);
        }
        

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int ContactId)
        {
            DBProvider.dbMapper.Delete("Contact.DeleteById", ContactId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public ContactModel GetById(int ContactId)
        {
            return DBProvider.dbMapper.SelectObject<ContactModel>("Contact.GetByID", ContactId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<ContactModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<ContactModel>("Contact.GetAll");
        }

        public virtual IList<Hashtable> GetAllContactListForPaging(PagingModel paging)
        {
            paging.TableName = "Contact";
            paging.FieldKey = "ContactId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"[ContactId],[CompanyName],[CaseType],[Abstract],[State],[ContactContent],[FeedbackContent],[FeedbackDate],[Creator],CreatId,[CreatDate],[ItemCode]";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "ContactId desc";

            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);

            return list;
        }
    }
}
