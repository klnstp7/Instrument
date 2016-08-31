using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class InstrumentPeriodcheckDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(PeriodcheckModel model)
        {
            DBProvider.dbMapper.Insert("Instrument_Periodcheck.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(PeriodcheckModel model)
        {
            DBProvider.dbMapper.Update("Instrument_Periodcheck.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int PeriodcheckId)
        {
            DBProvider.dbMapper.Delete("Instrument_Periodcheck.DeleteById", PeriodcheckId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public PeriodcheckModel GetById(int PeriodcheckId)
        {
            return DBProvider.dbMapper.SelectObject<PeriodcheckModel>("Instrument_Periodcheck.GetByID", PeriodcheckId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<PeriodcheckModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<PeriodcheckModel>("Instrument_Periodcheck.GetAll");
        }

        public IList<PeriodcheckModel> GetPeriodcheckListByWhere(string where)
        {
            return DBProvider.dbMapper.SelectList<PeriodcheckModel>("Instrument_Periodcheck.GetPeriodcheckListByWhere", where);
        }
        

        public IList<PeriodcheckModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.dbMapper.SelectList<PeriodcheckModel>("Instrument_Periodcheck.GetByInstrumentId", instrumentId);
        }


    }
}
