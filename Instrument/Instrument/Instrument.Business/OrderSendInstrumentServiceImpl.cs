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
using Global.Common.Models;
using Newtonsoft.Json.Linq;
using ToolsLib.Utility;

namespace Instrument.Business
{
    public class OrderSendInstrumentServiceImpl
    {
        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.OrderSendInstrumentDAO.DeleteByInstrumentId(InstrumentId);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteByOrderId(int OrderId)
        {
            DBProvider.OrderSendInstrumentDAO.DeleteByOrderId(OrderId);
        }

        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int AutoId)
        {
            DBProvider.OrderSendInstrumentDAO.DeleteById(AutoId);
        }

        public void DeleteByIds(IList<int> autoIdList)
        {
            DBProvider.OrderSendInstrumentDAO.DeleteByIds(autoIdList);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(OrderSendInstrumentModel model)
        {
            if (model.AutoId == 0) DBProvider.OrderSendInstrumentDAO.Add(model);
            else DBProvider.OrderSendInstrumentDAO.Update(model);
        }
        /// <summary>
        /// 根据Md5Code更新仪器完工和证书完工状态
        /// </summary>
        /// <param name="model"></param>
        public void UpdateIsComplete(OrderSendInstrumentModel model)
        {
            DBProvider.OrderSendInstrumentDAO.UpdateIsComplete(model);
        }

        /// <summary>
        /// 根据主键更新是否下载证书
        /// </summary>
        /// <param name="autoId"></param>
        public void UpdateIsDownloadCert(int autoId)
        {
            DBProvider.OrderSendInstrumentDAO.UpdateIsDownloadCert(autoId);
        }

        public IList<Hashtable> GetAllSendListForPaging(PagingModel paging)
        {
            //string GetAllAuthorityStr = "SendList-CheckAll";
            //bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

            //if (!IsGetAllAuthority)
            //{
            //    //获取当前用户所管辖的所有区域下的仪器SQL语句.
            //    StringBuilder subSqlStr = Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart");
            //    if (!string.IsNullOrWhiteSpace(paging.Where))
            //        paging.Where = string.Format(" {0} and {1}", subSqlStr, paging.Where);
            //    else
            //        paging.Where = subSqlStr.ToString();
            //}
            return DBProvider.OrderSendInstrumentDAO.GetAllSendListForPaging(paging);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public OrderSendInstrumentModel GetById(int AutoId)
        {
            return DBProvider.OrderSendInstrumentDAO.GetById(AutoId);
        }
        public OrderSendInstrumentModel GetByItemCode(string itemCode)
        {
            return DBProvider.OrderSendInstrumentDAO.GetByItemCode(itemCode);
        }

        public IList<OrderSendInstrumentModel> GetByOrderId(int orderId)
        {
            return DBProvider.OrderSendInstrumentDAO.GetByOrderId(orderId);
        }

        /// <summary>
        /// 获取送检单仪器列表
        /// </summary>
        /// <param name="orderIdList">送检单ID列表</param>
        /// <returns>送检单仪器列表</returns>
        public IList<OrderSendInstrumentModel> GetByOrderIdList(IList<int> orderIdList)
        {
            return DBProvider.OrderSendInstrumentDAO.GetByOrderIdList(orderIdList);
        }

        public IList<OrderSendInstrumentModel> GetByIds(IList<int> InstrumentIdList)
        {
            return DBProvider.OrderSendInstrumentDAO.GetByIds(InstrumentIdList);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<OrderSendInstrumentModel> GetAll()
        {
            return DBProvider.OrderSendInstrumentDAO.GetAll();
        }
         /// <summary>
        /// 根据条件获得记录.
        /// </summary>
        public IList<OrderSendInstrumentModel> GetAllSendInstrumentListByWhere(string sqlwhere)
        {
            return DBProvider.OrderSendInstrumentDAO.GetAllSendInstrumentListByWhere(sqlwhere);
        }

        /// <summary>
        /// 仪器送检
        /// 仪器手机App接口和网页功能公用方法
        /// </summary>
        /// <param name="instrumentIds">以逗号分隔的仪器标识（未加密）</param>
        /// <param name="orderJson">送检单Json数据（送检人SendUser，App和网页分别独立赋值）</param>
        /// <returns></returns>
        public string SendOrder(string instrumentIds, string orderJson, string OrgName,string UserName,int UserId)
        {
            string result = "";
            //组装数据
            JObject orderObj = JObject.Parse(orderJson);
            var instrumentIdArr = instrumentIds.Split(',');
            var instrumentIdList = instrumentIdArr.Select(l => Convert.ToInt32(l)).ToList();
            IList<InstrumentModel> instrumentList = ServiceProvider.InstrumentService.GetByIdList(instrumentIdList);//对应的仪器列表
            IList<InstrumentWaitSendModel> instrumentWaitSendList = ServiceProvider.InstrumentWaitSendService.GetByInstrumentIdsList(instrumentIdList,UserId);//对应的送检仪器备注
            //单号
            string orderNumber = ServiceProvider.OrderService.GenerateUniqueOrderNumber();
            //组装送检仪器MD5Code字典
            Dictionary<int, string> dictionaryMd5 = new Dictionary<int, string>();
            foreach (int instrumentId in instrumentIdList)
            {
                dictionaryMd5.Add(instrumentId, Guid.NewGuid().ToString());
            }
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel company = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.CompanyInfo);
            if (null == company) company = new Global.Common.Models.ParamModel();
            Global.Common.Models.ParamItemModel CompanyName = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司名称"));
            Global.Common.Models.ParamItemModel CompanyCode = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司编号"));

            orderObj.Add("OrderNumber", orderNumber);
            orderObj.Add("CompanyName", CompanyName == null ? "" : CompanyName.ParamItemValue);
            orderObj.Add("CompanyCode", CompanyCode == null ? "" : CompanyCode.ParamItemValue);
            //送检人部门

            orderObj.Add("SendUserOrgName", OrgName);
            orderObj.Add("InstrumentCount", instrumentList.Count);
            //orderObj.Add("SendUser", LoginHelper.LoginUser.UserName);//送检人

            //仪器Json数据组装
            JArray projectJArr = new JArray();
            int maxLen = int.Parse(string.IsNullOrEmpty(WebUtils.GetSettingsValue("SendOrderLength")) ? "1000" : WebUtils.GetSettingsValue("SendOrderLength"));//一次送检的最大仪器数量
            int leftCout = instrumentList.Count % maxLen;
            int count = 0;
            InstrumentWaitSendModel instrumentWaitSendModel = null;
            foreach (var item in instrumentList)
            {
                instrumentWaitSendModel = instrumentWaitSendList.SingleOrDefault(I => I.InstrumentId.Equals(item.InstrumentId));
                count++;
                JObject projectObj = new JObject();
                projectObj.Add("CertificationNumber", item.CertificateNo);
                projectObj.Add("InstrumentName", item.InstrumentName);
                projectObj.Add("Specification", item.Specification);
                projectObj.Add("ManageNumber", item.ManageNo);
                projectObj.Add("MadeNumber", item.SerialNo);
                projectObj.Add("InspectDate", item.DueStartDate);
                projectObj.Add("DueEndDate", item.DueEndDate);
                projectObj.Add("InspectOrg", item.InspectOrg);
                projectObj.Add("Remark", instrumentWaitSendModel.Remark);
                projectObj.Add("MD5Code", dictionaryMd5[item.InstrumentId]);
                projectJArr.Add(projectObj);
                if (count % maxLen == 0 || (count % maxLen == leftCout))
                {

                    string En_Order = SSOHelper.Encrypt(orderObj.ToString());
                    string En_Project = SSOHelper.Encrypt(projectJArr.ToString());

                    string jsonData = GRGTCommonUtils.WSProvider.EbusinessProvider.SendOrder(En_Order, En_Project, Global.Business.ServiceProvider.ParamService.GetaccessToken(Instrument.Common.Constants.SysParamType.CompanyInfo));
                    Dictionary<string, object> dic = ToolsLib.Utility.CommonUtils.JsonDeserialize(jsonData, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
                    result = dic["Msg"].ToString();
                    projectJArr.Clear();
                    if (!result.Equals("OK"))//一次不成功则终止推送
                        break;
                }
            }

            if (result.Equals("OK"))
            {
                OrderModel order = new OrderModel();
                order.OrderNumber = orderNumber;//单号
                order.SendUser = UserName;
                order.UserId = UserId;
                order.CreateUser = order.SendUser;
                order.ReceivedUser = "";
                order.InstrumentCount = instrumentIdList.Count;
                ServiceProvider.OrderService.Save(order);
                //新增送检仪器清单
                InstrumentModel instrumentModel = null;
                
                foreach (int instrumentId in instrumentIdList)
                {
                    OrderSendInstrumentModel model = new OrderSendInstrumentModel();
                    instrumentModel = instrumentList.SingleOrDefault(I => I.InstrumentId.Equals(instrumentId));
                    instrumentWaitSendModel = instrumentWaitSendList.SingleOrDefault(I => I.InstrumentId.Equals(instrumentId));
                    if (instrumentModel == null) continue;
                    model.InstrumentId = instrumentId;
                    model.ItemCode = dictionaryMd5[instrumentId];
                    model.OrderId = order.OrderId;
                    model.InstrumentName = instrumentModel.InstrumentName;
                    model.CertificationNumber = instrumentModel.CertificateNo;
                    model.SerialNo = instrumentModel.SerialNo;
                    model.Specification = instrumentModel.Specification;
                    model.ManageNo = instrumentModel.ManageNo;
                    model.InspectDate = instrumentModel.DueStartDate;
                    model.DueEndDate = instrumentModel.DueEndDate;
                    model.InspectOrg = "广电计量";//
                    model.Remark = instrumentWaitSendModel.Remark;
                    ServiceProvider.OrderSendInstrumentService.Save(model);
                }
            }

            return result;
        }
    }
}
