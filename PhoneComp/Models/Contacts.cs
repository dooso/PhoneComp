using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneComp.Models
{
    public class Contact
    {
        /// <summary>
        /// 嫌疑人通讯录编号
        /// </summary>
        [Key]
        public int ContactID { get; set; } 
        /// <summary>
        /// 嫌疑人编号
        /// </summary>
        [Required]
        public int SuspectID { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Required]
        public string LinkerName { get; set; }
        /// <summary>
        /// 联系人号码
        /// </summary>
        [Required]
        public string LinkerMobile { get; set; }
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

        /// <summary>
        /// 所属嫌疑人
        /// </summary>
        public virtual Suspects Suspects { get; set; }

        public virtual Member Member { get; set; }
    }
    
}