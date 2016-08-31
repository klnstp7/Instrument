using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using System.Data;
using System.Collections;
using GRGTCommonUtils;
using ToolsLib.Utility;

namespace Global.Business
{
    public class OrgServiceImpl
    {

        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int OrgId)
        {
            DBProvider.OrgDAO.DeleteById(OrgId);

        }
        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public OrgModel GetById(int OrgId)
        {
            OrgModel model = DBProvider.OrgDAO.GetById(OrgId);
            if (model == null) model = new OrgModel();           
            return model;
        }

        /// <summary>
        /// 获取公司部门
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public OrgModel GetByCode(string orgCode)
        {
            OrgModel model = DBProvider.OrgDAO.GetByCode(orgCode);
            return model;
        }

        public IList<Hashtable> GetByParentId(int parentId)
        {
            return DBProvider.OrgDAO.GetByParentId(parentId);
        }

        /// <summary>
        /// 根据组织标识自动生成子组织节点的OrgCode
        /// 找出最大的子组织编号+1，若无自组织，则编号为父组织编号+01【xxxxx01】
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public string BuildSubOrgCode(int parentId)
        {
            string subOrgCode = "";
            OrgModel parentOrg = ServiceProvider.OrgService.GetById(parentId);
            IList<OrgModel> subOrgs = DBProvider.OrgDAO.GetListByParentId(parentId);
            if (subOrgs == null) subOrgs = new List<OrgModel>();
            if (subOrgs.Count == 0)
                subOrgCode = parentOrg.OrgCode + "01";
            else
            {
                string maxOrgCode = subOrgs.OrderByDescending(o => o.OrgId).First().OrgCode;
                int code = 1;
                int.TryParse(maxOrgCode.Replace(parentOrg.OrgCode, ""), out code);
                code++;
                if (code < 10) subOrgCode = parentOrg.OrgCode + "0" + code;
                else subOrgCode = parentOrg.OrgCode + code;
            }
            return subOrgCode;
        }

        public string GetCodeById(int OrgId)
        {
            return DBProvider.OrgDAO.GetCodeById(OrgId);
        }

        /// <summary>
        /// 根据部门编码获取部门
        /// </summary>
        /// <param name="code">部门编码</param>
        /// <returns></returns>
        public IList<OrgModel> GetByOrgCode(string code)
        {
            return DBProvider.OrgDAO.GetByOrgCode(code);
        }

       /// <summary>
        /// 根据组织编码和类型获取组织下具有某类型的组织
       /// </summary>
       /// <param name="code"></param>
       /// <param name="orgType"></param>
       /// <returns></returns>
        public IList<OrgModel> GetByOrgCodeAndOrgType(string code,int orgType)
        {
            return DBProvider.OrgDAO.GetByOrgCodeAndOrgType(code,orgType);
        }


        /// <summary>
        /// 获取所有的记录对象(有效的记录).
        /// </summary>
        public IList<OrgModel> GetAll()
        {
            return DBProvider.OrgDAO.GetAll();
        }       
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgList"></param>
        /// <returns></returns>
        public string GetdhtmlxTree(IList<OrgModel> orgList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.AppendFormat("<tree id='0'>" + Environment.NewLine);
            RecursiveDump(orgList, 0, 1, sb);
            sb.Append("</tree>");

            return sb.ToString();
        }

        private void RecursiveDump(IList<OrgModel> orgList, int parentId,IList<OrgModel> newList,Hashtable ht)
        {
            IEnumerable<OrgModel> tempOrg = orgList.Where<OrgModel>(m => m.OrgId == parentId);

            foreach (OrgModel m in tempOrg)
            {
                if (!ht.ContainsKey(m.OrgCode))
                {
                    newList.Add(m);
                    ht.Add(m.OrgCode, m);
                    RecursiveDump(orgList, m.ParentOrgId, newList, ht);
                }
            }
        }

        private void RecursiveDump(IList<OrgModel> orgList, int parentId, int level, StringBuilder sb)
        {
            if (orgList == null) return;
            IEnumerable<OrgModel> tempOrg = orgList.Where<OrgModel>(m => m.ParentOrgId == parentId);

            string msg = "";
            foreach (OrgModel m in tempOrg)
            {
                msg = "<item text='{0}' id='{1}'>{2}";
                if (level == 1) msg = "<item text='{0}' id='{1}' open='1' select='1'>{2}";
                else if (level < 3) msg = "<item text='{0}' id='{1}' open='1'>{2}";

                sb.AppendFormat(msg, GRGTCommonUtils.UtilsHelper.XmlSpecialCharValidate(m.OrgName), UtilsHelper.Encrypt(m.OrgId.ToString()), Environment.NewLine);
                RecursiveDump(orgList, m.OrgId, level + 1, sb);
                sb.Append("</item>");
            }
        }



