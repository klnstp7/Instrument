using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRGTCommonUtils;
using Global.Common.Models;

namespace Global.Business
{
    public class OperateLogHelper
    {
        public static void SaveLog<T>(T model, UtilConstants.OperateType operateType)
        {
            OperateLogModel operateModel = new OperateLogModel();
            if (typeof(T) == typeof(UserModel))
            {
                UserModel mUser = model as UserModel;
                operateModel.OperateContent ="登录："+mUser.UserName;
                operateModel.TargetPK = mUser.JobNo;
            }

            operateModel.OperateType = (int)operateType;
            operateModel.Operator = LoginHelper.LoginUser.UserName;
            operateModel.TargetType = (int)UtilConstants.TargetType.系统登录日志;
            Global.Business.ServiceProvider.OperateLogService.Save(operateModel);
        }
    }
}
