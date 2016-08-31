using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarcodePrint.Models
{
    public class LoginResult
    {
        public string Msg { get; set; }
        public LoginInfo Data { get; set; }
    }
    public class LoginInfo
    {
        //用户帐号，登录帐号
        public int UserId { get; set; }
        public string LoginPwd{get;set;}
        public string UserName{get;set;}
        public string JobNo { get; set; }
        public string AccessToKen { get; set; }
        private static LoginInfo _CurrentUser = null;
        //应用单件模式，保存用户登录状态
        public static LoginInfo CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                    _CurrentUser = new LoginInfo();
                return _CurrentUser;
            }
        }
    }
}
