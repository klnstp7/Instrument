using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.Common.Models;

namespace Instrument.DataAccess
{
    public class InstrumentInnerCheckDaoImpl
    {
        public IList<InnerCheckModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.dbMapper.SelectList<InnerCheckModel>("Instrument_InnerCheck.GetByInstrumentId", instrumentId);
        }

        public InnerCheckModel GetById(int innercheckId)
        {
            return DBProvider.dbMapper.SelectObject<InnerCheckModel>("Instrument_InnerCheck.GetById", innercheckId);
        }

        public void Add(InnerCheckModel model)
        {
            DBProvider.dbMapper.Insert("Instrument_InnerCheck.Add", model);
        }

        public void Update(InnerCheckModel model)
        {
            DBProvider.dbMapper.Update("Instrument_InnerCheck.Update", model);
        }

        public void Delete(int innerCheckId)
        {
            DBProvider.dbMapper.Delete("Instrument_InnerCheck.Delete", innerCheckId);
        }


        public IList<InnerCheckModel> GetALL()
        {
            return DBProvider.dbMapper.SelectList<InnerCheckModel>("Instrument_InnerCheck.GetAll");
        }

        public IList<InnerCheckModel> GetInnerCheckListByWhere(string where)
        {
            return DBProvider.dbMapper.SelectList<InnerCheckModel>("Instrument_InnerCheck.GetInnerCheckListByWhere", where);
        }
    }
}
