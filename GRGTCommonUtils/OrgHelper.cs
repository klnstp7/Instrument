using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Global.Common.Models;

namespace GRGTCommonUtils
{
    public class OrgHelper
    {
        /// <summary>
        /// 以广州计量为例==GRGT为根接点
        /// </summary>
        public static readonly string topOrgCode = "GRGT";

        /// <summary>
        /// 依据要显示的组织编码，自动加载整棵树结构
        /// </summary>
        /// <param name="orgCodes">需要显示的组织编码字符串(','隔开)</param>
        /// <returns></returns>
        public static DataView GetTreeDataByOrgCodes(string orgCodes, DataView dv)
        {
            if (string.IsNullOrEmpty(orgCodes)) return null;
            
            IList<string> orgCode = new List<string>();
            string ouids = orgCodes;
            if (string.IsNullOrEmpty(ouids)) orgCode.Add(topOrgCode);

            string[] ouidarray = ouids.Split(',');
            if (ouidarray == null) orgCode.Add(topOrgCode);
            else
            {
                for (int i = 0; i < ouidarray.Length; i++)
                    orgCode.Add(ouidarray[i]);
            }

            return GetTreeDataByOrgCodes(orgCode,dv);
        }        


        /// <summary>
        /// 依据要显示的组织编码，自动加载整棵树结构
        /// </summary>
        /// <param name="ouId">需要显示的组织编码</param>
        /// <returns></returns>
        public static DataView GetTreeDataByOrgCodes(IList<string> ouId, DataView dv)
        {
            if (ouId == null) return new DataView();

            Hashtable hstCmp = new Hashtable();
            DataView tempDV = null;

            DataTable dt = new DataTable();
            dt.Columns.Add("OrgId", typeof(int));
            dt.Columns.Add("OrgName", typeof(string));
            dt.Columns.Add("OrgCode", typeof(string));
            dt.Columns.Add("ParentOrgId", typeof(int));
            dt.Columns.Add("ShowOrder", typeof(int));
            dt.Columns.Add("OrgType", typeof(int));

            foreach (string org in ouId)
            {
                if (org == null) continue;
                if (org.Trim() == "") continue;

                //获取所有下级组织
                tempDV = new DataView(dv.Table, string.Format("OrgCode like '{0}%'", org), "", DataViewRowState.CurrentRows);
                for (int i = 0; i < tempDV.Count; i++)
                {
                    if (!hstCmp.ContainsKey(tempDV[i]["OrgCode"].ToString().Trim()))
                    {
                        DataRow dr = dt.NewRow();
                        hstCmp.Add(tempDV[i]["OrgCode"].ToString().Trim(), "");
                        dr[0] = tempDV[i]["OrgId"].ToString().Trim();
                        dr[1] = tempDV[i]["OrgName"].ToString();
                        dr[2] = tempDV[i]["OrgCode"].ToString().Trim();
                        dr[3] = tempDV[i]["ParentOrgId"].ToString();
                        dr[4] = tempDV[i]["ShowOrder"].ToString();
                        dr[5] = tempDV[i]["OrgType"].ToString();
                        dt.Rows.Add(dr);
                    }
                }

                //添加自身
                tempDV = new DataView(dv.Table, string.Format("OrgCode='{0}'", org), "", DataViewRowState.CurrentRows);
                if (tempDV.Count == 1)
                    if (!hstCmp.ContainsKey(tempDV[0]["OrgCode"].ToString().Trim()))
                    {
                        DataRow dr = dt.NewRow();
                        hstCmp.Add(tempDV[0]["OrgCode"].ToString().Trim(), "");
                        dr[0] = tempDV[0]["OrgId"].ToString().Trim();
                        dr[1] = tempDV[0]["OrgName"].ToString();
                        dr[2] = tempDV[0]["OrgCode"].ToString().Trim();
                        dr[3] = tempDV[0]["ParentOrgId"].ToString();
                        dr[4] = tempDV[0]["ShowOrder"].ToString();
                        dr[5] = tempDV[0]["OrgType"].ToString();
                        dt.Rows.Add(dr);
                    }


                //获取所有上级编码
                bool isContinue = false;
                string porgCode = "";
                // 以广州计量为例==GRGT为根接点
                if (tempDV.Count == 1 && org != topOrgCode)
                {
                    isContinue = true;
                    //pid = tempDV[0]["ParentOrgId"].ToString().Trim();
                    porgCode = tempDV[0]["OrgCode"].ToString().Trim().Substring(0,tempDV[0]["OrgCode"].ToString().Trim().Length -2);
                }
                while (isContinue)
                {
                    isContinue = false;
                    if (!hstCmp.ContainsKey(porgCode))
                    {
                        tempDV = new DataView(dv.Table, string.Format("OrgCode='{0}'", porgCode), "", DataViewRowState.CurrentRows);
                        DataRow dr = dt.NewRow();
                        hstCmp.Add(tempDV[0]["OrgCode"].ToString().Trim(), "");
                        dr[0] = tempDV[0]["OrgId"].ToString().Trim();
                        dr[1] = tempDV[0]["OrgName"].ToString();
                        dr[2] = tempDV[0]["OrgCode"].ToString().Trim();
                        dr[3] = tempDV[0]["ParentOrgId"].ToString();
                        dr[4] = tempDV[0]["ShowOrder"].ToString();
                        dr[5] = tempDV[0]["OrgType"].ToString();
                        dt.Rows.Add(dr);
                    }

                    tempDV = new DataView(dv.Table, string.Format("OrgCode='{0}'", porgCode), "", DataViewRowState.CurrentRows);
                    if (tempDV.Count == 1)
                    {
                        porgCode = tempDV[0]["OrgCode"].ToString().Trim().Substring(0, tempDV[0]["OrgCode"].ToString().Trim().Length - 2);
                        if (string.Compare(porgCode, topOrgCode) >= 0) isContinue = true;
                    }
                }
            }

            dt.DefaultView.Sort = "OrgId ASC";
            return dt.DefaultView;
        }

