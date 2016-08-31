using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;
using Instrument.Common;

namespace Instrument.DataAccess
{
    public class BusinessAttachmentDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(BusinessAttachmentModel model)
        {
            DBProvider.dbMapper.Insert("Business_Attachment.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(BusinessAttachmentModel model)
        {
            DBProvider.dbMapper.Update("Business_Attachment.Update", model);
        }

        /// <summary>
        /// 更新备注
        /// </summary>
        public void UpdateRemark(BusinessAttachmentModel model)
        {
            DBProvider.dbMapper.Update("Business_Attachment.UpdateRemark", model);
        }
        /// <summary>
        /// 更新已申请受控
        /// </summary>
        public void UpdateApplyControlled(int id)
        {
            DBProvider.dbMapper.Update("Business_Attachment.UpdateApplyControlled", id);
        }

        /// <summary>
        /// 更新受控状态
        /// </summary>
        public void UpdateControlledState(int id,int state)
        {
            Hashtable ht = new Hashtable();
            ht["Id"] = id;
            ht["ControlledState"] = state;
            DBProvider.dbMapper.Update("Business_Attachment.UpdateControlledState", ht);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int Id)
        {
            DBProvider.dbMapper.Delete("Business_Attachment.DeleteById", Id);
        }

        /// <summary>
        /// 批量删除.
        /// </summary>
        public void DeleteByIds(string ids)
        {
            Hashtable ht = new Hashtable();
            ht["Id"] = ids.Split(',');
            DBProvider.dbMapper.Delete("Business_Attachment.DeleteByIds", ht);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public BusinessAttachmentModel GetById(int Id)
        {
            return DBProvider.dbMapper.SelectObject<BusinessAttachmentModel>("Business_Attachment.GetByID", Id);
        }

        public BusinessAttachmentModel GetByTypeAndKeyId(int type, int Id)
        {
            Hashtable ht = new Hashtable();
            ht["BusinessType"] = type;
            ht["BusinessKeyId"] = Id;
            return DBProvider.dbMapper.SelectObject<BusinessAttachmentModel>("Business_Attachment.GetByTypeAndKeyId", ht);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<BusinessAttachmentModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<BusinessAttachmentModel>("Business_Attachment.GetAll");
        }

        /// <summary>
        /// 获取某业务类型下的某张单的所有记录对象.
        /// </summary>
        public IList<BusinessAttachmentModel> GetByBusinessId(int businessKeyId, string businessType)
        {
            Hashtable table = new Hashtable();
            table.Add("BusinessType", businessType);
            table.Add("BusinessKeyId", businessKeyId);
            return DBProvider.dbMapper.SelectList<BusinessAttachmentModel>("Business_Attachment.GetByBusinessId", table);
        }

        /// <summary>
        /// 获取某业务类型下的某张单的单个记录对象.(一般用来查询仪器照片)
        /// </summary>
        public BusinessAttachmentModel GetByBusinesId(int businessKeyId, string businessType)
        {
            Hashtable table = new Hashtable();
            table.Add("BusinessType", businessType);
            table.Add("BusinessKeyId", businessKeyId);
            return DBProvider.dbMapper.SelectObject<BusinessAttachmentModel>("Business_Attachment.GetByBusinesId", table);
        }
        /// <summary>
        /// 获取申请受控,未审核列表
        /// </summary>
        /// <returns></returns>
        public IList<BusinessAttachmentModel> GetVerifyList()
        {
            return DBProvider.dbMapper.SelectList<BusinessAttachmentModel>("Business_Attachment.GetVerifyList");
        }


        /// <summary>
        /// 获取某业务类型下的某张单的所有记录对象.
        /// </summary>
        public IList<BusinessAttachmentModel> GetByBusinessTypeAndId(int businessType, int businessId)
        {
            Hashtable table = new Hashtable();
            table.Add("BusinessType", businessType);
            table.Add("BusinessId", businessId);
            return DBProvider.dbMapper.SelectList<BusinessAttachmentModel>("Business_Attachment.GetByBusinessTypeAndId", table);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public virtual IList<Hashtable> GetInstrumentManualForPaging(PagingModel paging)
        {
            paging.TableName = "Business_Attachment";
            paging.FieldKey = "Id";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "Id desc";
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListByPaging", paging);
            return list;
        }
    }
}
