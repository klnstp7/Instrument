using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrument.DataAccess;
using System.Collections;
using ToolsLib.IBatisNet;
using Instrument.Common;
using Instrument.Common.Models;
using GRGTCommonUtils;
using System.Data;
using Global.Common.Models;

namespace Instrument.Business
{
    public class InstrumentWaitSendServiceImpl
    {
        public void DeleteByInstrumentId(int InstrumentId)
        {
            DBProvider.InstrumentWaitSendDAO.DeleteByInstrumentId(InstrumentId);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="InstrumentId"></param>
        /// <param name="Userid"></param>
        public void Delete(int InstrumentId,int Userid)
        {
            DBProvider.InstrumentWaitSendDAO.DeleteByInstrumentId(InstrumentId, Userid);
        }
        /// <summary>
        /// 删除记录.
        /// </summary>
        public void DeleteById(int AutoId)
        {
            DBProvider.InstrumentWaitSendDAO.DeleteById(AutoId);
        }

        public void DeleteByIds(IList<int> autoIdList)
        {
            DBProvider.InstrumentWaitSendDAO.DeleteByIds(autoIdList);
        }
        /// <summary>
        /// 通过仪器id集合批量删除
        /// </summary>
        /// <param name="instrumentIdList"></param>
        public void DeleteByInstrumentIds(IList<int> instrumentIdList,int userId)
        {
            DBProvider.InstrumentWaitSendDAO.DeleteByInstrumentIds(instrumentIdList, userId);
        }

        /// <summary>
        /// 保存实体数据.
        /// </summary>
        public void Save(InstrumentWaitSendModel model)
        {
            if (model.AutoId == 0) DBProvider.InstrumentWaitSendDAO.Add(model);
            else DBProvider.InstrumentWaitSendDAO.Update(model);
        }

        /// <summary>
        /// 批量修改备注信息
        /// </summary>
        public void UpdateRemark(IList<int> instrumentIdList, string Remark, int userId)
        {
            DBProvider.InstrumentWaitSendDAO.UpdateRemark(instrumentIdList, Remark, userId);
        }


        //public IList<Hashtable> GetAllPreSendListForPaging(PagingModel paging)
        //{
        //    string GetAllAuthorityStr = "PreSendList-CheckAll";
        //    bool IsGetAllAuthority = LoginHelper.LoginUserAuthorize.ContainsKey(GetAllAuthorityStr.ToLower());

        //    if (!IsGetAllAuthority)
        //    {
        //        //获取当前用户所管辖的所有区域下的仪器SQL语句.
        //        StringBuilder subSqlStr = Global.Business.ServiceProvider.UserManageDepartService.GetSQL2MyMangeDepart("BelongDepart");
        //        if (!string.IsNullOrWhiteSpace(paging.Where))
        //            paging.Where = string.Format(" {0} and {1}", subSqlStr, paging.Where);
        //        else
        //            paging.Where = subSqlStr.ToString();
        //    }
        //    return DBProvider.InstrumentWaitSendDAO.GetAllPreSendListForPaging(paging);
        //}

        /// <summary>
        /// 获取一个记录对象.
        /// </summary>
        public InstrumentWaitSendModel GetById(int AutoId)
        {
            return DBProvider.InstrumentWaitSendDAO.GetById(AutoId);
        }

        public IList<InstrumentWaitSendModel> GetByAutoIdList(IList<int> autoIdList)
        {
            return DBProvider.InstrumentWaitSendDAO.GetByAutoIdList(autoIdList);
        }

        public IList<InstrumentWaitSendModel> GetByInstrumentIdsList(IList<int> instrumentIds,int UserId)
        {
            return DBProvider.InstrumentWaitSendDAO.GetByInstrumentIdsList(instrumentIds,UserId);
        }

        public IList<InstrumentWaitSendModel> GetByUserId(int userId)
        {
            return DBProvider.InstrumentWaitSendDAO.GetByUserId(userId);
        }

        /// <summary>
        /// 获取所有的记录对象.
        /// </summary>
        public IList<InstrumentWaitSendModel> GetAll()
        {
            return DBProvider.InstrumentWaitSendDAO.GetAll();
        }

        /// <summary>
        /// 检测用户的仪器是否已存在.
        /// </summary>
        public bool ExistsInstrument( int InstrumentId,int UserId)
        {
            return DBProvider.InstrumentWaitSendDAO.ExistsInstrument(InstrumentId,UserId);
        }

        /// <summary>
        /// 获取待送仪器信息;
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns>IList 【InstrumentName,Specification ,SerialNo,ManageNo ,AssetsNo,BarCode】</returns>
        public IList<Hashtable> GetInstrumentByUserId(int Userid)
        {
            return DBProvider.InstrumentWaitSendDAO.GetInstrumentByUserId(Userid);
        }
    }
}
