using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.Common.Models
{
    public class InstrumentModel
    {
        public InstrumentModel()
		{
            InstrumentCertification = new InstrumentCertificationModel();
        }
        #region Model
        /// <summary>
        /// 仪器标识
        /// </summary>
        public int InstrumentId { get; set; }
        ///// <summary>
        ///// 所属分公司
        ///// </summary>
        //public string BelongSubCompany { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string BelongDepart { get; set; }
        /// <summary>
        /// 存放地点
        /// </summary>
        public string StorePalce { get; set; }
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string InstrumentName { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 仪器型号
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 生产厂家联系信息
        /// </summary>
        public string ManufactureContactor { get; set; }
        /// <summary>
        /// 出厂编号
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 管理编号
        /// </summary>
        public string ManageNo { get; set; }
        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetsNo { get; set; }
        /// <summary>
        /// 管理级别
        /// </summary>
        public string ManageLevel { get; set; }
        /// <summary>
        /// 检验周期
        /// </summary>
        public string InspectCycle { get; set; }
        /// <summary>
        /// 检验机构
        /// </summary>
        public string InspectOrg { get; set; }
        /// <summary>
        /// 证书编号
        /// </summary>
        public string CertificateNo { get; set; }
        /// <summary>
        /// 证书有效期(开始)
        /// </summary>
        public DateTime ? DueStartDate { get; set; }
        /// <summary>
        /// 证书有效期(结束)
        /// </summary>
        public DateTime? DueEndDate { get; set; }
        /// <summary>
        /// 保管人
        /// </summary>
        public string LeaderName { get; set; }
        /// <summary>
        /// 说明书编号
        /// </summary>
        //public string SpecificationCode { get; set; }
        /// <summary>
        /// 项目组
        /// </summary>
        public string ProjectTeam { get; set; }
        /// <summary>
        /// 计量特性|技术指标
        /// </summary>
        public string MeasureCharacter { get; set; }
        /// <summary>
        /// 技术特征
        /// </summary>
        public string TechniqueCharacter { get; set; }
        /// <summary>
        /// 购置日期
        /// </summary>
        public DateTime ? BuyDate { get; set; }
        /// <summary>
        /// 设备种类
        /// </summary>
        public int InstrumentType { get; set; }
        /// <summary>
        /// 购置金额
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 使用年限
        /// </summary>
        public string DurableYears { get; set; }
        /// <summary>
        /// 工艺过程标识
        /// </summary>
        public int CraftId { get; set; }
        /// <summary>
        /// 设备分类
        /// </summary>
        public int InstrumentCate { get; set; }
        /// <summary>
        /// 设备子分类
        /// </summary>
        public int SubInstrumentCate { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public int InstrumentForm { get; set; }
        /// <summary>
        /// 资产属性
        /// </summary>
        public int CalibrationType { get; set; }
        /// <summary>
        /// 检定类别
        /// </summary>
        public int VerificationType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 0：正常，1：删除，2：封存，3：停用，4：限用，5：修理，6：不合格
        /// </summary>
        public int RecordState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemCode { get; set; }
      
        /// <summary>
        /// 最近盘点人
        /// </summary>
        public string LastCheckUser { get; set; }
        /// <summary>
        /// 最近盘点时间
        /// </summary>
        public DateTime? LastCheckDate { get; set; }
        /// <summary>
        /// 使用日期
        /// </summary>
        public DateTime? UseDate { get; set; }

        /// <summary>
        /// 最近修改人
        /// </summary>
        public string LastUpdateUser { get; set; }
        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime LastUpdateDate { get; set; }
        /// <summary>
        /// 配件类型
        /// </summary>
        public int CombinedType { get; set; }
        /// <summary>
        /// 配置父ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 证书详情的附加信息
        /// </summary>
        public InstrumentCertificationModel InstrumentCertification { get; set; }

        #endregion Model
        public List<InstrumentCertificationModel> ownCheckLogList { get; set; }
    }

}
