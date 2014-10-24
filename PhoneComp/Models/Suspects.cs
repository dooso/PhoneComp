using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneComp.Models
{
    public class Suspects
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int SuspectID { get; set; }
        /// <summary>
        /// 嫌疑人姓名
        /// </summary>
        [Required]
        [MinLength(2),MaxLength(50)]
        public string SuspectName { get; set; }
        /// <summary>
        /// 嫌疑人手机号
        /// </summary>
        [Required]
        [MinLength(6),MaxLength(24)]
        public string SuspectMobile { get; set; }
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

        /// <summary>
        /// 通话记录
        /// </summary>
        public virtual ICollection<CallRecord> CallRecords { get; set; }
        /// <summary>
        /// 手机通讯录
        /// </summary>
        public virtual ICollection<Contact> SuspectsContacts { get; set; }
    }
}