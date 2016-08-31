using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public  class InstrumentFlowDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(InstrumentFlowModel model)
        {
            DBProvider.dbMapper.Insert("Instrument_Flow.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(InstrumentFlowModel model)
        {
            DBProvider.dbMapper.Update("Instrument_Flow.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int FlowId)
        {
            DBProvider.dbMapper.Delete("Instrument_Flow.DeleteById", FlowId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public InstrumentFlowModel GetById(int FlowId)
        {
            return DBProvider.dbMapper.SelectObject<InstrumentFlowModel>("Instrument_Flow.GetByID", FlowId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<InstrumentFlowModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<InstrumentFlowModel>("Instrument_Flow.GetAll");
        }

        public IList<InstrumentFlowModel> GetByInstrumentId(int instrumentId)
        {
            return DBProvider.dbMapper.SelectList<InstrumentFlowModel>("Instrument_Flow.GetByInstrumentId", instrumentId);
        }
    }
}
