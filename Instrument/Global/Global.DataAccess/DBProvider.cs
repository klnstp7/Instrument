using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.DataAccess
{
    public class DBProvider
    {
        public static ToolsLib.IBatisNet.BaseMapper dbMapper { get; set; }

        public static UserDaoImpl UserDAO { get; set; }
        public static ParamDaoImpl ParamDAO { get; set; }
        public static ParamItemDaoImpl ParamItemDAO { get; set; }
        public static RoleDaoImpl RoleDAO { get; set; }
        public static PermissionDaoImpl PermissionDAO { get; set; }
        public static MenuDaoImpl MenuDAO { get; set; }
        public static AttachmentDaoImpl AttachmentDAO { get; set; }
        public static OrgDaoImpl OrgDAO { get; set; }
   
        public static UserManageDepartDaoImpl UserManageDepartDAO { get; set; }
        public static EmployeeDaoImpl EmployeeDAO { get; set; }
        public static DutyDaoImpl DutyDAO { get; set; }
       
        //日志记录
        public static Sys_BusinessLogDaoImpl Sys_BusinessLogDao { get; set; }
        //操作日志
        public static OperateLogDaoImpl OperateLogDao { get; set; }
    }
}
