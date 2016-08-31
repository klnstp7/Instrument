using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.DataAccess
{
    public class DBProvider
    {
        public static ToolsLib.IBatisNet.BaseMapper dbMapper { get; set; }

        #region === 基础业务数据 ===
        public static InstrumentDaoImpl InstrumentDAO { get; set; }
        public static InstrumentCheckLogDaoImpl InstrumentCheckLogDAO { get; set; }
        public static InstrumentRepairPlanDaoImpl InstrumentRepairPlanDAO { get; set; }
        public static InstrumentUsingPlanDaoImpl InstrumentUsingPlanDAO { get; set; }
        public static CraftDaoImpl CraftDAO { get; set; }
        public static OrderSendInstrumentDaoImpl OrderSendInstrumentDAO { get; set; }
        public static OrderDaoImpl OrderDAO { get; set; }
        public static InstrumentCertificationDaoImpl InstrumentCertificationDAO { get; set; }
        public static DocumentDaoImpl DocumentDAO { get; set; }
        public static InstrumentWaitSendDaoImpl InstrumentWaitSendDAO { get; set; }
        public static ContactDaoImpl ContactDAO { get; set; }
        public static InstrumentFlowDaoImpl InstrumentFlowDao { get; set; }
        public static BusinessAttachmentDaoImpl BusinessAttachmentDao { get; set; }
        public static InstrumentPeriodcheckDaoImpl InstrumentPeriodcheckDAO { get; set; }
        public static InstrumentInnerCheckDaoImpl InstrumentInnercheckDAO { get; set; }
        public static AssetCheckPlanDaoImpl AssetCheckPlanDAO { get; set; }
        public static AssetCheckPlanDetailDaoImpl AssetCheckPlanDetailDAO { get; set; }
        public static AssetCheckOperatorDaoImpl AssetCheckOperatorDAO { get; set; }
        public static KnowledgesDaoImpl KnowledgesDao { get; set; }
        #endregion
    }
}
