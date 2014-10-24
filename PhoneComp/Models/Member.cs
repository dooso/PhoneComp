using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PhoneComp.Models
{
    public class Member
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int MemberID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 用户密码明码
        /// </summary>
        [DataType(DataType.Password)]
        public string PasswordNotMD5 { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDcard { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Required]
        public int RoleID { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateMemberID { get; set; }
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
        /// 上次登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual Role Role { get; set; }
    }
}