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

namespace Instrument.Business
{
    /// <summary>
    /// 实验室标准器具
    /// </summary>
    public class OrderServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int OrderId)
        {
            ServiceProvider.OrderSendInstrumentService.DeleteByOrderId(OrderId);//送检仪器清单DeleteByOrderId
            DBProvider.OrderDAO.DeleteById(OrderId);
        }

        /// <summary>
        /// 更新送检单的受理状态.
        /// </summary>
        public void UpdateReceivedInfo(OrderModel model)
        {
            DBProvider.OrderDAO.UpdateReceivedInfo(model);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(OrderModel model)
        {
            if (model.OrderId == 0)
            {

                DBProvider.OrderDAO.Add(model);
            }
            else DBProvider.OrderDAO.Update(model);
        }

        /// <summary>
        /// 生成委托单号
        /// </summary>
        /// <returns></returns>
        public string GenerateUniqueOrderNumber()
        {
            IList<Global.Common.Models.ParamModel> paramList = Global.Business.ServiceProvider.ParamService.GetAll();
            Global.Common.Models.ParamModel company = paramList.SingleOrDefault(t => t.ParamCode == Constants.SysParamType.CompanyInfo);
            if (null == company) company = new Global.Common.Models.ParamModel();
            Global.Common.Models.ParamItemModel CompanyCode = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司编号"));
            string orderNumber = "";
            do
            {
                orderNumber = CompanyCode.ParamItemValue+ DateTime.Now.ToString("yyyyMMdd") + ToolsLib.Utility.StrUtils.GetRandomNumb(4);
            }
            while (DBProvider.OrderDAO.IsExistOrderNumber(orderNumber));
            return orderNumber;
        }


        public void UpdateDownloadCertState(OrderModel model)
        {
            DBProvider.OrderDAO.UpdateDownloadCertState(model);
        }
        

        public IList<Hashtable> GetAllOrderListForPaging(PagingModel paging)
        {
            string GetAllAuthorityStr = "Order-CheckAll";
            bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

            if (!IsGetAllAuthority)
            {
                //获取当前用户
                if (!string.IsNullOrWhiteSpace(paging.Where))
                    paging.Where = string.Format(" {0} and {1}", paging.Where, " UserId=" + LoginHelper.LoginUser.UserId);
                else
                    paging.Where = " UserId=" + LoginHelper.LoginUser.UserId;
            }
            return DBProvider.OrderDAO.GetAllOrderListForPaging(paging);
        }
        
        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public OrderModel GetById(int OrderId)
        {
            return DBProvider.OrderDAO.GetById(OrderId);
        }

        public IList<OrderModel> GetByIdList(IList<int> orderIdList)
        {
            return DBProvider.OrderDAO.GetByIdList(orderIdList);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<OrderModel> GetAll()
        {
            return DBProvider.OrderDAO.GetAll();
        }
           /// <summary>
        /// 根据条件获得所有记录.
        /// </summary>
        public IList<OrderModel> GetAllOrderListByWhere(string where)
        {
            return DBProvider.OrderDAO.GetAllOrderListByWhere(where);
        }
    }
}
