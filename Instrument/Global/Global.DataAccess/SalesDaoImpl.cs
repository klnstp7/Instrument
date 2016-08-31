using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using Global.Common;
using Global.Common.Models;
using Global.DataAccess;
using System.Collections;
using GRGTCommonUtils;
using ToolsLib.IBatisNet;

namespace Global.DataAccess
{
    public class SalesDaoImpl
    {
        /// <summary>
        /// 获取业务员所属业务区域
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<ParamItemModel> GetSaleOwnAreaByUserId(int userId)
        {
            //Hashtable ht = new Hashtable();
            //ht.Add("UserId", userId);
            //ht.Add("ParamCode", CMSCommon.Constants.SysParamType.SaleAreaCode);
            return DefaultMapper.GetMapper().SelectList<ParamItemModel>("Sales.GetSaleOwnAreaByUserId", userId);
        }

        /// <summary>
        /// 主管部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<OrgModel> GetSaleManageDepartByUserId(int userId)
        {
            return DefaultMapper.GetMapper().SelectList<OrgModel>("Organization.GetSaleManageDepartByUserId", userId);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>       
        public void DeleteManageDepartByUserId(int UserId)
        {
            DefaultMapper.GetMapper().Delete("User_ManageDepart.DeleteManageDepartByUserId", UserId);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>       
        public void AddManageDepart(int userId, int orgId,string orgCode)
        {
            Hashtable ht = new Hashtable();
            //int orgId = DefaultMapper.GetMapper().SelectObject<UserManageDepartModel>("User_ManageDepart.GetMaxIdModel", "").OrgId + 1;
            ht.Add("OrgId", orgId);
            ht.Add("UserID", userId);
            //ht.Add("JobNo", jobNo);
            ht.Add("OrgCode", orgCode);

            DefaultMapper.GetMapper().Insert("User_ManageDepart.InsertManageDepart", ht);
        }

    }

}
