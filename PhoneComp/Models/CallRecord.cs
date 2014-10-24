using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneComp.Models
{
    public class CallRecord
    {
        /// <summary>
        /// 通话编号
        /// </summary>
        [Key]
        public int CallRecordID { get; set; }
        /// <summary>
        /// 嫌疑人编号
        /// </summary>        
        [Required]
        public int SuspectID { get; set; }
        /// <summary>
        /// 呼叫的号码
        /// </summary>
        [Required]
        public string CalledMobile { get; set; }
        /// <summary>
        /// 主被叫方式
        /// </summary>
        [Required]
        public string LordCalled { get; set; }
        /// <summary>
        /// 通话时间
        /// </summary>
        public string CallTime { get; set; }
        /// <summary>
        /// 通话时长
        /// </summary>
        public string CallDuration { get; set; }
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