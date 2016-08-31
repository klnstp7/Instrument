using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class ParamItemDaoImpl
    {

        public void DeleteByParamId(int ParamId)
        {
            DBProvider.dbMapper.Delete("Sys_ParamItems.DeleteByParamId", ParamId);
        }
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(ParamItemModel model)
        {
            DBProvider.dbMapper.Insert("Sys_ParamItems.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(ParamItemModel model)
        {
            DBProvider.dbMapper.Update("Sys_ParamItems.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int ParamItemId)
        {
            DBProvider.dbMapper.Delete("Sys_ParamItems.DeleteById", ParamItemId);
        }

        /// <summary>
        /// 删除一条数据(仅改变状态).
        /// </summary>
        public void DeleteStatusById(int ParamItemId)
        {
            DBProvider.dbMapper.Delete("Sys_ParamItems.DeleteStatusById", ParamItemId);
        }

        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public ParamItemModel GetById(int ParamItemId)
        {
            return DBProvider.dbMapper.SelectObject<ParamItemModel>("Sys_ParamItems.GetByID", ParamItemId);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<ParamItemModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<ParamItemModel>("Sys_ParamItems.GetAll");
        }

        

        /// <summary>
        /// 获得所有记录(按参数编号).
        /// </summary>
        /// <returns></returns>
        public IList<ParamItemModel> GetByParamID(int ParamID)
        {
            return DBProvider.dbMapper.SelectList<ParamItemModel>("Sys_ParamItems.GetByParamId", ParamID);
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
            ParamItemModel comparedModel = new ParamItemModel()
            {
                ParamItemId = paramItemId,
                ParamId = paramId,
                ParamItemValue = paramItemValue
            };
            return DBProvider.dbMapper.SelectObject<int>("Sys_ParamItems.IsExistParamItemValue", comparedModel);
        }


        /// <summary>
        /// 根据机构名字模糊查找
        /// </summary>
        /// <param name="input">搜索条件</param>
        /// <returns></returns>
        public IList<Hashtable> SearchByName(string input,int id)
        {
            ParamItemModel paramItem = new ParamItemModel { ParamItemName = input,ParamId = id};
            return DBProvider.dbMapper.SelectList<Hashtable>("Sys_ParamItems.SearchByName", paramItem);
        }

        /// <summary>
        /// 通过ID列表得到对象实体集合.
        /// </summary>
        public IList<ParamItemModel> GetByParamItemIds(IList<int> ParanItemIdList)
        {
            Hashtable ht = new Hashtable();
            if (ParanItemIdList == null || ParanItemIdList.Count == 0)
                ParanItemIdList = new List<int>() { 0 };
            ht.Add("OrgIdList", ParanItemIdList);
            return DBProvider.dbMapper.SelectList<ParamItemModel>("Sys_ParamItems.GetByParamItemIds", ht);
        }
    }
}
