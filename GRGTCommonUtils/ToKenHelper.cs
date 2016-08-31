using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsLib.EncryptService;
using ToolsLib.Utility;

namespace GRGTCommonUtils
{
    public class AccessToKen
    {
        //登录帐号
        public string JobNo { get; set; }
        public string ToKen { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }

    }
    public class ToKenHelper
    {
        private static IList<AccessToKen> _ToKenList = null;
        private static object _lock = new object();
        //应用单件模式，保存用户登录状态
        public static IList<AccessToKen> ToKenList
        {
            get
            {
                if (_ToKenList == null)
                {
                    lock (_lock)
                    {
                        if (_ToKenList == null)
                            _ToKenList = new List<AccessToKen>();
                    }
                }
                return _ToKenList;
            }
        }

        /// <summary>
        /// 令牌验证
        /// </summary>
        /// <param name="accessToKen"></param>
        /// <returns></returns>
        public static bool CheckAccessToKen(string accessToKen)
        {
            bool flag = false;

            if (string.IsNullOrWhiteSpace(accessToKen))
                return flag;
            string token = SSOHelper.Decrypt(accessToKen);
            if (!token.Contains("|"))
                return flag;
            string[] result = token.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length != 2)
                return flag;

            lock (_lock)
            {
                AccessToKen model = ToKenHelper.ToKenList.Where(c => c.ToKen == accessToKen).SingleOrDefault();
                if (model != null)
                {
                    if (model.BeginTime <= DateTime.Now && DateTime.Now<=model.EndTime)
                    {
                        int index = ToKenHelper.ToKenList.IndexOf(model);
                        ToKenHelper.ToKenList[index].BeginTime = DateTime.Now;
                        ToKenHelper.ToKenList[index].EndTime = DateTime.Now.AddHours(2);
                        flag = true;
                    }
                    else
                        ToKenHelper.ToKenList.Where(c => c.ToKen == accessToKen).ToList().ForEach(x => { ToKenHelper.ToKenList.Remove(x); });
                }
            }


            return flag;
        }
        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="JobNo"></param>
        public static string CreateToKen(string JobNo)
        {
            string accessToken = SSOHelper.Encrypt(string.Format("{0}|{1}", JobNo, Guid.NewGuid().ToString()));
            lock (_lock)
            {
                ToKenHelper.ToKenList.Where(c => c.JobNo == JobNo).ToList().ForEach(x => { ToKenHelper.ToKenList.Remove(x); });
                AccessToKen model = new AccessToKen();
                model.ToKen = accessToken;
                model.JobNo = JobNo;
                model.BeginTime = DateTime.Now;
                model.EndTime = DateTime.Now.AddHours(2);
                ToKenHelper.ToKenList.Add(model);

            }
            return accessToken;
        }

    }
}
