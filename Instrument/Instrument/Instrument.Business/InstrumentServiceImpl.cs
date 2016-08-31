using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common;
using Instrument.Common.Models;
using GRGTCommonUtils;
using System.Data;
using System.Web;
using ToolsLib.Utility;
using System.IO;
using ToolsLib.Utility.Jquery;
using Global.Common.Models;


namespace Instrument.Business
{
    /// <summary>
    /// 实验室标准器具
    /// </summary>
    public class InstrumentServiceImpl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InstrumentServiceImpl));
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int InstrumentId)
        {
            try
            {
                DBProvider.dbMapper.mapper.BeginTransaction();

                ServiceProvider.OrderSendInstrumentService.DeleteByInstrumentId(InstrumentId);//送检仪器清单DeleteByInstrumentId
                ServiceProvider.InstrumentWaitSendService.DeleteByInstrumentId(InstrumentId);//待送检仪器清单
                ServiceProvider.InstrumentCertificationService.DeleteByInstrumentId(InstrumentId);//周检记录
                ServiceProvider.InstrumentRepairPlanService.DeleteByInstrumentId(InstrumentId);//待送检仪器清单
                ServiceProvider.DocumentService.DeleteByInstrumentId(InstrumentId);//体系文件适用的仪器
                ServiceProvider.InstrumentPeriodcheckService.DeleteById(InstrumentId);//期间核查
                DBProvider.InstrumentDAO.DeleteById(InstrumentId);

                DBProvider.dbMapper.mapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                DBProvider.dbMapper.mapper.RollBackTransaction();
            }
        }

        public IList<InstrumentModel> GetAllInstrumentListByWhere(string where)
        {
            return DBProvider.InstrumentDAO.GetAllInstrumentListByWhere(where);
        }
        /// <summary>
        /// 多处使用方法
        /// </summary>
        /// <param name="instrumentId"></param>
        public string BeginSynInstrument(int instrumentId)
        {
            try
            {
                Instrument.Common.Models.InstrumentModel model = ServiceProvider.InstrumentService.GetById(instrumentId);
                if (model == null) return "{\"Msg\":\"该仪器不存在\"}";
                model.ownCheckLogList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(instrumentId).Where(c => c.RecordState == Constants.InstrumentCertificationState.完成周检.GetHashCode()).ToList();
                return WSProvider.CommonProvider.SendInstrumentData(ToolsLib.Utility.CommonUtils.JsonSerialize(model));
            }
            catch (Exception ex)
            {
                return "仪器同步到业务系统时失败";
            }

        }
        ///// <summary>
        ///// 保存实体数据.
        ///// </summary>
        //public void Save(InstrumentModel model, InstrumentCertificationModel certModel, HttpFileCollectionBase Files)
        //{
        //    model.LastUpdateUser = LoginHelper.LoginUser.UserName;

        //    if (model.InstrumentId == 0)
        //    {
        //        if (Files != null && Files.Count > 0 && Files[0].ContentLength > 0 && certModel != null)
        //        {
        //            //if (certModel.RecordState == Constants.InstrumentCertificationState.未检.GetHashCode()) 

        //            string targetPath = WebUtils.GetSettingsValue("InstrumentCertificationPath") + @"/";
        //            string targetFile = string.Format(@"{0}/{1}{2}", targetPath, StrUtils.GetUniqueFileName(null), Path.GetExtension(Files[0].FileName));
        //            Global.Common.Models.AttachmentModel attModel = UtilsHelper.FileUpload(Files[0], targetFile, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
        //            attModel.FileType = (int)UtilConstants.AttachmentType.仪器周检证书;
        //            attModel.UserId = LoginHelper.LoginUser.UserId;
        //            attModel.UserName = LoginHelper.LoginUser.UserName;
        //            Global.Business.ServiceProvider.AttachmentService.Save(attModel);
        //            certModel.FileId = attModel.FileId;
        //            //certModel.FileVirtualPath = WebUtils.GetSettingsValue("WebFileServer")+targetFile;
        //        }

        //        model.BarCode = DateTime.Now.ToString("yyyyMMddff") + ToolsLib.Utility.StrUtils.GetRandomNumb(6);
        //        model.ItemCode = Guid.NewGuid().ToString();
        //        DBProvider.InstrumentDAO.Add(model);
        //        if (certModel != null)
        //        {
        //            if (!string.IsNullOrEmpty(certModel.CreateUser))
        //            {
        //                certModel.InstrumentId = model.InstrumentId;
        //                //新增仪器时当是完成周检且仪器状态合格则设为正在使用的
        //                if (model.RecordState == UtilConstants.InstrumentState.合格.GetHashCode()
        //                    && certModel.RecordState == Constants.InstrumentCertificationState.完成周检.GetHashCode()) certModel.IsUseding = true;
        //                certModel.ItemCode = Guid.NewGuid().ToString();
        //                DBProvider.InstrumentCertificationDAO.Add(certModel);
        //            }
        //        }
        //    }

        //}

        /// <summary>
        /// 仪器保存
        /// </summary>
        /// <param name="model"></param>
        public void Save4Instrument(InstrumentModel model)
        {
            model.LastUpdateUser = LoginHelper.LoginUser.UserName;
            if (model.ManageLevel != "C")//当管理级别是免检不需要判断
            {
                //当状态为正常或者停用的时候，根据有效期结束时间重新调整器具状态，有效期结束时间等于当天属于正常。
                if (model.RecordState == (int)UtilConstants.InstrumentState.合格 || model.RecordState == (int)UtilConstants.InstrumentState.过期禁用)
                    if (model.DueEndDate >= Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                        model.RecordState = (int)UtilConstants.InstrumentState.合格;
                    else
                        model.RecordState = (int)UtilConstants.InstrumentState.过期禁用;
            }
            if (model.InstrumentId == 0)
            {
                model.CreateUser = LoginHelper.LoginUser.UserName;
                model.BarCode = DateTime.Now.ToString("yyyyMMddff") + ToolsLib.Utility.StrUtils.GetRandomNumb(6);
                model.ItemCode = Guid.NewGuid().ToString();
                DBProvider.InstrumentDAO.Add4Instrument(model);
            }
            else
            {
                DBProvider.InstrumentDAO.Update4Instrument(model);
            }
            //同步仪器数据
            if (LoginHelper.LoginUserAuthorize.Contains("/Instrument/SynInstrument".ToLower()))
            {
                BeginSynInstrument(model.InstrumentId);
            }
        }

        /// <summary>
        /// 固定资产保存
        /// </summary>
        /// <param name="model"></param>
        public void Save4Assets(InstrumentModel model)
        {
            model.LastUpdateUser = LoginHelper.LoginUser.UserName;
            if (model.InstrumentId == 0)
            {
                model.CreateUser = LoginHelper.LoginUser.UserName;
                model.BarCode = DateTime.Now.ToString("yyyyMMddff") + ToolsLib.Utility.StrUtils.GetRandomNumb(6);
                model.ItemCode = Guid.NewGuid().ToString();
                DBProvider.InstrumentDAO.Add4Assets(model);
            }
            else
            {
                DBProvider.InstrumentDAO.Update4Assets(model);
            }
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public InstrumentModel GetById(int InstrumentId)
        {
            return DBProvider.InstrumentDAO.GetById(InstrumentId);
        }

        public InstrumentModel GetByBarCode(string barCode)
        {
            return DBProvider.InstrumentDAO.GetByBarCode(barCode);
        }

        public IList<InstrumentModel> GetByIdList(IList<int> instrumentIdList)
        {
            return DBProvider.InstrumentDAO.GetByIdList(instrumentIdList);
        }


        /// <summary>
        /// 更新仪器下对应的工艺过程
        /// </summary>
        /// <param name="craftId"></param>
        /// <param name="instrumentId"></param>
        public void UpdateInstrumentOfCraft(int craftId, int instrumentId)
        {
            DBProvider.InstrumentDAO.UpdateInstrumentOfCraft(craftId, instrumentId);
        }
        public void UpdateCertificationInfo(InstrumentModel model)
        {
            DBProvider.InstrumentDAO.UpdateCertificationInfo(model);
        }


        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<InstrumentModel> GetAll()
        {
            return DBProvider.InstrumentDAO.GetAll();
        }
        /// <summary>
        /// 根据条件获得记录.
        /// </summary>
        public IList<InstrumentModel> GetByWhere(string where)
        {
            return DBProvider.InstrumentDAO.GetByWhere(where);
        }
        public IList<Hashtable> GetAllInstrumentListForPaging(PagingModel paging)
        {
            return DBProvider.InstrumentDAO.GetAllInstrumentListForPaging(paging);
        }

        public IList<Hashtable> GetAllPeriodcheckListForPaging(PagingModel paging)
        {
            return DBProvider.InstrumentDAO.GetAllPeriodcheckListForPaging(paging);
        }
        public IList<Hashtable> GetAllInnerCheckListForPaging(PagingModel paging)
        {
            return DBProvider.InstrumentDAO.GetAllInnerCheckListForPaging(paging);
        }


        /// <summary>
        /// 通过craftId获得所有记录.
        /// </summary>
        public IList<InstrumentModel> GetByCraftIdList(int craftId)
        {
            return DBProvider.InstrumentDAO.GetByCraftIdList(craftId);
        }

        public IList<Hashtable> GetAllCertificationListForPaging(PagingModel paging)
        {
            return DBProvider.InstrumentDAO.GetAllCertificationListForPaging(paging);
        }


        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public InstrumentModel GetDetailsById(int instrumentId)
        {
            InstrumentModel model = DBProvider.InstrumentDAO.GetById(instrumentId);
            if (model == null) model = new InstrumentModel();
            else
            {
                model.InstrumentCertification = DBProvider.InstrumentCertificationDAO.GetById(instrumentId);
                if (model.InstrumentCertification == null) model.InstrumentCertification = new InstrumentCertificationModel();
            }
            return model;
        }

        /// <summary>
        /// 通过ID列表得到对象实体集合.
        /// </summary>
        public IList<InstrumentModel> GetByIds(IList<int> InstrumentIdList)
        {
            List<InstrumentModel> List = new List<InstrumentModel>();
            int maxLen = 500;
            int queryCount = InstrumentIdList.Count / maxLen + (InstrumentIdList.Count % maxLen == 0 ? 0 : 1);
            for (int i = 0; i < queryCount; i++)
            {
                var tempArray = InstrumentIdList.Skip(i * maxLen).Take(maxLen).ToList();
                IList<InstrumentModel> mOrg = DBProvider.InstrumentDAO.GetByIds(tempArray);
                List.AddRange(mOrg);
            }
            return List;
        }

        public void UpdateInstrumentSetCraftNull(int craftId)
        {
            DBProvider.InstrumentDAO.UpdateInstrumentSetCraftNull(craftId);
        }

        /// <summary>
        /// 上一次修改人和修改时间
        /// </summary>
        /// <param name="model"></param>
        public void UpdateLastUpdateInfo(InstrumentModel model)
        {
            DBProvider.InstrumentDAO.UpdateLastUpdateInfo(model);
        }

        /// <summary>
        /// 最近盘点时间和盘点人
        /// </summary>
        /// <param name="model"></param>
        public void UpdateLastCheckInfo(InstrumentModel model)
        {
            DBProvider.InstrumentDAO.UpdateLastCheckInfo(model);
        }


        /// <summary>
        /// 是否存在管理编号
        /// </summary>
        /// <param name="manageNo"></param>
        /// <returns></returns>
        public bool IsExistManageNo(int instrumentId, string manageNo)
        {
            InstrumentModel model = new InstrumentModel();
            model.ManageNo = manageNo;
            model.InstrumentId = instrumentId;
            return DBProvider.InstrumentDAO.IsExistManageNo(model);
        }

        /// <summary>
        /// 是否存在资产编号
        /// </summary>
        /// <param name="manageNo"></param>
        /// <returns></returns>
        public bool IsExistAssetsNo(int instrumentId, string assetsNo)
        {
            InstrumentModel model = new InstrumentModel();
            model.AssetsNo = assetsNo;
            model.InstrumentId = instrumentId;
            return DBProvider.InstrumentDAO.IsExistAssetsNo(model);
        }

        public void UpdateForOverDue()
        {
            DBProvider.InstrumentDAO.UpdateForOverDue();
        }

        public string BatchImportInstrument(DataTable dt, ref string sucessMsg)
        {
            StringBuilder result = new StringBuilder();
            Global.Common.Models.ParamItemModel paranItem = new Global.Common.Models.ParamItemModel();
            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            ////组织根目录
            //orgList.SingleOrDefault(o=>o.OrgCode
            //系统参数
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //器具状态
            Global.Common.Models.ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.InstrumentState);
            if (null == instrumentState) instrumentState = new Global.Common.Models.ParamModel();

            Global.Common.Models.ParamModel instrumentType = paramList.SingleOrDefault(t => t.ParamCode == UtilConstants.SysParamType.InstrumentType);
            if (null == instrumentType) instrumentType = new Global.Common.Models.ParamModel();

            //设备分类
            Global.Common.Models.ParamModel instrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate);
            if (null == instrumentCate) instrumentCate = new Global.Common.Models.ParamModel();

            //资产属性
            Global.Common.Models.ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new Global.Common.Models.ParamModel();

            //管理级别
            Global.Common.Models.ParamModel manageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel);
            if (null == manageLevel) manageLevel = new Global.Common.Models.ParamModel();

            //计量类别
            Global.Common.Models.ParamModel verificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType);
            if (null == verificationType) verificationType = new Global.Common.Models.ParamModel();

            IList<Global.Common.Models.OrgModel> belongDepartList = new List<Global.Common.Models.OrgModel>();
            IList<InstrumentModel> instrumentList = new List<InstrumentModel>();
            //判断是否需要检测管理编号存在
            string IsJudgeExistManageNo = WebUtils.GetSettingsValue("IsJudgeExistManageNo");
            if (IsJudgeExistManageNo == "true")
            {
                instrumentList = GetAll().Where(t => !string.IsNullOrWhiteSpace(t.ManageNo)).ToList();
            }
            IList<string> manageNoList = new List<string>();
            int count = 0;
            bool IsSynInstrument = LoginHelper.LoginUserAuthorize.Contains("/Instrument/SynInstrument".ToLower());
            InstrumentModel instrument = new InstrumentModel();
            int sucessCount = 0;
            foreach (DataRow dr in dt.Rows)
            {

                try
                {
                    if (string.IsNullOrWhiteSpace(dr["仪器名称"].ToString()))
                    {
                        //result.AppendLine("仪器名称不能为空");
                        continue;
                    }
                    instrument.InstrumentForm = Constants.InstrumentForm.仪器.GetHashCode();//仪器
                    instrument.InstrumentName = UtilsHelper.SpecialCharValidate(dr["仪器名称"].ToString());
                    //型号
                    instrument.Specification = UtilsHelper.SpecialCharValidate(dr["仪器型号"].ToString());
                    //管理编号
                    instrument.ManageNo = UtilsHelper.SpecialCharValidate(dr["管理编号"].ToString());

                    if (IsJudgeExistManageNo == "true")
                    {
                        //在库中查找管理编号
                        count = instrumentList.Where(t => t.ManageNo.Trim().Equals(instrument.ManageNo.Trim())).Count();
                        //在导入的数据中找管理编号
                        if (count == 0) count = manageNoList.Where(t => t.Trim().Equals(instrument.ManageNo.Trim())).Count();
                        if (count > 0)
                        {
                            result.AppendLine("存在相同的管理编号(" + instrument.ManageNo + ")");
                            continue;
                        }
                        if (!string.IsNullOrWhiteSpace(instrument.ManageNo)) manageNoList.Add(instrument.ManageNo);
                    }

                    //出厂编号
                    instrument.SerialNo = UtilsHelper.SpecialCharValidate(dr["出厂编号"].ToString());
                    //技术特征
                    instrument.TechniqueCharacter = UtilsHelper.SpecialCharValidate(dr["技术特征"].ToString());
                    //计量特性
                    instrument.MeasureCharacter = UtilsHelper.SpecialCharValidate(dr["计量特性"].ToString());
                    //设备状态
                    paranItem = instrumentState.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["设备状态"].ToString()));
                    instrument.RecordState = paranItem == null ? 0 : Convert.ToInt32(paranItem.ParamItemValue);

                    //设备分类
                    paranItem = instrumentCate.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["设备分类"].ToString()));
                    instrument.InstrumentCate = paranItem == null ? 1 : Convert.ToInt32(paranItem.ParamItemValue);

                    //资产属性
                    paranItem = calibrationType.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["资产属性"].ToString()));
                    instrument.CalibrationType = paranItem == null ? 0 : Convert.ToInt32(paranItem.ParamItemValue);

                    //设备类别
                    paranItem = instrumentType.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["设备类别"].ToString()));
                    instrument.InstrumentType = paranItem == null ? 0 : Convert.ToInt32(paranItem.ParamItemValue);

                    //管理级别
                    paranItem = manageLevel.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["管理级别"].ToString()));
                    instrument.ManageLevel = paranItem == null ? "A" : paranItem.ParamItemValue;

                    //计量类别
                    paranItem = verificationType.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["计量类别"].ToString()));
                    instrument.VerificationType = paranItem == null ? 0 : Convert.ToInt32(paranItem.ParamItemValue);
                    //英文名称
                    instrument.EnglishName = UtilsHelper.SpecialCharValidate(dr["英文名称"].ToString());
                    //所属部门
                    belongDepartList = orgList.Where(o => o.OrgName.Equals(dr["所属部门"].ToString().Trim())).ToList();
                    if (belongDepartList.Count > 0) instrument.BelongDepart = belongDepartList[0].OrgCode;
                    else instrument.BelongDepart = "";
                    //仪器证书信息
                    //证书编号
                    instrument.CertificateNo = UtilsHelper.SpecialCharValidate(dr["证书编号"].ToString());
                    if (!string.IsNullOrWhiteSpace(dr["校准日期"].ToString())) instrument.DueStartDate = Convert.ToDateTime(dr["校准日期"].ToString());
                    if (!string.IsNullOrWhiteSpace(dr["到期日期"].ToString())) instrument.DueEndDate = Convert.ToDateTime(dr["到期日期"].ToString());
                    //计量机构
                    instrument.InspectOrg = UtilsHelper.SpecialCharValidate(dr["计量机构"].ToString());

                    //资产编号
                    instrument.AssetsNo = UtilsHelper.SpecialCharValidate(dr["资产编号"].ToString());

                    //校验周期
                    instrument.InspectCycle = UtilsHelper.SpecialCharValidate(dr["检验周期"].ToString());
                    //使用年限
                    instrument.DurableYears = UtilsHelper.SpecialCharValidate(dr["使用年限"].ToString());
                    //保管人
                    instrument.LeaderName = UtilsHelper.SpecialCharValidate(dr["保管人"].ToString());
                    //项目组
                    instrument.ProjectTeam = UtilsHelper.SpecialCharValidate(dr["项目组"].ToString());
                    //说明书编号
                    //instrument.SpecificationCode = dr[23].ToString();
                    //存放地点
                    instrument.StorePalce = UtilsHelper.SpecialCharValidate(dr["存放地点"].ToString());
                    //购置日期
                    if (!string.IsNullOrWhiteSpace(dr["购置日期"].ToString())) instrument.BuyDate = Convert.ToDateTime(dr["购置日期"].ToString());
                    //购置金额
                    if (!string.IsNullOrWhiteSpace(dr["购置金额"].ToString())) instrument.Price = Convert.ToDecimal(dr["购置金额"].ToString());
                    //生产厂家
                    instrument.Manufacturer = UtilsHelper.SpecialCharValidate(dr["生产厂家"].ToString());
                    //厂家联系信息
                    instrument.ManufactureContactor = UtilsHelper.SpecialCharValidate(dr["厂家联系信息"].ToString());
                    //备注
                    instrument.Remark = UtilsHelper.SpecialCharValidate(dr["备注"].ToString());

                    instrument.CreateUser = LoginHelper.LoginUser.UserName;
                    instrument.LastUpdateUser = LoginHelper.LoginUser.UserName;
                    instrument.BarCode = DateTime.Now.ToString("yyyyMMddff") + ToolsLib.Utility.StrUtils.GetRandomNumb(6);
                    instrument.ItemCode = Guid.NewGuid().ToString();
                    //新增仪器
                    DBProvider.InstrumentDAO.Add4Instrument(instrument);

                    //仪器证书信息 当三个都不为空时保存
                    if (!string.IsNullOrWhiteSpace(instrument.CertificateNo) && (instrument.DueStartDate != null) && (instrument.DueEndDate != null))
                    {
                        InstrumentCertificationModel cert = new InstrumentCertificationModel();
                        cert.InstrumentId = instrument.InstrumentId;
                        cert.CertificationCode =  UtilsHelper.SpecialCharValidate(instrument.CertificateNo);
                        cert.CheckDate = instrument.DueStartDate;
                        cert.EndDate = instrument.DueEndDate;
                        cert.MeasureOrg = instrument.InspectOrg;
                        cert.RecordState = Constants.InstrumentCertificationState.完成周检.GetHashCode();
                        ServiceProvider.InstrumentCertificationService.SaveCert(cert);
                        ServiceProvider.InstrumentCertificationService.UpdateCertificationAndState(instrument.InstrumentId);
                    }
                    //同步到业务系统
                    if (IsSynInstrument && instrument.InstrumentCate > 0)
                    {
                        BeginSynInstrument(instrument.InstrumentId);
                    }
                    //导入成功记录
                    sucessCount++;

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    result.AppendLine("未成功导入仪器:名称(" + instrument.InstrumentName + ") 管理编号(" + instrument.ManageNo + ")");
                }
            }
            sucessMsg = "共导入" + sucessCount + "条记录";
            return result.ToString();
        }

        /// <summary>
        /// 批量导入固定资产
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sucessMsg"></param>
        /// <returns></returns>
        public string BatchImportAssets(DataTable dt, ref string sucessMsg)
        {
            StringBuilder result = new StringBuilder();
            Global.Common.Models.ParamItemModel paranItem = new Global.Common.Models.ParamItemModel();
            IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
            //系统参数
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            //器具状态
            Global.Common.Models.ParamModel instrumentState = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.AssetsState);
            if (null == instrumentState) instrumentState = new Global.Common.Models.ParamModel();
            //资产属性
            Global.Common.Models.ParamModel calibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType);
            if (null == calibrationType) calibrationType = new Global.Common.Models.ParamModel();

            IList<Global.Common.Models.OrgModel> belongDepartList = new List<Global.Common.Models.OrgModel>();
            InstrumentModel instrument = new InstrumentModel();
            int sucessCount = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(dr["资产名称"].ToString()))
                    {
                        continue;
                    }
                    instrument.InstrumentForm = Constants.InstrumentForm.固定资产.GetHashCode();
                    instrument.InstrumentName = UtilsHelper.SpecialCharValidate(dr["资产名称"].ToString());
                    instrument.Specification = UtilsHelper.SpecialCharValidate(dr["型号规格"].ToString());
                    instrument.SerialNo = UtilsHelper.SpecialCharValidate(dr["出厂编号"].ToString());
                    //设备状态
                    paranItem = instrumentState.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["设备状态"].ToString()));
                    instrument.RecordState = paranItem == null ? 0 : Convert.ToInt32(paranItem.ParamItemValue);
                    //资产属性
                    paranItem = calibrationType.itemsList.SingleOrDefault(i => i.ParamItemName.Equals(dr["资产属性"].ToString()));
                    instrument.CalibrationType = paranItem == null ? 0 : Convert.ToInt32(paranItem.ParamItemValue);
                    //所属部门
                    belongDepartList = orgList.Where(o => o.OrgName.Equals(dr["所属部门"].ToString().Trim())).ToList();
                    if (belongDepartList.Count > 0) instrument.BelongDepart = belongDepartList[0].OrgCode;
                    else instrument.BelongDepart = "";
                    instrument.AssetsNo = UtilsHelper.SpecialCharValidate(dr["资产编号"].ToString());
                    instrument.ManageNo = UtilsHelper.SpecialCharValidate(dr["管理编号"].ToString());
                    instrument.LeaderName = UtilsHelper.SpecialCharValidate(dr["保管人"].ToString());
                    instrument.StorePalce = UtilsHelper.SpecialCharValidate(dr["存放地点"].ToString());
                    if (!string.IsNullOrWhiteSpace(dr["购置日期"].ToString()))
                    {
                        DateTime buyDate;
                        if (DateTime.TryParse(dr["购置日期"].ToString(), out buyDate))
                            instrument.BuyDate = buyDate;
                        else
                        {
                            result.AppendLine("购置日期【" + dr["购置日期"].ToString() + "】格式不正确");
                            continue;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(dr["购置金额"].ToString())) instrument.Price = Convert.ToDecimal(dr["购置金额"].ToString());
                    instrument.Manufacturer = UtilsHelper.SpecialCharValidate(dr["生产厂家"].ToString());
                    instrument.ManufactureContactor = UtilsHelper.SpecialCharValidate(dr["厂家联系信息"].ToString());
                    instrument.Remark = UtilsHelper.SpecialCharValidate(dr["备注"].ToString());

                    instrument.CreateUser = LoginHelper.LoginUser.UserName;
                    instrument.LastUpdateUser = LoginHelper.LoginUser.UserName;
                    instrument.BarCode = DateTime.Now.ToString("yyyyMMddff") + ToolsLib.Utility.StrUtils.GetRandomNumb(6);
                    instrument.ItemCode = Guid.NewGuid().ToString();
                    //新增
                    DBProvider.InstrumentDAO.Add4Assets(instrument);

                    //导入成功记录
                    sucessCount++;

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    result.AppendLine("未成功导入资产:名称(" + instrument.InstrumentName + ") 资产编号(" + instrument.AssetsNo + ")");
                }
            }
            sucessMsg = "共导入" + sucessCount + "条记录";
            return result.ToString();
        }

        /// <summary>
        /// 上传仪器证书
        /// </summary>
        /// <param name="businessNumber"></param>
        /// <param name="model"></param>
        /// <param name="Files"></param>
        public string UploadCerts(HttpPostedFileBase Files, ref string LogFiledName)
        {
            string Msg = "无匹配证书！";
            string targetDir = WebUtils.GetSettingsValue("InstrumentCertificationPath");
            //获取后缀名
            string Extension = System.IO.Path.GetExtension(Files.FileName).ToLower();
            ToolsLib.FileService.FileCompress fc = new ToolsLib.FileService.FileCompress();
            //解压包的路径
            string newFile = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString() + Extension);
            //保存
            Files.SaveAs(newFile);
            string ExtractDir = ToolsLib.Utility.CommonUtils.GetPhysicsPath("/tempFile/" + Guid.NewGuid().ToString());
            string ext = fc.Extract(newFile, ExtractDir);
            //解压后删除临时文件
            File.Delete(newFile);
            //获取目录下的文件     
            string[] fileList = Directory.GetFiles(ExtractDir, "*", SearchOption.AllDirectories);
            IList<Hashtable> Recordlist = new List<Hashtable>();
            Global.Common.Models.AttachmentModel attachment = null;
            string CertName = string.Empty;
            foreach (var file in fileList)
            {
                Hashtable ht = new Hashtable();
                CertName = Path.GetFileNameWithoutExtension(file);
                if (System.IO.Path.GetExtension(file).ToLower() == ".pdf")
                {
                    IList<InstrumentCertificationModel> list = ServiceProvider.InstrumentCertificationService.GetListByCertificationCode(CertName);
                    if (list.Count > 0)
                    {
                        attachment = new Global.Common.Models.AttachmentModel();
                        string fileName = Guid.NewGuid().ToString();
                        string targetFileName = string.Format(@"{0}/{1}{2}", targetDir, fileName, Path.GetExtension(file));
                        try
                        {
                            //原始记录上传操作
                            attachment = UtilsHelper.FileUpload(file, targetFileName, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
                            //入库
                            Global.Business.ServiceProvider.AttachmentService.Save(attachment);
                            ServiceProvider.InstrumentCertificationService.UpdateCertFile(list, attachment);
                            Msg = "OK";
                            Recordlist.Add(setUploadLog(Path.GetFileName(file), "成功", ""));
                        }
                        catch (Exception ex)
                        {
                            Recordlist.Add(setUploadLog(Path.GetFileName(file), "失败", "上传证书出现异常"));
                            log.ErrorFormat("{0},证书上传出现异常：{1}", Path.GetFileName(file), ex);
                            continue;
                        }
                    }
                    else
                        Recordlist.Add(setUploadLog(Path.GetFileName(file), "失败", "证书信息不存在"));
                }
                else
                {
                    Recordlist.Add(setUploadLog(Path.GetFileName(file), "失败", "文件格式不正确"));
                }
                File.Delete(file);
            }
            LogFiledName = SetCertsLog(Recordlist);
            //删除目录
            Directory.Delete(ExtractDir, true);
            return Msg;
        }
        private Hashtable setUploadLog(string Record, string RecordState, string Remark)
        {
            Hashtable ht = new Hashtable();
            ht["Record"] = Record;// Path.GetFileName(file);
            ht["RecordState"] = RecordState;//"失败";
            ht["Remark"] = Remark;// "文件格式不正确";
            return ht;
        }
        private string SetCertsLog(IList<Hashtable> ItemList)
        {
            //数据列表
            DataTable dtData = new DataTable();
            dtData.Columns.Add("Record", typeof(string));    //记录
            dtData.Columns.Add("RecordState", typeof(string));    //状态
            dtData.Columns.Add("Remark", typeof(string));    //备注

            foreach (var item in ItemList)
            {
                DataRow drData = dtData.NewRow();
                foreach (DictionaryEntry h in item)
                {
                    drData[h.Key.ToString()] = h.Value.ToString();
                }
                dtData.Rows.Add(drData);
            }
            //导出
            List<string> headerList = new List<string>(new string[] { 
                "记录","状态","备注"});
            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "Sheet1", ToolsLib.LibConst.ExcelVersion.Excel2007);
            return Path.GetFileNameWithoutExtension(result);
            // ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}证书上传结果清单{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
        }

        /// <summary>
        /// 用in的方式批量删除
        /// </summary>
        public void DeleteBatchByIds(string InstrumentIds)
        {
            DBProvider.InstrumentDAO.DeleteBatchByIds(InstrumentIds);
        }

        /// <summary>
        /// 获取关键字查询仪器信息;
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IList<InstrumentModel> GetInstrumentListByKeyWorks(string key, string instrumentForm)
        {
            Hashtable ht = new Hashtable();
            ht["key"] = key;// 
            ht["instrumentForm"] = instrumentForm;//;
            return DBProvider.InstrumentDAO.GetInstrumentListByKeyWorks(ht);
        }

        public void ExportOverTimeAndWarnList(int state, int day)
        {
            StringBuilder sqlWhere = new StringBuilder();
            ParamItemModel mParamItem = new ParamItemModel();
            IList<ParamItemModel> overTimeList = new List<ParamItemModel>();
            IList<InstrumentWaitSendModel> preSendList = new List<InstrumentWaitSendModel>();
            if (state == 0)
            {
                sqlWhere.AppendFormat("'{0:yyyy-MM-dd}'>DueEndDate and ManageLevel !='C' and RecordState={1}", DateTime.Now, UtilConstants.InstrumentState.过期禁用.GetHashCode());
                preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            }
            if (state == 1)
                sqlWhere.AppendFormat("DueEndDate<='{0:yyyy-MM-dd}' and DueEndDate>='{1:yyyy-MM-dd}' and ManageLevel !='C'", DateTime.Now.AddDays(day), DateTime.Now);
            IList<Instrument.Common.Models.InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(sqlWhere.ToString());
            //数据列表
            DataTable dtData = new DataTable();
            if (state == 0)
            {
                dtData.Columns.Add("RecordState", typeof(string));    //仪器状态
                overTimeList = Global.Business.ServiceProvider.ParamService.GetAll().SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState).itemsList;
            }
            if (state == 1)
                dtData.Columns.Add("WarnDay", typeof(string));    //预警天数
            dtData.Columns.Add("InstrumentName", typeof(string));    //仪器名字
            dtData.Columns.Add("CertificateNo", typeof(string));    //证书编号
            dtData.Columns.Add("DueEndDate", typeof(string));    //到期日期
            dtData.Columns.Add("Specification", typeof(string));    //仪器型号
            dtData.Columns.Add("ManageNo", typeof(string));    //管理编号
            dtData.Columns.Add("SerialNo", typeof(string));    //出厂编号

            int warnDay;
            DateTime dueEndDate;
            InstrumentWaitSendModel instrumentWaitSendModel = null;
            foreach (var item in instrumentList)
            {
                DataRow drData = dtData.NewRow();
                if (state == 0)
                {
                    instrumentWaitSendModel = preSendList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                    if (instrumentWaitSendModel != null) continue;
                    mParamItem = overTimeList.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", item.RecordState));
                    drData["RecordState"] = mParamItem == null ? "" : mParamItem.ParamItemName;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(string.Format("{0}", item.DueEndDate)))
                        dueEndDate = Convert.ToDateTime(string.Format("{0:d}", item.DueEndDate));
                    else
                        dueEndDate = DateTime.MinValue;
                    warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                    warnDay = warnDay < 0 ? 0 : warnDay;
                    warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                    warnDay = warnDay < 0 ? 0 : warnDay;
                    drData["WarnDay"] = warnDay;
                }
                drData["InstrumentName"] = item.InstrumentName;
                drData["CertificateNo"] = item.CertificateNo;
                drData["DueEndDate"] = string.Format("{0:yyyy-MM-dd}",item.DueEndDate);
                drData["Specification"] = item.Specification;
                drData["ManageNo"] = item.ManageNo;
                drData["SerialNo"] = item.SerialNo;
                dtData.Rows.Add(drData);
            }
            //导出
            string[] head = new string[] { "仪器状态", "仪器名称", "证书编号", "到期日期", "仪器型号", "管理编号", "出厂编号" };
            if (state == 1)
                head[0] = "预警天数";
            List<string> headerList = new List<string>(head);

            string result = ToolsLib.FileService.ExcelFile.WriteDataToExcel(headerList, dtData, "Sheet1", ToolsLib.LibConst.ExcelVersion.Excel2007);
            ToolsLib.FileService.WebServer.DownLoadFile(result, string.Format("{0:yyyyMMddHHmmss}仪器预警{1}", DateTime.Now, System.IO.Path.GetExtension(result)), true);
        }

        //根据管理编号查仪器
        public InstrumentModel GetByManageNo(string manageNo)
        {
            return DBProvider.InstrumentDAO.GetByManageNo(manageNo);
        }

        //保存仪器照片
        public void SaveInstrumentPic(HttpFileCollectionBase Files, int instrumentId)
        {
            if (Files["InstrumentPic"] != null)
            {
                //保存仪器图片
                string targetPath = WebUtils.GetSettingsValue("InstrumentPicFilePath");
                string targetFile = string.Format(@"{0}/{1}{2}", targetPath, StrUtils.GetUniqueFileName(null), Path.GetExtension(Files["InstrumentPic"].FileName));
                AttachmentModel attModel = UtilsHelper.FileUpload(Files["InstrumentPic"], targetFile, (UtilConstants.ServerType)Convert.ToInt32(WebUtils.GetSettingsValue("WebFileType") == null ? "1" : WebUtils.GetSettingsValue("WebFileType")));
                attModel.FileType = (int)Instrument.Common.Constants.AttachmentBusinessType.仪器照片;
                attModel.UserId = LoginHelper.LoginUser.UserId;
                attModel.UserName = LoginHelper.LoginUser.UserName;
                Global.Business.ServiceProvider.AttachmentService.Save(attModel);
                BusinessAttachmentModel businessAttachmentModel = new BusinessAttachmentModel();
                businessAttachmentModel.BusinessKeyId = instrumentId;
                businessAttachmentModel.BusinessType = attModel.FileType;
                businessAttachmentModel.UserName = LoginHelper.LoginUser.UserName;
                businessAttachmentModel.FileName = attModel.FileName;
                businessAttachmentModel.FileId = attModel.FileId;
                Instrument.Business.ServiceProvider.BusinessAttachmentService.Save(businessAttachmentModel);
            }
        }

        //根据ParentID来查找配件设备
        public IList<InstrumentModel> GetInstrumentByParentId(int parentId)
        {
            return DBProvider.InstrumentDAO.GetInstrumentByParentId(parentId);
        }
    }

}
