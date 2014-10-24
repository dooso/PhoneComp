using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PhoneComp.Models
{
    public class UsedWorker
    { 
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int UsedWorkerID { get; set; }
        /// <summary>
        /// 二手从业人员姓名
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [Required]
        public string IDcard { get; set; }
        /// <summary>
        /// 从事行业
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// 从事行业地点
        /// </summary>
        public string JobAddress { get; set; }
        /// <summary>
        /// 家庭地址
        /// </summary>
        public string HomeAddress { get; set; }
        /// <summary>
        /// 联系方式1
        /// </summary>
        public string Phone1 { get; set; }
        /// <summary>
        /// 联系方式2
        /// </summary>
        public string Phone2 { get; set; }
        /// <summary>
        /// 联系方式3
        /// </summary>
        public string Phone3 { get; set; }
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [ForeignKey("Member")]
        public int? CreateUserID { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public int? LastUpdateUserID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? LastUpdateDate { get; set; }

        
        public virtual Member Member { get; set; }
    }
}