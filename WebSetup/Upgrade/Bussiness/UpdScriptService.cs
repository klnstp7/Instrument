using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Upgrade.DataAccess;
using Upgrade.Models;

namespace Upgrade.Bussiness
{
    public  class UpdScriptService
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static void ChkConnectSetting()
        {
            UpdScriptDao.ChkConnectSetting();
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="scriptPath"></param>
        public static void ExcuteScript(string scriptPath)
        {
            // 遍历所有的脚本文件 
            string[] fileList = System.IO.Directory.GetFiles(scriptPath);
            foreach (string file in fileList)
            {
                if (Path.GetExtension(file).ToLower() != ".sql") continue;
                FileStream fileStream = new FileStream(file, FileMode.Open);
                StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
                string cmdText = streamReader.ReadToEnd();
                streamReader.Close();
                fileStream.Close();

                if (cmdText.Trim().Length == 0) continue;

                //执行脚本
                UpdScriptDao.ExcuteScript(cmdText);
            }
        }


        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="model"></param>
        public static ParamModel GetParamByCode(string paramCode)
        {
            return UpdScriptDao.GetParamByCode(paramCode);
        }

       /// <summary>
        /// 插入版本号
        /// </summary>
        /// <param name="model"></param>
        public static void InsertParam(ParamModel model)
        {
            UpdScriptDao.InsertParam(model);
        }

         /// <summary>
        /// 更新版本号
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateParam(ParamModel model)
        {
            UpdScriptDao.UpdateParam(model);
        }
    }
}
