using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using GRGTCommonUtils;
using System.Collections;

namespace Global.Business
{
    public class ParamServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int ParamId)
        {
            DBProvider.ParamDAO.DeleteById(ParamId);
            ServiceProvider.ParamItemService.DeleteByParamId(ParamId);//删除该参数对应的明细表
        }

        /// <summary>
        /// 删除记录(仅改变状态).
        /// </summary>
        public void DeleteStatusById(int ParamId)
        {
            DBProvider.ParamDAO.DeleteStatusById(ParamId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(ParamModel model)
        {
            if (model.ParamId == 0) DBProvider.ParamDAO.Add(model);
            else DBProvider.ParamDAO.Update(model);
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public ParamModel GetById(int ParamId)
        {
            return DBProvider.ParamDAO.GetById(ParamId);
        }

        public ParamModel GetByCode(string ParamCode)
        {
            return DBProvider.ParamDAO.GetByCode(ParamCode);
        }

        public int IsExistParamCode(int paramId, string paramCode)
        {
            return DBProvider.ParamDAO.IsExistParamCode(paramId, paramCode);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<ParamModel> GetAll()
        {
            return DBProvider.ParamDAO.GetAll();
        }

        public string GetdhtmlxTree(IList<ParamModel> paramList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.Append("<tree id='0'>" + Environment.NewLine);
            string msg = "";
            msg = "<item text='{0}' id='{1}'>{2}";
            msg = "<item text='{0}' id='{1}' open='1' select='1'>{2}";
            sb.AppendFormat(msg, "系统参数", "-1", Environment.NewLine);
            int count = 0;
            foreach (ParamModel m in paramList)
            {               
                msg = "<item text='{0}({1})' id='{2}'>{3}";
                if (count == 0) //默认选中
                    msg = "<item text='{0}({1})' id='{2}' open='1' select='1'>{3}";
                else
                    msg = "<item text='{0}({1})' id='{2}' open='1'>{3}";
                sb.AppendFormat(msg, m.ParamName,m.ParamCode, m.ParamId, Environment.NewLine);
                sb.Append("</item>");
                count++;
            }
            sb.Append("</item>");
            sb.Append("</tree>");
            return sb.ToString();
        }

       
        /// <summary>
        /// 根据条件构造系统参数列表的Json  Add by Reven 2014-06-30 
        /// {sTitle:'" + p.ParamItemName + "', sValue:'" + p.ParamItemName }
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder BulidJsonbyNameName(string code)
        {
            ParamModel param = DBProvider.ParamDAO.GetAll().SingleOrDefault(p => p.ParamCode == code);

            return BulidJsonbyNameName(param.itemsList);
            //StringBuilder jsonBulider = new StringBuilder();
            //jsonBulider.Append("{sTitle: '全部', sValue: '', selected: true}");
            //foreach (ParamItemModel p in param.itemsList)
            //{
            //    jsonBulider.Append(",{sTitle:'" + p.ParamItemName + "', sValue:'" + p.ParamItemName + "', selected: false}");

            //}
            //var jsonStr = jsonBulider;
            //return jsonStr;
        }

        public StringBuilder BulidJsonbyNameName(IList<ParamItemModel> itemList)
        {
            StringBuilder jsonBulider = new StringBuilder();
            jsonBulider.Append("{sTitle: '全部', sValue: '', selected: true}");
            foreach (ParamItemModel p in itemList)
            {
                jsonBulider.Append(",{sTitle:'" + p.ParamItemName + "', sValue:'" + p.ParamItemName + "', selected: false}");
            }
            var jsonStr = jsonBulider;
            return jsonStr;
        }


        /// <summary>
        /// 根据条件构造系统参数列表的Json  Add by Reven 2014-06-30 
        /// {sTitle:'" + p.ParamItemName + "', sValue:'" + p.ParamItemValue }
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder BulidJsonbyNameValue(string code)
        {
            ParamModel param = DBProvider.ParamDAO.GetAll().SingleOrDefault(p => p.ParamCode == code);

            return BulidJsonbyNameValue(param.itemsList);

            //StringBuilder jsonBulider = new StringBuilder();
            //jsonBulider.Append("{sTitle: '全部', sValue: '', selected: true}");
            //foreach (ParamItemModel p in param.itemsList)
            //{
            //    jsonBulider.Append(",{sTitle:'" + p.ParamItemName + "', sValue:'" + p.ParamItemValue + "', selected: false}");

            //}

            //var jsonStr = jsonBulider;
            //return jsonStr;
        }

        /// <summary>
        /// 根据参数列表构建高级查询下拉框，过滤掉父节点不等于0的列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StringBuilder BulidJsonbyNameValue2(string code)
        {
            ParamModel param = DBProvider.ParamDAO.GetAll().SingleOrDefault(p => p.ParamCode == code);

            return BulidJsonbyNameValue(param.itemsList.Where(p=>p.ParentCode.Equals("0")).ToList());

        }

        public StringBuilder BulidJsonbyNameValue(IList<ParamItemModel> itemList)
        {
            StringBuilder jsonBulider = new StringBuilder();
            jsonBulider.Append("{sTitle: '全部', sValue: '', selected: true}");
            foreach (ParamItemModel p in itemList)
            {
                jsonBulider.Append(",{sTitle:'" + p.ParamItemName + "', sValue:'" + p.ParamItemValue + "', selected: false}");
            }

            var jsonStr = jsonBulider;
            return jsonStr;
        }

     

        /// <summary>
        /// 根据枚举构造列表的Json  Add by Reven 2014-07-01
        /// </summary>
        /// <param name="enumName">枚举对象</param>
        /// <returns></returns>
        public StringBuilder BulidJsonbyEnum(Enum enumName)
        {
            // 遍历所有的枚举元素
            StringBuilder jsonBulider = new StringBuilder();
            jsonBulider.Append("{sTitle: '全部', sValue: '', selected: true}");
            object oType = null;
            foreach (object o in Enum.GetValues(enumName.GetType()))
            {
                oType = (object)Enum.Parse(enumName.GetType(), o.ToString(), true);
                jsonBulider.Append(",{sTitle:'" + o + "', sValue:'" + (int)oType + "', selected: false}");

            }
            return jsonBulider;
        }


        /// <summary>
        /// 根据枚举构造列表的Json  Add by Reven 2014-07-01
        /// </summary>
        /// <param name="enumName">枚举对象</param>
        /// <returns></returns>
        public StringBuilder BulidJsonbyEnumbyName(Enum enumName)
        {
            // 遍历所有的枚举元素
            StringBuilder jsonBulider = new StringBuilder();
            jsonBulider.Append("{sTitle: '全部', sValue: '', selected: true}");
            object oType = null;

            foreach (object o in Enum.GetValues(enumName.GetType()))
            {
                oType = (object)Enum.Parse(enumName.GetType(), o.ToString(), true);
                jsonBulider.Append(",{sTitle:'" + o + "', sValue:'" + o + "', selected: false}");

            }
            return jsonBulider;
        }

        /// <summary>
        /// 根据枚举构造列表
        /// </summary>
        /// <param name="enumName">枚举对象</param>
        /// <returns></returns>
        public Dictionary<int, string> BulidListbyEnum(Enum enumName)
        {
            // 遍历所有的枚举元素
            Dictionary<int, string> dic = new Dictionary<int, string>();
            object oType = null;
            foreach (object o in Enum.GetValues(enumName.GetType()))
            {
                oType = (object)Enum.Parse(enumName.GetType(), o.ToString(), true);
                dic[(int)oType] = o.ToString();
            }
            return dic;
        }

        /// <summary>
        /// 获取公司编号
        /// </summary>
        /// <param name="CompanyInfo"></param>
        /// <returns></returns>
        public string GetCompanyCode(string CompanyInfo)
        {
            IList<ParamModel> paramList = ServiceProvider.ParamService.GetAll();
            ParamModel company = paramList.SingleOrDefault(t => t.ParamCode == CompanyInfo);
            if (null == company) company = new Global.Common.Models.ParamModel();
            ParamItemModel pCompany = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司编号"));
            return pCompany == null ? "" : pCompany.ParamItemValue;
        }

        /// <summary>
        /// 获取accessToken
        /// </summary>
        /// <returns></returns>
        public string GetaccessToken(string CompanyInfo)
        {
            IList<Global.Common.Models.ParamModel> paramList = ServiceProvider.ParamService.GetAll();
            ParamModel company = paramList.SingleOrDefault(t => t.ParamCode == CompanyInfo);
            if (null == company) company = new Global.Common.Models.ParamModel();
            ParamItemModel pCompany = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("公司编号"));
            ParamItemModel Pwd = company.itemsList.SingleOrDefault(s => s.ParamItemName.Equals("密码"));
            return SSOHelper.Encrypt(string.Format("{0}|{1}", pCompany == null ? "" : pCompany.ParamItemValue, Pwd == null ? "" : Pwd.ParamItemValue));
        }

    }
}
