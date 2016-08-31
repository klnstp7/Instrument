using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Business
{
    public class ServiceProvider
    {
        public static InstrumentServiceImpl InstrumentService { get; set; }
        public static InstrumentCheckLogServiceImpl InstrumentCheckLogService { get; set; }
        public static InstrumentRepairPlanServiceImpl InstrumentRepairPlanService { get; set; }
        public static InstrumentUsingPlanServiceImpl InstrumentUsingPlanService { get; set; }
        public static CraftServiceImpl CraftService { get; set; }
        public static OrderSendInstrumentServiceImpl OrderSendInstrumentService { get; set; }
        public static OrderServiceImpl OrderService { get; set; }
        public static InstrumentCertificationServiceImpl InstrumentCertificationService { get; set; }
        public static EmployeeServiceImpl EmployeeService { get; set; }
        public static DocumentServiceImpl DocumentService { get; set; }
        public static InstrumentWaitSendServiceImpl InstrumentWaitSendService { get; set; }
        public static ContactServiceImpl ContactService { get; set; }
        public static InstrumentFlowServiceImpl InstrumentFlowService { get; set; }
        public static BusinessAttachmentServiceImpl BusinessAttachmentService { get; set; }
        public static InstrumentPeriodcheckServiceImpl InstrumentPeriodcheckService { get; set; }
        public static InstrumentInnerCheckServiceImpl InstrumentInnerCheckService { get; set; }
        public static AssetCheckPlanServiceImpl AssetCheckPlanService { get; set; }
        public static AssetCheckPlanDetailServiceImpl AssetCheckPlanDetailService { get; set; }
        public static AssetCheckOperatorServiceImpl AssetCheckOperatorService { get; set; }
        public static KnowledgesServiceImpl KnowledgesService { get; set; }
        
    }
}
