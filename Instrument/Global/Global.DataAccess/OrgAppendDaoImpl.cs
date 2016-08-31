using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common.Models;
using ToolsLib.IBatisNet;
using System.Collections;

namespace Global.DataAccess
{
    public class OrgAppendDaoImpl
    {
    /// <summary>
		/// 增加一条数据.
		/// </summary>
		public void Add(OrgAppendModel model)
		{
			DefaultMapper.GetMapper().Insert("Org_AppendInfo.Insert", model);
		}

		/// <summary>
		/// 更新一条数据.
		/// </summary>
		public void Update(OrgAppendModel model)
		{
			DefaultMapper.GetMapper().Update("Org_AppendInfo.Update", model);
		}

		/// <summary>
		/// 删除一条数据.
		/// </summary>
		public void DeleteById(int orgId)
		{
            DefaultMapper.GetMapper().Delete("Org_AppendInfo.DeleteById", orgId);
		}


		/// <summary>
		/// 得到一个对象实体.
		/// </summary>
		public OrgAppendModel GetById(int orgId)
		{
			return DefaultMapper.GetMapper().SelectObject<OrgAppendModel>("Org_AppendInfo.GetByID",orgId);
		}

        public int isExist(int orgId)
        {
            return DefaultMapper.GetMapper().SelectObject<int>("Org_AppendInfo.isExist", orgId); 
        }

		/// <summary>
		/// 获得所有记录.
		/// </summary>
		public IList<OrgAppendModel> GetAll()
		{
			return DefaultMapper.GetMapper().SelectList<OrgAppendModel>("Org_AppendInfo.GetAll");
		}}
}
