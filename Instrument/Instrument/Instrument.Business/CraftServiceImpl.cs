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
    /// <summary>
    /// 实验室标准器具
    /// </summary>
    public class CraftServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int CraftId)
        {
            ServiceProvider.InstrumentService.UpdateInstrumentSetCraftNull(CraftId);
            DBProvider.CraftDAO.DeleteById(CraftId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(CraftModel model)
        {
            if (model.CraftId == 0)
            {
                model.CreateUser = LoginHelper.LoginUser.UserName;
                DBProvider.CraftDAO.Add(model);
            }
            else DBProvider.CraftDAO.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public CraftModel GetById(int CraftId)
        {
            return DBProvider.CraftDAO.GetById(CraftId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<CraftModel> GetAll()
        {
            return DBProvider.CraftDAO.GetAll();
        }

        public IList<Hashtable> GetAllCraftListForPaging(PagingModel paging)
        {
            return DBProvider.CraftDAO.GetAllCraftListForPaging(paging);
        }
    }
}
