using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class DutyDaoImpl
    {
        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<DutyModel> GetAllByOrgCode(string orgCode)
        {
            return DBProvider.dbMapper.SelectList<DutyModel>("Sys_Duty.GetAllByOrgCode", orgCode);
        }

        /// <summary>
        /// 根据组织编码获取所有职位
        /// </summary>
        /// <param name="orgCode">组织编码</param>
        /// <returns></returns>
        public IList<Hashtable> GetAllDutyByOrgCode(string orgCode)
        {
            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_Duty.GetAllDutyByOrgCode", orgCode);
        }
    }
}
