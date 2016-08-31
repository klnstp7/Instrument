using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Instrument.Common.Models;
using System.Collections;

namespace Instrument.DataAccess
{
    public class InstrumentDaoImplForSQLite : InstrumentDaoImpl
    {
        public override IList<Hashtable> GetAllInstrumentListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_BaseInfo";
            paging.FieldKey = "InstrumentId";
            if (string.IsNullOrEmpty(paging.FieldShow))
            {
                paging.FieldShow = @"InstrumentId,BelongDepart,StorePalce,InstrumentName,EnglishName,Specification,Manufacturer,SerialNo
                ,ManageNo,AssetsNo,ManageLevel,InspectCycle,InspectOrg,CertificateNo,DueStartDate,DueEndDate,LeaderName,ProjectTeam,MeasureCharacter
                ,TechniqueCharacter,BuyDate,InstrumentType,Price,DurableYears,CraftId,InstrumentCate,CalibrationType,VerificationType,Number,BarCode,RecordState,CreateDate,CreateUser,LastUpdateDate";
            }
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "LastUpdateDate desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);

            return list;
        }

        public override IList<Hashtable> GetAllCertificationListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_BaseInfo a left join Instrument_Certification b on a.InstrumentId=b.InstrumentId";
            paging.FieldKey = "InstrumentId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "InstrumentId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            return list;
        }

        public override IList<Hashtable> GetAllPeriodcheckListForPaging(PagingModel paging)
        {
            paging.TableName = "Instrument_Periodcheck";
            paging.FieldKey = "PeriodcheckId";
            if (string.IsNullOrEmpty(paging.FieldOrder))
                paging.FieldOrder = "PeriodcheckId desc";
            //记录总数
            paging.RecordCount = DBProvider.dbMapper.SelectObject<int>("Paging.GetRecordCount", paging);
            //记录列表
            IList<Hashtable> list = DBProvider.dbMapper.SelectPaginatedList<Hashtable>("Paging.GetListForSQLitePaging", paging);
            return list;
        }
    }
}
