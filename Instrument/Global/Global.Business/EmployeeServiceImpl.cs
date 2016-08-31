using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Global.DataAccess;
using Global.Common.Models;
using System.Collections;

namespace Global.Business
{
    public class EmployeeServiceImpl
    {
        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public EmployeeModel GetById(int UserId)
        {
            EmployeeModel employee = DBProvider.EmployeeDAO.GetById(UserId);
            //if (employee != null) employee.ContactInfo = DBProvider.EmpContactDAO.GetById(UserId);

            //return employee;
            return null;
        }

        public IList<Hashtable> SearchByNameJobNumb(string input)
        {
            return DBProvider.EmployeeDAO.SearchByNameJobNumb(input);
        }
    }
}
