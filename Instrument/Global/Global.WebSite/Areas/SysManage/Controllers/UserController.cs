using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Global.Business;
using Global.Common.Models;
using System.Collections;
using ToolsLib.Utility;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using System.Text;
using Global.Common;
using GRGTCommonUtils;
//using GRGTCommonUtils.WS;
using System.Configuration;


namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /SysManage/User/

        public ActionResult Index()
        {
            ViewBag.OrgId = UtilsHelper.Encrypt("1");

            return View();
        }

        public ActionResult UserList(string orgId)
        {
            //orgId = UtilsHelper.Decrypt(orgId);
            //IList<UserModel> userList = ServiceProvider.UserService.GetListByOrgId(Convert.ToInt32(orgId));
            //IList<Hashtable> userRole = ServiceProvider.UserService.GetAllUserRole();

            //StringBuilder sb = new StringBuilder();
            //StringBuilder roleName = new StringBuilder();
            //int count = 0;
            //sb.Append("[");
            //foreach (var user in userList)
            //{
            //    roleName.Clear();
            //    IEnumerable<Hashtable> where = userRole.Where(ur => int.Equals(ur["UserId"], user.UserId));
            //    foreach (var role in where)
            //    {
            //        roleName.AppendFormat("{0}{1}", role["RoleName"], count == 0 ? "" : "<br/>");
            //        count += 1;
            //    }


            //    sb.AppendFormat("[\"<a href='#' onclick='fnNewWindow({8});return false;'>角色设置</a><br/>"
            //        //+ "<a href='#' onclick='fnConfirmWithoutF5('确定重置账户【{0}】密码？','/SysManage/User/ResetPWD?userId={8}');return false;'>重置密码</a>"
            //        + "<a href='#' onclick='fnUpdateUser({9});return false;'>编辑资料</a><br/>"
            //        + "<a href='#' onclick='fnResetPWD({9});return false;'>重置密码</a><br/>"
            //        + "<a href='#' onclick='fnOpenCloseUser({10});return false;'>禁用启用</a><br/>"
            //        + "<a href='#' onclick='fnDeleteUser({11});return false;'>删除用户</a><br/>"
            //        + "<a href='#' onclick='fnSaleManageDeptWindow({11});return false;'>管理部门</a><br/>"
            //        //+ "<a href='#' onclick='fnTestLogin({12});return false;'>登录验证</a>"
            //        + "\",\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\""
            //        + "],", user.UserName,
            //        user.Sex ? "男" : "女", user.JobNo, user.DepartName, user.Duty, user.IsEnabled == 0 ? "启用" : "禁用",
            //        user.EmployeeState == 0 ? "在职" : "离职", roleName, user.UserId, user.UserId, user.UserId, user.UserId, user.UserId);


            //}
            //if (userList.Count > 0) sb.Remove(sb.Length - 1, 1);
            //sb.Append("]");

            //ViewBag.UserData = sb.ToString();

            return View();
        }

        public string GetUserList(string orgId)
        {
            orgId = UtilsHelper.Decrypt(orgId);
            IList<UserModel> userList = ServiceProvider.UserService.GetListByOrgId(Convert.ToInt32(orgId));
            IList<Hashtable> userRole = ServiceProvider.UserService.GetAllUserRole();

            StringBuilder sb = new StringBuilder();
            StringBuilder roleName = new StringBuilder();
            int count = 0;
            sb.Append("{\"data\":[");
            foreach (var user in userList)
            {
                roleName.Clear();
                IEnumerable<Hashtable> where = userRole.Where(ur => int.Equals(ur["UserId"], user.UserId));
                foreach (var role in where)
                {
                    roleName.AppendFormat("{0}{1}", role["RoleName"], count == 0 ? "" : "<br/>");
                    count += 1;
                }


                sb.AppendFormat("[\"<a href='#' onclick='fnNewWindow({6});return false;'>角色设置</a><br/>"
                    //+ "<a href='#' onclick='fnConfirmWithoutF5('确定重置账户【{0}】密码？','/SysManage/User/ResetPWD?userId={8}');return false;'>重置密码</a>"
                    + "<a href='#' onclick='fnUpdateUser({7});return false;'>编辑资料</a><br/>"
                    + "<a href='#' onclick='fnResetPWD({7});return false;'>重置密码</a><br/>"
                    + "<a href='#' onclick='fnOpenCloseUser({8});return false;'>禁用启用</a><br/>"
                    + "<a href='#' onclick='fnDeleteUser({9});return false;'>删除用户</a><br/>"
                    + "<a href='#' onclick='fnSaleManageDeptWindow({9});return false;'>管理部门</a><br/>"
                    //+ "<a href='#' onclick='fnTestLogin({12});return false;'>登录验证</a>"
                    + "\",\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\""
                    + "],", user.UserName,
                    //user.Sex ? "男" : "女", user.JobNo, user.DepartName, user.Duty, user.IsEnabled == 0 ? "启用" : "禁用",
                    //user.EmployeeState == 0 ? "在职" : "离职", roleName, user.UserId, user.UserId, user.UserId, user.UserId, user.UserId);
                    user.JobNo, user.DepartName, user.Duty,
                    roleName, user.UserId, user.UserId, user.UserId, user.UserId, user.UserId);


            }
            if (userList.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }


        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <param name="collection">表单</param>
        /// <returns></returns>
        public ActionResult SaveUserRoles(FormCollection collection)
        {
            int userId = int.Parse(collection["userId"]);
            string roleId = collection["roleId"];
            ServiceProvider.UserService.SaveUserRoles(userId, roleId);
            return Content("OK");
        }


        public ActionResult EditUserPwd()
        {
            ViewBag.JobNo = ((UserModel)Session["LoginUser"]).JobNo;
            return View("EditUserPwd");
        }

        [HttpPost]
        public ActionResult SaveUserPwd(string loginPwd, string newPwd)
        {
            UserModel user = ((UserModel)Session["LoginUser"]);
            bool IsSSOLogin = Convert.ToBoolean(WebUtils.GetSettingsValue("IsSSOLogin"));
            string result ="OK";
            if (IsSSOLogin)
                result = GRGTCommonUtils.WSProvider.HRProvider.ResetPassword(user.JobNo, loginPwd, newPwd);
            else
            {
                if (loginPwd != LoginHelper.LoginUser.LoginPwd)
                    return Content("旧密码不正确");
                user.LoginPwd = ToolsLib.Utility.StrUtils.Encrypt(newPwd, ToolsLib.LibConst.EncryptFormat.SHA1);//加密操作
                ServiceProvider.UserService.UpdateUserModelByUserId(user);
            }
            return Content(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public ActionResult ResetPWD(int userId)
        {
            UserModel user = ServiceProvider.UserService.GetById(userId);
            string loginPwd = ToolsLib.Utility.WebUtils.GetSettingsValue("DefaultPassword"); //默认密码
            user.LoginPwd = ToolsLib.Utility.StrUtils.Encrypt(loginPwd, ToolsLib.LibConst.EncryptFormat.SHA1);//加密操作
            ServiceProvider.UserService.UpdateUserModelByUserId(user);
            return Content("OK");
        }
        /// <summary>
        /// 禁用/启用账号
        /// </summary>
        public ActionResult OpenCloseUser(int userId)
        {
            UserModel user = ServiceProvider.UserService.GetById(userId);
            if (user.IsEnabled == 1)
            {
                user.IsEnabled = 0;
            }
            else
            {
                user.IsEnabled = 1;
            }
            ServiceProvider.UserService.UpdateUserModelByUserId(user);
            return Content("OK");
        }
        /// <summary>
        /// 禁用/启用账号
        /// </summary>
        public ActionResult OpenUser(int userId)
        {
            UserModel user = ServiceProvider.UserService.GetById(userId);
            user.IsEnabled = 0;
            ServiceProvider.UserService.UpdateUserModelByUserId(user);
            return Content("OK");
        }
        /// <summary>
        /// 禁用账号
        /// </summary>
        public ActionResult ClsoeUser(int userId)
        {
            UserModel user = ServiceProvider.UserService.GetById(userId);
            user.IsEnabled = 1;
            ServiceProvider.UserService.UpdateUserModelByUserId(user);
            return Content("OK");
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        public ActionResult DeleteUser(int userId)
        {
            ServiceProvider.UserService.DeleteUserById(userId);
            return Content("OK");
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        public ActionResult TestLogin(int userId)
        {
            return Content("OK");
        }

        public ActionResult GetServerJsonData(int orgId)
        {
            //提取DataTable参数
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();

            string code = ServiceProvider.OrgService.GetCodeById(orgId);

            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.Where = "1=1";
            if (!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                paging.Where = dtm.FieldCondition;
            }
            paging.Where = string.Format(" {0} and BelongDepart like '{1}%'", paging.Where, code);
            if (!string.IsNullOrEmpty(dtm.KeyWord))
                paging.Where = string.Format("{0} and (UserName like '{1}%' or LoginName like '{1}%')", paging.Where, dtm.KeyWord);

            //数据库查询数据
            IList<Hashtable> userList = ServiceProvider.UserService.GetListForPaging(paging);

            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            foreach (var row in userList)
            {
                string userId = UtilsHelper.Encrypt(row["UserId"].ToString());
                dtm.aaData.Add(new List<string>());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='/Employee/PersonInfo?userId={0}' target=\"_blank\">{1}</a>", userId, row["UserName"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(row["Sex"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(row["LoginName"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(row["OrgName"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(row["Duty"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(row["IsEnabled"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(row["EmployeeState"].ToString());
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("<a href='#' onclick=\"fnNewWindow({0});return false;\">角色设置</a>&nbsp;&nbsp;<a href='#' onclick=\"fnConfirmWithoutF5('确定重置账户【{1}】密码？','/SysManage/User/ResetPassword?userId={0}');return false;\">重置密码</a>&nbsp;&nbsp;<a href='#' onclick=\"fnConfirmWithF5('确定要禁用账户：{1}','/SysManage/User/DisableAccout?userId={0}');return false;\">禁用</a>&nbsp;&nbsp;<a href='#' onclick=\"fnConfirmWithF5('确定要启用账户：{1}','/SysManage/User/EnableAccout?userId={0}');return false;\">启用</a>&nbsp;&nbsp;<a href='#' onclick=\"fnConfirmWithF5('确定要删除用户：{1}','/SysManage/User/DeleteUser?userId={0}');return false;\">删除</a>", row["UserId"], row["UserName"]));
            }

            JsonResult jr = Json(new
            {
                sEcho = dtm.sEcho,
                iTotalRecords = dtm.iTotalRecords,
                iTotalDisplayRecords = dtm.iTotalDisplayRecords,
                aaData = dtm.aaData
            }, JsonRequestBehavior.AllowGet);


            return jr;
        }

        public ActionResult UserManageDepart()
        {
            return View();
        }

        public ActionResult EditUser(int userId, int orgId = 1)
        {
            UserModel userModel = null;
            //int user_id = 0;

            ////解密处理userid
            //if (!string.IsNullOrWhiteSpace(userId) && userId != "0")
            //{
            //    //userId = UtilsHelper.Decrypt(userId);
            //    user_id = int.Parse(userId);
            //    userModel = ServiceProvider.UserService.GetById(user_id);
            //}
            userModel = ServiceProvider.UserService.GetById(userId);
            if (userModel == null)
            {
                userModel = new UserModel();
                userModel.BelongDepart = ServiceProvider.OrgService.GetCodeById(orgId);
            }

            //部门...........生成一个下拉框树所需的数据源
            ViewBag.OrgList = ServiceProvider.OrgService.GetAll();

            ////员工状态
            //ParamModel paramModel = ServiceProvider.ParamService.GetByCode(GlobalConstants.SysParamType.EmployeeState);
            IList<ParamModel> list = ServiceProvider.ParamService.GetAll();
            ParamModel paramModel = list.SingleOrDefault(p => p.ParamCode == GlobalConstants.SysParamType.EmployeeState);
            //ParamModel paramModel = ServiceProvider.ParamService.GetByCode(GlobalConstants.SysParamType.EmployeeState);
            ViewBag.EmployeeStateList = new SelectList(paramModel.itemsList, "ParamItemValue", "ParamItemName", userModel.EmployeeState);

            if (userId != 0) ViewBag.JobNumb = ServiceProvider.UserService.GetById(userId).UserName;

            return View("EditUser", userModel);
        }

        public ActionResult Save(UserModel user, FormCollection collection)
        {
            //UserModel user = new UserModel();
            //user.UserName = collection["UserName"];
            //user.Sex = collection["Sex"].Equals("True");
            //user.JobNo = collection["JobNo"];
            user.BelongDepart = collection["OrgName"];
            user.DepartName = ServiceProvider.OrgService.GetByCode(user.BelongDepart).OrgName;
            //user.Duty = collection["Duty"];
            //user.Mobile = collection["Mobile"];
            //user.Email = collection["Email"];
            //user.EmployeeState = int.Parse(collection["EmployeeState"]);
            ServiceProvider.UserService.SaveUser(user);       //新增或修改员工
            return Content("OK");
        }


        public ActionResult JudgeJobNumb(int userId, string jobNo)
        {
            bool IsExist = false;
            int count = ServiceProvider.UserService.IsExistjobNo(userId, jobNo);
            if (count > 0)
            {
                IsExist = true;
            }
            return Content(IsExist.ToString());
        }

        public ActionResult SearchByNameJobNumb(string term)
        {
            IList<Hashtable> rs = ServiceProvider.UserService.SearchByNameJobNumb(term);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (Hashtable ht in rs)
            {
                sb.Append("{");
                sb.AppendFormat("\"value\":\"{0}\",\"label\":\"{1},{2},{3}\"", ht["UserId"], ht["UserName"], ht["LoginName"], ht["OrgName"]);
                sb.Append("},");
            }
            if (rs.Count > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return Content(sb.ToString());
        }

        public ActionResult LoadOrgJavascript(string userId)
        {
            //userId = UtilsHelper.Decrypt(userId);

            StringBuilder js = new StringBuilder();
            IList<OrgModel> checkedOrg = ServiceProvider.UserManageDepartService.GetUserManageDepartByUserId(int.Parse(userId));
            if (checkedOrg != null)
            {
                foreach (OrgModel item in checkedOrg)
                {
                    IEnumerable<OrgModel> parentOrg = checkedOrg.Where(org => org.OrgCode.IndexOf(item.OrgCode) == 0 && org.OrgCode != item.OrgCode);
                    //当前组织仅仅为叶节点时
                    if (parentOrg.Count() == 0)
                        js.AppendFormat("tree.setCheck('{0}',1);{1}", UtilsHelper.Encrypt(item.OrgId.ToString()), Environment.NewLine);
                }
            }
            return JavaScript(js.ToString());
        }

        public ActionResult SaveManageDepart(string orgCodeCheck, string orgCodePartialCheck, string userId)
        {
            try
            {
                //userId = UtilsHelper.Decrypt(userId);
                if (!string.IsNullOrEmpty(orgCodePartialCheck))
                    orgCodeCheck = orgCodeCheck.Replace(orgCodePartialCheck, "");
                ServiceProvider.UserManageDepartService.SaveUserManageDepart(int.Parse(userId), orgCodeCheck);
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
    }
}
