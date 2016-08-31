using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using ToolsLib.Utility;
using GRGTCommonUtils;
using Instrument.Common.Models;
using Instrument.Common;
using Instrument.Business;
using Global.Common.Models;

namespace Instrument.WebSite.Controllers
{
    public class RemindController : Controller
    {
       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        public ActionResult IsHasRemind()
        {
            int count = 0;

            #region 过期仪器提醒
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.Append("1=1");
            if (!LoginHelper.LoginUserAuthorize.ContainsKey("Instrument-CheckAll".ToLower()))
            {
                sqlWhere.AppendFormat(" and {0}", Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart"));
            }
            sqlWhere.AppendFormat(" and '{0:yyyy-MM-dd}'>DueEndDate and ManageLevel !='C' and RecordState={1}", DateTime.Now, UtilConstants.InstrumentState.过期禁用.GetHashCode());

            IList<Instrument.Common.Models.InstrumentModel> overTimeList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(sqlWhere.ToString());
            //当前用户下已加入清单但未送检的仪器
            IList<InstrumentWaitSendModel> preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            InstrumentWaitSendModel instrumentWaitSendModel = null;
           
            foreach (Instrument.Common.Models.InstrumentModel item in overTimeList)
            {
                instrumentWaitSendModel = preSendList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                if (instrumentWaitSendModel != null) continue;
                count++;
                break;
            }
            #endregion

            #region 软件过期提醒
            if (count == 0)
            {
                string PublishKey = WebUtils.GetSettingsValue("PublishDate");
                if (string.IsNullOrEmpty(PublishKey) == true)
                {
                    PublishKey = Global.Common.GlobalConstants.PublishDate;
                }
                else
                {
                    PublishKey = SSOHelper.Decrypt(PublishKey);
                }
                DateTime publishDate = DateTime.ParseExact(PublishKey, "yyyy-MM-dd", null);
                count = new TimeSpan(DateTime.Now.Ticks - (publishDate.AddYears(1).Ticks)).Days;
            }
            #endregion

            JsonResult jr = Json(new
            {
               hasremind=count>0?true:false
            }, JsonRequestBehavior.AllowGet);
            return jr;
        }


        public ActionResult RemindDetail()
        {
            #region 过期仪器提醒
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.Append("1=1");
            if (!LoginHelper.LoginUserAuthorize.ContainsKey("Instrument-CheckAll".ToLower()))
            {
                sqlWhere.AppendFormat(" and {0}", Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart"));
            }
            sqlWhere.AppendFormat(" and '{0:yyyy-MM-dd}'>DueEndDate and ManageLevel !='C' and RecordState={1}", DateTime.Now, UtilConstants.InstrumentState.过期禁用.GetHashCode());

            IList<Instrument.Common.Models.InstrumentModel> overTimeList = ServiceProvider.InstrumentService.GetAllInstrumentListByWhere(sqlWhere.ToString());
            //当前用户下已加入清单但未送检的仪器
            IList<InstrumentWaitSendModel> preSendList = ServiceProvider.InstrumentWaitSendService.GetByUserId(LoginHelper.LoginUser.UserId);
            InstrumentWaitSendModel instrumentWaitSendModel = null;
            int count = 0;
            foreach (Instrument.Common.Models.InstrumentModel item in overTimeList)
            {
                instrumentWaitSendModel = preSendList.SingleOrDefault(i => i.InstrumentId.Equals(item.InstrumentId));
                if (instrumentWaitSendModel != null) continue;
                count++;
            }
            ViewBag.InstrumentRemindCount = count;
            #endregion

            #region 软件过期
            string PublishKey = WebUtils.GetSettingsValue("PublishDate");
            //string PublishKey1 = SSOHelper.Encrypt("2015-12-31");
            if (string.IsNullOrEmpty(PublishKey) == true)
            {
                PublishKey = Global.Common.GlobalConstants.PublishDate;
            }
            else
            {
                PublishKey = SSOHelper.Decrypt(PublishKey);
            }
            DateTime publishDate = DateTime.ParseExact(PublishKey, "yyyy-MM-dd", null);
            ViewBag.ExpireDays =  new TimeSpan( DateTime.Now.Ticks - (publishDate.AddYears(1).Ticks)).Days;

            #endregion
            return View();
        }
    }
}
