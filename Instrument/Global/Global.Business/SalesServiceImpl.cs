using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.Common;
using Global.Common.Models;
using Global.DataAccess;
using GRGTCommonUtils;

namespace Global.Business
{
    public class SalesServiceImpl
    {

        /// <summary>
        /// 获取业务员所属业务区域
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<ParamItemModel> GetSaleOwnAreaByUserId(int userId)
        {
            return DBProvider.SalesDAO.GetSaleOwnAreaByUserId(userId);
        }

        /// <summary>
        /// 树形方式加载当前用户能够查看的营销区域数据
        /// </summary>
        /// <param name="saleAreaList"></param>
        /// <returns></returns>
        public string GetOwnSaleAreadhtmlxTree(IList<ParamItemModel> saleAreaList)
        {
            ParamModel p = Global.Business.ServiceProvider.ParamService.GetByCode(GlobalConstants.SysParamType.SaleAreaCode);

            IList<ParamItemModel> newList = new List<ParamItemModel>();

            foreach (ParamItemModel item in saleAreaList)
            {
                //找上级营销区域
                IEnumerable<ParamItemModel> temppi;
                //长度为2，则为最上级，此时找下级
                if (item.ParamItemValue.Length == 2)
                    temppi = p.itemsList.Where<ParamItemModel>(m => m.ParamItemValue.Contains(item.ParamItemValue));
                else //找上级
                    temppi = p.itemsList.Where<ParamItemModel>(m => item.ParamItemValue.Contains(m.ParamItemValue));
                foreach (ParamItemModel model in temppi)
                {
                    if (!newList.Contains(model))
                    {
                        newList.Add(model);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.AppendFormat("<tree id='0'>" + Environment.NewLine);
            RecursiveDump(newList, "0", 1, sb);
            sb.Append("</tree>");

            return sb.ToString();
        }

        private void RecursiveDump(IList<ParamItemModel> list, string parentCode, int level, StringBuilder sb)
        {
            IEnumerable<ParamItemModel> tempItem = list.Where<ParamItemModel>(m => m.ParentCode == parentCode);

            string msg = "";
            foreach (ParamItemModel m in tempItem)
            {
                msg = "<item text='{0}' id='{1}' open='1'>{2}";
                sb.AppendFormat(msg, m.ParamItemName, UtilsHelper.Encrypt(m.ParamItemValue), Environment.NewLine);
                RecursiveDump(list, m.ParamItemValue, level + 1, sb);
                sb.Append("</item>");
            }
        }

        /// <summary>
        /// 主管部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<OrgModel> GetSaleManageDepartByUserId(int userId)
        {
            return DBProvider.SalesDAO.GetSaleManageDepartByUserId(userId);
        }

        /// <summary>
        /// 保存管理部门
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void SaveUserManageDepart(int userId, string orgCode)
        {
            //UserModel user = Global.Business.ServiceProvider.UserService.GetById(userId);
            DBProvider.SalesDAO.DeleteManageDepartByUserId(userId);
            if (string.IsNullOrEmpty(orgCode)) return;

            IList<OrgModel> orgList = Global.Business.ServiceProvider.OrgService.GetAll();            
            string code = string.Empty;            
            string[] idArray = orgCode.Split(',');
            for (int i = 0; i < idArray.Length; i++)
            {
                if (string.IsNullOrEmpty(idArray[i])) break;
                idArray[i] = UtilsHelper.Decrypt(idArray[i]);
                code = Global.Business.ServiceProvider.OrgService.GetById(int.Parse(idArray[i])).OrgCode;
                DBProvider.SalesDAO.AddManageDepart(userId,  int.Parse(idArray[i]),code);
            }
        }

    }
}
