using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class CompanyDaoImpl
    {
        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<CompanyModel> GetAll()
        {
            return DefaultMapper.GetMapper().SelectList<CompanyModel>("Company_BaseInfo.GetAll");
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(CompanyModel model)
        {
            DefaultMapper.GetMapper().Update("Company_BaseInfo.Update", model);
        }

    }
}
