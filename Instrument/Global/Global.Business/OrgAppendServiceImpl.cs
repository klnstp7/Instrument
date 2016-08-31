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
    public class OrgAppendServiceImpl
    {
    /// <summary>
		/// 删除记录.
		/// </summary>
		public void DeleteById(int orgId)
		{
			DBProvider.OrgAppendDAO.DeleteById(orgId);
		}

		/// <summary>
		/// 保存实体数据.
		/// </summary>
		public void Save(OrgAppendModel model)
		{
            if (model.OrgId == 0) DBProvider.OrgAppendDAO.Add(model);
			else DBProvider.OrgAppendDAO.Update(model);
		}

		/// <summary>
		/// 获取一个记录对象.
		/// </summary>
        public OrgAppendModel GetById(int orgId)
		{
            return DBProvider.OrgAppendDAO.GetById(orgId);
		}

		/// <summary>
		/// 获取所有的记录对象.
		/// </summary>
		public IList<OrgAppendModel> GetAll()
		{
			return DBProvider.OrgAppendDAO.GetAll();
		}}
}
