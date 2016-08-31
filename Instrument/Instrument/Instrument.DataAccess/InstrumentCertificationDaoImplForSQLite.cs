using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class InstrumentCertificationDaoImplForSQLite : InstrumentCertificationDaoImpl
    {
        public override IList<Hashtable> GetInstrumentCertificationListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_Certification";
            paging.FieldKey = "InstrumentId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "InstrumentId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            return list;
        }

        /// <summary>
        /// 获取仪器下最大的到期日期证书
        /// </summary>
        public override InstrumentCertificationModel GetMaxEndDateByInstrumentId(int instrumentId, string currentDate)
        {
            Hashtable ht = new Hashtable();
            ht["InstrumentId"] = instrumentId;
            ht["CurrentDate"] = currentDate;
            return DBProvider.dbMapper.SelectObject<InstrumentCertificationModel>("Instrument_Certification.GetMaxEndDateByInstrumentIdForSQLite", ht);
        }
    }
}
