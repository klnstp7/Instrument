using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class OrgDaoImpl
    {
        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int OrgId)
        {
            string code = GetCodeById(OrgId);
            DBProvider.dbMapper.Delete("Organization.DeleteByCode", code);
        }

        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(OrgModel model)
        {
            DBProvider.dbMapper.Insert("Organization.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(OrgModel model)
        {
            DBProvider.dbMapper.Update("Organization.Update", model);
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public OrgModel GetById(int OrgId)
        {
            return DBProvider.dbMapper.SelectObject<OrgModel>("Organization.GetByID", OrgId);
        }

        /// <summary>
        /// 获取公司部门
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public OrgModel GetByCode(string orgCode)
        {
            return DBProvider.dbMapper.SelectObject<OrgModel>("Organization.GetByCode", orgCode);
        }

        public string GetCodeById(int OrgId)
        {
            return DBProvider.dbMapper.SelectObject<string>("Organization.GetCodeById", OrgId);
        }

        public IList<Hashtable> GetByParentId(int parentId)
        {
            string code = GetCodeById(parentId);
            return DBProvider.dbMapper.SelectList<Hashtable>("Organization.GetAllByOrgCode", code);
        }


        public IList<OrgModel> GetListByParentId(int parentId)
        {
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetByParentId", parentId);
        }

        /// <summary>
        /// 根据部门编码获取部门
        /// </summary>
        /// <param name="code">部门编码</param>
        /// <returns></returns>
        public IList<OrgModel> GetByOrgCode(string code)
        {
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetByOrgCode", code);
        }

        /// <summary>
        /// 根据组织编码和类型获取组织及其所有下级组织
        /// </summary>
        /// <param name="code"></param>
        /// <param name="orgType"></param>
        /// <returns></returns>
        public IList<OrgModel> GetByOrgCodeAndOrgType(string code,int orgType)
        {
            Hashtable ht = new Hashtable();
            ht.Add("OrgCode", code);
            ht.Add("OrgType", orgType);
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetByOrgCodeAndOrgType", ht);
        }

        /// <summary>
        /// 获取组织(根据组织类型)
        /// </summary>
        /// <param name="orgType"></param>
        /// <returns></returns>
        public IList<OrgModel> GetByOrgType(int orgType)
        {
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetByOrgType", orgType);
        }

        /// <summary>
        /// 获得所有记录(有效的记录).
        /// </summary>
        public IList<OrgModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetAll");
        }

        /// <summary>
        /// 获得所有的记录.
        /// </summary>
        public IList<OrgModel> GetAll2()
        {
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetAll2");
        }

        public void SetOrgState(int orgId, bool state)
        {
            string code = GetCodeById(orgId);
            OrgModel model = new OrgModel { OrgCode = code, IsEnabled = state };

            DBProvider.dbMapper.Update("Organization.SetOrgState", model);
        }

        /// <summary>
        /// 通过ID列表得到对象实体集合.
        /// </summary>
        public IList<OrgModel> GetByOrgIds(IList<int> OrgIdList)
        {
            Hashtable ht = new Hashtable();
            if (OrgIdList == null || OrgIdList.Count == 0)
                OrgIdList = new List<int>() { 0 };
            ht.Add("OrgIdList", OrgIdList);
            return DBProvider.dbMapper.SelectList<OrgModel>("Organization.GetByOrgIds", ht);
        }
    }
}