        /// <summary>
        /// 根据用户管理的部门获取管理的部门及上级部门，并组装成树形结构
        /// </summary>
        /// <param name="ownList"></param>
        /// <returns></returns>
        public string GetParentdhtmlxTree(IList<OrgModel> ownList)
        {
            StringBuilder sb = new StringBuilder();
            if (ownList.Count == 0)
            {
                sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
                sb.AppendFormat("<tree id='0'><item text='无管理部门，请联系系统管理员' > </item>" + Environment.NewLine);
                sb.Append("</tree>");
                return sb.ToString();
            }

            IList<OrgModel> orgList = DBProvider.OrgDAO.GetAll();
            //管理员账号设置为GRGT，则获取所有子节点
            if (ownList[0].OrgCode == "GRGT")
                return GetdhtmlxTree(orgList);
            //找出上级部门
            IList<OrgModel> newList = new List<OrgModel>();
            Hashtable ht = new Hashtable();
            foreach (OrgModel org in ownList)
            {
                OrgModel model = new OrgModel();
                model.OrgId = org.OrgId;
                model.OrgName = org.OrgName;
                model.OrgCode = org.OrgCode;
                model.ParentOrgId = org.ParentOrgId;
                ht.Add(org.OrgCode, org.OrgCode);
                newList.Add(model);
                RecursiveOwnDump(orgList, model.ParentOrgId, newList, ht);
            }
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.AppendFormat("<tree id='0'>" + Environment.NewLine);
            RecursiveDump(newList, 0, 1, sb);
            sb.Append("</tree>");

            return sb.ToString();
        }

        /// <summary>
        /// 根据用户管理的部门获取管理的部门及上级部门，并组装成树形结构
        /// </summary>
        /// <param name="orgList"></param>
        /// <param name="ownList"></param>
        /// <returns></returns>
        public string GetOwndhtmlxTree(IList<OrgModel> orgList, IList<OrgModel> ownList)
        {
            //找出上级部门
            IList<OrgModel> newList = new List<OrgModel>();
            Hashtable ht = new Hashtable();
            foreach (OrgModel org in ownList)
            {
                OrgModel model = new OrgModel();
                model.OrgId = org.OrgId;
                model.OrgName = org.OrgName;
                model.OrgCode = org.OrgCode;
                model.ParentOrgId = org.ParentOrgId;
                ht.Add(org.OrgCode, org.OrgCode);
                newList.Add(model);
                RecursiveOwnDump(orgList, model.ParentOrgId, newList, ht);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.AppendFormat("<tree id='0'>" + Environment.NewLine);
            RecursiveDump(newList, 0, 1, sb);
            sb.Append("</tree>");

            return sb.ToString();
        }

        /// <summary>
        /// 根据用户所属组织，加载用户具有某种标记组织的整棵树，包括上级
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="orgType"></param>
        /// <returns></returns>
        public string GetdhtmlxTreeByOrgCodes(string orgCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.AppendFormat("<tree id='0'>" + Environment.NewLine);
            IList<Hashtable> htList = new List<Hashtable>();
            IList<OrgModel> newList = new List<OrgModel>();
            //根据orgCode找出所有子组织
            IList<OrgModel> allOrgList = GetAll();

            DataView dvList = ToolsLib.Utility.CommonUtils.IList2DataTable<OrgModel>(allOrgList).DefaultView;

            DataView dv = GRGTCommonUtils.OrgHelper.GetTreeDataByOrgCodes(orgCode, dvList);
            if(dv!=null) newList = ToolsLib.Utility.CommonUtils.DataTable2IList<OrgModel>(dv.Table);

            RecursiveDump(newList, 0, 1, sb);
            //sb.Append("</item>");
            sb.Append("</tree>");

            return sb.ToString();
        }


        private void RecursiveOwnDump(IList<OrgModel> orgList, int parentOrgId, IList<OrgModel> newList, Hashtable ht)
        {
            IEnumerable<OrgModel> tempOrg = orgList.Where<OrgModel>(m => m.OrgId == parentOrgId);

            foreach (OrgModel m in tempOrg)
            {
                if (!ht.Contains(m.OrgCode))
                {
                    ht.Add(m.OrgCode, m.OrgCode);
                    newList.Add(m);
                }
                RecursiveOwnDump(orgList, m.ParentOrgId, newList, ht);
            }
        }


        /// <summary>
        /// 获取组织(根据组织类型)
        /// </summary>
        /// <param name="orgType"></param>
        /// <returns></returns>
        public IList<OrgModel> GetByOrgType(int orgType)
        {
            return DBProvider.OrgDAO.GetByOrgType(orgType);
        }

        /// <summary>
        /// 获得所有的记录
        /// </summary>
        /// <returns></returns>
        public IList<OrgModel> GetAll2()
        {
            return DBProvider.OrgDAO.GetAll2();
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(OrgModel model)
        {
            if (model.OrgId == 0)
            {
                DBProvider.OrgDAO.Add(model);               
            }
            else
            {
                DBProvider.OrgDAO.Update(model);               
            }
        }

        public void SetOrgState(int orgId, bool state)
        {
            DBProvider.OrgDAO.SetOrgState(orgId, state);
        }

        /// <summary>
        /// 通过ID列表得到对象实体集合.
        /// </summary>
        public IList<OrgModel> GetByOrgIds(IList<int> OrgIdList)
        {
            List<OrgModel> orgList = new List<OrgModel>();
            int maxLen = 500;
            int queryCount = OrgIdList.Count / maxLen + (OrgIdList.Count % maxLen == 0 ? 0 : 1);
            for (int i = 0; i < queryCount; i++)
            {
                var tempArray = OrgIdList.Skip(i * maxLen).Take(maxLen).ToList();
                IList<OrgModel> mOrg = DBProvider.OrgDAO.GetByOrgIds(tempArray);
                orgList.AddRange(mOrg);
            }
            return orgList;
        }
    }
}
