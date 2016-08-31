using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Instrument.Business;
using Instrument.Common.Models;
using System.Collections;
using Instrument.Common;
using ToolsLib.Utility;
using Global.Common.Models;
using GRGTCommonUtils;

namespace Instrument.Business.WCF
{
    [ServiceContract]
    public class InstrumentWCFServices
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InstrumentWCFServices));
        private string commonErrorMsg = "连接超时,请重新登录!";
        /// <summary>
        /// 登陆接口
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string AppLogin(string JobNo, string loginPwd, string authorCode)
        {
            try
            {
                string errMsg = "OK";
                bool IsSSOLogin = Convert.ToBoolean(WebUtils.GetSettingsValue("IsSSOLogin"));
                //单点登录
                if (IsSSOLogin)
                {
                    errMsg = GRGTCommonUtils.WSProvider.HRProvider.Login(JobNo, loginPwd, string.Empty);
                }
                Global.Common.Models.UserModel user = Global.Business.ServiceProvider.UserService.GetByLoginName(JobNo);
                if (user == null) errMsg = "用户不存在！";
                if (!IsSSOLogin && user != null)
                {
                    if (user.LoginPwd != ToolsLib.Utility.StrUtils.Encrypt(loginPwd, ToolsLib.LibConst.EncryptFormat.SHA1))
                    {
                        errMsg = "密码错误！";
                    }
                }
                if (errMsg != "OK")
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = errMsg });
                }
                else
                {
                    string AccessToKen = ToKenHelper.CreateToKen(JobNo);
                    ///判断是否有送检权限
                    bool hasRoleSend = false;
                    for (int i = 0; i < user.roleList.Count; i++)
                    {
                        IList<PermissionModel> list = user.roleList[i].permissionList.Where(c => !string.IsNullOrEmpty(c.PermissionResource)).ToList();
                        if (list.Where(c => c.PermissionResource.ToLower().Contains("/SendInstrument/SendInstrument".ToLower())).Count() > 0)
                        {
                            hasRoleSend = true;
                        }
                    }
                    string VesionCode = Global.Common.GlobalConstants.VesionCode;
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new
                    {
                        Msg = errMsg,
                        Data = new
                        {
                            RoleInstrumenSend = hasRoleSend,
                            Url = "",
                            AccessToKen = AccessToKen
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                object msg = new { Msg = ex.Message };
                return ToolsLib.Utility.CommonUtils.JsonSerialize(msg);
            }
        }
        
        /// <summary>
        /// 内部登陆接口--不需要密码
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string InternalLogin(string json)
        {
            try
            {
                string errMsg = "OK";
                bool IsSSOLogin = Convert.ToBoolean(WebUtils.GetSettingsValue("IsSSOLogin"));
                var jObject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                string JobNo = jObject.Property("JobNo").Value.ToString();
                Global.Common.Models.UserModel user = Global.Business.ServiceProvider.UserService.GetByLoginName(JobNo);
                if (user == null) errMsg = "用户不存在！";
                if (errMsg != "OK")
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = errMsg });
                }
                else
                {
                    string AccessToKen = ToKenHelper.CreateToKen(JobNo);
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new
                    {
                        Msg = errMsg,
                        Data = new
                        {
                            AccessToKen = AccessToKen
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                object msg = new { Msg = ex.Message };
                return ToolsLib.Utility.CommonUtils.JsonSerialize(msg);
            }
        }
        /// <summary>
        /// 获取仪器基本信息
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetBaseInfoByBarCode(string barCode, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }

                InstrumentModel model = ServiceProvider.InstrumentService.GetByBarCode(barCode);
                if (model == null)
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "设备不存在！" });
                }
                object result = new { Msg = "OK", Data = new { InstrumentName = model.InstrumentName, Specification = model.Specification, SerialNo = model.SerialNo, AssetsNo = model.AssetsNo, ManageNo = model.ManageNo, BarCode = model.BarCode, InstrumentId = model.InstrumentId, DueEndDate = model.DueEndDate == null ? string.Empty : string.Format("{0:yyyy-MM-dd}", model.DueEndDate) } };
                return ToolsLib.Utility.CommonUtils.JsonSerialize(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                object msg = new { Msg = ex.Message };
                return ToolsLib.Utility.CommonUtils.JsonSerialize(msg);
            }
        }
        /// <summary>
        /// 获取仪器完整信息(构建数据的列名和值传给调用者循环处理)
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetInstrumentDetailByBarCode(string barCode, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }
                InstrumentModel model = ServiceProvider.InstrumentService.GetByBarCode(barCode);
                if (model == null)
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "设备不存在！" });
                }
                #region 数据处理
                IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
                Global.Common.Models.OrgModel org = orgList.SingleOrDefault(S => S.OrgCode == model.BelongDepart);
                //系统参数
                IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
                IList<ParamItemModel> InstrumentCate = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCate).itemsList;
                IList<ParamItemModel> CalibrationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.CalibrationType).itemsList;
                IList<ParamItemModel> InstrumentType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentType).itemsList;
                IList<ParamItemModel> RecordState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentState).itemsList;
                IList<ParamItemModel> VerificationType = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.VerificationType).itemsList;
                IList<ParamItemModel> AssetsState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.AssetsState).itemsList;
                IList<ParamItemModel> manageLevel = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.ManageLevel).itemsList;
                ParamItemModel mInstrumentCate = InstrumentCate.SingleOrDefault(p => p.ParamItemValue.Equals(model.InstrumentCate.ToString()));
                ParamItemModel mSubInstrumentCate = InstrumentCate.SingleOrDefault(p => p.ParamItemValue.Equals(model.SubInstrumentCate.ToString()));
                ParamItemModel mCalibrationType = CalibrationType.SingleOrDefault(p => p.ParamItemValue.Equals(model.CalibrationType.ToString()));
                ParamItemModel mInstrumentType = InstrumentType.SingleOrDefault(p => p.ParamItemValue.Equals(model.InstrumentType.ToString()));
                ParamItemModel mRecordState = RecordState.SingleOrDefault(p => p.ParamItemValue.Equals(model.RecordState.ToString()));
                ParamItemModel mAssetsState = AssetsState.SingleOrDefault(p => p.ParamItemValue.Equals(model.RecordState.ToString()));
                ParamItemModel mVerificationType = VerificationType.SingleOrDefault(p => p.ParamItemValue.Equals(model.VerificationType.ToString()));
                ParamItemModel mmanageLevel = manageLevel.SingleOrDefault(p => p.ParamItemValue.Equals(string.IsNullOrEmpty(model.ManageLevel) ? "" : model.ManageLevel.ToString()));
                DateTime dueEndDate;
                bool isOverTime = false;
                int warnDay = 0;
                if (!string.IsNullOrWhiteSpace(string.Format("{0}", model.DueEndDate)))
                    dueEndDate = Convert.ToDateTime(string.Format("{0:d}", model.DueEndDate));
                else
                    dueEndDate = DateTime.MinValue;
                //预警天数
                isOverTime = dueEndDate < Convert.ToDateTime(string.Format("{0:d}", DateTime.Now));
                warnDay = (dueEndDate - Convert.ToDateTime(string.Format("{0:d}", DateTime.Now))).Days;
                warnDay = warnDay < 0 ? 0 : warnDay;
                //超期无预警天数
                if (isOverTime) warnDay = 0;

                IList<Hashtable> htList = new List<Hashtable>();
                Hashtable ht = new Hashtable();
                ht.Add("Name", model.InstrumentForm == Instrument.Common.Constants.InstrumentForm.仪器.GetHashCode() ? "仪器名称" : "资产名称");
                ht.Add("Value", string.Format("{0}", model.InstrumentName));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", model.InstrumentForm == Instrument.Common.Constants.InstrumentForm.仪器.GetHashCode() ? "仪器型号" : "型号规格");
                ht.Add("Value", string.Format("{0}", model.Specification));
                htList.Add(ht);
                if (model.InstrumentForm == Instrument.Common.Constants.InstrumentForm.仪器.GetHashCode())
                {
                    ht = new Hashtable();
                    ht.Add("Name", "证书超期");
                    ht.Add("Value", model.DueEndDate == null ? "" : isOverTime ? "已超期" : "未超期");
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "超期预警");
                    ht.Add("Value", string.Format("{0}", (warnDay == 0) ? "无预警" : warnDay.ToString() + "天"));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "到期日期");
                    ht.Add("Value", model.DueEndDate == null ? string.Empty : DateTime.Parse(model.DueEndDate.ToString()).ToString("yyyy-MM-dd"));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "设备状态");
                    ht.Add("Value", string.Format("{0}", mRecordState == null ? string.Empty : mRecordState.ParamItemName));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "证书编号");
                    ht.Add("Value", string.Format("{0}", model.CertificateNo));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "校准日期");
                    ht.Add("Value", model.DueStartDate == null ? string.Empty : DateTime.Parse(model.DueStartDate.ToString()).ToString("yyyy-MM-dd"));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "管理级别");
                    ht.Add("Value", string.Format("{0}", mmanageLevel == null ? string.Empty : mmanageLevel.ParamItemName));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "校准周期");
                    ht.Add("Value", string.Format("{0}", model.InspectCycle));
                    htList.Add(ht);
                    ht = new Hashtable();
                }
                else
                {
                    ht = new Hashtable();
                    ht.Add("Name", "资产状态");
                    ht.Add("Value", string.Format("{0}", mAssetsState == null ? string.Empty : mAssetsState.ParamItemName));
                    htList.Add(ht);
                }
                ht = new Hashtable();
                ht.Add("Name", "管理编号");
                ht.Add("Value", string.Format("{0}", model.ManageNo));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "资产编号");
                ht.Add("Value", string.Format("{0}", model.AssetsNo));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "出厂编号");
                ht.Add("Value", string.Format("{0}", model.SerialNo));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "所属部门");
                ht.Add("Value", string.Format("{0}", org == null ? string.Empty : org.OrgName));
                htList.Add(ht);
                if (model.InstrumentForm == Instrument.Common.Constants.InstrumentForm.仪器.GetHashCode())
                {
                    ht = new Hashtable();
                    ht.Add("Name", "设备分类");
                    ht.Add("Value", string.Format("{0}", mInstrumentCate == null ? string.Empty : mInstrumentCate.ParamItemName));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "设备子分类");
                    ht.Add("Value", string.Format("{0}", mSubInstrumentCate == null ? string.Empty : mSubInstrumentCate.ParamItemName));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "计量类别");
                    ht.Add("Value", string.Format("{0}", mVerificationType == null ? string.Empty : mVerificationType.ParamItemName));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "技术特征");
                    ht.Add("Value", string.Format("{0}", model.TechniqueCharacter));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "计量特性");
                    ht.Add("Value", string.Format("{0}", model.MeasureCharacter));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "存放地点");
                    ht.Add("Value", string.Format("{0}", model.StorePalce));
                    htList.Add(ht);
                    ht = new Hashtable();
                    ht.Add("Name", "设备类别");
                    ht.Add("Value", string.Format("{0}", mInstrumentType == null ? string.Empty : mInstrumentType.ParamItemName));
                    htList.Add(ht);
                }
                ht = new Hashtable();
                ht.Add("Name", "资产属性");
                ht.Add("Value", string.Format("{0}", mCalibrationType == null ? string.Empty : mCalibrationType.ParamItemName));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "保管人");
                ht.Add("Value", string.Format("{0}", model.LeaderName));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "存放地点");
                ht.Add("Value", string.Format("{0}", model.StorePalce));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name",  "购置日期");
                ht.Add("Value", model.BuyDate == null ? string.Empty : DateTime.Parse(model.BuyDate.ToString()).ToString("yyyy-MM-dd"));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "购置金额");
                ht.Add("Value", string.Format("{0:F2}", model.Price));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "生产厂家");
                ht.Add("Value", string.Format("{0}", model.Manufacturer));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "联系信息");
                ht.Add("Value", string.Format("{0}", model.ManufactureContactor));
                htList.Add(ht);
                ht = new Hashtable();
                ht.Add("Name", "备注");
                ht.Add("Value", string.Format("{0}", model.Remark));
                htList.Add(ht);
                #endregion
                object result = new { Msg = "OK", Data = htList };
                return ToolsLib.Utility.CommonUtils.JsonSerialize(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                object msg = new { Msg = ex.Message };
                return ToolsLib.Utility.CommonUtils.JsonSerialize(msg);
            }
        }
        /// <summary>
        /// 获取仪器列表信息
        /// </summary>
        /// <param name="instrumentName"></param>
        /// <param name="Specification"></param>
        /// <param name="SerialNo"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetInstrumentList(string instrumentName, string specification, string serialNo, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }

                StringBuilder where = new StringBuilder();
                where.Append("1=1");
                if (!string.IsNullOrWhiteSpace(instrumentName)) where.AppendFormat(" And InstrumentName Like '%{0}%'", instrumentName);
                if (!string.IsNullOrWhiteSpace(specification)) where.AppendFormat(" And Specification Like '%{0}%'", specification);
                if (!string.IsNullOrWhiteSpace(serialNo)) where.AppendFormat(" And SerialNo Like '%{0}%'", serialNo);
                IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(where.ToString());

                //所属部门
                IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
                IList<object> resultList = new List<object>();
                foreach (InstrumentModel e in instrumentList)
                {
                    Global.Common.Models.OrgModel org = orgList.SingleOrDefault(S => S.OrgCode == e.BelongDepart);
                    string orgName = org == null ? string.Empty : org.OrgName;
                    resultList.Add(new { InstrumentName = e.InstrumentName, AssetsNo = e.AssetsNo, SerialNo = e.SerialNo, ManageNo = e.ManageNo, Specification = e.Specification, BelongDepart = orgName, BarCode = e.BarCode, InstrumentId = e.InstrumentId });
                }

                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = resultList });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 仪器盘点
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="operatorJobNo"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string InstrumentCheck(string barCode, string operatorJobNo, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, operatorJobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;

                string msg = ServiceProvider.AssetCheckPlanDetailService.AssetCheck(barCode, user.UserId, user.UserName);

                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = msg });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 盘点地点是否一致
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="operatorJobNo"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string AssetIsRightAddressAndRemark(string barCode, string operatorJobNo,string remark, int isRightAddress, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, operatorJobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                string msg = ServiceProvider.AssetCheckPlanDetailService.AssetIsRightAddressAndRemark(barCode, user.UserId, remark, isRightAddress);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = msg });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 盘盈
        /// </summary>
        /// <param name="instrumentName"></param>
        /// <param name="sOperator"></param>
        /// <param name="specification"></param>
        /// <param name="manufacturer"></param>
        /// <param name="serialNo"></param>
        /// <param name="planId"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string AddInstrument(string instrumentName, string sOperator, string specification, string manufacturer, string serialNo, int planId, string remark, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, sOperator, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                AssetCheckPlanDetailModel model = new AssetCheckPlanDetailModel();
                model.InstrumentName = instrumentName;
                model.Checkor = user.UserName;
                model.CreateUser = user.UserName;
                model.CreateDate = DateTime.Now;
                model.CheckDate = DateTime.Now;
                model.Specification = specification;
                model.Manufacturer = manufacturer;
                model.SerialNo = serialNo;
                model.PlanId = planId;
                model.Remark = remark;
                model.Statuse = Constants.AssetsCheckStatus.盘盈.GetHashCode();
                ServiceProvider.AssetCheckPlanDetailService.Add(model);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK" });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 通过条码获取资产流转信息;
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetCirculationRecordList(string barCode, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }

                InstrumentModel instrumentModel = ServiceProvider.InstrumentService.GetByBarCode(barCode);
                if (instrumentModel == null) return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "不存在该设备!" });

                IList<InstrumentFlowModel> flowList = ServiceProvider.InstrumentFlowService.GetByInstrumentId(instrumentModel.InstrumentId);
                IList<Hashtable> list = new List<Hashtable>();
                Hashtable ht;
                for (int i = 0; i < flowList.Count; i++)
                {
                    ht = new Hashtable();
                    ht["FlowId"] = flowList[i].FlowId;
                    ht["CreateDate"] = flowList[i].CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                    ht["Creator"] = flowList[i].Creator;
                    ht["Flow_Type"] = flowList[i].Flow_Type;
                    ht["InstrumentId"] = flowList[i].InstrumentId;
                    ht["Place"] = flowList[i].Place;
                    ht["Reason"] = flowList[i].Reason;
                    list.Add(ht);
                }
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = list });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 添加流转记录;
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="sOperator"></param>
        /// <param name="currentPosition"></param>
        /// <param name="reason"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string AddCirculationRecord(string barCode, string sOperator, string currentPosition, string reason, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }

                InstrumentModel instrumentModel = ServiceProvider.InstrumentService.GetByBarCode(barCode);
                if (instrumentModel == null) return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "不存在该设备!" });
                Global.Common.Models.UserModel user = Global.Business.ServiceProvider.UserService.GetByLoginName(sOperator);
                if (user == null) return "用户不存在！";
                InstrumentFlowModel model = new InstrumentFlowModel();
                model.Creator = user.UserName;
                model.Flow_Type = 1;
                model.InstrumentId = instrumentModel.InstrumentId;
                model.Place = currentPosition;
                model.Reason = reason;

                ServiceProvider.InstrumentFlowService.Save(model);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK" });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 获取正在进行的盘点计划统计信息;
        /// </summary>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetCheckingPlanBaseInfoList(string jobNo, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                IList<AssetCheckPlanModel> planList = ServiceProvider.AssetCheckPlanService.GetByUserId(user.UserId);
                List<Hashtable> resultList = new List<Hashtable>();
                foreach (AssetCheckPlanModel model in planList)
                {
                    IList<AssetCheckPlanDetailModel> detailList = ServiceProvider.AssetCheckPlanDetailService.GetByPlanId(model.PlanId);

                    Hashtable ht = new Hashtable();
                    ht["PlanName"] = model.PlanName;
                    ht["AssetCheckSum"] = detailList.Where(W => W.Statuse != Constants.AssetsCheckStatus.盘盈.GetHashCode()).Count();
                    ht["AssetCheckedNum"] = detailList.Where(W => W.Statuse == Constants.AssetsCheckStatus.已盘点.GetHashCode()).Count();
                    ht["AssetCheckingNum"] = detailList.Where(W => W.Statuse == Constants.AssetsCheckStatus.盘亏.GetHashCode()).Count();
                    ht["OverageNum"] = detailList.Where(W => W.Statuse == Constants.AssetsCheckStatus.盘盈.GetHashCode()).Count();

                    resultList.Add(ht);
                }

                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = resultList });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 获取用户所有有效的盘点计划
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetPlanLsit(string jobNo, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                IList<AssetCheckPlanModel> planList = ServiceProvider.AssetCheckPlanService.GetByUserId(user.UserId);

                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = planList });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 根据barCode查询仪器周检记录
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetCetificationList(string barCode, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }

                InstrumentModel instrumentModel = ServiceProvider.InstrumentService.GetByBarCode(barCode);
                if (instrumentModel == null) return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "不存在该设备!" });
                IList<InstrumentCertificationModel> certList = ServiceProvider.InstrumentCertificationService.GetByInstrumentId(instrumentModel.InstrumentId);
                if (certList.Count < 1) return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "周检记录为空!" });
                ////系统参数
                IList<ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
                IList<ParamItemModel> CertificationState = paramList.SingleOrDefault(t => t.ParamCode == Instrument.Common.Constants.SysParamType.InstrumentCertificationState).itemsList;
                ParamItemModel mParamItem = new ParamItemModel();
                for (int i = 0; i < certList.Count; i++)
                {
                    mParamItem = CertificationState.SingleOrDefault(c => c.ParamItemValue == string.Format("{0}", certList[i].RecordState));
                    certList[i].RecordStateName = mParamItem == null ? string.Empty : mParamItem.ParamItemName;
                }
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = certList });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }
        /// <summary>
        /// 仪器送检
        /// </summary>
        /// <param name="instrumentIds"></param>
        /// <param name="orderJson"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string SendOrder(string instrumentIds, string orderJson, string jobNo, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                string result = ServiceProvider.OrderSendInstrumentService.SendOrder(instrumentIds, orderJson, user.BelongDepart, user.UserName, user.UserId);
                if (result.Equals("OK"))
                {
                    ServiceProvider.InstrumentWaitSendService.DeleteByInstrumentIds(instrumentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(l => Convert.ToInt32(l)).ToList(), user.UserId);
                }
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = result });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumentForm">为空查询所有仪器 0仪器仪表 1固定资产</param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetInstrumentListByKeyWorks(string keyworks, string instrumentForm, string accessToKen)
        {
            try
            {
                if (!ToKenHelper.CheckAccessToKen(accessToKen))
                {
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
                }
                IList<object> resultList = new List<object>();
                if (string.IsNullOrWhiteSpace(keyworks)) return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = resultList });


                IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetInstrumentListByKeyWorks(keyworks, instrumentForm);

                //所属部门
                IList<Global.Common.Models.OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();
                for (int i = 0; i < (instrumentList.Count > 100 ? 100 : instrumentList.Count); i++)
                {
                    Global.Common.Models.OrgModel org = orgList.SingleOrDefault(S => S.OrgCode == instrumentList[i].BelongDepart);
                    string orgName = org == null ? string.Empty : org.OrgName;
                    resultList.Add(new { InstrumentName = instrumentList[i].InstrumentName, AssetsNo = instrumentList[i].AssetsNo, SerialNo = instrumentList[i].SerialNo, ManageNo = instrumentList[i].ManageNo, Specification = instrumentList[i].Specification, BelongDepart = orgName, BarCode = instrumentList[i].BarCode });
                }

                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = resultList });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }

        /// <summary>
        /// 获取所有的仪器数据
        /// </summary>
        [OperationContract]
        public string GetAllInstrument(int instrumentForm, string jobNo, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                string where = Global.Business.ServiceProvider.UserManageDepartService.GetManageAndDepartSearchCondition(user.UserId.ToString());
                where = string.Format("{0} and instrumentForm={1}", where, instrumentForm);
                IList<InstrumentModel> list = ServiceProvider.InstrumentService.GetByWhere(where);
                IList<OrgModel> departList = Global.Business.ServiceProvider.OrgService.GetAll();

                var InstrumentList = list
                 .Select(p =>
                 {
                     var d = departList.FirstOrDefault(i => i.OrgCode == p.BelongDepart);
                     return new
                     {
                         AssetsNo = p.AssetsNo,
                         BarCode = p.BarCode,
                         DepartName = d == null ? "" : d.OrgName,
                         InstrumentForm = p.InstrumentForm,
                         InstrumentName = p.InstrumentName,
                         ManageNo = p.ManageNo,
                         Specification = p.Specification,
                         SerialNo = p.SerialNo,
                         LeaderName = p.LeaderName
                     };
                 });
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = InstrumentList });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }
        /// <summary>
        /// 计量仪器-获取我的购物车内容
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string GetMyShoppingCart(string jobNo, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                IList<Hashtable> list = ServiceProvider.InstrumentWaitSendService.GetInstrumentByUserId(user.UserId);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = list });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }
        /// <summary>
        /// 计量仪器-添加仪器到购物车
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string AddToShoppingCart(string jobNo, string barCode, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                InstrumentModel instrument = ServiceProvider.InstrumentService.GetByBarCode(barCode);
                if (instrument == null)
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "仪器不存在!" });
                if (instrument.InstrumentForm == Instrument.Common.Constants.InstrumentForm.固定资产.GetHashCode())
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "固定资产不能送检!" });
                bool result = ServiceProvider.InstrumentWaitSendService.ExistsInstrument(instrument.InstrumentId, user.UserId);
                if (result)
                    return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "仪器已存在待送清单中!" });
                InstrumentWaitSendModel model = new InstrumentWaitSendModel();
                model.InstrumentId = instrument.InstrumentId;
                model.UserId = user.UserId;
                ServiceProvider.InstrumentWaitSendService.Save(model);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK" });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }
        /// <summary>
        ///计量仪器-从购物车删除仪器
        /// </summary>
        /// <param name="jobNo"></param>
        /// <param name="barCode"></param>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        [OperationContract]
        public string DeleteFromShoppingCart(string jobNo, string InstrumentIds, string accessToKen)
        {
            try
            {
                UserModel user = null;
                string Checkresult = CheckAccess(accessToKen, jobNo, ref user);
                if (Checkresult != "OK")
                    return Checkresult;
                var ids = InstrumentIds.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => Convert.ToInt32(o)).ToList();
                ServiceProvider.InstrumentWaitSendService.DeleteByInstrumentIds(ids, user.UserId);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "OK", Data = "" });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = ex.Message });
            }
        }
        /// <summary>
        /// 账号检测（访问权限和账号检查）
        /// </summary>
        /// <param name="accessToKen"></param>
        /// <param name="jobNo"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CheckAccess(string accessToKen, string jobNo, ref UserModel user)
        {
            if (!ToKenHelper.CheckAccessToKen(accessToKen))
            {
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = commonErrorMsg });
            }
            user = Global.Business.ServiceProvider.UserService.GetByLoginName(jobNo);
            if (user == null)
                return ToolsLib.Utility.CommonUtils.JsonSerialize(new { Msg = "用户不存在!" });
            return "OK";
        }
        
    }
}
