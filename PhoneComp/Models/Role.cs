using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneComp.Models
{
    public class Role
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Key]
        public int RoleID { get; set; }
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
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
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色说明
        /// </summary>
        public string Remark { get; set; }

    }
}