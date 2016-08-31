using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Common.Models
{
    public class EmployeeModel
    {

        public EmployeeModel()
        { }

        #region Model
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 工牌号
        /// </summary>
        public string WorkCardCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// 最高学历
        /// </summary>
        public int TopEducation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WorkingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string JoinGrgtDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BelongDepart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WorkDepart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Duty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 0：在职，-1：离职
        /// </summary>
        public int EmployeeState { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public string DimissionDate { get; set; }
        /// <summary>
        /// 离职部门
        /// </summary>
        public string DimissionDepart { get; set; }
        /// <summary>
        /// 离职职位
        /// </summary>
        public string DimissionDuty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 联系人信息
        /// </summary>
        public EmpContactModel ContactInfo { get; set; }
        #endregion Model
    }
}