        /// <summary>
        /// 依据要显示的组织编码，自动加载本组织及下级组织
        /// </summary>
        /// <param name="ouId">需要显示的组织编码</param>
        /// <returns></returns>
        public static DataView GetSonTreeDataByOrgCodes(IList<string> ouId, DataView dv)
        {
            if (ouId == null) return new DataView();

            Hashtable hstCmp = new Hashtable();
            DataView tempDV = null;

            DataTable dt = new DataTable();
            dt.Columns.Add("OrgId", typeof(int));
            dt.Columns.Add("OrgName", typeof(string));
            dt.Columns.Add("OrgCode", typeof(string));
            dt.Columns.Add("ParentOrgId", typeof(int));
            dt.Columns.Add("ShowOrder", typeof(int));
            dt.Columns.Add("OrgType", typeof(int));

            foreach (string org in ouId)
            {
                if (org == null) continue;
                if (org.Trim() == "") continue;

                //获取所有下级组织
                tempDV = new DataView(dv.Table, string.Format("OrgCode like '{0}%'", org), "", DataViewRowState.CurrentRows);
                for (int i = 0; i < tempDV.Count; i++)
                {
                    if (!hstCmp.ContainsKey(tempDV[i]["OrgCode"].ToString().Trim()))
                    {
                        DataRow dr = dt.NewRow();
                        hstCmp.Add(tempDV[i]["OrgCode"].ToString().Trim(), "");
                        dr[0] = tempDV[i]["OrgId"].ToString().Trim();
                        dr[1] = tempDV[i]["OrgName"].ToString();
                        dr[2] = tempDV[i]["OrgCode"].ToString().Trim();
                        dr[3] = tempDV[i]["ParentOrgId"].ToString();
                        dr[4] = tempDV[i]["ShowOrder"].ToString();
                        dr[5] = tempDV[i]["OrgType"].ToString();
                        dt.Rows.Add(dr);
                    }
                }

                //添加自身
                tempDV = new DataView(dv.Table, string.Format("OrgCode='{0}'", org), "", DataViewRowState.CurrentRows);
                if (tempDV.Count == 1)
                    if (!hstCmp.ContainsKey(tempDV[0]["OrgCode"].ToString().Trim()))
                    {
                        DataRow dr = dt.NewRow();
                        hstCmp.Add(tempDV[0]["OrgCode"].ToString().Trim(), "");
                        dr[0] = tempDV[0]["OrgId"].ToString().Trim();
                        dr[1] = tempDV[0]["OrgName"].ToString();
                        dr[2] = tempDV[0]["OrgCode"].ToString().Trim();
                        dr[3] = tempDV[0]["ParentOrgId"].ToString();
                        dr[4] = tempDV[0]["ShowOrder"].ToString();
                        dr[5] = tempDV[0]["OrgType"].ToString();
                        dt.Rows.Add(dr);
                    }

            }

            dt.DefaultView.Sort = "OrgId ASC";
            return dt.DefaultView;
        }


        public static string GetOrgNameTreeByOrgId(int  parentOrgId, IList<OrgModel> orgList,string strTree)
        {            
            OrgModel model  = orgList.SingleOrDefault(o => o.OrgId == parentOrgId);
            if (model == null) return strTree;
            strTree = model.OrgName + "—>" + strTree;
            return GetOrgNameTreeByOrgId(model.ParentOrgId, orgList, strTree);
        }
    }
}
