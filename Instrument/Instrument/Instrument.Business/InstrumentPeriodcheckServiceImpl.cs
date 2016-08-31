using System.Collections.Generic;
using Instrument.Common.Models;
using Instrument.DataAccess;

namespace Instrument.Business
{
    public class InstrumentPeriodcheckServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int PeriodcheckId)
        {
            DBProvider.InstrumentPeriodcheckDAO.DeleteById(PeriodcheckId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(PeriodcheckModel model)
        {
            if (model.PeriodcheckId == 0) DBProvider.InstrumentPeriodcheckDAO.Add(model);
            else DBProvider.InstrumentPeriodcheckDAO.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public PeriodcheckModel GetById(int PeriodcheckId)
        {
            return DBProvider.InstrumentPeriodcheckDAO.GetById(PeriodcheckId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<PeriodcheckModel> GetAll()
        {
            return DBProvider.InstrumentPeriodcheckDAO.GetAll();
        }

        public IList<PeriodcheckModel> GetPeriodcheckListByWhere(string where)
        {
            return DBProvider.InstrumentPeriodcheckDAO.GetPeriodcheckListByWhere(where);
        }

        public IList<PeriodcheckModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.InstrumentPeriodcheckDAO.GetByInstrumentId(instrumentId);
        }
        


    }
}
