using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using GRGTCommonUtils;
using ToolsLib.Utility;
using System.Collections;

namespace Global.Business
{
    public class DutyServiceImpl
    {
        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<DutyModel> GetAllByOrgCode(string orgCode)
        {
            return DBProvider.DutyDAO.GetAllByOrgCode(orgCode);
        }

        public int GetDutyCountByOrgId(int orgId)
        {
            OrgModel orgModel = DBProvider.OrgDAO.GetById(orgId);
            IList<Hashtable> list = DBProvider.DutyDAO.GetAllDutyByOrgCode(orgModel.OrgCode);
            if (list != null)
                return list.Count;
            else
                return 0;
        }
    }
}
