using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Business
{
    public class ServiceProvider
    {
        public static UserServiceImpl UserService { get; set; }
        public static ParamServiceImpl ParamService { get; set; }
        public static ParamItemServiceImpl ParamItemService { get; set; }
        public static RoleServiceImpl RoleService { get; set; }
        public static PermissionServiceImpl PermissionService { get; set; }
        public static MenuServiceImpl MenuService { get; set; }
        public static AttachmentServiceImpl AttachmentService { get; set; }
        public static OrgServiceImpl OrgService { get; set; }
        //public static SalesManageDepartServiceImpl SalesManageDepartService { get; set; }
        public static UserManageDepartServiceImpl UserManageDepartService { get; set; }
        public static EmployeeServiceImpl EmployeeService { get; set; }
        public static DutyServiceImpl DutyService { get; set; }
        //public static SalesServiceImpl SalesService { get; set; }
        //日志记录
        public static Sys_BusinessLogServiceImpl Sys_BusinessLogService { get; set; }
        //操作日志
        public static OperateLogServiceImpl OperateLogService { get; set; }
    }
}
