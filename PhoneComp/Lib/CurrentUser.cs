using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using PhoneComp.Models;
using PhoneComp.DAL;

namespace PhoneComp.Lib
{
    public class CurrentUser
    {
        #region 属性
        /// <summary>
        /// 登录用户IDer
        /// </summary>
        public static int UID
        {
            get
            {
                string uid = CookieHelper.GetCookie("UID");
                return string.IsNullOrEmpty(uid) ? 0 : int.Parse(uid);
            }
        }
        /// <summary>
        /// 登录用户角色ID
        /// </summary>
        public static int RoleID
        {
            get
            {
                string roleID = CookieHelper.GetCookie("RoleID");
                return string.IsNullOrEmpty(roleID) ? 0 : int.Parse(roleID);
            }
        }
        /// <summary>
        /// 登录用户UserName
        /// </summary>
        public static string UserName
        {
            get
            {
                PhoneCompContext db = new PhoneCompContext();
                var CurrentUser = db.Members.Where(m => m.MemberID == UID).FirstOrDefault();
                return CurrentUser.RealName;
            }
        }

        #endregion
        
        
        #region 方法
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="auto"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool AdminLogOn(string username, string password, bool auto, out string message)
        {
            PhoneCompContext db = new PhoneCompContext();
            var passwordCL =Character.EncrytPassword(password);
            message = string.Empty;
            Member member = db.Members.SingleOrDefault(u => u.IsDeleted == false && u.Password == passwordCL && u.UserName == username);
            if (member != null)
            {
                if (auto)//cookie保存
                {
                    CookieHelper.SetCookie("UID", member.MemberID.ToString(), 1);
                    CookieHelper.SetCookie("RoleID", member.RoleID.ToString(), 1);
                    CookieHelper.SetCookie("UserName", member.UserName.ToString(), 1);
                    member.LastLoginTime = DateTime.Now;
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                }
                message = "登录成功，欢迎回来 " + member.RealName + "（" + member.UserName + "）";
                return true;
            }
            else
            {
                message = "登录失败，账户信息错误";
                return false;
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        public static void LogOut()
        {
            //清除Cookie
            CookieHelper.DelCookie("UID");
            CookieHelper.DelCookie("RoleID");
            CookieHelper.DelCookie("UserName");
            //删除Session
            System.Web.HttpContext.Current.Session.Clear();

        }
        #endregion
    }
}