using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.Common.Models;
using Instrument.DataAccess;

namespace Instrument.Business
{
    public class InstrumentInnerCheckServiceImpl
    {
        public IList<InnerCheckModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.InstrumentInnercheckDAO.GetByInstrumentId(instrumentId);
        }

        public InnerCheckModel GetById(int innercheckId)
        {
            return DBProvider.InstrumentInnercheckDAO.GetById(innercheckId);
        }
        public void Delect(int innerCheckId)
        {
            DBProvider.InstrumentInnercheckDAO.Delete(innerCheckId);
        }
        public void Save(InnerCheckModel model)
        {
            if (model.InnerCheckId == 0) DBProvider.InstrumentInnercheckDAO.Add(model);
            else DBProvider.InstrumentInnercheckDAO.Update(model);
        }
        public IList<InnerCheckModel> GetInnerCheckListByWhere(string where)
        {
            return DBProvider.InstrumentInnercheckDAO.GetInnerCheckListByWhere(where);
        }
    }
}
