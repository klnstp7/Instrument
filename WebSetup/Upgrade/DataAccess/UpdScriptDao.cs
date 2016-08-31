using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Upgrade.Common;
using Upgrade.Models;

namespace Upgrade.DataAccess
{
    public class UpdScriptDao
    {
        /// <summary>
        /// 检查连接字符串的正确性
        /// </summary>
        public static void ChkConnectSetting()
        {
            SqlConnection connection = new SqlConnection(SqlHelper.ConnectionString);
            if (connection.State != ConnectionState.Open)
                    connection.Open();
            if (connection.State != ConnectionState.Closed)
                connection.Close();
              
        }
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        public static void ExcuteScript(string cmdText)
        {
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionString, CommandType.Text, cmdText, null);
        }


        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="model"></param>
        public static ParamModel GetParamByCode(string paramCode)
        {
            ParamModel model=null;
            string cmdText = "SELECT ParamName FROM Sys_Params WHERE ParamCode= @ParamCode";
            SqlParameter[] param = { new SqlParameter("@ParamCode", paramCode) };
           
            IDataReader reader=SqlHelper.ExecuteReader(SqlHelper.ConnectionString, CommandType.Text, cmdText, param);
            while (reader.Read())
            {
                model=new ParamModel();
                model.ParamName=String.Format("{0}",reader["ParamName"]);
            }
            return model;
        }

        /// <summary>
        /// 插入版本号
        /// </summary>
        /// <param name="model"></param>
        public static void InsertParam(ParamModel model)
        {
            string cmdText = "INSERT INTO Sys_Params (ParamName, ParamCode, status, CreateDate) VALUES (@ParamName, @ParamCode, @status,getdate())";
            SqlParameter[] param = {new SqlParameter("@ParamName",model.ParamName),
                                    new SqlParameter("@ParamCode",model.ParamCode),
                                    new SqlParameter("@status",1),
                                   };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionString, CommandType.Text, cmdText, param);

        }

        /// <summary>
        /// 更新版本号
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateParam(ParamModel model)
        {
            string cmdText = "UPDATE  Sys_Params Set ParamName=@ParamName WHERE ParamCode=@ParamCode";
            SqlParameter[] param = {new SqlParameter("@ParamName",model.ParamName),
                                    new SqlParameter("@ParamCode",model.ParamCode)
                                   };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionString, CommandType.Text, cmdText, param);
        }
    }
}
