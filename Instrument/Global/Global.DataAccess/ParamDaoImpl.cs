using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;

namespace Global.DataAccess
{
    public class ParamDaoImpl
    {
        /// <summary>
        /// 增加一条数据.
        /// </summary>
        public void Add(ParamModel model)
        {
            DBProvider.dbMapper.Insert("Sys_Params.Insert", model);
        }

        /// <summary>
        /// 更新一条数据.
        /// </summary>
        public void Update(ParamModel model)
        {
            DBProvider.dbMapper.Update("Sys_Params.Update", model);
        }

        /// <summary>
        /// 删除一条数据.
        /// </summary>
        public void DeleteById(int ParamId)
        {
            DBProvider.dbMapper.Delete("Sys_Params.DeleteById", ParamId);
        }

        /// <summary>
        /// 删除一条数据(仅改变状态).
        /// </summary>
        public void DeleteStatusById(int ParamId)
        {
            DBProvider.dbMapper.Delete("Sys_Params.DeleteStatusById", ParamId);
        }


        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public ParamModel GetById(int ParamId)
        {
            return DBProvider.dbMapper.SelectObject<ParamModel>("Sys_Params.GetByID", ParamId);
        }

        public ParamModel GetByCode(string ParamCode)
        {
            return DBProvider.dbMapper.SelectObject<ParamModel>("Sys_Params.GetByCode", ParamCode);
        }

        public int IsExistParamCode(int paramId,string paramCode)
        {
            ParamModel m = new ParamModel { ParamId = paramId, ParamCode = paramCode };
            return DBProvider.dbMapper.SelectObject<int>("Sys_Params.IsExistParamCode", m);
        }

        /// <summary>
        /// 获得所有记录.
        /// </summary>
        public IList<ParamModel> GetAll()
        {
            return DBProvider.dbMapper.SelectList<ParamModel>("Sys_Params.GetAll");
        }
    }
}
