using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using System.Collections;
using GRGTCommonUtils;

namespace Global.Business
{
    public class ParamItemServiceImpl
    {
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int ParamItemId)
        {
            DBProvider.ParamItemDAO.DeleteById(ParamItemId);
        }
        public void DeleteByParamId(int ParamId)
        {
            DBProvider.ParamItemDAO.DeleteByParamId(ParamId);
        }

        /// <summary>
        /// 删除记录(仅改变状态).
        /// </summary>
        public void DeleteStatusById(int ParamItemId)
        {
            DBProvider.ParamItemDAO.DeleteStatusById(ParamItemId);
        }

        public string DeleteParamItem(int itemId,string itemValue)
        {
            try
            {
                DeleteStatusById(itemId);
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(ParamItemModel model)
        {
            if (model.ParamItemId == 0) DBProvider.ParamItemDAO.Add(model);
            else DBProvider.ParamItemDAO.Update(model);
        }

        public string SaveParamItem(ParamItemModel model)
        {
            try
            {
                Save(model);
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public Boolean JudgeValue(string paramItemValue)
        {
            for (int i = 0; i < 10; i++)
            {
                if (paramItemValue.Equals(i.ToString())) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public ParamItemModel GetById(int ParamItemId)
        {
            return DBProvider.ParamItemDAO.GetById(ParamItemId);
        }

        public string GetdhtmlxTree(IList<ParamItemModel> itemList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine);
            sb.Append("<tree id='0'>" + Environment.NewLine);
            RecursiveDump(itemList, "0", 1, sb);
            sb.Append("</tree>");
            return sb.ToString();
        }

        public void RecursiveDump(IList<ParamItemModel> list, string parentCode, int level, StringBuilder sb)
        {
            IEnumerable<ParamItemModel> tempItem = list.Where<ParamItemModel>(m => m.ParentCode.Equals(parentCode));

            string msg = "";
            foreach (ParamItemModel m in tempItem)
            {
                msg = "<item text='{0}' id='{1}' open='1'>{2}";
                if (level == 1) msg = "<item text='{0}' id='{1}' open='1' select='1'>{2}";
                else if (level < 3) msg = "<item text='{0}' id='{1}' open='1'>{2}";
                sb.AppendFormat(msg, m.ParamItemName, UtilsHelper.Encrypt(m.ParamItemValue), Environment.NewLine);
                RecursiveDump(list, m.ParamItemValue, level + 1, sb);
                sb.Append("</item>");
            }
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<ParamItemModel> GetAll()
        {
            return DBProvider.ParamItemDAO.GetAll();
        }


        /// <summary>
        /// 获得所有记录(按参数编号).
        /// </summary>
        /// <param name="ParamID"></param>
        /// <returns></returns>
        public IList<ParamItemModel> GetByParamID(int ParamID)
        {
            return DBProvider.ParamItemDAO.GetByParamID(ParamID);
        }

        /// <summary>
        /// 判断是否存在指定的参数项值
        /// </summary>
        /// <param name="paramItemId">要排除比较的参数项的Id</param>
        /// <param name="paramId">参数项所属的分类的Id</param>
        /// <param name="paramItemValue">要比较的值</param>
        /// <returns></returns>
        public int IsExistParamItemValue(int paramItemId, int paramId, string paramItemValue)
        {
            return DBProvider.ParamItemDAO.IsExistParamItemValue(paramItemId, paramId, paramItemValue);
        }

        public IList<Hashtable> SearchByName(string input,int id)
        {
            return DBProvider.ParamItemDAO.SearchByName(input,id);
        }

        /// <summary>
        /// 通过ID列表得到对象实体集合.
        /// </summary>
        public IList<ParamItemModel> GetByParamItemIds(IList<int> paramItemIdList)
        {
            List<ParamItemModel> paramList = new List<ParamItemModel>();
            int maxLen = 500;
            int queryCount = paramItemIdList.Count / maxLen + (paramItemIdList.Count % maxLen == 0 ? 0 : 1);
            for (int i = 0; i < queryCount; i++)
            {
                var tempArray = paramItemIdList.Skip(i * maxLen).Take(maxLen).ToList();
                IList<ParamItemModel> mOrg = DBProvider.ParamItemDAO.GetByParamItemIds(tempArray);
                paramList.AddRange(mOrg);
            }
            return paramList;
        }
    }
}
