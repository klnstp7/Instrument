using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common;
using Instrument.Common.Models;
using GRGTCommonUtils;
using System.Data;
using Global.Common.Models;

namespace Instrument.Business
{
    public class CompanyServiceImpl
    {

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<CompanyModel> GetAll()
        {
            return DBProvider.CompanyDAO.GetAll();
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(CompanyModel model)
        {
            DBProvider.CompanyDAO.Update(model);
        }

    }
}
