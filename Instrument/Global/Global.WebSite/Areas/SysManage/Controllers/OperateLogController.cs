using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRGTCommonUtils;
using System.Text;
using System.Collections;
using Global.Business;
using ToolsLib.Utility.Jquery;
using ToolsLib.IBatisNet;
using Global.Common.Models;

namespace Global.WebSite.Areas.SysManage.Controllers
{
    public class OperateLogController : Controller
    {
        //
        // GET: /SysManage/OperateLog/

        public ActionResult Index()
        {
            ViewBag.OperateType = Global.Business.ServiceProvider.ParamService.BulidJsonbyNameValue(UtilConstants.SysParamType.OperateType).ToString();
            ViewBag.TargetType = Global.Business.ServiceProvider.ParamService.BulidJsonbyEnum(new UtilConstants.TargetType()).ToString();
            return View();
        }

        public JsonResult GetOperateLogListJsonData()
        {
            //提取DataTable参数
            DataTableUtils.DataTableModel dtm = DataTableUtils.GetJquerydataTableParams();
            //构造输入参数
            PagingModel paging = new PagingModel();
            paging.PageSize = dtm.PageSize;
            paging.PageCurrent = dtm.PageIndex;
            paging.FieldShow = "OperateType,Operator,OperateDate,OperateIP,OperateContent,TargetPK,TargetType";
            paging.Where = "1=1";
            if(!string.IsNullOrWhiteSpace(dtm.FieldCondition))
            {
                paging.Where = dtm.FieldCondition;
            }
            //paging.Where = string.Format(" 1 = 1 and {0}", dtm.FieldCondition);
            if (!string.IsNullOrEmpty(dtm.KeyWord))
                paging.Where = string.Format("{0} and (Operator like '%{1}%' or OperateContent like '%{1}%' or TargetType like '%{1}%')", paging.Where, dtm.KeyWord);

            IList<ParamModel> paramAllList = Global.Business.ServiceProvider.ParamService.GetAll();
            ParamModel operateTypeParam = paramAllList.SingleOrDefault(S => S.ParamCode == UtilConstants.SysParamType.OperateType);
            ParamItemModel item = null;

            //数据库查询数据
            IList<Hashtable> operateList = ServiceProvider.OperateLogService.GetAllOperateLogListForPaging(paging);
            //Json数据格式组装
            dtm.iTotalRecords = paging.RecordCount;
            dtm.iTotalDisplayRecords = dtm.iTotalRecords;
            dtm.aaData = new List<List<string>>();
            StringBuilder sb = new StringBuilder();
            string orderId = string.Empty;

            foreach (Hashtable row in operateList)
            {
                dtm.aaData.Add(new List<string>());
                item = operateTypeParam.itemsList.SingleOrDefault(p => p.ParamItemValue.Equals(row["OperateType"].ToString()));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", item==null?"":item.ParamItemName));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["Operator"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["OperateIP"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["OperateContent"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}", row["TargetPK"]));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0}",(UtilConstants.TargetType)Convert.ToInt32(row["TargetType"])));
                dtm.aaData[dtm.aaData.Count - 1].Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", row["OperateDate"]));
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

        #region ===  获取操作日志列表 ===
        public string GetOperateLogListByTargetPK(string targetPK, int targetType)
        {
            IList<OperateLogModel> logList = Global.Business.ServiceProvider.OperateLogService.GetByTargetPKAndType(targetPK, targetType).OrderByDescending(o=>o.LogId).ToList();
            StringBuilder sb = new StringBuilder();
            int count = 0;
            sb.Insert(0, "{\"data\":[");
            foreach (OperateLogModel log in logList)
            {
                count++;
                sb.AppendFormat("[\"{0} \"", count);
                sb.AppendFormat(",\"{0}\"", (UtilConstants.OperateType)log.OperateType);              
                //sb.AppendFormat(",\"{0}\"", log.TargetPK);
                sb.AppendFormat(",\"{0}\"", log.OperateContent);
                sb.AppendFormat(",\"{0}\"", log.Operator);
                sb.AppendFormat(",\"{0}\"", log.OperateDate);
                sb.AppendFormat(",\"{0}\"", log.OperateIP);                
                //sb.AppendFormat(",\"{0}\"",log.TargetType);               
                sb.Append("],");
            }

            if (logList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");

            return sb.ToString();
        }
      
        #endregion
    }
}
