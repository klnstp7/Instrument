using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.IBatisNet;
using Global.Common.Models;
using System.Collections;

namespace Global.DataAccess
{
    public class EmployeeDaoImpl
    {
        /// <summary>
        /// 得到一个对象实体.
        /// </summary>
        public EmployeeModel GetById(int UserId)
        {
            return DBProvider.dbMapper.SelectObject<EmployeeModel>("Employee_BaseInfo.GetByID", UserId);
        }

        public IList<Hashtable> SearchByNameJobNumb(string input)
        {
            return DBProvider.dbMapper.SelectList<Hashtable>("Employee_BaseInfo.SearchByNameJobNumb", input);
        }

    }
}
